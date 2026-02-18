using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Taller;
using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Interfaces.Taller
{
    public interface ICitaService
    {
        // Consultas
        Task<ApiResponse<CitaDTO>> GetByIdAsync(int citaId);
        Task<ApiResponse<CitaDTO>> GetByCodigoAsync(string codigoCita);
        Task<ApiResponse<List<CitaDTO>>> GetAllAsync();
        Task<ApiResponse<List<CitaDTO>>> GetByFechaAsync(DateTime fecha, int? sucursalId = null);
        Task<ApiResponse<List<CitaDTO>>> GetByRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin, int? sucursalId = null);
        Task<ApiResponse<List<CitaDTO>>> GetByClienteAsync(int clienteId);
        Task<ApiResponse<List<CitaDTO>>> GetByVehiculoAsync(int vehiculoId);
        Task<ApiResponse<List<CitaDTO>>> GetByEstadoAsync(EstadoCita estado, int? sucursalId = null);
        Task<ApiResponse<List<CitaDTO>>> GetActivasDelDiaAsync(DateTime fecha, int? sucursalId = null);

        // Acciones
        Task<ApiResponse<CitaDTO>> AgendarAsync(CreateCitaDTO dto, string usuarioId);
        Task<ApiResponse<CitaDTO>> UpdateAsync(UpdateCitaDTO dto, string usuarioId);
        Task<ApiResponse<bool>> ConfirmarAsync(int citaId, string usuarioId);
        Task<ApiResponse<bool>> CancelarAsync(CancelarCitaDTO dto, string usuarioId);
        Task<ApiResponse<bool>> MarcarNoShowAsync(int citaId, string usuarioId);
        Task<ApiResponse<bool>> IniciarAtencionAsync(int citaId, string usuarioId);
        Task<ApiResponse<bool>> CompletarAsync(int citaId, string usuarioId);
        Task<ApiResponse<CitaDTO>> TransferirAsync(TransferirCitaDTO dto, string usuarioId);
    }
}
