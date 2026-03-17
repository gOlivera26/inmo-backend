namespace Inmo24.Application.RequestDto.Usuarios;

public class UsuarioUpdatePerfilRequestDto
{
    [Required]
    public string Nombre { get; set; } = string.Empty;
    [Required]
    public string Apellido { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
}
