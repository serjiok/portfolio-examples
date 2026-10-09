using Microsoft.EntityFrameworkCore;
using WebApiDatabase.Models;

namespace WebApiDatabase;

public class ContextDB : DbContext
{
    public DbSet<User> Users { get; init; }

    public ContextDB(DbContextOptions<ContextDB> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(x => new { x.Login }).IsUnique();
    }
}
