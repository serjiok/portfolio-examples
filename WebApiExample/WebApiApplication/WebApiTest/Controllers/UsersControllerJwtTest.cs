using WebApiApplication.Models;
using static WebApiTest.Helper;

namespace WebApiTest.Controllers;

[TestClass]
public sealed class UsersControllerJwtTest
{
    [TestMethod]
    public async Task Test1_Add()
    {
        var client = await Login(auth: EnAuth.None);
        var result = await Requests.AddUser(client, new UserWithPassword() { Id = 1_000, Login = "test", Password = "test" }, CancellationToken.None);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task Test2_Get()
    {
        var client = await Login(auth: EnAuth.JWT);
        var users = await Requests.GetUsers(client, new Filter(), CancellationToken.None);
        Assert.IsNotNull(users.Length > 0);
    }

    [TestMethod]
    public async Task Test3_Search()
    {
        var client = await Login(auth: EnAuth.JWT);
        var users = await Requests.GetUsers(client, new Filter() { Search = "te"}, CancellationToken.None);
        Assert.IsNotNull(users.Length > 0);
    }

    [TestMethod]
    public async Task Test4_Update()
    {
        var client = await Login(auth: EnAuth.JWT);
        var user = await Requests.GetUser(client, 1_000, CancellationToken.None);
        var result = await Requests.UpdateUser(client, new UserWithPassword() { Id = user.Id, Login = user.Login, Password = "123" }, CancellationToken.None);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task Test5_Delete()
    {
        var client = await Login(auth: EnAuth.JWT);
        var user = await Requests.GetUser(client, 1_000, CancellationToken.None);
        var result = await Requests.DeleteUser(client, user.Id, CancellationToken.None);
        Assert.IsTrue(result);
    }
}
