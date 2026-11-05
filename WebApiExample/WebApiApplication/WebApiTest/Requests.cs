using System.Net.Http.Json;
using WebApiApplication.Models;
using WebApiTest.Extensions;

namespace WebApiTest;

internal static class Requests
{
    public static async Task<bool> AddUser(HttpClient httpClient, UserWithPassword user, CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync("/Users/Add", user, cancellationToken);
        return response.StatusCode == System.Net.HttpStatusCode.OK;
    }

    public static async Task<bool> UpdateUser(HttpClient httpClient, UserWithPassword user, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("/Users/Update", user, cancellationToken);
        return response.StatusCode == System.Net.HttpStatusCode.OK;
    }

    public static async Task<bool> DeleteUser(HttpClient httpClient, int id, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"/Users/Delete?id={id}", cancellationToken);
        return response.StatusCode == System.Net.HttpStatusCode.OK;
    }

    public static Task<User[]?> GetUsers(HttpClient httpClient, Filter filter, CancellationToken cancellationToken)
        => httpClient.PostResponseAsync<User[], Filter>("/Users/Get", filter, cancellationToken);

    public static Task<User?> GetUser(HttpClient httpClient, int id, CancellationToken cancellationToken)
        => httpClient.GetResponseAsync<User>($"/Users/GetById?id={id}", cancellationToken);
}
