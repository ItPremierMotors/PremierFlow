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
    public class LeadsController : ControllerBase
    {
        private readonly ILeadService _leadService;
        public LeadsController(ILeadService leadService)
        {
            _leadService = leadService;
        }

        private string? GetUserId() => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        [Authorize(Roles = "AsesorVentas,JefeVentas,Gerencia,AdminTI")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int? sucursalId)
        {
            var vendedorId = User.IsInRole("AsesorVentas") ? GetUserId() : null;
            var result = await _leadService.GetAllAsync(sucursalId, vendedorId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetById/{leadId}")]
        public async Task<IActionResult> GetById(int leadId)
        {
            var result = await _leadService.GetByIdAsync(leadId);
            return StatusCode(result.StatusCode, result);
        }

         [Authorize(Roles = "AsesorVentas,JefeVentas,Gerencia,AdminTI")]
        [HttpGet("GetByEstado/{estado}")]
        public async Task<IActionResult> GetByEstado(EstadoLead estado, [FromQuery] int? sucursalId)
        {
               var vendedorId = User.IsInRole("AsesorVentas") ? GetUserId() : null;
            var result = await _leadService.GetByEstadoAsync(estado, sucursalId, vendedorId);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByVendedor/{vendedorId}")]
        public async Task<IActionResult> GetByVendedor(string vendedorId)
        {
            var result = await _leadService.GetByVendedorAsync(vendedorId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetSinAsignar")]
        public async Task<IActionResult> GetSinAsignar([FromQuery] int? sucursalId)
        {
            var result = await _leadService.GetSinAsignarAsync(sucursalId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Search/{termino}")]
        public async Task<IActionResult> Search(string termino)
        {
            var result = await _leadService.SearchAsync(termino);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateLeadDTO dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Usuario no autenticado." });

            var result = await _leadService.CreateAsync(dto, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateLeadDTO dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Usuario no autenticado." });

            var result = await _leadService.UpdateAsync(dto, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("AsignarVendedor")]
        public async Task<IActionResult> AsignarVendedor([FromBody] AsignarVendedorLeadDTO dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Usuario no autenticado." });

            var result = await _leadService.AsignarVendedorAsync(dto, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Calificar")]
        public async Task<IActionResult> Calificar([FromBody] CalificarLeadDTO dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Usuario no autenticado." });

            var result = await _leadService.CalificarAsync(dto, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Descartar")]
        public async Task<IActionResult> Descartar([FromBody] DescartarLeadDTO dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Usuario no autenticado." });

            var result = await _leadService.DescartarAsync(dto, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{leadId}/ConvertirAOportunidad")]
        public async Task<IActionResult> ConvertirAOportunidad(int leadId, [FromBody] CreateOportunidadDTO dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Usuario no autenticado." });

            var result = await _leadService.ConvertirAOportunidadAsync(leadId, dto, userId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
