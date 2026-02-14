using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Dtos.Marcas;
using PremierFlow.Application.Interfaces.Catalogo;
using System.Security.Claims;

namespace PremierFlow.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MarcasController : ControllerBase
    {
        private readonly IMarcaService _marcaService;
        public MarcasController(IMarcaService marcaService)
        {
            _marcaService = marcaService;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _marcaService.GetAllAsync();

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetById/{marcaId}")]
        public async Task<IActionResult> GetById(int marcaId)
        {
            var result = await _marcaService.GetByIdAsync(marcaId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] MarcaDTO marcaDto)
        {
            // ✅ Opción 1: Usar ClaimTypes.NameIdentifier (recomendado en ASP.NET Core)
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // ✅ Opción 2: Usar "sub" (estándar JWT)
            // var userIdFromToken = User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _marcaService.CreateAsync(marcaDto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateMarcaDTO marcaDto)
        {
            // Obtener el ID del usuario desde el token JWT
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _marcaService.UpdateAsync(marcaDto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("Delete/{marcaId}")]
        public async Task<IActionResult> Delete(int marcaId)
        {
            // Obtener el ID del usuario desde el token JWT
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _marcaService.DeleteAsync(marcaId, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
    }
}
