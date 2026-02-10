using Microsoft.EntityFrameworkCore;
using Parameters.Domain.Entities;

namespace Parameters.Infrastructure.Persistence;

/// <summary>
/// DbContext para o módulo de Parâmetros
/// </summary>
public class ParametersDbContext : DbContext
{
    public ParametersDbContext(DbContextOptions<ParametersDbContext> options)
        : base(options)
    {
    }

    public DbSet<E1> E1 { get; set; } = null!;
    public DbSet<E4> E4 { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new E1Configuration());
        modelBuilder.ApplyConfiguration(new E4Configuration());
    }
}
