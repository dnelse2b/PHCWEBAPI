using Microsoft.EntityFrameworkCore;
using Providers.Domain.Entities;

namespace Providers.Infrastructure.Persistence;

/// <summary>
/// DbContext EF Core para o módulo de Providers
/// </summary>
public class ProvidersDbContextEFCore : DbContext
{
    public ProvidersDbContextEFCore(DbContextOptions<ProvidersDbContextEFCore> options)
        : base(options)
    {
    }

    public DbSet<Provider> Providers { get; set; } = null!;
    public DbSet<ProviderValue> ProviderValues { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new ProviderConfigurationEFCore());
        modelBuilder.ApplyConfiguration(new ProviderValueConfigurationEFCore());
    }
}
