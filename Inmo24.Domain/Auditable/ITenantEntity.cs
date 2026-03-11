namespace Inmo24.Domain.Auditable;

public interface ITenantEntity
{
    Guid TenantId { get; set; }
}