using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Taller;
using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Interfaces.Taller
{
    public interface IBloqueHorarioService
    {
        Task<ApiResponse<BloqueHorarioDTO>> GetByIdAsync(int bloqueId);
        Task<ApiResponse<List<BloqueHorarioDTO>>> GetByCapacidadIdAsync(int capacidadId);
        Task<ApiResponse<List<BloqueHorarioDTO>>> GetByFechaAsync(DateTime fecha, int? sucursalId = null);
        Task<ApiResponse<List<BloqueHorarioDTO>>> GetDisponiblesByFechaAsync(DateTime fecha, int? sucursalId = null);
        Task<ApiResponse<List<BloqueHorarioDTO>>> GetDisponiblesByFechaYTipoAsync(DateTime fecha, TipoBloqueHorario tipo, int? sucursalId = null);
        Task<ApiResponse<BloqueHorarioDTO>> CreateAsync(CreateBloqueHorarioDTO dto, string usuarioId);
        Task<ApiResponse<BloqueHorarioDTO>> UpdateAsync(UpdateBloqueHorarioDTO dto, string usuarioId);
        Task<ApiResponse<bool>> DeleteAsync(int bloqueId, string usuarioId);
        Task<ApiResponse<bool>> AgendarVehiculoAsync(int bloqueId);
        Task<ApiResponse<bool>> LiberarEspacioAsync(int bloqueId);
        Task<ApiResponse<List<BloqueHorarioDTO>>> GenerarBloquesAutomaticosAsync(GenerarBloquesDTO dto, string usuarioId);
    }
}
