namespace Inmo24.Application.ResponseDto.Common;

public class CatalogosResponseDto
{
    public List<ItemCatalogoDto> Zonas { get; set; } = new();
    public List<ItemCatalogoDto> TiposPropiedad { get; set; } = new();
    public List<ItemCatalogoDto> Operaciones { get; set; } = new();
    public List<ItemCatalogoDto> FasesCarga { get; set; } = new();
    public List<ItemCatalogoDto> EstadosComerciales { get; set; } = new();
}
