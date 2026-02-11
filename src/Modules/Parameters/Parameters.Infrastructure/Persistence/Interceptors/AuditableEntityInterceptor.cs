using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Parameters.Domain.Common;

namespace Parameters.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Interceptor para preencher automaticamente campos de auditoria
/// </summary>
public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void UpdateAuditableEntities(DbContext? context)
    {
        if (context == null) return;

        var now = DateTime.Now;
        var nowDate = now.Date;
        var nowTime = now.ToString("HH:mm:ss");

        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                // Criação - só preenche se ainda não foi preenchido manualmente
                if (entry.Entity.OUsrData == default)
                {
                    entry.Entity.GetType()
                        .GetProperty(nameof(AuditableEntity.OUsrData))!
                        .SetValue(entry.Entity, nowDate);
                }

                if (string.IsNullOrEmpty(entry.Entity.OUsrHora))
                {
                    entry.Entity.GetType()
                        .GetProperty(nameof(AuditableEntity.OUsrHora))!
                        .SetValue(entry.Entity, nowTime);
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                // Atualização
                entry.Entity.GetType()
                    .GetProperty(nameof(AuditableEntity.UsrData))!
                    .SetValue(entry.Entity, nowDate);

                entry.Entity.GetType()
                    .GetProperty(nameof(AuditableEntity.UsrHora))!
                    .SetValue(entry.Entity, nowTime);
            }
        }
    }
}
