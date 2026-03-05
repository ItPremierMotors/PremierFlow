using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Interfaces.Dashboard;

namespace PremierFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("Resumen")]
        public async Task<IActionResult> GetResumen()
        {
            var result = await _dashboardService.GetResumenAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Taller")]
        public async Task<IActionResult> GetTaller(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            if (fechaInicio == default || fechaFin == default)
                return BadRequest("Las fechas son requeridas.");
            if (fechaInicio > fechaFin)
                return BadRequest("La fecha de inicio debe ser menor o igual a la fecha fin.");
            if ((fechaFin - fechaInicio).TotalDays > 90)
                return BadRequest("El rango máximo permitido es de 90 días.");

            var result = await _dashboardService.GetTallerAsync(fechaInicio, fechaFin);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Ventas")]
        public async Task<IActionResult> GetVentas(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            if (fechaInicio == default || fechaFin == default)
                return BadRequest("Las fechas son requeridas.");
            if (fechaInicio > fechaFin)
                return BadRequest("La fecha de inicio debe ser menor o igual a la fecha fin.");

            var result = await _dashboardService.GetVentasAsync(fechaInicio, fechaFin);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Inventario")]
        public async Task<IActionResult> GetInventario()
        {
            var result = await _dashboardService.GetInventarioAsync();
            return StatusCode(result.StatusCode, result);
        }
    }
}
