using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Dtos.Catalogo;
using PremierFlow.Application.Interfaces.Catalogo;
using System.Security.Claims;

namespace PremierFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ModelosController : ControllerBase
    {
        private readonly IModeloService _modeloService;
        public ModelosController(IModeloService modeloService)
        {
            _modeloService = modeloService;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _modeloService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetById/{modeloId}")]
        public async Task<IActionResult> GetById(int modeloId)
        {
            var result = await _modeloService.GetByIdAsync(modeloId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetByMarca/{marcaId}")]
        public async Task<IActionResult> GetByMarca(int marcaId)
        {
            var result = await _modeloService.GetByMarcaAsync(marcaId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] ModeloDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _modeloService.CreateAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateModeloDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _modeloService.UpdateAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("Delete/{modeloId}")]
        public async Task<IActionResult> Delete(int modeloId)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _modeloService.DeleteAsync(modeloId, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
    }
}