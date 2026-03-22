using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Dtos.Crm;
using PremierFlow.Application.Dtos.User;
using PremierFlow.Application.Interfaces.Auth;
using PremierFlow.Application.Interfaces.Crm;
using System.Security.Claims;

namespace PremierFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VendedoresSucursalController : ControllerBase
    {
        private readonly IVendedorSucursalService service;
        public VendedoresSucursalController(IVendedorSucursalService service)
        {
            this.service = service;
        }

        private string? GetUserId() => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [HttpPost("Asignar")]
        public async Task<IActionResult> Asignar([FromBody] AsignarVendedorSucursalDTO dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var result = await service.AsignarAsync(dto, userId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPut("Desactivar/{vendedorSucursalId}")]
        public async Task<IActionResult> Desactivar(int vendedorSucursalId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var result = await service.DesactivarAsync(vendedorSucursalId, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Activar/{vendedorSucursalId}")]
        public async Task<IActionResult> Activar(int vendedorSucursalId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var result = await service.ActivarAsync(vendedorSucursalId, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetBySucursal/{sucursalId}")]
        public async Task<IActionResult> GetBySucursal(int sucursalId)
        {
            var result = await service.GetBySucursalAsync(sucursalId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetByVendedor/{vendedorId}")]
        public async Task<IActionResult> GetByVendedor(string vendedorId)
        {
            var result = await service.GetByVendedorAsync(vendedorId);
            return StatusCode(result.StatusCode, result);
        }

    }
}