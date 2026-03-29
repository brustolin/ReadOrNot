using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ReadOrNot.Application.Options;

namespace ReadOrNot.Infrastructure.Persistence;

public sealed class ReadOrNotDbContextFactory : IDesignTimeDbContextFactory<ReadOrNotDbContext>
{
    public ReadOrNotDbContext CreateDbContext(string[] args)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var databaseOptions = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>() ?? new DatabaseOptions();
        var builder = new DbContextOptionsBuilder<ReadOrNotDbContext>();
        var connectionString = configuration.GetConnectionString(databaseOptions.ConnectionStringName)
            ?? "Server=(localdb)\\mssqllocaldb;Database=ReadOrNotDesignTime;Trusted_Connection=True;TrustServerCertificate=True";

        ConfigureProvider(builder, databaseOptions, connectionString, typeof(ReadOrNotDbContext).Assembly.FullName!);

        return new ReadOrNotDbContext(builder.Options);
    }

    internal static void ConfigureProvider(
        DbContextOptionsBuilder builder,
        DatabaseOptions databaseOptions,
        string connectionString,
        string migrationsAssembly)
    {
        if (databaseOptions.Provider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
        {
            builder.UseMySql(
                connectionString,
                ServerVersion.Parse(databaseOptions.MySqlServerVersion),
                options => options.MigrationsAssembly(migrationsAssembly));

            return;
        }

        if (databaseOptions.Provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
        {
            builder.UseSqlite(connectionString, options => options.MigrationsAssembly(migrationsAssembly));
            return;
        }

        builder.UseSqlServer(connectionString, options => options.MigrationsAssembly(migrationsAssembly));
    }
}
