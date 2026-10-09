using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using WebApiApplication.Interfaces;
using WebApiApplication.Services;
using WebApiDatabase;
using WebApiDatabase.Interfaces;
using WebApiDatabase.Models;
using WebApiDatabase.Repositories;
using System.Reflection;

namespace WebApiApplication.Configuration;

public static class Settings
{
    extension(IServiceCollection services)
    {
        public IServiceCollection UseSQLiteDBSetting()
        {
            services.AddDbContext<ContextDB>((s, o) =>
                {
                    o.UseSqlite($"Data Source={Path.Combine(AppContext.BaseDirectory, "database.db")}");
                }, ServiceLifetime.Transient);
            services.AddTransient<IWrite<int, User>, UserRepository>();
            services.AddTransient<IRead <int, User>, UserRepository>();
            services.AddTransient<IEntityService<int, User>, UserService>();
            return services;
        }

        public IServiceCollection AddMediatr()
            => services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }
}
