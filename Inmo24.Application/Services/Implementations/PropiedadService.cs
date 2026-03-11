using Inmo24.Application.RequestDto.Propiedades;
using Inmo24.Application.ResponseDto.Common;
using Inmo24.Application.ResponseDto.Propiedades;

namespace Inmo24.Application.Services.Implementations;

public class PropiedadesService : BaseService, IPropiedadService
{
    public PropiedadesService(
        InmobiliariaContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IMemoryCache cache)
        : base(context, mapper, httpContextAccessor, cache)
    {
    }

    public async Task<OperationResponse<Propiedades>> CrearBorradorAsync(PropiedadCreateRequestDto request)
    {
        var nuevaPropiedad = _mapper.Map<Propiedades>(request);

        nuevaPropiedad.FaseCargaId = 1;
        nuevaPropiedad.EstadoComercialId = 1;

        return await InsertAsync<Propiedades, Propiedades>(nuevaPropiedad, _context);
    }

    public async Task<OperationResponse<List<PropiedadBackofficeDto>>> ObtenerMisPropiedadesAsync(int page, int pageSize)
    {
        var query = _context.Set<Propiedades>()
            .Include(p => p.Zona)
            .Include(p => p.Tipo)
            .Include(p => p.Operacion)
            .Include(p => p.FaseCarga)
            .Include(p => p.EstadoComercial)
            .OrderByDescending(p => p.CreadoEl);

        return await GetPagedDataAsync<Propiedades, PropiedadBackofficeDto>(page, pageSize, query);
    }

    public async Task<OperationResponse<List<PropiedadPublicaDto>>> ObtenerCatalogoPublicoAsync(int page, int pageSize)
    {
        var query = _context.Set<Propiedades>()
            .IgnoreQueryFilters()
            .Include(p => p.Zona)
            .Include(p => p.Tenant)
            .Include(p => p.Tipo) 
            .Include(p => p.Operacion)
            .Where(p => !p.IsDeleted
                     && p.FaseCargaId == 3 // 3 = 'publicada'
                     && p.EstadoComercialId == 1 // 1 = 'disponible'
                     && p.Tenant.Activo
                     && !p.Tenant.IsDeleted)
            .OrderByDescending(p => p.Destacada)
            .ThenByDescending(p => p.CreadoEl);

        return await GetPagedDataAsync<Propiedades, PropiedadPublicaDto>(page, pageSize, query);
    }
}