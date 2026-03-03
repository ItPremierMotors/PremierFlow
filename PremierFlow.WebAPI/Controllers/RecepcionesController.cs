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
    public class RecepcionesController : ControllerBase
    {
        private readonly IRecepcionService _recepcionService;

        public RecepcionesController(IRecepcionService recepcionService)
        {
            _recepcionService = recepcionService;
        }
        [HttpGet("GetById/{recepcionId}")]
        public async Task<IActionResult> GetById(int recepcionId)
        {
            var result = await _recepcionService.GetByIdAsync(recepcionId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetByOs/{recepcionId}")]
        public async Task<IActionResult> GetByOs(int recepcionId)
        {
            var result = await _recepcionService.GetByOsIdAsync(recepcionId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetPendientesFirma")]
        public async Task<IActionResult> GetPendientesFirma([FromQuery] int? sucursalId = null)
        {
            var result = await _recepcionService.GetPendientesFirmaAsync(sucursalId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRecepcionDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no esta Autenticado.");
            }
            var result = await _recepcionService.CreateAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateRecepcionDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no esta Autenticado.");
            }
            var result = await _recepcionService.UpdateAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("completar-checklist/{recepcionId}")]
        public async Task<IActionResult> CompletarChecklist(int recepcionId)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no esta Autenticado.");
            }
            var result = await _recepcionService.CompletarChecklistAsync(recepcionId, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("firma")]
        public async Task<IActionResult> RegistrarFirma([FromBody] RegistrarFirmaDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no esta Autenticado.");
            }
            var result = await _recepcionService.RegistrarFirmaAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("DatosCita/{citaId}")]
        public async Task<IActionResult> GetDatosCita(int citaId)
        {
            var result = await _recepcionService.GetDatosCitaAsync(citaId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("IniciarDesdeCita")]
        public async Task<IActionResult> IniciarDesdeCita([FromBody] IniciarRecepcionDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no esta Autenticado.");
            }
            var result = await _recepcionService.IniciarDesdeCitaAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("IniciarWalkIn")]
        public async Task<IActionResult> IniciarWalkIn([FromBody] IniciarRecepcionWalkInDTO dto)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized("Usuario no esta Autenticado.");
            }
            var result = await _recepcionService.IniciarWalkInAsync(dto, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
    }
}
