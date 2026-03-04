using Microsoft.AspNetCore.Identity;
using PremierFlow.Application.Common;
using PremierFlow.Application.Interfaces.Auth;
using System.Security.Claims;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.AuthServices
{
    public class RoleClaimService : IRoleClaimService
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleClaimService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        #region READS

        public Task<ApiResponse<List<string>>> GetAllPermissionsAsync()
        {
            return Task.FromResult(
                ApiResponse<List<string>>.ok(Permissions.All, "Permisos disponibles obtenidos")
            );
        }

        public async Task<ApiResponse<List<string>>> GetPermissionsByRoleAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return ApiResponse<List<string>>.fail(404, null, "Rol no encontrado.");

            var claims = await _roleManager.GetClaimsAsync(role);

            var permissions = claims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
                .ToList();

            return ApiResponse<List<string>>.ok(permissions, $"Permisos del rol '{role.Name}' obtenidos");
        }

        #endregion

        #region WRITES

        public async Task<ApiResponse<bool>> AssignPermissionsAsync(string roleId, List<string> permissions)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return ApiResponse<bool>.fail(404, null, "Rol no encontrado.");

            if (permissions == null || !permissions.Any())
                return ApiResponse<bool>.fail(400, null, "Debe proporcionar al menos un permiso.");

            // Validar que todos los permisos sean válidos
            var invalid = permissions.Where(p => !Permissions.All.Contains(p)).ToList();
            if (invalid.Any())
                return ApiResponse<bool>.fail(400, null, $"Permisos no válidos: {string.Join(", ", invalid)}");

            // Obtener permisos actuales para no duplicar
            var currentClaims = await _roleManager.GetClaimsAsync(role);
            var currentPermissions = currentClaims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
                .ToList();

            var toAdd = permissions.Where(p => !currentPermissions.Contains(p)).ToList();

            foreach (var permission in toAdd)
            {
                await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            }

            return ApiResponse<bool>.ok(true, $"Se asignaron {toAdd.Count} permiso(s) al rol '{role.Name}'");
        }

        public async Task<ApiResponse<bool>> RemovePermissionsAsync(string roleId, List<string> permissions)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return ApiResponse<bool>.fail(404, null, "Rol no encontrado.");

            if (permissions == null || !permissions.Any())
                return ApiResponse<bool>.fail(400, null, "Debe proporcionar al menos un permiso.");

            var currentClaims = await _roleManager.GetClaimsAsync(role);
            var toRemove = currentClaims
                .Where(c => c.Type == "Permission" && permissions.Contains(c.Value))
                .ToList();

            foreach (var claim in toRemove)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            return ApiResponse<bool>.ok(true, $"Se removieron {toRemove.Count} permiso(s) del rol '{role.Name}'");
        }

        public async Task<ApiResponse<bool>> SetPermissionsAsync(string roleId, List<string> permissions)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return ApiResponse<bool>.fail(404, null, "Rol no encontrado.");

            // Validar que todos los permisos sean válidos
            var invalid = permissions.Where(p => !Permissions.All.Contains(p)).ToList();
            if (invalid.Any())
                return ApiResponse<bool>.fail(400, null, $"Permisos no válidos: {string.Join(", ", invalid)}");

            // Eliminar todos los permisos actuales
            var currentClaims = await _roleManager.GetClaimsAsync(role);
            var permissionClaims = currentClaims.Where(c => c.Type == "Permission").ToList();

            foreach (var claim in permissionClaims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            // Agregar los nuevos
            foreach (var permission in permissions.Distinct())
            {
                await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            }

            return ApiResponse<bool>.ok(true, $"Se establecieron {permissions.Count} permiso(s) para el rol '{role.Name}'");
        }

        #endregion
    }
}
