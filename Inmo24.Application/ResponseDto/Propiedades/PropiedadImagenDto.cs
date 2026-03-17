namespace Inmo24.Application.ResponseDto.Propiedades;

public class PropiedadImagenDto
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool EsPrincipal { get; set; }
    public short Orden { get; set; }
}
