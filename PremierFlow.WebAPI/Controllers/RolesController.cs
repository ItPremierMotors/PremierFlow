using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PremierFlow.Application.Dtos.User;
using PremierFlow.Application.Interfaces.Auth;

namespace PremierFlow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "AdminTI")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        #region READS

        /// <summary>
        /// Obtiene todos los roles del sistema
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _roleService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Obtiene un rol por su ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _roleService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Obtiene un rol por su nombre
        /// </summary>
        [HttpGet("by-name/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var result = await _roleService.GetByNameAsync(name);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Verifica si un rol existe
        /// </summary>
        [HttpGet("exists/{roleName}")]
        public async Task<IActionResult> RoleExists(string roleName)
        {
            var result = await _roleService.RoleExistsAsync(roleName);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Obtiene los roles de un usuario específico
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var result = await _roleService.GetUserRolesAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        #endregion

        #region WRITES

        /// <summary>
        /// Crea un nuevo rol
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
        {
            var result = await _roleService.CreateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Actualiza el nombre de un rol
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateRoleRequest request)
        {
            var result = await _roleService.UpdateAsync(id, request);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Elimina un rol
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _roleService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        #endregion

        #region ASIGNACIÓN DE ROLES

        /// <summary>
        /// Asigna roles adicionales a un usuario (sin quitar los existentes)
        /// </summary>
        [HttpPost("assign/{userId}")]
        public async Task<IActionResult> AssignRoles(string userId, [FromBody] List<string> roles)
        {
            var result = await _roleService.AssignRolesToUserAsync(userId, roles);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Remueve roles específicos de un usuario
        /// </summary>
        [HttpPost("remove/{userId}")]
        public async Task<IActionResult> RemoveRoles(string userId, [FromBody] List<string> roles)
        {
            var result = await _roleService.RemoveRolesFromUserAsync(userId, roles);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Establece los roles de un usuario (reemplaza todos los existentes)
        /// </summary>
        [HttpPut("set/{userId}")]
        public async Task<IActionResult> SetUserRoles(string userId, [FromBody] List<string> roles)
        {
            var result = await _roleService.SetUserRolesAsync(userId, roles);
            return StatusCode(result.StatusCode, result);
        }

        #endregion
    }
}
