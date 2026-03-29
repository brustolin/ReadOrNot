using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ReadOrNot.Api.IntegrationTests.Infrastructure;
using ReadOrNot.Application.DTOs.Tokens;

namespace ReadOrNot.Api.IntegrationTests.Scenarios;

public sealed class TrackingEndpointTests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
{
    [Fact]
    public async Task TrackingEndpoint_ReturnsAsset_AndLogsOpenEvent()
    {
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
        var accessToken = await TestAuthHelpers.RegisterAndGetTokenAsync(client, "tracking");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var createResponse = await client.PostAsJsonAsync("/api/v1/tokens", new CreateTrackingTokenRequest
        {
            Name = "Release email"
        });

        createResponse.EnsureSuccessStatusCode();
        var token = await createResponse.Content.ReadFromJsonAsync<TrackingTokenDetailsDto>();

        client.DefaultRequestHeaders.Authorization = null;
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        client.DefaultRequestHeaders.Referrer = new Uri("https://example.test/newsletter");
        client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US");

        var trackingResponse = await client.GetAsync($"/t/{token!.PublicIdentifier}");
        trackingResponse.EnsureSuccessStatusCode();

        var mediaType = trackingResponse.Content.Headers.ContentType!.MediaType;
        Assert.Equal("image/svg+xml", mediaType);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var detailsResponse = await client.GetAsync($"/api/v1/tokens/{token.Id}");
        detailsResponse.EnsureSuccessStatusCode();

        var updatedToken = await detailsResponse.Content.ReadFromJsonAsync<TrackingTokenDetailsDto>();
        Assert.Equal(1, updatedToken!.Report.TotalOpens);
        Assert.Single(updatedToken.Report.Events);
    }
}
