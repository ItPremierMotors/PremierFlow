using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Interfaces.Catalogo;

namespace PremierFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SucursalesController : ControllerBase
    {
        private readonly ISucursalService _sucursalService;

        public SucursalesController(ISucursalService sucursalService)
        {
            _sucursalService = sucursalService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _sucursalService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetById/{sucursalId}")]
        public async Task<IActionResult> GetById(int sucursalId)
        {
            var result = await _sucursalService.GetByIdAsync(sucursalId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
