using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Interfaces.Auth
{
    public interface IRoleService
    {
        // READS
        Task<ApiResponse<List<RoleDTO>>> GetAllAsync();
        Task<ApiResponse<RoleDTO>> GetByIdAsync(string id);
        Task<ApiResponse<RoleDTO>> GetByNameAsync(string name);
        Task<ApiResponse<List<string>>> GetUserRolesAsync(string userId);
        Task<ApiResponse<bool>> RoleExistsAsync(string roleName);

        /// WRITES
        Task<ApiResponse<string>> CreateAsync(CreateRoleRequest request);
        Task<ApiResponse<bool>> UpdateAsync(string id, UpdateRoleRequest request);
        Task<ApiResponse<bool>> DeleteAsync(string id);

        // ASIGNACIÓN DE ROLES A USUARIOS
        Task<ApiResponse<bool>> AssignRolesToUserAsync(string userId, List<string> roles);
        Task<ApiResponse<bool>> RemoveRolesFromUserAsync(string userId, List<string> roles);
        Task<ApiResponse<bool>> SetUserRolesAsync(string userId, List<string> roles);
    }
}
