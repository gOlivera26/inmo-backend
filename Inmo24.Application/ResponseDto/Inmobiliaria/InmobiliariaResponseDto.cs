namespace Inmo24.Application.ResponseDto.Inmobiliaria;

public class InmobiliariaResponseDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string EmailContacto { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
}
