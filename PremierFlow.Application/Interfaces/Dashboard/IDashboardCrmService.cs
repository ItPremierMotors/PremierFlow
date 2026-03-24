
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Dashboard;

namespace PremierFlow.Application.Interfaces.Dashboard
{
    public interface IDashboardCrmService
    {
        Task<ApiResponse<DashboardLeadsDTO>> GetLeadsKpisAsync(DateTime desde, DateTime hasta, int? sucursalId);
        Task<ApiResponse<DashboardPipelineDTO>> GetPipelineKpisAsync(DateTime desde, DateTime hasta, int? sucursalId);
        Task<ApiResponse<DashboardEquipoDTO>> GetEquipoKpisAsync(DateTime desde, DateTime hasta, int? sucursalId);
        Task<ApiResponse<DashboardRetencionDTO>> GetRetencionKpisAsync(DateTime desde, DateTime hasta, int? sucursalId);
        Task<ApiResponse<DashboardActividadDTO>> GetActividadKpisAsync(DateTime desde, DateTime hasta, int? sucursalId);
        Task<ApiResponse<DashboardOrigenDTO>> GetOrigenKpisAsync(DateTime desde, DateTime hasta, int? sucursalId);
        Task<ApiResponse<DashboardTiempoDTO>> GetTiempoKpisAsync(DateTime desde, DateTime hasta, int? sucursalId);

    }
     
}