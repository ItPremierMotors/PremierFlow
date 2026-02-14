using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Catalogo;
using PremierFlow.Application.Interfaces.Catalogo;
using System.Security.Claims;

namespace PremierFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VersionesVehiculoController : ControllerBase
    {
        private readonly IVersionVehiculoService _versionVehiculoService;
        public VersionesVehiculoController(IVersionVehiculoService versionVehiculoService)
        {
            _versionVehiculoService = versionVehiculoService;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _versionVehiculoService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetById/{versionVehiculoId}")]
        public async Task<IActionResult> GetById(int versionVehiculoId)
        {
            var result = await _versionVehiculoService.GetByIdAsync(versionVehiculoId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetByModelo/{modeloId}")]
        public async Task<IActionResult> GetByModelo(int modeloId)
        {
            var result = await _versionVehiculoService.GetByModeloAsync(modeloId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateVersionVehiculoDTO dto)
        {
            
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _versionVehiculoService.CreateAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateVersionVehiculoDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _versionVehiculoService.UpdateAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
        [HttpDelete("Delete/{versionVehiculoId}")]
        public async Task<IActionResult> Delete(int versionVehiculoId)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _versionVehiculoService.DeleteAsync(versionVehiculoId, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
    }
}
//Task<ApiResponse<VersionVehiculoDTO>> GetByIdAsync(int versionId);
//Task<ApiResponse<List<VersionVehiculoDTO>>> GetAllAsync();
//Task<ApiResponse<List<VersionVehiculoDTO>>> GetByModeloAsync(int modeloId);  // Filtrar por modelo
//Task<ApiResponse<VersionVehiculoDTO>> CreateAsync(CreateVersionVehiculoDTO dto, string usuarioId);
//Task<ApiResponse<VersionVehiculoDTO>> UpdateAsync(UpdateVersionVehiculoDTO dto, string usuarioId);
//Task<ApiResponse<bool>> DeleteAsync(int versionId, string usuarioId);