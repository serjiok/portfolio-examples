using Microsoft.Extensions.DependencyInjection;
using PatternsExample.Infrastructure;
using PatternsExample.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using PatternsExample.Tests;

namespace PatternsExample.Application.Extensions;

internal static class Extensions
{
    public static IServiceCollection UseDBSetting(this IServiceCollection serviceCollections)
    {
        serviceCollections.AddDbContext<ContextDB>((s, o) =>
        {
            o.UseSqlite($"Data Source={Path.Combine(AppContext.BaseDirectory, "database.db")}");
        }, ServiceLifetime.Transient);
        serviceCollections.AddTransient<IWriteDB<int, MessageDB>, MessagesRepository>();
        serviceCollections.AddTransient<IReadDB<int, MessageDB>, MessagesRepository>();
        return serviceCollections;
    }

    public static IServiceCollection UseTestings(this IServiceCollection serviceCollections)
    {
        serviceCollections.AddTransient<TestWriter>();
        serviceCollections.AddTransient<TestReader>();
        return serviceCollections;
    }
}