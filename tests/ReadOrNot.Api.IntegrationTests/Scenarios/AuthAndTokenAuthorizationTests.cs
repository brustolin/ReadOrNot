using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ReadOrNot.Api.IntegrationTests.Infrastructure;
using ReadOrNot.Application.DTOs.Tokens;

namespace ReadOrNot.Api.IntegrationTests.Scenarios;

public sealed class AuthAndTokenAuthorizationTests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
{
    [Fact]
    public async Task TokensEndpoint_ReturnsUnauthorized_WithoutBearerToken()
    {
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var response = await client.GetAsync("/api/v1/tokens");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task User_CannotAccessAnotherUsersToken()
    {
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
        var firstToken = await TestAuthHelpers.RegisterAndGetTokenAsync(client, "owner");
        var secondToken = await TestAuthHelpers.RegisterAndGetTokenAsync(client, "other");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", firstToken);
        var createResponse = await client.PostAsJsonAsync("/api/v1/tokens", new CreateTrackingTokenRequest
        {
            Name = "Launch campaign",
            Description = "First wave"
        });

        createResponse.EnsureSuccessStatusCode();
        var createdToken = await createResponse.Content.ReadFromJsonAsync<TrackingTokenDetailsDto>();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secondToken);
        var response = await client.GetAsync($"/api/v1/tokens/{createdToken!.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
