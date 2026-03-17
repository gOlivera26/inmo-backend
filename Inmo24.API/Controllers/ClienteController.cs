using Inmo24.Application.RequestDto.Clientes;
using Inmo24.Application.ResponseDto.Clientes;

namespace Inmo24.API.Controllers;

[ApiController]
public class ClientesController(IClienteService clienteService) : BaseController
{
    private readonly IClienteService _clienteService = clienteService;

    [HttpGet("resumen")]
    [ProducesResponseType(typeof(OperationResponse<CrmResumenResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetResumenCrm() =>
            Return(await _clienteService.ObtenerResumenCrmAsync());

    [HttpGet]
    [ProducesResponseType(typeof(OperationResponse<List<ClienteResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClientes([FromQuery] ClienteFilterRequest request) =>
        Return(await _clienteService.ObtenerClientesAsync(request));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OperationResponse<ClienteDetalleResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResponse<ClienteDetalleResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetClienteById(Guid id) =>
        Return(await _clienteService.ObtenerClientePorIdAsync(id));

    [HttpPost]
    [ProducesResponseType(typeof(OperationResponse<ClienteResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCliente([FromBody] ClienteCreateRequestDto request) =>
        Return(await _clienteService.CrearClienteAsync(request));

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(OperationResponse<ClienteResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateCliente(Guid id, [FromBody] ClienteUpdateRequestDto request) =>
        Return(await _clienteService.ActualizarClienteAsync(id, request));
}