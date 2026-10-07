using Microsoft.Extensions.DependencyInjection;
using PatternsExample.Application.Extensions;
using PatternsExample.Application.Factories;

namespace PatternsExample.Application;

internal static class Configurate
{
    private static IServiceProvider? serviceProvider;

    public static IServiceProvider Config()
    {
        serviceProvider ??= new ServiceCollection()
                .UseDBSetting()
                .UseTestings()
                .AddTransient<Fabric>()
                .BuildServiceProvider();
        return serviceProvider;
    }
}
