using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ReadOrNot.Infrastructure.Persistence;

namespace ReadOrNot.Migrations.MySql;

public sealed class MySqlReadOrNotDbContextFactory : IDesignTimeDbContextFactory<ReadOrNotDbContext>
{
    public ReadOrNotDbContext CreateDbContext(string[] args)
    {
        var connectionString = ResolveArgument(args, "connection")
            ?? "Server=localhost;Port=3306;Database=ReadOrNot;User=root;Password=password;";
        var serverVersion = ResolveArgument(args, "serverVersion") ?? "8.0.36";

        var optionsBuilder = new DbContextOptionsBuilder<ReadOrNotDbContext>();
        optionsBuilder.UseMySql(
            connectionString,
            ServerVersion.Parse(serverVersion),
            options => options.MigrationsAssembly(typeof(MySqlReadOrNotDbContextFactory).Assembly.FullName));

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
