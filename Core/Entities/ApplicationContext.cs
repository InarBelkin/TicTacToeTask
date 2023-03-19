using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartEnum.EFCore;

namespace Core.Entities;

public class ApplicationContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Game> Games => Set<Game>();
    public DbSet<GameRound> GameRounds => Set<GameRound>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ConfigureSmartEnum();
    }
}