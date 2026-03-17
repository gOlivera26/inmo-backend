namespace Inmo24.Application.ResponseDto.Documentos;

public class DocumentoResponseDto
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public Guid? EntidadReferenciaId { get; set; }
    public string TipoEntidad { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string NombreArchivo { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public long TamanioBytes { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime CreadoEl { get; set; }
}
