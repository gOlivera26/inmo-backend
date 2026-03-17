namespace Inmo24.Application.RequestDto.Usuarios;

public class UsuarioChangePasswordRequestDto
{
    [Required]
    public string PasswordActual { get; set; } = string.Empty;

    [Required]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    public string PasswordNueva { get; set; } = string.Empty;
}
