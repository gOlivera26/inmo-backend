using Inmo24.Application.RequestDto.Visitas;
using Inmo24.Application.ResponseDto.Visitas;

namespace Inmo24.Application.Services.Implementations;

public class VisitaService : BaseService, IVisitaService
{
    public VisitaService(
        InmobiliariaContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IMemoryCache cache)
        : base(context, mapper, httpContextAccessor, cache)
    {
    }

    public async Task<OperationResponse<VisitaResponseDto>> AgendarVisitaAsync(VisitaCreateRequestDto request)
    {
        var tenantId = GetCurrentTenantId();

        // 1. INTELIGENCIA DE MATCHING (Para el Bot)
        if (request.ClienteId == null && !string.IsNullOrEmpty(request.Telefono))
        {
            var clienteDb = await _context.Set<Cliente>()
                .FirstOrDefaultAsync(c => c.Telefono.Contains(request.Telefono));

            if (clienteDb != null)
            {
                request.ClienteId = clienteDb.Id;
                request.NombreCliente = $"{clienteDb.Nombre} {clienteDb.Apellido}".Trim();
            }
        }

        if (request.PropiedadId == null && !string.IsNullOrEmpty(request.DireccionCasa))
        {
            var propDb = await _context.Set<Propiedades>()
                .FirstOrDefaultAsync(p => p.Direccion.ToLower().Contains(request.DireccionCasa.ToLower()) ||
                                          p.Titulo.ToLower().Contains(request.DireccionCasa.ToLower()) ||
                                          p.Codigo.ToLower() == request.DireccionCasa.ToLower());

            if (propDb != null) request.PropiedadId = propDb.Id;
        }

        // 👇 2. NUEVA VALIDACIÓN DE SUPERPOSICIÓN DE HORARIOS 👇
        var inicioPropuesto = request.DiaVisita.ToUniversalTime();
        var finPropuesto = inicioPropuesto.AddMinutes(request.DuracionMinutos);
        var inicioDia = inicioPropuesto.Date; // Buscamos solo en este día para ser eficientes
        var finDia = inicioDia.AddDays(1);

        var visitasDelDia = await _context.Set<Visita>()
            .Where(v => v.TenantId == tenantId &&
                        !v.IsDeleted &&
                        v.Estado != "CANCELADA" &&
                        v.FechaVisita >= inicioDia &&
                        v.FechaVisita < finDia)
            .ToListAsync();

        // Chequeamos si el inicio de la nueva es antes del fin de la existente, y si el fin de la nueva es después del inicio de la existente.
        var superposicion = visitasDelDia.FirstOrDefault(v =>
            inicioPropuesto < v.FechaVisita.AddMinutes(v.DuracionMinutos ?? 60) &&
            finPropuesto > v.FechaVisita);

        if (superposicion != null)
        {
            // Pasamos la fecha a la zona horaria de Argentina (UTC-3) para el mensaje de error
            var horaLocal = superposicion.FechaVisita.AddHours(-3);
            return BadRequest<VisitaResponseDto>($"Ya tienes una visita agendada en ese horario (a las {horaLocal:HH:mm} hs). Por favor, elige otro horario.");
        }
        // 👆 FIN NUEVA VALIDACIÓN 👆

        // 3. CREACIÓN DEL REGISTRO
        var nuevaVisita = new Visita
        {
            TenantId = tenantId,
            ClienteId = request.ClienteId,
            PropiedadId = request.PropiedadId,
            NombreCliente = request.NombreCliente ?? "Cliente Desconocido",
            TelefonoCliente = request.Telefono ?? "Sin Teléfono",
            DireccionCasa = request.DireccionCasa ?? "Propiedad no especificada",
            FechaVisita = inicioPropuesto,
            DuracionMinutos = request.DuracionMinutos,
            Estado = "AGENDADA",
            Notas = request.Notas
        };

        PrepareAuditableEntity(nuevaVisita, isNew: true);
        _context.Set<Visita>().Add(nuevaVisita);
        await _context.SaveChangesAsync();

        return await ObtenerVisitaPorIdAsync(nuevaVisita.Id);
    }

    public async Task<OperationResponse<List<VisitaResponseDto>>> ObtenerVisitasMesAsync(int anio, int mes)
    {
        var inicioMes = new DateTime(anio, mes, 1, 0, 0, 0, DateTimeKind.Utc);
        var finMes = inicioMes.AddMonths(1).AddTicks(-1);

        var visitasDb = await _context.Set<Visita>()
            .Include(v => v.Cliente)
            .Include(v => v.Propiedad)
                .ThenInclude(p => p.PropiedadImagenes)
            .Where(v => !v.IsDeleted && v.FechaVisita >= inicioMes && v.FechaVisita <= finMes)
            .OrderBy(v => v.FechaVisita)
            .ToListAsync();

        var dtos = visitasDb.Select(MapearADto).ToList();
        return Ok(dtos);
    }

    public async Task<OperationResponse<bool>> CambiarEstadoVisitaAsync(Guid id, string nuevoEstado)
    {
        var visita = await _context.Set<Visita>().FirstOrDefaultAsync(v => v.Id == id);
        if (visita == null) return NotFound<bool>();

        visita.Estado = nuevoEstado.ToUpper();

        PrepareAuditableEntity(visita, isNew: false);
        _context.Set<Visita>().Update(visita);
        await _context.SaveChangesAsync();

        return Ok(true);
    }

    public async Task<OperationResponse<bool>> CancelarVisitaBotAsync(VisitaCancelRequestDto request)
    {
        var tenantId = GetCurrentTenantId();
        var fechaBusqueda = request.DiaVisita.ToUniversalTime();

        // Buscamos una visita activa (que no esté ya cancelada ni borrada) 
        // que coincida con el teléfono y la fecha exacta.
        var visita = await _context.Set<Visita>()
            .FirstOrDefaultAsync(v =>
                v.TenantId == tenantId &&
                !v.IsDeleted &&
                v.Estado != "CANCELADA" &&
                v.TelefonoCliente.Contains(request.Telefono) &&
                v.FechaVisita == fechaBusqueda);

        if (visita == null)
        {
            return NotFound<bool>();
        }

        // Si la encontramos, la cancelamos
        visita.Estado = "CANCELADA";
        visita.Notas = string.IsNullOrEmpty(visita.Notas)
            ? "Cancelada por el cliente vía WhatsApp."
            : visita.Notas + "\n[Cancelada por el cliente vía WhatsApp]";

        PrepareAuditableEntity(visita, isNew: false);
        _context.Set<Visita>().Update(visita);
        await _context.SaveChangesAsync();

        return Ok(true);
    }

    private async Task<OperationResponse<VisitaResponseDto>> ObtenerVisitaPorIdAsync(Guid id)
    {
        var visita = await _context.Set<Visita>()
            .Include(v => v.Cliente)
            .Include(v => v.Propiedad)
                .ThenInclude(p => p.PropiedadImagenes)
            .FirstOrDefaultAsync(v => v.Id == id);

        return Ok(MapearADto(visita!));
    }

    private VisitaResponseDto MapearADto(Visita v)
    {
        return new VisitaResponseDto
        {
            Id = v.Id,
            FechaVisita = v.FechaVisita,
            Estado = v.Estado,
            DuracionMinutos = v.DuracionMinutos ?? 60,

            ClienteId = v.ClienteId,
            NombreCliente = v.Cliente != null ? $"{v.Cliente.Nombre} {v.Cliente.Apellido}".Trim() : v.NombreCliente,
            TelefonoCliente = v.Cliente != null ? v.Cliente.Telefono : v.TelefonoCliente,

            PropiedadId = v.PropiedadId,
            PropiedadTitulo = v.Propiedad != null ? (v.Propiedad.Titulo ?? v.Propiedad.Direccion) : v.DireccionCasa,
            DireccionCasa = v.Propiedad != null ? v.Propiedad.Direccion : v.DireccionCasa,
            PropiedadImagenUrl = v.Propiedad?.PropiedadImagenes.FirstOrDefault(i => i.EsPrincipal && !i.IsDeleted)?.Url
                                 ?? v.Propiedad?.PropiedadImagenes.FirstOrDefault(i => !i.IsDeleted)?.Url
                                 ?? ""
        };
    }
}