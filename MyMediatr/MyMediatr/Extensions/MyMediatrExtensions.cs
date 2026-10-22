using Microsoft.Extensions.DependencyInjection;
using MyMediatr.Interfaces;
using System.Reflection;

namespace MyMediatr.Extensions;

public static class MyMediatrExtensions
{
    private static readonly TypeFilter typeFilter = static (type, criteria) => type.IsGenericType && type.GetGenericTypeDefinition() == (Type)criteria!;

    extension (IServiceCollection services)
    {
        public IServiceCollection AddMyMediatr(Assembly? assembly)
        {
            assembly ??= Assembly.GetExecutingAssembly();
            services.RegistrateNotification(assembly)
                    .RegistrateRequest(assembly)
                    .RegistrateRequestResponse(assembly)
                    .AddSingleton<IMediator, Mediator>();

            return services;
        }

        private IServiceCollection RegistrateNotification(Assembly assembly)
        {
            var notifications = FindTypesImplementingGenericInterface(assembly, typeof(INotificationHandler<>));

            foreach (var notification in notifications)
            {
                Type typeRequest = notification.FindInterfaces(typeFilter, typeof(INotificationHandler<>)).First().GenericTypeArguments[0];
                Type handlerType = typeof(INotificationHandler<>).MakeGenericType(typeRequest);
                
                var handlers = assembly.GetTypes().Where(x => handlerType.IsAssignableFrom(x));
                foreach (var handler in handlers)
                {
                    services.Add(new ServiceDescriptor(handlerType, handler, ServiceLifetime.Transient));
                }
            }

            return services;
        }

        private IServiceCollection RegistrateRequest(Assembly assembly)
        {
            var handlers = FindTypesImplementingGenericInterface(assembly, typeof(IRequestHandler<>));

            foreach (var handler in handlers)
            {
                Type typeRequest = handler.FindInterfaces(typeFilter, typeof(IRequestHandler<>)).First().GenericTypeArguments[0];
                Type handlerType = typeof(IRequestHandler<>).MakeGenericType([typeRequest]);

                services.Add(new ServiceDescriptor(
                    handlerType,
                    handler,
                    ServiceLifetime.Transient
                ));
            }

            services.AddSingleton<IMediator, Mediator>();

            return services;
        }

        private IServiceCollection RegistrateRequestResponse(Assembly assembly)
        {
            var handlers = FindTypesImplementingGenericInterface(assembly, typeof(IRequestHandler<,>));

            foreach (var handler in handlers)
            {
                Type types = handler.FindInterfaces(typeFilter, typeof(IRequestHandler<,>)).First();
                Type typeRequest = types.GenericTypeArguments[0];
                Type typeResponse = types.GenericTypeArguments[1];
                Type handlerType = typeof(IRequestHandler<,>).MakeGenericType([typeRequest, typeResponse]);

                services.Add(new ServiceDescriptor(
                    handlerType,
                    handler,
                    ServiceLifetime.Transient
                ));
            }

            services.AddSingleton<IMediator, Mediator>();

            return services;
        }
    }

    private static IEnumerable<Type> FindTypesImplementingGenericInterface(
        Assembly assembly,
        Type genericInterfaceDefinition)
    {
        if (!genericInterfaceDefinition.IsGenericTypeDefinition)
            throw new ArgumentException("typeof(T<>)", nameof(genericInterfaceDefinition));

        return assembly.GetTypes()
            .Where(t => !t.IsInterface && !t.IsAbstract)
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType &&
                          i.GetGenericTypeDefinition() == genericInterfaceDefinition));
    }
}
