using Inmo24.Application.RequestDto.Zonas;
using Inmo24.Application.ResponseDto.Zonas;

namespace Inmo24.Application.Services.Implementations;

public class ZonaService : BaseService, IZonaService
{
    public ZonaService(
        InmobiliariaContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IMemoryCache cache)
        : base(context, mapper, httpContextAccessor, cache)
    {
    }

    public async Task<OperationResponse<List<ZonaResponseDto>>> ObtenerZonasAsync()
    {
        var tenantId = GetCurrentTenantId();

        var zonas = await _context.Set<Zona>()
            .Where(z => z.TenantId == tenantId && !z.IsDeleted)
            .OrderBy(z => z.Nombre)
            .Select(z => new ZonaResponseDto
            {
                Id = z.Id,
                Nombre = z.Nombre,
                Descripcion = z.Descripcion ?? "",
                Activa = z.Activa
            })
            .ToListAsync();

        return Ok(zonas);
    }

    public async Task<OperationResponse<ZonaResponseDto>> CrearZonaAsync(ZonaCreateRequestDto request)
    {
        var tenantId = GetCurrentTenantId();

        // Validamos que no exista otra zona con el mismo nombre para este tenant
        var existe = await _context.Set<Zona>()
            .AnyAsync(z => z.TenantId == tenantId && !z.IsDeleted && z.Nombre.ToLower() == request.Nombre.ToLower());

        if (existe) return BadRequest<ZonaResponseDto>("Ya existe una zona con ese nombre.");

        var nuevaZona = new Zona
        {
            TenantId = tenantId,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Activa = true
        };

        PrepareAuditableEntity(nuevaZona, isNew: true);
        _context.Set<Zona>().Add(nuevaZona);
        await _context.SaveChangesAsync();

        return Ok(new ZonaResponseDto
        {
            Id = nuevaZona.Id,
            Nombre = nuevaZona.Nombre,
            Descripcion = nuevaZona.Descripcion ?? "",
            Activa = nuevaZona.Activa
        });
    }

    public async Task<OperationResponse<ZonaResponseDto>> ActualizarZonaAsync(int id, ZonaUpdateRequestDto request)
    {
        var tenantId = GetCurrentTenantId();
        var zona = await _context.Set<Zona>().FirstOrDefaultAsync(z => z.Id == id && z.TenantId == tenantId);

        if (zona == null) return NotFound<ZonaResponseDto>();

        // Validar nombre duplicado (excluyendo la zona actual)
        var existe = await _context.Set<Zona>()
            .AnyAsync(z => z.TenantId == tenantId && !z.IsDeleted && z.Id != id && z.Nombre.ToLower() == request.Nombre.ToLower());

        if (existe) return BadRequest<ZonaResponseDto>("Ya existe otra zona con ese nombre.");

        zona.Nombre = request.Nombre;
        zona.Descripcion = request.Descripcion;
        zona.Activa = request.Activa;

        PrepareAuditableEntity(zona, isNew: false);
        _context.Set<Zona>().Update(zona);
        await _context.SaveChangesAsync();

        return Ok(new ZonaResponseDto
        {
            Id = zona.Id,
            Nombre = zona.Nombre,
            Descripcion = zona.Descripcion ?? "",
            Activa = zona.Activa
        });
    }

    public async Task<OperationResponse<bool>> EliminarZonaAsync(int id)
    {
        var tenantId = GetCurrentTenantId();
        var zona = await _context.Set<Zona>().FirstOrDefaultAsync(z => z.Id == id && z.TenantId == tenantId);

        if (zona == null) return NotFound<bool>();

        // Verificamos si la zona está siendo usada por alguna propiedad
        var enUso = await _context.Set<Propiedades>().AnyAsync(p => p.ZonaId == id && !p.IsDeleted);
        if (enUso) return BadRequest<bool>("No se puede eliminar la zona porque hay propiedades vinculadas a ella. Puedes desactivarla en su lugar.");

        PrepareAuditableEntity(zona, isNew: false, isDeleted: true);
        _context.Set<Zona>().Update(zona);
        await _context.SaveChangesAsync();

        return Ok(true);
    }
}