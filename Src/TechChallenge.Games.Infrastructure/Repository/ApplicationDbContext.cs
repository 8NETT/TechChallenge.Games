using TechChallenge.Games.Core.Entity;
using Microsoft.EntityFrameworkCore;

namespace TechChallenge.Games.Infrastructure.Repository;

public class ApplicationDbContext : DbContext
{
    private string? _connectionString;

    public ApplicationDbContext() : base() { }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }    
    
    public ApplicationDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<Jogo> Jogo { get; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = "Server=localhost;Database=gamesDb;User Id=sa;Password=Lagavi30!;TrustServerCertificate=True;";
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationException).Assembly);
}