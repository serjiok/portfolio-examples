using Microsoft.Extensions.DependencyInjection;
using MyMediatr.Interfaces;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;

namespace MyMediatr;

public sealed class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public static readonly ConcurrentDictionary<Type, MethodInfo> methods = [];
    public static readonly ConcurrentDictionary<Type, Type> types = [];

    public Task Publish(INotification request, CancellationToken cancellationToken = default)
    {
        var consumerType = GetCacheType(request.GetType(), typeof(INotificationHandler<>));

        var serviceHandlers = serviceProvider.GetServices(consumerType).Where(x => x != null).DistinctBy(x => x!.GetType());
        if (serviceHandlers == null) return Task.CompletedTask;

        List<Task>? tasks = null;
        int count = serviceHandlers.Count();
        if (count > 1) tasks = new(count);
        foreach (var handler in serviceHandlers)
        {
            var m = methods.GetOrAdd(handler!.GetType(), _ => handler.GetType()
                    .GetMethod(nameof(INotificationHandler<>.Handle)) ?? throw new NullReferenceException());

            if (count == 1) return m.Invoke(handler, [request, cancellationToken]) as Task ?? throw new NullReferenceException();
            
            tasks!.Add(m.Invoke(handler, [request, cancellationToken]) as Task ?? throw new NullReferenceException());
        }
        return tasks == null ? Task.CompletedTask : Task.WhenAll(tasks);
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        where TResponse : notnull
    {
        var consumerType = GetCacheType(request.GetType(), typeof(IRequestHandler<,>), typeof(TResponse));

        var handler = serviceProvider.GetRequiredService(consumerType);

        var m = methods.GetOrAdd(handler!.GetType(), _ => handler.GetType()
                .GetMethod(nameof(IRequestHandler<,>.Handle)) ?? throw new NullReferenceException());

        return m.Invoke(handler, [request, cancellationToken]) as Task<TResponse> ?? throw new NullReferenceException();
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : notnull, IRequest
    {
        var consumerType = GetCacheType(request.GetType(), typeof(IRequestHandler<>));

        var handler = serviceProvider.GetRequiredService(consumerType);

        var m = methods.GetOrAdd(handler!.GetType(), _ => handler.GetType()
                .GetMethod(nameof(IRequestHandler<>.Handle)) ?? throw new NullReferenceException());

        return m.Invoke(handler, [request, cancellationToken]) as Task ?? throw new NullReferenceException();
    }

    private static Type GetCacheType(Type type, Type interFaceType, Type? responseType = null)
        => types.GetOrAdd(type, _ =>
            responseType == null ? interFaceType.MakeGenericType(type) : interFaceType.MakeGenericType(type, responseType));
}
