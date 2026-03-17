using Inmo24.Application.RequestDto.Inmobiliaria;
using Inmo24.Application.ResponseDto.Inmobiliaria;

namespace Inmo24.Application.Services.Implementations;

public class InmobiliariaService : BaseService, IInmobiliariaService
{
    private readonly IStorageService _storageService;

    public InmobiliariaService(
        InmobiliariaContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IMemoryCache cache,
        IStorageService storageService)
        : base(context, mapper, httpContextAccessor, cache)
    {
        _storageService = storageService;
    }

    public async Task<OperationResponse<InmobiliariaResponseDto>> ObtenerMiAgenciaAsync()
    {
        var tenantId = GetCurrentTenantId();
        var tenant = await _context.Set<Tenant>().FirstOrDefaultAsync(t => t.Id == tenantId && !t.IsDeleted);

        if (tenant == null) return NotFound<InmobiliariaResponseDto>();

        return Ok(new InmobiliariaResponseDto
        {
            Id = tenant.Id,
            Nombre = tenant.Nombre,
            EmailContacto = tenant.EmailContacto,
            Telefono = tenant.Telefono ?? "",
            LogoUrl = tenant.LogoUrl ?? ""
        });
    }

    public async Task<OperationResponse<InmobiliariaResponseDto>> ActualizarMiAgenciaAsync(InmobiliariaUpdateRequestDto request)
    {
        var tenantId = GetCurrentTenantId();
        var tenant = await _context.Set<Tenant>().FirstOrDefaultAsync(t => t.Id == tenantId && !t.IsDeleted);

        if (tenant == null) return NotFound<InmobiliariaResponseDto>();

        tenant.Nombre = request.Nombre;
        tenant.EmailContacto = request.EmailContacto;
        tenant.Telefono = request.Telefono;

        PrepareAuditableEntity(tenant, isNew: false);
        _context.Set<Tenant>().Update(tenant);
        await _context.SaveChangesAsync();

        return await ObtenerMiAgenciaAsync();
    }

    public async Task<OperationResponse<string>> ActualizarLogoAsync(IFormFile file)
    {
        var tenantId = GetCurrentTenantId();
        var tenant = await _context.Set<Tenant>().FirstOrDefaultAsync(t => t.Id == tenantId && !t.IsDeleted);

        if (tenant == null) return NotFound<string>();

        // Subimos a Cloudflare R2 en la carpeta "logos"
        var newUrl = await _storageService.SubirImagenAsync(file, tenantId, "logos");

        // Borramos el logo viejo si existía
        if (!string.IsNullOrEmpty(tenant.LogoUrl))
        {
            await _storageService.BorrarImagenAsync(tenant.LogoUrl);
        }

        tenant.LogoUrl = newUrl;

        PrepareAuditableEntity(tenant, isNew: false);
        _context.Set<Tenant>().Update(tenant);
        await _context.SaveChangesAsync();

        return Ok(newUrl);
    }
}