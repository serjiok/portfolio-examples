using Microsoft.Extensions.DependencyInjection;
using MyMediatr.Interfaces;
using System.Collections.Concurrent;
using System.Reflection;

namespace MyMediatr;

public sealed class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public static ConcurrentDictionary<Type, MethodInfo> methods = [];
    public static ConcurrentDictionary<Type, Type> types = [];

    public Task Publish(INotification request, CancellationToken cancellationToken = default)
    {
        var typeRequest = request.GetType();
        Type consumerType = typeof(INotificationHandler<>);
        consumerType = consumerType.MakeGenericType(typeRequest);

        var serviceHandlers = serviceProvider.GetServices(consumerType).Where(x => x != null).DistinctBy(x=>x.GetType());
        if (serviceHandlers == null) return Task.CompletedTask;

        List<Task> tasks = new(serviceHandlers.Count());
        foreach (var handler in serviceHandlers)
        {
            if (!methods.TryGetValue(handler!.GetType(), out var m))
            {
                m = handler.GetType().GetMethod(nameof(INotificationHandler<>.Handle))!;
                methods.TryAdd(handler.GetType(), m);
            }
            tasks.Add(Task.Run(()=> m.Invoke(handler, [request, cancellationToken]), cancellationToken));
        }
        return Task.WhenAll(tasks);
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) 
        where TResponse: notnull
    {
        var consumerType = GetCacheType(request.GetType(), typeof(IRequestHandler<,>), typeof(TResponse));

        var handler = serviceProvider.GetRequiredService(consumerType);

        if (!methods.TryGetValue(handler!.GetType(), out var m))
        {
            m = handler.GetType().GetMethod(nameof(IRequestHandler<,>.Handle))!;
            methods.TryAdd(handler.GetType(), m);
        }
        var result = m.Invoke(handler, [request, cancellationToken]) as Task<TResponse> ?? throw new NullReferenceException();
        return result;
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) 
        where TRequest : notnull, IRequest
    {
        var consumerType = GetCacheType(request.GetType(), typeof(IRequestHandler<>));

        var handler = serviceProvider.GetRequiredService(consumerType);
        if (!methods.TryGetValue(handler!.GetType(), out var m))
        {
            m = handler.GetType().GetMethod(nameof(IRequestHandler<>.Handle))!;
            methods.TryAdd(handler.GetType(), m);
        }
        var result = m.Invoke(handler, [request, cancellationToken]) as Task ?? throw new NullReferenceException();
        return result;
    }

    private Type GetCacheType(Type type, Type interFaceType, Type? responseType = null)
    {
        if (!types.TryGetValue(type, out var consumerType))
        {
            consumerType = interFaceType;
            consumerType = responseType == null ? consumerType.MakeGenericType(type) : consumerType.MakeGenericType(type, responseType);
            types.TryAdd(type, consumerType);
        }
        return consumerType;
    }
}
