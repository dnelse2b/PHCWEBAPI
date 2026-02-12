using Microsoft.EntityFrameworkCore;
using Parameters.Domain.Entities;

namespace Parameters.Infrastructure.Persistence;

/// <summary>
/// DbContext EF Core para o módulo de Parâmetros
/// </summary>
public class ParametersDbContextEFCore : DbContext
{
    public ParametersDbContextEFCore(DbContextOptions<ParametersDbContextEFCore> options)
        : base(options)
    {
    }

    public DbSet<Para1> Para1 { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new Para1ConfigurationEFCore());
    }
}
