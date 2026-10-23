using Microsoft.Extensions.DependencyInjection;
using MyMediatr.Interfaces;
using System.Reflection;

namespace MyMediatr.Extensions;

public static class MyMediatrExtensions
{
    private static readonly TypeFilter typeFilter = static (type, criteria) => type.IsGenericType && type.GetGenericTypeDefinition() == (Type)criteria!;

    extension (IServiceCollection services)
    {
        public IServiceCollection AddMyMediatr(Action<MyMediatrServiceConfiguration> action)
        {
            var configuration = new MyMediatrServiceConfiguration();
            action.Invoke(configuration);

            if (!configuration.Assemblies.Any()) configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            services.RegistrateNotification(configuration)
                    .RegistrateRequest(configuration)
                    .RegistrateRequestResponse(configuration)
                    .AddSingleton(configuration)
                    .AddSingleton<IMediator, Mediator>();

            return services;
        }

        private IServiceCollection RegistrateNotification(MyMediatrServiceConfiguration configuration)
        {
            var notifications = FindTypesImplementingGenericInterface(configuration.Assemblies, typeof(INotificationHandler<>));

            foreach (var notification in notifications)
            {
                Type typeRequest = notification.FindInterfaces(typeFilter, typeof(INotificationHandler<>)).First().GenericTypeArguments[0];
                Type handlerType = typeof(INotificationHandler<>).MakeGenericType(typeRequest);
                
                var handlers = configuration.Assemblies.SelectMany(a => a.GetTypes()).Where(x => handlerType.IsAssignableFrom(x));
                foreach (var handler in handlers)
                {
                    services.Add(new ServiceDescriptor(handlerType, handler, configuration.LifeTime));
                }
            }

            return services;
        }

        private IServiceCollection RegistrateRequest(MyMediatrServiceConfiguration configuration)
        {
            var handlers = FindTypesImplementingGenericInterface(configuration.Assemblies, typeof(IRequestHandler<>));

            foreach (var handler in handlers)
            {
                Type typeRequest = handler.FindInterfaces(typeFilter, typeof(IRequestHandler<>)).First().GenericTypeArguments[0];
                Type handlerType = typeof(IRequestHandler<>).MakeGenericType([typeRequest]);

                services.Add(new ServiceDescriptor(handlerType, handler, configuration.LifeTime));
            }

            services.AddSingleton<IMediator, Mediator>();

            return services;
        }

        private IServiceCollection RegistrateRequestResponse(MyMediatrServiceConfiguration configuration)
        {
            var handlers = FindTypesImplementingGenericInterface(configuration.Assemblies, typeof(IRequestHandler<,>));

            foreach (var handler in handlers)
            {
                Type types = handler.FindInterfaces(typeFilter, typeof(IRequestHandler<,>)).First();
                Type typeRequest = types.GenericTypeArguments[0];
                Type typeResponse = types.GenericTypeArguments[1];
                Type handlerType = typeof(IRequestHandler<,>).MakeGenericType([typeRequest, typeResponse]);

                services.Add(new ServiceDescriptor(handlerType, handler, configuration.LifeTime));
            }

            services.AddSingleton<IMediator, Mediator>();

            return services;
        }
    }

    private static IEnumerable<Type> FindTypesImplementingGenericInterface(
        IEnumerable<Assembly> assembly,
        Type genericInterfaceDefinition)
    {
        if (!genericInterfaceDefinition.IsGenericTypeDefinition)
            throw new ArgumentException("typeof(T<>)", nameof(genericInterfaceDefinition));

        return assembly.SelectMany(x=>x.GetTypes())
            .Where(t => !t.IsInterface && !t.IsAbstract)
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType &&
                          i.GetGenericTypeDefinition() == genericInterfaceDefinition));
    }
}
