using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Dtos.Taller;
using PremierFlow.Application.Interfaces.Taller;
using System.Security.Claims;

namespace PremierFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OsServiciosController : ControllerBase
    {
        private readonly IOsServicioService _osServicioService;

        public OsServiciosController(IOsServicioService osServicioService)
        {
            _osServicioService = osServicioService;
        }

        [HttpGet("GetById{osServicioId}")]
        public async Task<IActionResult> GetById(int osServicioId)
        {
            var result = await _osServicioService.GetByIdAsync(osServicioId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByOsId/{osId}")]
        public async Task<IActionResult> GetByOsId(int osId)
        {
            var result = await _osServicioService.GetByOsIdAsync(osId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Agregar([FromBody] AgregarServicioDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no esta Autenticado.");
            }
            var result = await _osServicioService.AgregarServicioAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateOsServicioDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no esta Autenticado.");
            }
            var result = await _osServicioService.UpdateAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("QuitarServicio{osServicioId}")]
        public async Task<IActionResult> Quitar(int osServicioId)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no esta Autenticado.");
            }
            var result = await _osServicioService.QuitarServicioAsync(osServicioId, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("iniciarTrabajo/{osServicioId}")]
        public async Task<IActionResult> IniciarTrabajo(int osServicioId)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no esta Autenticado.");
            }
            var result = await _osServicioService.IniciarTrabajoAsync(osServicioId, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("completar/{osServicioId}")]
        public async Task<IActionResult> CompletarTrabajo(int osServicioId)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no esta Autenticado.");
            }
            var result = await _osServicioService.CompletarTrabajoAsync(osServicioId, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("cancelar/{osServicioId}")]
        public async Task<IActionResult> Cancelar(int osServicioId)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no esta Autenticado.");
            }
            var result = await _osServicioService.CancelarServicioAsync(osServicioId, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
    }
}
