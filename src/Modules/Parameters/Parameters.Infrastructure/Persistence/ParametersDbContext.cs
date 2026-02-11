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

    public DbSet<Para1> Para1 { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new Para1Configuration());
    }
}
