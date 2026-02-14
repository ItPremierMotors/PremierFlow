using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Dtos.Taller;
using PremierFlow.Application.Interfaces.Taller;
using System.Security.Claims;

namespace PremierFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CapacidadTallerController : ControllerBase
    {
        private readonly ICapacidadTallerService _capacidadService;
        public CapacidadTallerController(ICapacidadTallerService capacidadTallerService)
        {
            _capacidadService = capacidadTallerService;
        }

        [HttpGet("GetById/{capacidadId}")]
        public async Task<IActionResult> GetById(int capacidadId)
        {
            var result = await _capacidadService.GetByIdAsync(capacidadId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetByFecha/{fecha}")]
        public async Task<IActionResult> GetByFecha(DateTime fecha, [FromQuery] int? sucursalId = null)
        {
            var result = await _capacidadService.GetByFechaAsync(fecha, sucursalId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetByrangoFecha")]
        public async Task<IActionResult> GetByrangoFecha([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin, [FromQuery] int? sucursalId = null)
        {
            var result = await _capacidadService.GetByRangoFechasAsync(fechaInicio, fechaFin, sucursalId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetBySucursal/{sucursalId}")]
        public async Task<IActionResult> GetBySucursal(int sucursalId)
        {
            var result = await _capacidadService.GetBySucursalAsync(sucursalId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("disponibilidad")]
        public async Task<IActionResult> TieneCapacidad([FromQuery] DateTime fecha, [FromQuery] int minutosRequeridos, [FromQuery] int? sucursalId = null)
        {
            var result = await _capacidadService.TieneCapacidadAsync(fecha, minutosRequeridos, sucursalId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateCapacidadTallerDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _capacidadService.CreateAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateCapacidadTallerDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _capacidadService.UpdateAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("bloquear/{capacidadId}")]
        public async Task<IActionResult> BloquearDia(int capacidadId, [FromBody] string motivo)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _capacidadService.BloquearDiaAsync(capacidadId, motivo, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("Desbloquear/{capacidadId}")]
        public async Task<IActionResult> DesBloquearDia(int capacidadId)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _capacidadService.DesbloquearDiaAsync(capacidadId, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("generar-semana")]
        public async Task<IActionResult> GenerarSemana([FromQuery] DateTime fechaInicio, [FromBody] CreateCapacidadTallerDTO plantilla)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _capacidadService.GenerarCapacidadSemanalAsync(fechaInicio, plantilla, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("ReservarMins/{CapacidadId}")]
        public async Task<IActionResult> ReservarMinutos(int CapacidadId, [FromQuery] int mins)
        {
            var result =await _capacidadService.ReservarMinutosAsync(CapacidadId, mins);
            return StatusCode(result.StatusCode, result);   
        }
        [HttpPost("LiberarMinutos/{capacidadId}")]
        public async Task<IActionResult> LiberarMinutos(int capacidadId,[FromQuery] int mins)
        {
            var result = await _capacidadService.LiberarMinutosAsync(capacidadId, mins);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("RegistrarTiempoTrabajado/{capacidadId}")]
        public async Task<IActionResult> RegistrarTiempoTrabajado(int capacidadId, [FromQuery] int mins)
        {
            var result = await _capacidadService.RegistrarTiempoTrabajadoAsync(capacidadId, mins);
            return StatusCode(result.StatusCode, result);
        }
    }
}
