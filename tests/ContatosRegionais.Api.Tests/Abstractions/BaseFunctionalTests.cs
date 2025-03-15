using ContatosRegionais.Application.DTO;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ContatosRegionais.Api.Tests.Abstractions;

public class BaseFunctionalTests : IClassFixture<FunctionalTestWebAppFactory>
{
    protected HttpClient HttpClient { get; init; }

    public BaseFunctionalTests(FunctionalTestWebAppFactory webAppFactory)
    {
        HttpClient = webAppFactory.CreateClient();
        Login().GetAwaiter().GetResult();
    }

    public async Task Login()
    {
        var usuario = new UserDto { Username = "admin", Password = "admin@123" };
        var responseToken = await HttpClient.PostAsJsonAsync($"api/token", usuario); // Retorna token JWT Bearer
        responseToken.EnsureSuccessStatusCode();

        var tokenContent = await responseToken.Content.ReadAsStringAsync();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenContent);
    }
}