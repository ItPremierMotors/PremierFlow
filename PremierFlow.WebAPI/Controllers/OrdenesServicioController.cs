using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Taller;
using PremierFlow.Application.Interfaces.Taller;
using System.Security.Claims;

namespace PremierFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdenesServicioController : ControllerBase
    {
        private readonly IOrdenServicioService _osService;

        public OrdenesServicioController(IOrdenServicioService osService)
        {
            _osService = osService;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _osService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _osService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetDetalle/{id}")]
        public async Task<IActionResult> GetDetalle(int id)
        {
            var result = await _osService.GetDetalleByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetByNumero/{numero}")]
        public async Task<IActionResult> GetByNumero(string numero)
        {
            var result = await _osService.GetByNumeroAsync(numero);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetAbiertas")]
        public async Task<IActionResult> GetAbiertas([FromQuery] int? sucursalId = null)
        {
            var result = await _osService.GetAbiertasAsync(sucursalId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetByFecha/{fecha}")]
        public async Task<IActionResult> GetByFecha(DateTime fecha, [FromQuery] int? sucursalId = null)
        {
            var result = await _osService.GetByFechaAsync(fecha, sucursalId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetByRango")]
        public async Task<IActionResult> GetByRango([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin, [FromQuery] int? sucursalId = null)
        {
            // 1. Validar que las fechas no estén vacías
            if (fechaInicio == default || fechaFin == default)
                return BadRequest("Las fechas son requeridas.");

            // 2. Validar que fecha inicio sea menor o igual a fecha fin
            if (fechaInicio > fechaFin)
                return BadRequest("La fecha de inicio debe ser menor o igual a la fecha fin.");

            // 3. Validar rango máximo (opcional - ejemplo: máximo 90 días)
            if ((fechaFin - fechaInicio).TotalDays > 90)
                return BadRequest( "El rango máximo permitido es de 90 días.");

            //// 4. Validar que no sean fechas futuras (opcional)
            //if (fechaInicio > DateTime.Today || fechaFin > DateTime.Today)
            //    return BadRequest(ApiResponse<List<OrdenServicioDTO>>.fail(400, null, "No se permiten fechas futuras."));
            var result = await _osService.GetByRangoFechasAsync(fechaInicio, fechaFin, sucursalId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetByCliente/{clienteId}")]
        public async Task<IActionResult> GetByCliente(int clienteId)
        {
            var result = await _osService.GetByClienteAsync(clienteId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByVehiculo/{vehiculoId}")]
        public async Task<IActionResult> GetByVehiculo(int vehiculoId)
        {
            var result = await _osService.GetByVehiculoAsync(vehiculoId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetHistorialServicio/{vehiculoId}")]
        public async Task<IActionResult> GetHistorialServicio(int vehiculoId)
        {
            var result = await _osService.GetHistorialServicioVehiculoAsync(vehiculoId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByEstado/{estadoId}")]
        public async Task<IActionResult> GetByEstado(int estadoId, [FromQuery] int? sucursalId = null)
        {
            var result = await _osService.GetByEstadoAsync(estadoId, sucursalId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("CreateFromCita")]
        public async Task<IActionResult> CreateFromCita([FromBody] CreateOsFromCitaDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (String.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no autorizado. ");
            }
            var result = await _osService.CreateFromCitaAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("CreateWalkIn")]
        public async Task<IActionResult> CreateWalkIn([FromBody] CreateOsWalkInDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (String.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no autorizado. ");
            }
            var result = await _osService.CreateWalkInAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateOrdenServicioDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (String.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no autorizado. ");
            }
            var result = await _osService.UpdateAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("cambiar-estado")]
        public async Task<IActionResult> CambiarEstado([FromBody] CambiarEstadoOsDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (String.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no autorizado. ");
            }
            var result = await _osService.CambiarEstadoAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("cerrar")]
        public async Task<IActionResult> Cerrar([FromBody] CerrarOsDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (String.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no autorizado. ");
            }
            var result = await _osService.CerrarAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result); 
        }

        [HttpPost("cancelar/{id}")]
        public async Task<IActionResult> Cancelar(int id, [FromBody] string motivo)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (String.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no autorizado. ");
            }
            var result = await _osService.CancelarAsync(id, motivo, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{id}/recalcular")]
        public async Task<IActionResult> RecalcularTotales(int id)
        {
            var result = await _osService.RecalcularTotalesAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetTransicionesValidas/{osId}")]
        public async Task<IActionResult> GetTransicionesValidas(int osId)
        {
            var result = await _osService.GetTransicionesValidasAsync(osId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
