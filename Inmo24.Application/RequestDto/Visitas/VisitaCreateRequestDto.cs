using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inmo24.Application.RequestDto.Visitas;

public class VisitaCreateRequestDto
{
    public Guid? ClienteId { get; set; }
    public Guid? PropiedadId { get; set; }

    public string? NombreCliente { get; set; }
    public string? Telefono { get; set; }
    public string? DireccionCasa { get; set; } // En n8n lo llamabas "id_casa"

    [Required]
    public DateTime DiaVisita { get; set; }

    public short DuracionMinutos { get; set; } = 60;
    public string? Notas { get; set; }
}
