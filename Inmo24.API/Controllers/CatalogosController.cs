namespace Inmo24.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogosController(ICatalogoService catalogoService) : BaseController
{
    private readonly ICatalogoService _catalogoService = catalogoService;

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(OperationResponse<CatalogosResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCatalogos() =>
        Return(await _catalogoService.ObtenerTodosAsync());
}