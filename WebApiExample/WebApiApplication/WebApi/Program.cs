using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NLog;
using WebApi.Configurations;
using WebApiApplication.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration();

builder.Services
    .UseSQLiteDBSetting()
    .UseNlog()
    .UseSwagger()
    .UseMapper()
    .AddMediatr()
    .AddAuthorization()
    .UseJwtAuthorization(builder.Configuration)
    .UseCookieAuthorization()
    .AddAuthentication(options =>
    {
        options.DefaultScheme = "Smart";
        options.DefaultAuthenticateScheme = "Smart";
        options.DefaultChallengeScheme = "Smart";
    })
    .AddPolicyScheme("Smart", "Cookie or JWT", options =>
    {
        options.ForwardDefaultSelector = context =>
        {
            if (context.Request.Headers.ContainsKey("Authorization"))
                return JwtBearerDefaults.AuthenticationScheme;

            return CookieAuthenticationDefaults.AuthenticationScheme;
        };
    });

var app = builder.Build();

app.UseExceptionHandler()
    .UseSwaggers()
    .UseMetrics()
    .MapHttp()
    .UseAuthorization();

LogManager.GetCurrentClassLogger().Info("{info}", builder.Configuration["Kestrel:EndPoints:Https:Url"]);
app.Run();
