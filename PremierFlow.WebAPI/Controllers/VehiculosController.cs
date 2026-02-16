using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Vehiculos;
using PremierFlow.Application.Interfaces.Vehiculo;
using PremierFlow.Domain.Entities;
using PremierFlow.Domain.Enums;
using System.Security.Claims;

namespace PremierFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VehiculosController : ControllerBase
    {
        private readonly IVehiculoService _vehiculoService;
        public VehiculosController(IVehiculoService vehiculoService)
        {
            _vehiculoService = vehiculoService;   
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        { 
           var result =await _vehiculoService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _vehiculoService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetDetalle/{id}")]
        public async Task<IActionResult> GetDetalle(int id)
        {
            var result = await _vehiculoService.GetDetalleByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByCliente/{clienteId}")]
        public async Task<IActionResult> GetByCliente(int clienteId)
        {
            var result = await _vehiculoService.GetByClienteAsync(clienteId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByEstado/{estado}")]
        public async Task<IActionResult> GetByEstado(string estado)
        {
            // convertir estado a enum EstadoVehiculo
            if (!Enum.TryParse<EstadoVehiculo>(estado, true, out var estadoEnum))
             return BadRequest("Clasificación inválida");
            var result = await _vehiculoService.GetByEstadoAsync(estadoEnum);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetBySucursal/{sucursalId}")]
        public async Task<IActionResult> GetBySucursal(int sucursalId)
        {
            var result = await _vehiculoService.GetBySucursalAsync(sucursalId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByVin/{vin}")]
        public async Task<IActionResult> GetByVin(string vin)
        {
            var result = await _vehiculoService.GetByVinAsync(vin);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByPlaca/{placa}")]
        public async Task<IActionResult> GetByPlaca(string placa)
        {
            var result = await _vehiculoService.GetByPlacaAsync(placa);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("search/{term}")]
        public async Task<IActionResult> Search(string term)
        {
            var result = await _vehiculoService.SearchAsync(term);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetDisponiblesParaVenta")]
        public async Task<IActionResult> GetDisponiblesParaVenta()
        {
            var result = await _vehiculoService.GetDisponiblesParaVentaAsync();
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateVehiculoDTO dto)
        {
            var UserFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //validar enums
            if (!Enum.IsDefined(typeof(TipoCombustible), dto.TipoCombustible))
                return BadRequest("Tipo de combustible inválido");
            if (dto.Transmision != null)
                if (!Enum.IsDefined(typeof(TipoTransmision), dto.Transmision))
                    return BadRequest("Tipo de transmisión inválido");
            if (!Enum.IsDefined(typeof(EstadoVehiculo), dto.Estado))
                return BadRequest("Estado del vehículo inválido");

            if (string.IsNullOrEmpty(UserFromToken))
            {
                return Unauthorized(new { Messaje = "Usuario no autenticado. " });
            }
            var result = await _vehiculoService.CreateAsync(dto, UserFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateVehiculoDTO dto)
        {
            var UserFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(UserFromToken))
            {
                return Unauthorized(new { Messaje = "Usuario no autenticado. " });
            }
            var result = await _vehiculoService.UpdateAsync(dto, UserFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var UserFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(UserFromToken))
            {
                return Unauthorized(new { Messaje = "Usuario no autenticado. " });
            }
            var result = await _vehiculoService.DeleteAsync(id, UserFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("ActualizarKilometraje/{vehiculoId}")]
        public async Task<IActionResult> ActualizarKilometraje(int vehiculoId, [FromBody] int nuevoKm)
        {
            var UserFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(UserFromToken))
            {
                return Unauthorized(new { Messaje = "Usuario no autenticado. " });
            }
            var result = await _vehiculoService.ActualizarKilometrajeAsync(vehiculoId, nuevoKm, UserFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("CambiarEstado/{vehiculoId}")]
        public async Task<IActionResult> CambiarEstado(int vehiculoId, [FromBody] CambiarEstadoDTO dto)
        {
            var UserFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(UserFromToken))
            {
                return Unauthorized(new { Messaje = "Usuario no autenticado. " });
            }

            // Solo "JefeVentas" puede reservar
            if (dto.NuevoEstado == EstadoVehiculo.Reservado && !User.IsInRole("JefeVentas"))
                return StatusCode(403, ApiResponse<bool>.fail(403, null, "Solo el rol 'Jefe Ventas' puede reservar vehículos."));

            var result = await _vehiculoService.CambiarEstadoAsync(vehiculoId, dto.NuevoEstado, UserFromToken, dto.ClienteId, dto.VendedorId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("CancelarReservasVencidas")]
        public async Task<IActionResult> CancelarReservasVencidas()
        {
            var UserFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(UserFromToken))
            {
                return Unauthorized(new { Messaje = "Usuario no autenticado. " });
            }
            var result = await _vehiculoService.CancelarReservasVencidasAsync(UserFromToken);
            return StatusCode(result.StatusCode, result);
        }
    }
}
