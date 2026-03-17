namespace Inmo24.Application.Services.Implementations;

public class JwtService : BaseService, IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(
        InmobiliariaContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IMemoryCache cache,
        IConfiguration configuration)
        : base(context, mapper, httpContextAccessor, cache)
    {
        _configuration = configuration;
    }

    public async Task<OperationResponse<string>> CrearTokenAsync(LoginRequestDto request)
    {
        var usuario = await _context.Set<Usuario>()
            .IgnoreQueryFilters()
            .Include(u => u.Tenant)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (usuario == null || !usuario.Activo || usuario.IsDeleted || !PasswordHelper.VerifyPassword(request.Password, usuario.PasswordHash))
            return BadRequest<string>("Email o contraseña incorrectos.");

        if (!usuario.Tenant.Activo || usuario.Tenant.IsDeleted)
            return BadRequest<string>("El Tenant (Inmobiliaria) se encuentra inactivo.");

        var claims = new List<Claim>
        {
            new("Id", usuario.Id.ToString()),
            new("TenantId", usuario.TenantId.ToString()),
            new("Email", usuario.Email),
            new("NombreCompleto", $"{usuario.Nombre} {usuario.Apellido}"),
            new("Rol", usuario.Rol),
            new("TenantNombre", usuario.Tenant.Nombre),
            new("TenantPlan", usuario.Tenant.Plan)
        };

        if (!string.IsNullOrEmpty(usuario.AvatarUrl))
        {
            claims.Add(new Claim("AvatarUrl", usuario.AvatarUrl));
        }

        var token = ReturnToken(
            new ClaimsIdentity(claims),
            _configuration["Jwt:Audience"]!,
            _configuration["Jwt:Issuer"]!,
            _configuration["Jwt:SecretKey"]!,
            int.Parse(_configuration["Jwt:Minutes"] ?? "1440"));

        return Ok(token);
    }

    public OperationResponse<UsuarioResponse> GetDataToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(token);

        var dataToken = new UsuarioResponse
        {
            Id = Guid.Parse(jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "Id")?.Value ?? Guid.Empty.ToString()),
            TenantId = Guid.Parse(jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "TenantId")?.Value ?? Guid.Empty.ToString()),
            Email = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "Email")?.Value ?? string.Empty,
            NombreCompleto = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "NombreCompleto")?.Value ?? string.Empty,
            Rol = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "Rol")?.Value ?? string.Empty,
            TenantNombre = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "TenantNombre")?.Value ?? string.Empty,
            TenantPlan = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "TenantPlan")?.Value ?? string.Empty,
            AvatarUrl = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "AvatarUrl")?.Value
        };

        return Ok(dataToken);
    }

    private static string ReturnToken(ClaimsIdentity claims, string audience, string issuer, string secretKey, int minutes)
    {
        var key = Encoding.ASCII.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Expires = DateTime.UtcNow.AddMinutes(minutes),
            NotBefore = DateTime.UtcNow,
            IssuedAt = DateTime.UtcNow,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Audience = audience,
            Issuer = issuer,
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var createdToken = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(createdToken);
    }
}