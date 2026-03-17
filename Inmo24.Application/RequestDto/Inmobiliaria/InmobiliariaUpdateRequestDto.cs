namespace Inmo24.Application.RequestDto.Inmobiliaria;

public class InmobiliariaUpdateRequestDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email de contacto es obligatorio")]
    [EmailAddress]
    public string EmailContacto { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;
}
