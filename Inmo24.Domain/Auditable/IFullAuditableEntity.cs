namespace Inmo24.Domain.Auditable;

public interface IFullAuditableEntity : IAuditableEntity
{
    bool IsDeleted { get; set; }
    string? EliminadoPor { get; set; }
    DateTime? EliminadoEl { get; set; }
    string? MotivoBaja { get; set; }
}
