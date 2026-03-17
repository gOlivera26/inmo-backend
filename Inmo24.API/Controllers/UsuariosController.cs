using Inmo24.Application.RequestDto.Usuarios;
using Inmo24.Application.ResponseDto.Usuarios;

namespace Inmo24.API.Controllers;

[ApiController]
public class UsuariosController(IUsuarioService usuarioService) : BaseController
{
    private readonly IUsuarioService _usuarioService = usuarioService;

    [HttpGet("mi-perfil")]
    [ProducesResponseType(typeof(OperationResponse<UsuarioPerfilResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMiPerfil() =>
        Return(await _usuarioService.ObtenerMiPerfilAsync());

    [HttpPut("mi-perfil")]
    [ProducesResponseType(typeof(OperationResponse<UsuarioPerfilResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateMiPerfil([FromBody] UsuarioUpdatePerfilRequestDto request) =>
        Return(await _usuarioService.ActualizarMiPerfilAsync(request));

    [HttpPut("mi-perfil/password")]
    [ProducesResponseType(typeof(OperationResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ChangePassword([FromBody] UsuarioChangePasswordRequestDto request) =>
        Return(await _usuarioService.CambiarMiPasswordAsync(request));

    [HttpPost("mi-perfil/avatar")]
    [ProducesResponseType(typeof(OperationResponse<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadAvatar(IFormFile file) =>
        Return(await _usuarioService.CambiarMiAvatarAsync(file));
}