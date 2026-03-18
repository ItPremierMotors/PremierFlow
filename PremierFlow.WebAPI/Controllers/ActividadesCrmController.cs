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
    public class ActividadesCrmController : ControllerBase
    {
        private readonly IActividadCrmService _actividadService;
        public ActividadesCrmController(IActividadCrmService actividadService)
        {
            _actividadService = actividadService;
        }

        private string? GetUserId() => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [HttpGet("GetByLead/{leadId}")]
        public async Task<IActionResult> GetByLead(int leadId)
        {
            var result = await _actividadService.GetByLeadAsync(leadId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByOportunidad/{oportunidadId}")]
        public async Task<IActionResult> GetByOportunidad(int oportunidadId)
        {
            var result = await _actividadService.GetByOportunidadAsync(oportunidadId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetPendientesVendedor/{vendedorId}")]
        public async Task<IActionResult> GetPendientesVendedor(string vendedorId)
        {
            var result = await _actividadService.GetPendientesVendedorAsync(vendedorId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetVencidas")]
        public async Task<IActionResult> GetVencidas([FromQuery] int? sucursalId)
        {
            var result = await _actividadService.GetVencidasAsync(sucursalId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetProximosContactos/{vendedorId}")]
        public async Task<IActionResult> GetProximosContactos(string vendedorId, [FromQuery] DateTime fecha)
        {
            var result = await _actividadService.GetProximosContactosAsync(vendedorId, fecha);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateActividadCrmDTO dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Usuario no autenticado." });

            var result = await _actividadService.CreateAsync(dto, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Completar")]
        public async Task<IActionResult> Completar([FromBody] CompletarActividadDTO dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Usuario no autenticado." });

            var result = await _actividadService.CompletarAsync(dto, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Cancelar/{actividadCrmId}")]
        public async Task<IActionResult> Cancelar(int actividadCrmId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Usuario no autenticado." });

            var result = await _actividadService.CancelarAsync(actividadCrmId, userId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
