using System.Net.Http.Json;
using WebApiApplication.Models;
using WebApiTest.Extensions;

namespace WebApiTest.Controllers;

[TestClass]
public sealed class UsersControllerTest
{
    IServiceProvider serviceProvider = Helper.GetServiceProviderServer();

    [TestMethod]
    public async Task Test2_Add()
    {
        var client = await Login();
        var result = await AddUser(client, new UserWithPassword() { Id = 1_000, Login = "test", Password = "test" }, CancellationToken.None);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task Test3_Get()
    {
        var client = await Login();
        var users = await GetUsers(client, new Filter(), CancellationToken.None);
        Assert.IsNotNull(users.Length > 0);
    }

    [TestMethod]
    public async Task Test4_Search()
    {
        var client = await Login();
        var users = await GetUsers(client, new Filter() { Search = "te"}, CancellationToken.None);
        Assert.IsNotNull(users.Length > 0);
    }

    [TestMethod]
    public async Task Test5_Update()
    {
        var client = await Login();
        var user = await GetUser(client, 1_000, CancellationToken.None);
        var result = await UpdateUser(client, new UserWithPassword() { Id = user.Id, Login = user.Login, Password = "123" }, CancellationToken.None);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task Test6_Delete()
    {
        var client = await Login();
        var user = await GetUser(client, 1_000, CancellationToken.None);
        var result = await DeleteUser(client, user.Id, CancellationToken.None);
        Assert.IsTrue(result);
    }

    private async Task<HttpClient> Login(string login = "string", string password = "string")
    {
        var client = Helper.GetHttpClient(serviceProvider, string.Empty);
        var sToken = await client.PostContentAsync<string, LoginAndPassword>(
            "/Login",
            new LoginAndPassword() { Login = login, Password = password }, CancellationToken.None) ?? throw new NullReferenceException();
        client.AuthorizationBearer(sToken);
        return client;
    }

    private async Task<bool> AddUser(HttpClient httpClient, UserWithPassword user, CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync("/Users/Add", user, cancellationToken);
        return response.StatusCode == System.Net.HttpStatusCode.OK;
    }

    private async Task<bool> UpdateUser(HttpClient httpClient, UserWithPassword user, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("/Users/Update", user, cancellationToken);
        return response.StatusCode == System.Net.HttpStatusCode.OK;
    }

    private async Task<bool> DeleteUser(HttpClient httpClient, int id, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"/Users/Delete?id={id}", cancellationToken);
        return response.StatusCode == System.Net.HttpStatusCode.OK;
    }

    private Task<User[]?> GetUsers(HttpClient httpClient, Filter filter, CancellationToken cancellationToken)
        => httpClient.PostResponseAsync<User[], Filter>("/Users/Get", filter, cancellationToken);

    private Task<User?> GetUser(HttpClient httpClient, int id, CancellationToken cancellationToken)
        => httpClient.GetResponseAsync<User>($"/Users/GetById?id={id}", cancellationToken);
}
