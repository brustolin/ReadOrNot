using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ReadOrNot.Infrastructure.Persistence;

namespace ReadOrNot.Migrations.SqlServer;

public sealed class SqlServerReadOrNotDbContextFactory : IDesignTimeDbContextFactory<ReadOrNotDbContext>
{
    public ReadOrNotDbContext CreateDbContext(string[] args)
    {
        var connectionString = ResolveArgument(args, "connection")
            ?? "Server=localhost;Database=ReadOrNot;User Id=sa;Password=password;TrustServerCertificate=True";

        var optionsBuilder = new DbContextOptionsBuilder<ReadOrNotDbContext>();
        optionsBuilder.UseSqlServer(
            connectionString,
            options => options.MigrationsAssembly(typeof(SqlServerReadOrNotDbContextFactory).Assembly.FullName));

        return new ReadOrNotDbContext(optionsBuilder.Options);
    }

    private static string? ResolveArgument(string[] args, string key)
    {
        return args
            .Select(argument => argument.Split('=', 2, StringSplitOptions.TrimEntries))
            .Where(parts => parts.Length == 2)
            .FirstOrDefault(parts => parts[0].Equals(key, StringComparison.OrdinalIgnoreCase))?[1];
    }
}
