namespace Inmo24.Application.Services.Implementations;

public class PropiedadesService : BaseService, IPropiedadService
{
    private readonly IStorageService _storageService;

    public PropiedadesService(
          InmobiliariaContext context,
          IMapper mapper,
          IHttpContextAccessor httpContextAccessor,
          IMemoryCache cache,
          IStorageService storageService)
          : base(context, mapper, httpContextAccessor, cache)
    {
        _storageService = storageService;
    }

    public async Task<OperationResponse<Propiedades>> CrearBorradorAsync(PropiedadCreateRequestDto request)
    {
        var nuevaPropiedad = _mapper.Map<Propiedades>(request);

        var hash = Guid.NewGuid().ToString().Substring(0, 4).ToUpper();
        nuevaPropiedad.Codigo = $"INMO-{hash}";

        nuevaPropiedad.FaseCargaId = 1;
        nuevaPropiedad.EstadoComercialId = 1;

        return await InsertAsync<Propiedades, Propiedades>(nuevaPropiedad, _context);
    }

    public async Task<OperationResponse<List<PropiedadBackofficeDto>>> ObtenerMisPropiedadesAsync(PropiedadBackofficeFilterRequest request)
    {
        var query = _context.Set<Propiedades>()
            .Include(p => p.Zona)
            .Include(p => p.Tipo)
            .Include(p => p.Operacion)
            .Include(p => p.FaseCarga)
            .Include(p => p.EstadoComercial)
            .Include(p => p.PropiedadImagenes)
            .AsQueryable();

        query = AplicarFiltros(query, request);
        query = query.OrderByDescending(p => p.CreadoEl);

        return await GetPagedDataAsync<Propiedades, PropiedadBackofficeDto>(request.Page, request.PageSize, query);
    }

    public async Task<OperationResponse<List<PropiedadPublicaDto>>> ObtenerCatalogoPublicoAsync(PropiedadPublicaFilterRequest request)
    {
        var query = _context.Set<Propiedades>()
            .IgnoreQueryFilters()
            .Include(p => p.Zona)
            .Include(p => p.Tenant)
            .Include(p => p.Tipo)
            .Include(p => p.Operacion)
            .Include(p => p.PropiedadImagenes)
            .Where(p => !p.IsDeleted
                     && p.FaseCargaId == 3 // 3 = 'publicada'
                     && p.EstadoComercialId == 1 // 1 = 'disponible'
                     && p.Tenant.Activo
                     && !p.Tenant.IsDeleted);

        if (request.TenantId.HasValue)
        {
            query = query.Where(p => p.TenantId == request.TenantId.Value);
        }

        // Filtro: Operación (Venta, Alquiler, etc.)
        if (!string.IsNullOrWhiteSpace(request.Operacion))
        {
            var opSearch = request.Operacion.ToLower().Trim();
            query = query.Where(p => p.Operacion.Nombre.ToLower().Contains(opSearch));
        }

        // Filtro: Tipo de Propiedad (Casa, Depto, etc.)
        if (!string.IsNullOrWhiteSpace(request.TipoPropiedad))
        {
            var tipoSearch = request.TipoPropiedad.ToLower().Trim();
            query = query.Where(p => p.Tipo.Nombre.ToLower().Contains(tipoSearch));
        }

        // Filtro: Búsqueda libre (Dirección o Zona)
        if (!string.IsNullOrWhiteSpace(request.Busqueda))
        {
            var search = request.Busqueda.ToLower().Trim();
            query = query.Where(p =>
                p.Direccion.ToLower().Contains(search) ||
                (p.Zona != null && p.Zona.Nombre.ToLower().Contains(search))
            );
        }

        query = query.OrderByDescending(p => p.Destacada)
            .ThenByDescending(p => p.CreadoEl);

        return await GetPagedDataAsync<Propiedades, PropiedadPublicaDto>(request.Page, request.PageSize, query);
    }

    public async Task<OperationResponse<PropiedadDetalleDto>> ObtenerPorIdAsync(Guid id)
    {
        var propiedad = await _context.Set<Propiedades>()
            .Include(p => p.Zona)
            .Include(p => p.Tipo)
            .Include(p => p.Operacion)
            .Include(p => p.FaseCarga)
            .Include(p => p.EstadoComercial)
            .Include(p => p.PropiedadImagenes)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (propiedad == null) return NotFound<PropiedadDetalleDto>();

        return Ok(_mapper.Map<PropiedadDetalleDto>(propiedad));
    }

    public async Task<OperationResponse<PropiedadDetalleDto>> ActualizarAsync(Guid id, PropiedadUpdateRequestDto request)
    {
        var propiedadExistente = await _context.Set<Propiedades>()
            .Include(p => p.Zona)
            .Include(p => p.Tipo)
            .Include(p => p.Operacion)
            .Include(p => p.FaseCarga)
            .Include(p => p.EstadoComercial)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (propiedadExistente == null)
            return NotFound<PropiedadDetalleDto>();

        _mapper.Map(request, propiedadExistente);

        return await UpdateAsync<Propiedades, PropiedadDetalleDto>(propiedadExistente, _context);
    }

    public async Task<OperationResponse<bool>> EliminarAsync(Guid id)
    {
        var propiedad = await _context.Set<Propiedades>().FirstOrDefaultAsync(p => p.Id == id);

        if (propiedad == null)
            return NotFound<bool>();

        return await DeleteAsync(propiedad, _context);
    }

    public async Task<OperationResponse<PropiedadImagenDto>> SubirImagenAsync(Guid propiedadId, IFormFile file)
    {
        // A. Verificamos que la propiedad exista
        var propiedad = await _context.Set<Propiedades>()
            .Include(p => p.PropiedadImagenes)
            .FirstOrDefaultAsync(p => p.Id == propiedadId);

        if (propiedad == null)
            return NotFound<PropiedadImagenDto>();

        var tenantId = GetCurrentTenantId();
        var imageUrl = await _storageService.SubirImagenAsync(file, tenantId);

        var esPrimeraFoto = !propiedad.PropiedadImagenes.Any(i => !i.IsDeleted);

        var nuevaImagen = new PropiedadImagene
        {
            PropiedadId = propiedadId,
            Url = imageUrl,
            EsPrincipal = esPrimeraFoto,
            Orden = (short)(propiedad.PropiedadImagenes.Count + 1)
        };

        PrepareAuditableEntity(nuevaImagen, isNew: true);
        _context.Set<PropiedadImagene>().Add(nuevaImagen);

        // Si era la primera foto, significa que pasó de no tener media a sí tenerla.
        if (esPrimeraFoto)
        {
            // Solo la publicamos automáticamente si estaba en Borrador (1) o Esperando Media (2).
            // Si estaba Oculta (4), la dejamos como está.
            if (propiedad.FaseCargaId == 1 || propiedad.FaseCargaId == 2)
            {
                propiedad.FaseCargaId = 3;

                PrepareAuditableEntity(propiedad, isNew: false);
                _context.Set<Propiedades>().Update(propiedad);
            }
        }

        await _context.SaveChangesAsync();

        return Ok(_mapper.Map<PropiedadImagenDto>(nuevaImagen));
    }
    public async Task<OperationResponse<bool>> EliminarImagenAsync(Guid propiedadId, Guid imagenId)
    {
        var imagen = await _context.Set<PropiedadImagene>()
            .FirstOrDefaultAsync(i => i.Id == imagenId && i.PropiedadId == propiedadId);

        if (imagen == null) return NotFound<bool>();
        await _storageService.BorrarImagenAsync(imagen.Url);

        _context.Set<PropiedadImagene>().Remove(imagen);
        await _context.SaveChangesAsync();

        return Ok(true);
    }

    public async Task<OperationResponse<bool>> EstablecerImagenPrincipalAsync(Guid propiedadId, Guid imagenId)
    {
        var imagenes = await _context.Set<PropiedadImagene>()
            .Where(i => i.PropiedadId == propiedadId && !i.IsDeleted)
            .ToListAsync();

        if (!imagenes.Any(i => i.Id == imagenId))
            return NotFound<bool>();

        foreach (var img in imagenes)
        {
            img.EsPrincipal = (img.Id == imagenId);
            PrepareAuditableEntity(img, isNew: false);
            _context.Set<PropiedadImagene>().Update(img);
        }

        await _context.SaveChangesAsync();
        return Ok(true);
    }

    public async Task<OperationResponse<List<PropiedadHistorialResponseDto>>> ObtenerHistorialAsync(Guid propiedadId)
    {
        var existe = await _context.Set<Propiedades>().AnyAsync(p => p.Id == propiedadId);
        if (!existe) return NotFound<List<PropiedadHistorialResponseDto>>();

        var estados = await _context.Set<EstadosComerciale>()
            .ToDictionaryAsync(e => e.Id, e => e.Nombre);

        var historialCrudo = await _context.Set<PropiedadHistorialCambio>()
            .Where(h => h.PropiedadId == propiedadId)
            .OrderByDescending(h => h.CreadoEl)
            .ToListAsync();

        var historial = historialCrudo.Select(h => new PropiedadHistorialResponseDto
        {
            Id = h.Id,
            Fecha = h.CreadoEl,
            EstadoAnterior = h.EstadoComAntId.HasValue && estados.ContainsKey(h.EstadoComAntId.Value)
                             ? estados[h.EstadoComAntId.Value]
                             : "Ninguno",
            EstadoNuevo = h.EstadoComNuevoId.HasValue && estados.ContainsKey(h.EstadoComNuevoId.Value)
                             ? estados[h.EstadoComNuevoId.Value]
                             : "Desconocido",
            Observacion = LimpiarObservacionParaFrontend(h.Observacion ?? "Cambio de estado registrado."),
            Usuario = h.CreadoPor ?? "sistema"
        }).ToList();

        return Ok(historial);
    }

    public async Task<OperationResponse<List<string>>> ObtenerUrlsImagenesAsync(Guid propiedadId)
    {
        var imagenes = await _context.Set<PropiedadImagene>()
            .IgnoreQueryFilters()
            .Where(i => i.PropiedadId == propiedadId && !i.IsDeleted)
            .OrderByDescending(i => i.EsPrincipal)
            .ThenBy(i => i.Orden)
            .Select(i => i.Url)
            .ToListAsync();

        if (imagenes == null || imagenes.Count == 0)
        {
            return NotFound<List<string>>();
        }

        return Ok(imagenes);
    }

    public async Task<OperationResponse<PropiedadPublicaDto>> ObtenerDetallePublicoAsync(Guid id)
    {
        var propiedad = await _context.Set<Propiedades>()
            .IgnoreQueryFilters()
            .Include(p => p.Zona)
            .Include(p => p.Tenant)
            .Include(p => p.Tipo)
            .Include(p => p.Operacion)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted && p.FaseCargaId == 3 && p.EstadoComercialId == 1 && p.Tenant.Activo && !p.Tenant.IsDeleted);

        if (propiedad == null) return NotFound<PropiedadPublicaDto>();

        return Ok(_mapper.Map<PropiedadPublicaDto>(propiedad));
    }

    private static string LimpiarObservacionParaFrontend(string observacion)
    {
        return System.Text.RegularExpressions.Regex.Replace(observacion, @"\s*\(ID:.*?\)", "").Trim();
    }

    private static IQueryable<Propiedades> AplicarFiltros(IQueryable<Propiedades> query, PropiedadBackofficeFilterRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Busqueda))
        {
            var texto = request.Busqueda.ToLower().Trim();
            query = query.Where(p =>
                p.Direccion.ToLower().Contains(texto) ||
                p.Codigo.ToLower().Contains(texto));
        }

        if (request.FaseCargaId.HasValue)
        {
            query = query.Where(p => p.FaseCargaId == request.FaseCargaId.Value);
        }

        if (request.EstadoComercialId.HasValue)
        {
            query = query.Where(p => p.EstadoComercialId == request.EstadoComercialId.Value);
        }

        return query;
    }
}