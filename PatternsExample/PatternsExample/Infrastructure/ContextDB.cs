using Microsoft.EntityFrameworkCore;

namespace PatternsExample.Infrastructure;

internal sealed class ContextDB : DbContext
{
    public DbSet<MessageDB> Messages { get; init; }

    public ContextDB(DbContextOptions<ContextDB> options):base(options)
    {
        Database.EnsureCreated();
    }
}
