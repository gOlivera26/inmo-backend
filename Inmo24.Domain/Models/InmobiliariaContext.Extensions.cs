using Inmo24.Domain.Auditable;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Inmo24.Domain.Models;

public partial class InmobiliariaContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public Guid CurrentTenantId { get; }

    public InmobiliariaContext(
        DbContextOptions<InmobiliariaContext> options,
        IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;

        var tenantClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("TenantId")?.Value;
        if (Guid.TryParse(tenantClaim, out Guid tenantId))
        {
            CurrentTenantId = tenantId;
        }
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        ApplyGlobalFilters(modelBuilder);
    }

    private void ApplyGlobalFilters(ModelBuilder modelBuilder)
    {
        // Obtenemos todas las entidades que definimos en el DbContext
        var entityTypes = modelBuilder.Model.GetEntityTypes().Select(e => e.ClrType);

        foreach (var entityType in entityTypes)
        {
            var isAuditable = typeof(IFullAuditableEntity).IsAssignableFrom(entityType);
            var isTenant = typeof(ITenantEntity).IsAssignableFrom(entityType);

            // Si la entidad implementa alguna de las dos interfaces, aplicamos la configuración
            if (isAuditable || isTenant)
            {
                var method = typeof(InmobiliariaContext)
                    .GetMethod(nameof(ConfigureFilters), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.MakeGenericMethod(entityType);

                method?.Invoke(this, new object[] { modelBuilder });
            }
        }
    }

    // Este método genérico permite escribir lambdas normales que EF Core sí puede traducir
    private void ConfigureFilters<TEntity>(ModelBuilder modelBuilder) where TEntity : class
    {
        var isAuditable = typeof(IFullAuditableEntity).IsAssignableFrom(typeof(TEntity));
        var isTenant = typeof(ITenantEntity).IsAssignableFrom(typeof(TEntity));

        // NOTA: EF Core solo permite UN HasQueryFilter por entidad, por eso los combinamos acá.
        if (isAuditable && isTenant)
        {
            modelBuilder.Entity<TEntity>().HasQueryFilter(e =>
                !((IFullAuditableEntity)e).IsDeleted &&
                ((ITenantEntity)e).TenantId == CurrentTenantId);
        }
        else if (isAuditable)
        {
            modelBuilder.Entity<TEntity>().HasQueryFilter(e => !((IFullAuditableEntity)e).IsDeleted);
        }
        else if (isTenant)
        {
            modelBuilder.Entity<TEntity>().HasQueryFilter(e => ((ITenantEntity)e).TenantId == CurrentTenantId);
        }
    }
}