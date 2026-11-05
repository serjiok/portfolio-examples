using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Security.Claims;
using WebApiTest.Extensions;

namespace WebApiTest;

internal class Helper
{
    private static IServiceProvider? serviceProvider;

    public static Mock<IHttpContextAccessor> GetHttpContextAccessor(string id, string login)
    {
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var claims = new List<Claim>()
        {
            new (ClaimTypes.Name, login),
            new (ClaimTypes.NameIdentifier, id)
        };

        ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity(claims));
        httpContextAccessorMock.Setup(h => h.HttpContext.User).Returns(user);
        return httpContextAccessorMock;
    }

    public static IServiceProvider GetServiceProviderServer()
    {
        if (serviceProvider == null)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false).Build() ?? throw new ArgumentNullException("configuration");

            ServiceCollection serviceCollection = new();
            serviceCollection.AddHttpClient("test").ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            });
            serviceCollection.AddTransient<IHttpContextAccessor>(_ => GetHttpContextAccessor("test", "test")!.Object);
            serviceProvider = serviceCollection.BuildServiceProvider();
        }
        return serviceProvider;
    }

    public static HttpClient GetHttpClient(IServiceProvider serviceProvider, string? securityToken)
    {
        var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("test");
        httpClient.BaseAddress = new Uri("https://127.0.0.1:8081");
        if (!string.IsNullOrEmpty(securityToken))
        {
            httpClient.AuthorizationBearer(securityToken);
        }
        return httpClient;
    }
}
