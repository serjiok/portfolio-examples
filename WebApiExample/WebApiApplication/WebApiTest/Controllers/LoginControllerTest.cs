using WebApiApplication.Models;
using WebApiTest.Extensions;

namespace WebApiTest.Controllers;

[TestClass]
public sealed class LoginControllerTest
{
    IServiceProvider serviceProvider = Helper.GetServiceProviderServer();

    [TestMethod]
    public async Task Test1_Login()
    {
        var client = Helper.GetHttpClient(serviceProvider, string.Empty);
        var sToken = await client.PostContentAsync<string, LoginAndPassword>(
            "/Login",
            new LoginAndPassword() { Login = "string", Password = "string" }, CancellationToken.None);
        Assert.IsNotNull(sToken);
    }
}
