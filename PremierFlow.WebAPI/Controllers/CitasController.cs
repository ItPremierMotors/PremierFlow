using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Taller;
using PremierFlow.Application.Interfaces.Taller;
using PremierFlow.Domain.Enums;
using PremierFlow.Infrastructure.Persistence.Repositories.Services.Taller;
using System.Security.Claims;

namespace PremierFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CitasController : ControllerBase
    {
        private readonly ICitaService _citaService;
        public CitasController(ICitaService citaService)
        {
            _citaService = citaService;
        }
        [HttpGet("GetAll")]
        private async Task<IActionResult> GetAll()
        {
            var result = await _citaService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetById/{citaId}")]
        private async Task<IActionResult> GetById(int citaId)
        {
            var result = await _citaService.GetByIdAsync(citaId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByCodigo/{codigoCita}")]
        private async Task<IActionResult> GetByCodigo(string codigoCita)
        {
            if (string.IsNullOrWhiteSpace(codigoCita))
                return BadRequest(new { Message = "El codigo no debe estar vacio. " });
            var result = await _citaService.GetByCodigoAsync(codigoCita);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByFecha/{fecha}")]
        public async Task<IActionResult> GetByFecha(DateTime fecha, [FromQuery] int? sucursalId = null)
        {
            var result = await _citaService.GetByFechaAsync(fecha, sucursalId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetByRango")]
        public async Task<IActionResult> GetByRango([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin, [FromQuery] int? sucursalId = null)
        {
            var result = await _citaService.GetByRangoFechasAsync(fechaInicio, fechaFin, sucursalId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByCliente/{clienteId}")]
        public async Task<IActionResult> GetByCliente(int clienteId)
        {
            var result = await _citaService.GetByClienteAsync(clienteId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByVehiculo/{VehiculoId}")]
        public async Task<IActionResult> GetByVehiculo(int VehiculoId)
        {
            var result = await _citaService.GetByVehiculoAsync(VehiculoId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetByEstado/{estado}")]
        public async Task<IActionResult> GetByEstado(EstadoCita estado, [FromQuery] int? sucursalId = null)
        {
            if (!Enum.IsDefined(typeof(EstadoCita), estado))
                return BadRequest("Estado de cita inválido.");
            var result = await _citaService.GetByEstadoAsync(estado, sucursalId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetActivasDelDia/{fecha}")]
        public async Task<IActionResult> GetActivasDelDia(DateTime fecha, [FromQuery] int? sucursalId = null)
        {
            var result = await _citaService.GetActivasDelDiaAsync(fecha, sucursalId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("Agendar")]
        public async Task<IActionResult> Agendar([FromBody] CreateCitaDTO dto) 
        {
            var userIdFromToken=User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result= await _citaService.AgendarAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateCitaDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _citaService.UpdateAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("Confirmar/{citaId}")]
        public async Task<IActionResult> Confirmar(int citaId)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _citaService.ConfirmarAsync(citaId, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("Cancelar")]
        public async Task<IActionResult> Confirmar([FromBody] CancelarCitaDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _citaService.CancelarAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("MarcarNoShow/{citaId}")]
        public async Task<IActionResult> MarcarNoShow(int citaId)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _citaService.MarcarNoShowAsync(citaId, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("IniciarAtencion/{citaId}")]
        public async Task<IActionResult> IniciarAtencion(int citaId)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _citaService.IniciarAtencionAsync(citaId, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("Completar/{citaId}")]
        public async Task<IActionResult> Completar(int citaId)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _citaService.CompletarAsync(citaId, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("Transferir")]
        public async Task<IActionResult> Transferir([FromBody] TransferirCitaDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _citaService.TransferirAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

    }
}
