using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReadOrNot.Domain.Entities;
using ReadOrNot.Infrastructure.Identity;

namespace ReadOrNot.Infrastructure.Persistence;

public sealed class ReadOrNotDbContext(DbContextOptions<ReadOrNotDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<TrackingToken> TrackingTokens => Set<TrackingToken>();

    public DbSet<OpenEvent> OpenEvents => Set<OpenEvent>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ReadOrNotDbContext).Assembly);
    }
}
