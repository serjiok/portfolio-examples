using NLog;
using WebApi.Configurations;
using WebApiApplication.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration();

builder.Services
    .UseSQLiteDBSetting()
    .UseNlog()
    .UseSwagger()
    .UseJwt(builder.Configuration)
    .UseMapper()
    .AddMediatr();

var app = builder.Build();

app.UseExceptionHandler()
    .UseSwaggers()
    .UseMetrics()
    .MapHttp()
    .UseAuthorization();

LogManager.GetCurrentClassLogger().Info("{info}", builder.Configuration["Kestrel:EndPoints:Https:Url"]);
app.Run();
