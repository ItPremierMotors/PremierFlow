using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Interfaces.Catalogo;

namespace PremierFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UbicacionesController : ControllerBase
    {
        private readonly IUbicacionService _ubicacionService;

        public UbicacionesController(IUbicacionService ubicacionService)
        {
            _ubicacionService = ubicacionService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _ubicacionService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetActivas")]
        public async Task<IActionResult> GetActivas()
        {
            var result = await _ubicacionService.GetActivasAsync();
            return StatusCode(result.StatusCode, result);
        }
    }
}
