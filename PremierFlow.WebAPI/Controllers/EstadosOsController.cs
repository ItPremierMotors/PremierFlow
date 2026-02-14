using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Interfaces.Catalogo;

namespace PremierFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EstadosOsController : ControllerBase
    {
        private readonly IEstadoOsService _estadoOsService;
        public EstadosOsController(IEstadoOsService estadoOsService)
        {
            _estadoOsService = estadoOsService;
        }
        [HttpGet("GetAlL")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _estadoOsService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetById{estadoId}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _estadoOsService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetByCodigo{codigo}")]
        public async Task<IActionResult> GetByCodigo(string codigo)
        {
            if(string.IsNullOrWhiteSpace(codigo))
            {
                return BadRequest("El código no puede estar vacío.");
            }
            var result = await _estadoOsService.GetByCodigoAsync(codigo);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetTransicionesValidas{estadoActualId}")]
        public async Task<IActionResult> GetTransicionesValidas(int estadoActualId)
        {
            var result = await _estadoOsService.GetTransicionesValidasAsync(estadoActualId);
            return StatusCode(result.StatusCode, result);
        }


    }

}
