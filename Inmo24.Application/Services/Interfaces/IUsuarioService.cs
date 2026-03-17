using Inmo24.Application.RequestDto.Usuarios;
using Inmo24.Application.ResponseDto.Usuarios;

namespace Inmo24.Application.Services.Interfaces;

public interface IUsuarioService
{
    Task<OperationResponse<UsuarioPerfilResponseDto>> ObtenerMiPerfilAsync();
    Task<OperationResponse<UsuarioPerfilResponseDto>> ActualizarMiPerfilAsync(UsuarioUpdatePerfilRequestDto request);
    Task<OperationResponse<bool>> CambiarMiPasswordAsync(UsuarioChangePasswordRequestDto request);
    Task<OperationResponse<string>> CambiarMiAvatarAsync(IFormFile file);
}
