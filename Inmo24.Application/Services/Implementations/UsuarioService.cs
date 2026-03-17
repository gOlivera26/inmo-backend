using Inmo24.Application.RequestDto.Usuarios;
using Inmo24.Application.ResponseDto.Usuarios;

namespace Inmo24.Application.Services.Implementations;

public class UsuarioService : BaseService, IUsuarioService
{
    private readonly IStorageService _storageService;

    public UsuarioService(
        InmobiliariaContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IMemoryCache cache,
        IStorageService storageService)
        : base(context, mapper, httpContextAccessor, cache)
    {
        _storageService = storageService;
    }

    private Guid GetCurrentUsuarioGuid()
    {
        var claimId = _httpContextAccessor?.HttpContext?.User?.FindFirst("Id")?.Value;
        return Guid.TryParse(claimId, out Guid id) ? id : Guid.Empty;
    }

    public async Task<OperationResponse<UsuarioPerfilResponseDto>> ObtenerMiPerfilAsync()
    {
        var userId = GetCurrentUsuarioGuid();
        var usuario = await _context.Set<Usuario>().FirstOrDefaultAsync(u => u.Id == userId);

        if (usuario == null) return NotFound<UsuarioPerfilResponseDto>();

        return Ok(new UsuarioPerfilResponseDto
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email,
            Telefono = usuario.Telefono ?? "",
            AvatarUrl = usuario.AvatarUrl ?? "",
            Rol = usuario.Rol
        });
    }

    public async Task<OperationResponse<UsuarioPerfilResponseDto>> ActualizarMiPerfilAsync(UsuarioUpdatePerfilRequestDto request)
    {
        var userId = GetCurrentUsuarioGuid();
        var usuario = await _context.Set<Usuario>().FirstOrDefaultAsync(u => u.Id == userId);

        if (usuario == null) return NotFound<UsuarioPerfilResponseDto>();

        usuario.Nombre = request.Nombre;
        usuario.Apellido = request.Apellido;
        usuario.Telefono = request.Telefono;

        PrepareAuditableEntity(usuario, isNew: false);
        _context.Set<Usuario>().Update(usuario);
        await _context.SaveChangesAsync();

        return await ObtenerMiPerfilAsync();
    }

    public async Task<OperationResponse<bool>> CambiarMiPasswordAsync(UsuarioChangePasswordRequestDto request)
    {
        var userId = GetCurrentUsuarioGuid();
        var usuario = await _context.Set<Usuario>().FirstOrDefaultAsync(u => u.Id == userId);

        if (usuario == null) return NotFound<bool>();

        // Verificamos que la contraseña actual coincida usando el helper que ya tienes
        if (!Utilities.PasswordHelper.VerifyPassword(request.PasswordActual, usuario.PasswordHash))
        {
            return BadRequest<bool>("La contraseña actual ingresada es incorrecta.");
        }

        // Hasheamos la nueva y guardamos
        usuario.PasswordHash = Utilities.PasswordHelper.HashPassword(request.PasswordNueva);

        PrepareAuditableEntity(usuario, isNew: false);
        _context.Set<Usuario>().Update(usuario);
        await _context.SaveChangesAsync();

        return Ok(true);
    }

    public async Task<OperationResponse<string>> CambiarMiAvatarAsync(IFormFile file)
    {
        var userId = GetCurrentUsuarioGuid();
        var usuario = await _context.Set<Usuario>().FirstOrDefaultAsync(u => u.Id == userId);

        if (usuario == null) return NotFound<string>();

        var tenantId = GetCurrentTenantId();

        // Usamos el servicio de Cloudflare R2 con el prefijo "avatares"
        var newUrl = await _storageService.SubirImagenAsync(file, tenantId, "avatares");

        // Si ya tenía una foto vieja, la borramos de Cloudflare para ahorrar espacio
        if (!string.IsNullOrEmpty(usuario.AvatarUrl))
        {
            await _storageService.BorrarImagenAsync(usuario.AvatarUrl);
        }

        usuario.AvatarUrl = newUrl;

        PrepareAuditableEntity(usuario, isNew: false);
        _context.Set<Usuario>().Update(usuario);
        await _context.SaveChangesAsync();

        return Ok(newUrl);
    }
}