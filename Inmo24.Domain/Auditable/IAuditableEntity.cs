namespace Inmo24.Domain.Auditable;

public interface IAuditableEntity
{
    string CreadoPor { get; set; }
    DateTime CreadoEl { get; set; }
    string? ModificadoPor { get; set; }
    DateTime? ModificadoEl { get; set; }
}
