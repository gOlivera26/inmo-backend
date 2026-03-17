namespace Inmo24.Application.ResponseDto.Auth;

public class UsuarioResponse
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public string TenantNombre { get; set; } = string.Empty;
    public string TenantPlan { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}