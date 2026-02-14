using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Dtos.Catalogo;
using PremierFlow.Application.Interfaces.Catalogo;
using PremierFlow.Domain.Enums;
using System.Security.Claims;

namespace PremierFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TiposServicioController : ControllerBase
    {
        private readonly ITipoServicioService _tipoServicioService;
        public TiposServicioController(ITipoServicioService tipoServicioService)
        {
            _tipoServicioService = tipoServicioService;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _tipoServicioService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetById/{tipoServicioId}")]
        public async Task<IActionResult> GetById(int tipoServicioId)
        {
            var result = await _tipoServicioService.GetByIdAsync(tipoServicioId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByClasificacion/{clasificacion}")]
        public async Task<IActionResult> GetByClasificacion(string clasificacion)
        {
            //convertir clasificacion a enum ClasificacionServicio
            if (!Enum.TryParse<ClasificacionServicio>(clasificacion, true, out var clasificacionServicio))
                return BadRequest("Clasificación inválida");
            var result = await _tipoServicioService.GetByClasificacionAsync(clasificacionServicio);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetWalkin")]
        public async Task<IActionResult> GetWalkin()
        {
            var result = await _tipoServicioService.GetWalkInAsync();
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateTipoServicioDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _tipoServicioService.CreateAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);

        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateTipoServicioDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _tipoServicioService.UpdateAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
        [HttpDelete("Delete/{tipoServicioId}")]
        public async Task<IActionResult> Delete(int tipoServicioId)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _tipoServicioService.DeleteAsync(tipoServicioId, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
    }
}
