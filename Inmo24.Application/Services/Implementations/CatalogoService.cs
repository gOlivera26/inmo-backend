namespace Inmo24.Application.Services.Implementations;

public class CatalogoService : BaseService, ICatalogoService
{
    public CatalogoService(
        InmobiliariaContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IMemoryCache cache)
        : base(context, mapper, httpContextAccessor, cache)
    {
    }

    public async Task<OperationResponse<CatalogosResponseDto>> ObtenerTodosAsync()
    {
        var response = new CatalogosResponseDto
        {
            Zonas = await _context.Set<Zona>()
                .Where(z => z.Activa)
                .Select(z => new ItemCatalogoDto { Id = z.Id, Nombre = z.Nombre })
                .OrderBy(z => z.Nombre)
                .ToListAsync(),

            TiposPropiedad = await _context.Set<TiposPropiedad>()
                .Select(t => new ItemCatalogoDto { Id = t.Id, Nombre = t.Nombre }).ToListAsync(),

            Operaciones = await _context.Set<TiposOperacion>()
                .Select(o => new ItemCatalogoDto { Id = o.Id, Nombre = o.Nombre }).ToListAsync(),

            FasesCarga = await _context.Set<FasesCarga>()
                .Select(f => new ItemCatalogoDto { Id = f.Id, Nombre = f.Nombre }).ToListAsync(),

            EstadosComerciales = await _context.Set<EstadosComerciale>()
                .Select(e => new ItemCatalogoDto { Id = e.Id, Nombre = e.Nombre }).ToListAsync()
        };

        return Ok(response);
    }
}