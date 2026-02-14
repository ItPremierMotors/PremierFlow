using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Taller;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Interfaces.Taller
{
    public interface ICapacidadTallerService
    {
        Task<ApiResponse<CapacidadTallerDTO>> GetByIdAsync(int capacidadId);
        Task<ApiResponse<CapacidadTallerDTO>> GetByFechaAsync(DateTime fecha, int? sucursalId = null);
        Task<ApiResponse<List<CapacidadTallerDTO>>> GetByRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin, int? sucursalId = null);
        Task<ApiResponse<List<CapacidadTallerDTO>>> GetBySucursalAsync(int sucursalId);
        Task<ApiResponse<CapacidadTallerDTO>> CreateAsync(CreateCapacidadTallerDTO dto, string usuarioId);
        Task<ApiResponse<CapacidadTallerDTO>> UpdateAsync(UpdateCapacidadTallerDTO dto, string usuarioId);
        Task<ApiResponse<bool>> BloquearDiaAsync(int capacidadId, string motivo, string usuarioId);
        Task<ApiResponse<bool>> DesbloquearDiaAsync(int capacidadId, string usuarioId);
        Task<ApiResponse<bool>> TieneCapacidadAsync(DateTime fecha, int minutosRequeridos, int? sucursalId = null);
        Task<ApiResponse<bool>> ReservarMinutosAsync(int capacidadId, int minutos);
        Task<ApiResponse<bool>> LiberarMinutosAsync(int capacidadId, int minutos);
        Task<ApiResponse<bool>> RegistrarTiempoTrabajadoAsync(int capacidadId, int minutos);
        Task<ApiResponse<List<CapacidadTallerDTO>>> GenerarCapacidadSemanalAsync(DateTime fechaInicio, CreateCapacidadTallerDTO plantilla, string usuarioId);
    }
}
