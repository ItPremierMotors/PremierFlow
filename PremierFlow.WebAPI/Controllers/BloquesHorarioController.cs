using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Taller;
using PremierFlow.Application.Interfaces.Taller;
using PremierFlow.Domain.Entities;
using PremierFlow.Domain.Enums;
using PremierFlow.Infrastructure.Persistence.Repositories.Services.Taller;
using System.Security.Claims;

namespace PremierFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BloquesHorarioController : ControllerBase
    {
        private readonly IBloqueHorarioService _BloqueServices;
        public BloquesHorarioController(IBloqueHorarioService bloqueHorarioService)
        {
            _BloqueServices = bloqueHorarioService;
        }

        [HttpGet("GetById/{BloqueId}")]
        public async Task<IActionResult> GetById(int BloqueId)
        {
            var result = await _BloqueServices.GetByIdAsync(BloqueId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByCapacidadId/{BloqueId}")]
        public async Task<IActionResult> GetByCapacidadId(int BloqueId)
        {
            var result = await _BloqueServices.GetByCapacidadIdAsync(BloqueId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetDisponibles/{fecha}")]
        public async Task<IActionResult> GetDisponibles(DateTime fecha, [FromQuery] int? sucursalId = null)
        {
            var result = await _BloqueServices.GetDisponiblesByFechaAsync(fecha, sucursalId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByFecha/{fecha}")]
        public async Task<IActionResult> GetByFecha(DateTime fecha, [FromQuery] int? sucursalId = null)
        {
            var result = await _BloqueServices.GetByFechaAsync(fecha, sucursalId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetDisponiblesByTipo/{fecha}/tipo/{tipo}")]
        public async Task<IActionResult> GetDisponiblesByTipo(DateTime fecha, TipoBloqueHorario tipo, [FromQuery] int? sucursalId = null)
        {
            if (!Enum.IsDefined(typeof(TipoBloqueHorario), tipo))
                return BadRequest("Tipo de Bloque invalido");
            var result = await _BloqueServices.GetDisponiblesByFechaYTipoAsync(fecha, tipo,sucursalId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateBloqueHorarioDTO dto) 
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //validar enum del dto
            if (!Enum.IsDefined(typeof(TipoBloqueHorario), dto.TipoBloque))
                return BadRequest("Tipo de Bloque invalido");
            if (userIdFromToken == null)
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _BloqueServices.CreateAsync(dto,userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(UpdateBloqueHorarioDTO dto) 
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //validar enum del dto
            if (!Enum.IsDefined(typeof(TipoBloqueHorario), dto.TipoBloque))
                return BadRequest("Tipo de Bloque invalido");
            if (userIdFromToken == null)
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _BloqueServices.UpdateAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
        [HttpDelete("Delete/{bloqueId}")]
        public async Task<IActionResult> Delete(int bloqueId)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdFromToken == null)
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _BloqueServices.DeleteAsync(bloqueId, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("AgendarVehiculo/{bloqueId}")]
        public async Task<IActionResult> AgendarVehiculo(int bloqueId)
        {
            var result = await _BloqueServices.AgendarVehiculoAsync(bloqueId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("LiberarEspacio/{bloqueId}")]
        public async Task<IActionResult> LiberarEspacio(int bloqueId)
        {
            var result = await _BloqueServices.LiberarEspacioAsync(bloqueId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("generar-automaticos")]
        public async Task<IActionResult> GenerarBloquesAutomaticos([FromBody] GenerarBloquesDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdFromToken == null)
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }

            var resultado = await _BloqueServices.GenerarBloquesAutomaticosAsync(dto, userIdFromToken);

            return StatusCode(resultado.StatusCode, resultado);
        }
    }
}
