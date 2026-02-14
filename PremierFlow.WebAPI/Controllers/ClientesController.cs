using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Dtos.Cliente;
using PremierFlow.Application.Interfaces.Cliente;
using PremierFlow.Domain.Enums;
using System.Security.Claims;

namespace PremierFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _clienteService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetById/{ClienteId}")]
        public async Task<IActionResult> GetById(int ClienteId)
        {
            var result =await _clienteService.GetByIdAsync(ClienteId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetByDNI/{ClienteDNI}")]
        public async Task<IActionResult> GetByDNI(String ClienteDNI)
        {
            var result = await _clienteService.GetByDNIAsync(ClienteDNI);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetByRTN/{ClienteRTN}")]
        public async Task<IActionResult> GetByRTN(String ClienteRTN)
        {
            var result = await _clienteService.GetByRTNAsync(ClienteRTN);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetByTelefono/{TelefonoCliente}")]
        public async Task<IActionResult> GetByTelefono(String TelenoCliente)
        {
            var result = await _clienteService.GetByTelefonoAsync(TelenoCliente);
            return StatusCode(result.StatusCode, result);
        }
       [HttpGet("Search/{termino}")]
        public async Task<IActionResult> Search(String termino)
        {
            var result = await _clienteService.SearchAsync(termino);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateClienteDTO Cliente)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //validar que el enum exista
            if (!Enum.IsDefined(typeof(TipoCliente), Cliente.TipoCliente))
                return BadRequest("Tipo de cliente inválido");
            //Validacion tipo de cliente
                //persona natural
            if (Cliente.TipoCliente == TipoCliente.Persona &&
                 string.IsNullOrWhiteSpace(Cliente.Apellidos))
                return BadRequest("Apellidos requeridos para persona natural");
            //empresa
            if (Cliente.TipoCliente == TipoCliente.Empresa
                && String.IsNullOrEmpty(Cliente.RTN))
                return BadRequest("Rtn es requerido para las empresas");
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado." });
            }
            var result = await _clienteService.CreateAsync(Cliente, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateClienteDTO Cliente)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Message = "Usuario no autenticado. " });
            }
            var result=await _clienteService.UpdateAsync(Cliente, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("Delete/{ClienteId}")]
        public async Task<IActionResult> Delete(int ClienteId)
        {
            var userIdFromToken = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { Messaje = "Usuario no autenticado. " });
            }
            var result =await _clienteService.DeleteAsync(ClienteId, userIdFromToken);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("{ClienteId}/noshow")]
        public async Task<IActionResult> IncrementarNoShow(int ClienteId)
        {
           
            var result = await _clienteService.IncrementarNoShowAsync(ClienteId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
