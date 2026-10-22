using Microsoft.Extensions.DependencyInjection;
using MyMediatr.Interfaces;
using System.Reflection;

namespace MyMediatr.Extensions;

public static class MyMediatrExtensions
{

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
                var i = notification.GetInterfaces().First(x => x.GetGenericTypeDefinition() == typeof(INotificationHandler<>));
                var type = i.GenericTypeArguments[0];
                Type consumerType = typeof(INotificationHandler<>);
                consumerType = consumerType.MakeGenericType(type);
                var handlers = assembly.GetTypes().Where(x => consumerType.IsAssignableFrom(x));
                foreach (var handler in handlers)
                {
                    services.Add(new ServiceDescriptor(consumerType, handler, ServiceLifetime.Transient));
                }
            }

            return services;
        }

        private IServiceCollection RegistrateRequest(Assembly assembly)
        {
            var handlers = FindTypesImplementingGenericInterface(assembly, typeof(IRequestHandler<>));

            foreach (var handler in handlers)
            {
                var i = handler.GetInterfaces().First(x => x.GetGenericTypeDefinition() == typeof(IRequestHandler<>));
                var type = i.GenericTypeArguments[0];

                Type consumerType = typeof(IRequestHandler<>);
                consumerType = consumerType.MakeGenericType([type]);

                services.Add(new ServiceDescriptor(
                    consumerType,
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
                var i = handler.GetInterfaces().First(x => x.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));
                var type = i.GenericTypeArguments[0];
                var tresponse = i.GenericTypeArguments[1];

                Type consumerType = typeof(IRequestHandler<,>);
                consumerType = consumerType.MakeGenericType([type, tresponse]);

                services.Add(new ServiceDescriptor(
                    consumerType,
                    handler,
                    ServiceLifetime.Transient
                ));
            }

            services.AddSingleton<IMediator, Mediator>();

            return services;
        }
    }

    private static Type[] FindTypesImplementingGenericInterface(
        Assembly assembly,
        Type genericInterfaceDefinition)
    {
        if (!genericInterfaceDefinition.IsGenericTypeDefinition)
            throw new ArgumentException("typeof(T<>)", nameof(genericInterfaceDefinition));

        return assembly.GetTypes()
            .Where(t => !t.IsInterface && !t.IsAbstract)
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType &&
                          i.GetGenericTypeDefinition() == genericInterfaceDefinition))
            .ToArray();
    }
}
