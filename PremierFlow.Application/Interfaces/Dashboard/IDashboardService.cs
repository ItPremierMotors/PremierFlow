using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Dashboard;

namespace PremierFlow.Application.Interfaces.Dashboard
{
    public interface IDashboardService
    {
        Task<ApiResponse<DashboardResumenDTO>> GetResumenAsync();
        Task<ApiResponse<DashboardTallerDTO>> GetTallerAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<ApiResponse<DashboardVentasDTO>> GetVentasAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<ApiResponse<DashboardInventarioDTO>> GetInventarioAsync();
    }
}
