using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using NLog.Extensions.Logging;
using System.Text;

namespace WebApi.Configurations;

internal static class Settings
{
    extension (IServiceCollection services)
    {
        public IServiceCollection UseSwagger()
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    Description = "Put **_ONLY_** your JWT Bearer token, format: Bearer token",
                });

                options.AddSecurityRequirement((document) => new OpenApiSecurityRequirement()
                {
                    [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document)] = []
                });
            });

            return services;
        }

        public IServiceCollection UseMapper()
        {
            services.AddSingleton(o =>
            {
                var config = new TypeAdapterConfig();
                new ConfigMapster().Register(config);
                return config;
            }).AddScoped<IMapper, ServiceMapper>();
            return services;
        }

        public IServiceCollection UseJwt(IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration.ISSUER,
                    ValidateAudience = true,
                    ValidAudience = configuration.AUDIENCE,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.SECRETKEY)),
                    ValidateIssuerSigningKey = true,
                };
            });
            return services.AddAuthorization();
        }

        public IServiceCollection UseNlog()
        {
            return services.AddLogging(o =>
            {
                o.ClearProviders();
                o.AddNLog();
            });
        }
    }

    extension (WebApplicationBuilder builder)
    {
        public void Configuration()
        {
            builder.Configuration.SetBasePath(AppContext.BaseDirectory);
            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            builder.Configuration.AddEnvironmentVariables();

            builder.Services.AddControllers();
        }
    }

    extension (WebApplication app)
    {
        public WebApplication UseSwaggers()
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            return app;
        }

        public WebApplication UseExceptionHandler()
        {
            app.UseExceptionHandler("/ErrorHandling/ProcessError");
            return app;
        }

        public WebApplication UseMetrics()
        {
            app.Map("/metrics", metricsApp =>
            {
                //prometeus
                //app.UseMetricServer();
                //app.UseHttpMetrics();
            });

            return app;
        }

        public WebApplication MapHttp()
        {
            app.UseHttpsRedirection();
            app.MapControllers();

            return app;
        }
    }
}
