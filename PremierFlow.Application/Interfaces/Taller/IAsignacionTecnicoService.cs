using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Taller;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Interfaces.Taller
{
    public interface IAsignacionTecnicoService
    {
        Task<ApiResponse<AsignacionTecnicoDTO>> GetByIdAsync(int asignacionId);
        Task<ApiResponse<List<AsignacionTecnicoDTO>>> GetByOsIdAsync(int osId);
        Task<ApiResponse<List<AsignacionTecnicoDTO>>> GetByTecnicoIdAsync(int tecnicoId);
        Task<ApiResponse<List<AsignacionTecnicoDTO>>> GetActivasByTecnicoIdAsync(int tecnicoId);
        Task<ApiResponse<AsignacionTecnicoDTO>> AsignarAsync(CreateAsignacionDTO dto, string usuarioId);
        Task<ApiResponse<bool>> IniciarTrabajoAsync(int asignacionId, string usuarioId);
        Task<ApiResponse<bool>> PausarAsync(int asignacionId, string usuarioId);
        Task<ApiResponse<bool>> ReanudarAsync(int asignacionId, string usuarioId);
        Task<ApiResponse<bool>> CompletarAsync(int asignacionId, string usuarioId);
        Task<ApiResponse<AsignacionTecnicoDTO>> ReasignarAsync(ReasignarDTO dto, string usuarioId);
        Task<ApiResponse<bool>> CancelarAsync(int asignacionId, string usuarioId);
    }
}
