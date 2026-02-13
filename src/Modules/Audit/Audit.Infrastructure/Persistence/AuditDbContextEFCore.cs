using Microsoft.EntityFrameworkCore;
using Audit.Domain.Entities;

namespace Audit.Infrastructure.Persistence;


public class AuditDbContextEFCore : DbContext
{
    public AuditDbContextEFCore(DbContextOptions<AuditDbContextEFCore> options)
        : base(options)
    {
    }

    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new AuditLogConfigurationEFCore());
    }
}
