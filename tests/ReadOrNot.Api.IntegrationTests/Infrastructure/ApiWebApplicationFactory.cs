using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReadOrNot.Infrastructure.Persistence;

namespace ReadOrNot.Api.IntegrationTests.Infrastructure;

public sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly string _databaseName = $"file:readornot-tests-{Guid.NewGuid():N}?mode=memory&cache=shared";
    private readonly SqliteConnection _connection;

    public ApiWebApplicationFactory()
    {
        _connection = new SqliteConnection($"Data Source={_databaseName}");
    }

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();
    }

    public new async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");

        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:Provider"] = "Sqlite",
                ["Database:ConnectionStringName"] = "Sqlite",
                ["ConnectionStrings:Sqlite"] = $"Data Source={_databaseName}",
                ["Jwt:Issuer"] = "ReadOrNot.Tests",
                ["Jwt:Audience"] = "ReadOrNot.Tests.Client",
                ["Jwt:SigningKey"] = "integration-tests-signing-key-with-adequate-length",
                ["Tracking:PublicBaseUrl"] = "https://localhost",
                ["Frontend:BaseUrl"] = "https://localhost",
                ["Privacy:IpStorageMode"] = "Hashed",
                ["Privacy:IpHashKey"] = "integration-tests-ip-hash-key",
                ["Retention:PurgeEnabled"] = "false",
                ["Cors:AllowedOrigins:0"] = "https://localhost"
            });
        });

        builder.ConfigureServices(services =>
        {
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ReadOrNotDbContext>();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        });
    }
}
