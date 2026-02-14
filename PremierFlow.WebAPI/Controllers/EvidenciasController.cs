using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Dtos.Taller;
using PremierFlow.Application.Interfaces.Taller;
using PremierFlow.Domain.Entities;
using PremierFlow.Domain.Enums;
using System.Security.Claims;

namespace PremierFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EvidenciasController : ControllerBase
    {
        private readonly IEvidenciaService _evidenciaService;

        public EvidenciasController(IEvidenciaService evidenciaService)
        {
            _evidenciaService = evidenciaService;
        }
        [HttpGet("GetById/{evidenciaId}")]
        public async Task<IActionResult> GetById(int evidenciaId)
        {
            var result = await _evidenciaService.GetByIdAsync(evidenciaId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByOsId/{osId}")]
        public async Task<IActionResult> GetByOsId(int osId)
        {
            var result = await _evidenciaService.GetByOsIdAsync(osId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByRecepcionId/{recepcionId}")]
        public async Task<IActionResult> GetByRecepcion(int recepcionId)
        {
            var result = await _evidenciaService.GetByRecepcionIdAsync(recepcionId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByTipo/{osId}/tipo/{tipo}")]
        public async Task<IActionResult> GetByTipo(int osId, TipoEvidencia tipo)
        {
            if (!Enum.IsDefined(typeof(TipoEvidencia),tipo))
                return BadRequest("Tipo Evidencia inválido");
            var result = await _evidenciaService.GetByTipoAsync(osId, tipo);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetFotosDano/{osId}")]
        public async Task<IActionResult> GetFotosDano(int osId)
        {
            var result = await _evidenciaService.GetFotosDanoAsync(osId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] CreateEvidenciaDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Enum.IsDefined(typeof(TipoEvidencia), dto.TipoEvidencia))
                return BadRequest("Tipo Evidencia inválido");
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _evidenciaService.AgregarAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("base64")]
        public async Task<IActionResult> AgregarBase64([FromBody] CreateEvidenciaBase64DTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _evidenciaService.AgregarBase64Async(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("Delete/{evidenciaId}")]
        public async Task<IActionResult> Eliminar(int evidenciaId)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _evidenciaService.EliminarAsync(evidenciaId, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

    }
}
