using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Taller;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Interfaces.Taller
{
    public interface IOsServicioService
    {
        Task<ApiResponse<OsServicioDTO>> GetByIdAsync(int osServicioId);
        Task<ApiResponse<List<OsServicioDTO>>> GetByOsIdAsync(int osId);
        Task<ApiResponse<OsServicioDTO>> AgregarServicioAsync(AgregarServicioDTO dto, string usuarioId);
        Task<ApiResponse<OsServicioDTO>> UpdateAsync(UpdateOsServicioDTO dto, string usuarioId);
        Task<ApiResponse<bool>> QuitarServicioAsync(int osServicioId, string usuarioId);
        Task<ApiResponse<bool>> IniciarTrabajoAsync(int osServicioId, string usuarioId);
        Task<ApiResponse<bool>> CompletarTrabajoAsync(int osServicioId, string usuarioId);
        Task<ApiResponse<bool>> CancelarServicioAsync(int osServicioId, string usuarioId);
    }
}
