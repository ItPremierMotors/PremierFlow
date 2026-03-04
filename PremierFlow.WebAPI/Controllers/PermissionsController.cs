using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Interfaces.Auth;

namespace PremierFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "AdminTI")]
    public class PermissionsController : ControllerBase
    {
        private readonly IRoleClaimService _roleClaimService;

        public PermissionsController(IRoleClaimService roleClaimService)
        {
            _roleClaimService = roleClaimService;
        }

        /// <summary>
        /// Obtiene todos los permisos disponibles en el sistema
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _roleClaimService.GetAllPermissionsAsync();
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Obtiene los permisos asignados a un rol
        /// </summary>
        [HttpGet("role/{roleId}")]
        public async Task<IActionResult> GetByRole(string roleId)
        {
            var result = await _roleClaimService.GetPermissionsByRoleAsync(roleId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Establece los permisos de un rol (reemplaza todos los existentes)
        /// </summary>
        [HttpPut("role/{roleId}")]
        public async Task<IActionResult> SetPermissions(string roleId, [FromBody] List<string> permissions)
        {
            var result = await _roleClaimService.SetPermissionsAsync(roleId, permissions);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Agrega permisos a un rol (sin quitar los existentes)
        /// </summary>
        [HttpPost("role/{roleId}")]
        public async Task<IActionResult> AssignPermissions(string roleId, [FromBody] List<string> permissions)
        {
            var result = await _roleClaimService.AssignPermissionsAsync(roleId, permissions);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Remueve permisos específicos de un rol
        /// </summary>
        [HttpDelete("role/{roleId}")]
        public async Task<IActionResult> RemovePermissions(string roleId, [FromBody] List<string> permissions)
        {
            var result = await _roleClaimService.RemovePermissionsAsync(roleId, permissions);
            return StatusCode(result.StatusCode, result);
        }
    }
}
