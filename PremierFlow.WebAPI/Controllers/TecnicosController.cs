using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Dtos.Catalogo;
using PremierFlow.Application.Interfaces.Catalogo;
using PremierFlow.Domain.Entities;
using System.Security.Claims;

namespace PremierFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TecnicosController : ControllerBase
    {
        private readonly ITecnicoService _tecnicoService;
        public TecnicosController(ITecnicoService tecnicoService)
        {
               _tecnicoService = tecnicoService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _tecnicoService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetById/{tecnicoId}")]
        public async Task<IActionResult> GetById(int tecnicoId)
        {
            var result = await _tecnicoService.GetByIdAsync(tecnicoId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetByUsuarioId/{usuarioId}")]
        public async Task<IActionResult> GetByUsuarioId(string usuarioId)
        {
            var result = await _tecnicoService.GetByUsuarioIdAsync(usuarioId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetBySucursal/{sucursalId}")]
        public async Task<IActionResult> GetBySucursal(int sucursalId)
        {
            var result = await _tecnicoService.GetBySucursalAsync(sucursalId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetDisponibles/{sucursalId}")]
        public async Task<IActionResult> GetDisponibles(int sucursalId)
        {
            var result = await _tecnicoService.GetDisponiblesAsync(sucursalId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateTecnicoDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _tecnicoService.CreateAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update(UpdateTecnicoDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _tecnicoService.UpdateAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("Delete/{tecnicoId}")]
        public async Task<IActionResult> Delete(int tecnicoId)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _tecnicoService.DeleteAsync(tecnicoId, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
    }
}
