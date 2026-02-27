using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Catalogo;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Interfaces.Catalogo
{
    public interface ITecnicoService
    {
        Task<ApiResponse<TecnicoDTO>> GetByIdAsync(int tecnicoId);
        Task<ApiResponse<TecnicoDTO>> GetByUsuarioIdAsync(string usuarioId);
        Task<ApiResponse<List<TecnicoDTO>>> GetAllAsync();
        Task<ApiResponse<List<TecnicoDTO>>> GetBySucursalAsync(int sucursalId);  // Filtrar por sucursal
        Task<ApiResponse<List<TecnicoDTO>>> GetDisponiblesAsync(int sucursalId); // Sin OS activa
        Task<ApiResponse<TecnicoDTO>> CreateAsync(CreateTecnicoDTO dto, string usuarioId);
        Task<ApiResponse<TecnicoDTO>> UpdateAsync(UpdateTecnicoDTO dto, string usuarioId);
        Task<ApiResponse<bool>> DeleteAsync(int tecnicoId, string usuarioId);
    }
}
