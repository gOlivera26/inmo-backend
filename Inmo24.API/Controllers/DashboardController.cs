using Inmo24.Application.ResponseDto.Dashboard;

namespace Inmo24.API.Controllers;

[ApiController]
public class DashboardController(IDashboardService dashboardService) : BaseController
{
    private readonly IDashboardService _dashboardService = dashboardService;

    [HttpGet("resumen")]
    [ProducesResponseType(typeof(OperationResponse<DashboardResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetResumenDashboard() =>
        Return(await _dashboardService.ObtenerResumenAsync());
}