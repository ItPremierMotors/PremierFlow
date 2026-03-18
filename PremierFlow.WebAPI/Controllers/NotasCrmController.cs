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
    public class NotasCrmController : ControllerBase
    {
        private readonly INotaCrmService _notaService;
        public NotasCrmController(INotaCrmService notaService)
        {
            _notaService = notaService;
        }

        private string? GetUserId() => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [HttpGet("GetByLead/{leadId}")]
        public async Task<IActionResult> GetByLead(int leadId)
        {
            var result = await _notaService.GetByLeadAsync(leadId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByOportunidad/{oportunidadId}")]
        public async Task<IActionResult> GetByOportunidad(int oportunidadId)
        {
            var result = await _notaService.GetByOportunidadAsync(oportunidadId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateNotaCrmDTO dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Usuario no autenticado." });

            var result = await _notaService.CreateAsync(dto, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("Delete/{notaCrmId}")]
        public async Task<IActionResult> Delete(int notaCrmId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Usuario no autenticado." });

            var result = await _notaService.DeleteAsync(notaCrmId, userId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
