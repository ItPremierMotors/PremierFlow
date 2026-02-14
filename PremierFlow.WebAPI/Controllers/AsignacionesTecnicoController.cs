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
    public class AsignacionesTecnicoController : ControllerBase
    {
        private readonly IAsignacionTecnicoService _asignacionService;

        public AsignacionesTecnicoController(IAsignacionTecnicoService asignacionService)
        {
            _asignacionService = asignacionService;
        }
        [HttpGet("GetById/{asignacionId}")]
        public async Task<IActionResult> GetById(int asignacionId)
        {
            var result = await _asignacionService.GetByIdAsync(asignacionId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByOsId/{osId}")]
        public async Task<IActionResult> GetByOsId(int osId)
        {
            var result = await _asignacionService.GetByOsIdAsync(osId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByTecnicoId/{tecnicoId}")]
        public async Task<IActionResult> GetByTecnico(int tecnicoId)
        {
            var result = await _asignacionService.GetByTecnicoIdAsync(tecnicoId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetActivasByTecnicoId/{tecnicoId}")]
        public async Task<IActionResult> GetActivasByTecnico(int tecnicoId)
        {
            var result = await _asignacionService.GetActivasByTecnicoIdAsync(tecnicoId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("Asignar")]
        public async Task<IActionResult> Asignar([FromBody] CreateAsignacionDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no esta Autenticado.");
            }
            var result = await _asignacionService.AsignarAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("iniciar/{asignacionId}")]
        public async Task<IActionResult> IniciarTrabajo(int asignacionId)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no esta Autenticado.");
            }
            var result = await _asignacionService.IniciarTrabajoAsync(asignacionId, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("pausar/{asignacionId}")]
        public async Task<IActionResult> Pausar(int asignacionId)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no esta Autenticado.");
            }
            var result = await _asignacionService.PausarAsync(asignacionId, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("reanudar/{asignacionId}")]
        public async Task<IActionResult> Reanudar(int asignacionId)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no esta Autenticado.");
            }
            var result = await _asignacionService.ReanudarAsync(asignacionId, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("completar/{asignacionId}")]
        public async Task<IActionResult> Completar(int asignacionId)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no esta Autenticado.");
            }
            var result = await _asignacionService.CompletarAsync(asignacionId, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("reasignar")]
        public async Task<IActionResult> Reasignar([FromBody] ReasignarDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no esta Autenticado.");
            }
            var result = await _asignacionService.ReasignarAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("Cancelar{asignacionId}")]
        public async Task<IActionResult> Cancelar(int asignacionId)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no esta Autenticado.");
            }
            var result = await _asignacionService.CancelarAsync(asignacionId, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
    }
}
