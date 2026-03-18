using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Dtos.Crm;
using PremierFlow.Application.Interfaces.Crm;
using System.Security.Claims;

namespace PremierFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CotizacionesVehiculoController : ControllerBase
    {
        private readonly ICotizacionVehiculoService _cotizacionService;
        public CotizacionesVehiculoController(ICotizacionVehiculoService cotizacionService)
        {
            _cotizacionService = cotizacionService;
        }

        private string? GetUserId() => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [HttpGet("GetByOportunidad/{oportunidadId}")]
        public async Task<IActionResult> GetByOportunidad(int oportunidadId)
        {
            var result = await _cotizacionService.GetByOportunidadAsync(oportunidadId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateCotizacionVehiculoDTO dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Usuario no autenticado." });

            var result = await _cotizacionService.CreateAsync(dto, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Aceptar/{cotizacionVehiculoId}")]
        public async Task<IActionResult> Aceptar(int cotizacionVehiculoId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Usuario no autenticado." });

            var result = await _cotizacionService.AceptarAsync(cotizacionVehiculoId, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Rechazar/{cotizacionVehiculoId}")]
        public async Task<IActionResult> Rechazar(int cotizacionVehiculoId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Usuario no autenticado." });

            var result = await _cotizacionService.RechazarAsync(cotizacionVehiculoId, userId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
