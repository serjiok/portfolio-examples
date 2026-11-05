using WebApiApplication.Models;
using WebApiTest.Extensions;

namespace WebApiTest.Controllers;

[TestClass]
public sealed class LoginControllerTest
{
    IServiceProvider serviceProvider = Helper.GetServiceProvider();

    [TestMethod]
    public async Task Test1_Login()
    {
        var client = Helper.GetHttpClient(serviceProvider);
        var sToken = await client.PostContentAsync<string, LoginAndPassword>(
            "/Login",
            new LoginAndPassword() { Login = "string", Password = "string" }, CancellationToken.None);
        Assert.IsNotNull(sToken);
    }

    [TestMethod]
    public async Task Test2_Login_Cookie()
    {
        var client = Helper.GetHttpClientCookie(serviceProvider);
        var sToken = await client.PostContentAsync<string, LoginAndPassword>(
            "/LoginCookie",
            new LoginAndPassword() { Login = "string", Password = "string" }, CancellationToken.None);
        Assert.IsNotNull(sToken);

        var users = await client.PostResponseAsync<User[], Filter>("/Users/Get", new Filter() { }, CancellationToken.None);
    }
}
