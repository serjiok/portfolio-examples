using Microsoft.Extensions.DependencyInjection;
using MyMediatr.Interfaces;
using System.Collections.Concurrent;
using System.Reflection;

namespace MyMediatr;

public sealed class Mediator(IServiceProvider serviceProvider, MyMediatrServiceConfiguration configuration) : IMediator
{
    public static readonly ConcurrentDictionary<Type, MethodInfo> methods = [];
    public static readonly ConcurrentDictionary<Type, Type> types = [];

    public Task Publish(INotification request, CancellationToken cancellationToken = default)
    {
        var consumerType = GetCacheType(request.GetType(), typeof(INotificationHandler<>));

        var serviceHandlers = GetServiceProvider(serviceProvider, configuration.LifeTime).
                              GetServices(consumerType).Where(x => x != null).DistinctBy(x => x!.GetType());

        if (!serviceHandlers.Any()) return Task.CompletedTask;

        List<Task> tasks = new(serviceHandlers.Count());
        foreach (var handler in serviceHandlers)
        {
            var m = methods.GetOrAdd(handler!.GetType(), _ => handler.GetType()
                    .GetMethod(nameof(INotificationHandler<>.Handle)) ?? throw new NullReferenceException());
            
            tasks!.Add(m.Invoke(handler, [request, cancellationToken]) as Task ?? throw new NullReferenceException());
        }
        return Task.WhenAll(tasks);
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        where TResponse : notnull
    {
        var consumerType = GetCacheType(request.GetType(), typeof(IRequestHandler<,>), typeof(TResponse));

        var handler = GetServiceProvider(serviceProvider, configuration.LifeTime).GetRequiredService(consumerType);

        var m = methods.GetOrAdd(handler!.GetType(), _ => handler.GetType()
                .GetMethod(nameof(IRequestHandler<,>.Handle)) ?? throw new NullReferenceException());

        return m.Invoke(handler, [request, cancellationToken]) as Task<TResponse> ?? throw new NullReferenceException();
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : notnull, IRequest
    {
        var consumerType = GetCacheType(request.GetType(), typeof(IRequestHandler<>));
        
        var handler = GetServiceProvider(serviceProvider, configuration.LifeTime).GetRequiredService(consumerType);

        var m = methods.GetOrAdd(handler!.GetType(), _ => handler.GetType()
                .GetMethod(nameof(IRequestHandler<>.Handle)) ?? throw new NullReferenceException());

        return m.Invoke(handler, [request, cancellationToken]) as Task ?? throw new NullReferenceException();
    }

    private static Type GetCacheType(Type type, Type interFaceType, Type? responseType = null)
        => types.GetOrAdd(type, _ =>
            responseType == null ? interFaceType.MakeGenericType(type) : interFaceType.MakeGenericType(type, responseType));

    private IServiceProvider GetServiceProvider(IServiceProvider serviceProvider, ServiceLifetime serviceLifetime) =>
        serviceLifetime == ServiceLifetime.Singleton ? serviceProvider : serviceProvider.CreateScope().ServiceProvider;
}
