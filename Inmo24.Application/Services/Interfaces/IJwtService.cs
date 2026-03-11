namespace Inmo24.Application.Services.Implementations
{
    public interface IJwtService
    {
        Task<OperationResponse<string>> CrearTokenAsync(LoginRequestDto request);
        OperationResponse<UsuarioResponse> GetDataToken(string token);
    }
}
