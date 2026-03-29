using System.Net.Http.Json;
using ReadOrNot.Application.DTOs.Auth;

namespace ReadOrNot.Api.IntegrationTests.Infrastructure;

internal static class TestAuthHelpers
{
    public static async Task<string> RegisterAndGetTokenAsync(HttpClient client, string emailPrefix)
    {
        var response = await client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest
        {
            DisplayName = $"User {emailPrefix}",
            Email = $"{emailPrefix}@example.test",
            Password = "Passw0rd!Passw0rd!",
            ConfirmPassword = "Passw0rd!Passw0rd!"
        });

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return payload!.AccessToken;
    }
}
