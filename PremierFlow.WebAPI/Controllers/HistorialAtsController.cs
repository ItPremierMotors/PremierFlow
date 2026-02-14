using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Dtos.Taller;
using PremierFlow.Application.Interfaces.Taller;
using System.Security.Claims;

namespace PremierFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialAtsController : ControllerBase
    {
        private readonly IHistorialAtsService _historialService;

        public HistorialAtsController(IHistorialAtsService historialService)
        {
            _historialService = historialService;
        }
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _historialService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByOsId/{osId}")]
        public async Task<IActionResult> GetByOsId(int osId)
        {
            var result = await _historialService.GetByOsIdAsync(osId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByVehiculo/{vehiculoId}")]
        public async Task<IActionResult> GetByVehiculo(int vehiculoId)
        {
            var result = await _historialService.GetByVehiculoIdAsync(vehiculoId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByVin/{vin}")]
        public async Task<IActionResult> GetByVin(string vin)
        {
            var result = await _historialService.GetByVinAsync(vin);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByPlaca/{placa}")]
        public async Task<IActionResult> GetByPlaca(string placa)
        {
            var result = await _historialService.GetByPlacaAsync(placa);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetConRecomendacionPendiente")]
        public async Task<IActionResult> GetConRecomendacionPendiente()
        {
            var result = await _historialService.GetConRecomendacionPendienteAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("CrearDesdeOs")]
        public async Task<IActionResult> CrearDesdeOs([FromBody] CreateHistorialAtsDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _historialService.CrearDesdeOsAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
    }
}
