using Inmo24.Application.RequestDto.Auth;
using Inmo24.Application.ResponseDto.Auth;
using Inmo24.Application.Services.Implementations;
using System.ComponentModel.DataAnnotations;

namespace Inmo24.API.Controllers;

[ApiController]
public class LoginController(IJwtService jwtService) : BaseController
{
    private readonly IJwtService _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));

    /// <summary>
    /// Obtiene los datos del agente a traves del token.
    /// </summary>
    /// <param name="token">El token JWT.</param>
    /// <returns>ActionResult con los datos extraídos del token.</returns>
    [HttpGet("[action]")]
    [ProducesResponseType(typeof(OperationResponse<UsuarioResponse>), StatusCodes.Status200OK)]
    public IActionResult GetDataToken([FromQuery, Required] string token) =>
        Return(_jwtService.GetDataToken(token));

    /// <summary>
    /// Inicia sesión y devuelve el token JWT del usuario.
    /// </summary>
    /// <param name="request">Credenciales del usuario (Email y Password)</param>
    /// <returns>ActionResult con el token JWT string.</returns>
    [HttpPost("[action]")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(OperationResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request) =>
        Return(await _jwtService.CrearTokenAsync(request));
}
