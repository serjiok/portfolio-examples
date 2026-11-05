using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using WebApiApplication.Models;
using WebApiTest.Extensions;

namespace WebApiTest;

internal class Helper
{
    private static IServiceProvider? serviceProvider;

    public enum EnAuth { JWT, Cookie, None };

    public static IServiceProvider GetServiceProvider()
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

            #region Cookie
            serviceCollection.AddSingleton<CookieContainer>();
            serviceCollection.AddHttpClient("cookie").ConfigurePrimaryHttpMessageHandler(sp => new HttpClientHandler
                {
                    UseCookies = true,
                    CookieContainer = sp.GetRequiredService<CookieContainer>(),
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
                });
            #endregion
            serviceProvider = serviceCollection.BuildServiceProvider();
        }
        return serviceProvider;
    }

    public static HttpClient GetHttpClient(IServiceProvider serviceProvider, string? securityToken = null)
    {
        var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("test");
        httpClient.BaseAddress = new Uri("https://127.0.0.1:8081");
        if (!string.IsNullOrEmpty(securityToken))
        {
            httpClient.AuthorizationBearer(securityToken);
        }
        return httpClient;
    }

    public static HttpClient GetHttpClientCookie(IServiceProvider serviceProvider)
    {
        var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("cookie");
        httpClient.BaseAddress = new Uri("https://127.0.0.1:8081");
        return httpClient;
    }

    public static async Task<HttpClient> Login(string login = "string", string password = "string", EnAuth auth = EnAuth.None)
    {
        if (serviceProvider == null) serviceProvider = GetServiceProvider();
        HttpClient? client = null;
        switch (auth)
        {
            case EnAuth.None:
                {
                    client = Helper.GetHttpClient(serviceProvider);
                }
                break;
            case EnAuth.JWT:
                {
                    client = Helper.GetHttpClient(serviceProvider);
                    var sToken = await client.PostContentAsync<string, LoginAndPassword>(
                    "/Login",
                    new LoginAndPassword() { Login = login, Password = password }, CancellationToken.None) ?? throw new NullReferenceException();
                    client.AuthorizationBearer(sToken);
                }
                break;
            case EnAuth.Cookie:
                {
                    client = Helper.GetHttpClientCookie(serviceProvider);
                    _ = await client.PostContentAsync<string, LoginAndPassword>(
                        "/LoginCookie",
                        new LoginAndPassword() { Login = login, Password = password }, CancellationToken.None);
                }
                break;
        }

        return client!;
    }
}
