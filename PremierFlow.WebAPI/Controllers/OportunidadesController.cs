using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Dtos.Crm;
using PremierFlow.Application.Interfaces.Crm;
using PremierFlow.Domain.Enums;
using System.Security.Claims;

namespace PremierFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OportunidadesController : ControllerBase
    {
        private readonly IOportunidadService _oportunidadService;
        public OportunidadesController(IOportunidadService oportunidadService)
        {
            _oportunidadService = oportunidadService;
        }

        private string? GetUserId() => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

       [Authorize(Roles = "AsesorVentas,JefeVentas,Gerencia,AdminTI")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int? sucursalId)
        {
           var vendedorId = User.IsInRole("AsesorVentas") ? GetUserId() : null;
            var result = await _oportunidadService.GetAllAsync(sucursalId, vendedorId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetById/{oportunidadId}")]
        public async Task<IActionResult> GetById(int oportunidadId)
        {
            var result = await _oportunidadService.GetByIdAsync(oportunidadId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetDetalle/{oportunidadId}")]
        public async Task<IActionResult> GetDetalle(int oportunidadId)
        {
            var result = await _oportunidadService.GetDetalleAsync(oportunidadId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByVendedor/{vendedorId}")]
        public async Task<IActionResult> GetByVendedor(string vendedorId)
        {
            var result = await _oportunidadService.GetByVendedorAsync(vendedorId);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "AsesorVentas,JefeVentas,Gerencia,AdminTI")]
        [HttpGet("GetByEtapa/{etapa}")]
        public async Task<IActionResult> GetByEtapa(EtapaOportunidad etapa, [FromQuery] int? sucursalId)
        {
            var vendedorId = User.IsInRole("AsesorVentas") ? GetUserId() : null;
            var result = await _oportunidadService.GetByEtapaAsync(etapa, sucursalId, vendedorId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetPipeline")]
        public async Task<IActionResult> GetPipeline([FromQuery] int? sucursalId)
        {
            var result = await _oportunidadService.GetPipelineAsync(sucursalId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetVencidas")]
        public async Task<IActionResult> GetVencidas([FromQuery] int? sucursalId, [FromQuery] int diasSinActividad = 7)
        {
            var result = await _oportunidadService.GetVencidasAsync(sucursalId, diasSinActividad);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateOportunidadDTO dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Usuario no autenticado." });

            var result = await _oportunidadService.CreateAsync(dto, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateOportunidadDTO dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Usuario no autenticado." });

            var result = await _oportunidadService.UpdateAsync(dto, userId);
            return StatusCode(result.StatusCode, result);
        }

        

        [HttpPut("CambiarEtapa")]
        public async Task<IActionResult> CambiarEtapa([FromBody] CambiarEtapaDTO dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Usuario no autenticado." });

            var result = await _oportunidadService.CambiarEtapaAsync(dto, userId);
            return StatusCode(result.StatusCode, result);
        }


        [HttpPut("CerrarGanada")]
        public async Task<IActionResult> CerrarGanada([FromBody] CerrarGanadaDTO dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Usuario no autenticado." });

            var result = await _oportunidadService.CerrarGanadaAsync(dto, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("CerrarPerdida")]
        public async Task<IActionResult> CerrarPerdida([FromBody] CerrarOportunidadDTO dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Usuario no autenticado." });

            var result = await _oportunidadService.CerrarPerdidaAsync(dto, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Cancelar")]
        public async Task<IActionResult> Cancelar([FromBody] CerrarOportunidadDTO dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Usuario no autenticado." });

            var result = await _oportunidadService.CancelarAsync(dto, userId);
            return StatusCode(result.StatusCode, result);
        }

       
    }
}
