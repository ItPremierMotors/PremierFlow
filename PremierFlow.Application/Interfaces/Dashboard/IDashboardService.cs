using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Dashboard;

namespace PremierFlow.Application.Interfaces.Dashboard
{
    public interface IDashboardService
    {
        Task<ApiResponse<DashboardResumenDTO>> GetResumenAsync(int? sucursalId = null);
        Task<ApiResponse<DashboardTallerDTO>> GetTallerAsync(DateTime fechaInicio, DateTime fechaFin, int? sucursalId = null);
        Task<ApiResponse<DashboardVentasDTO>> GetVentasAsync(DateTime fechaInicio, DateTime fechaFin, int? sucursalId = null);
        Task<ApiResponse<DashboardInventarioDTO>> GetInventarioAsync(int? sucursalId = null);
    }
}
