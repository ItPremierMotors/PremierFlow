
using PremierFlow.Application.Common;

namespace PremierFlow.Application.Interfaces.Auth
{
    public interface IRoleClaimService
    {
        Task<ApiResponse<List<string>>> GetAllPermissionsAsync();
        Task<ApiResponse<List<string>>> GetPermissionsByRoleAsync(string roleId);

        Task<ApiResponse<bool>> SetPermissionsAsync(string roleId, List<string> permissions);
        Task<ApiResponse<bool>> AssignPermissionsAsync(string roleId, List<string> permissions);
        Task<ApiResponse<bool>> RemovePermissionsAsync(string roleId, List<string> permissions);

    }
}