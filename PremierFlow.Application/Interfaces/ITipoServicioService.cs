using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Catalogo;
using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Interfaces
{
    public interface ITipoServicioService
    {
        Task<ApiResponse<TipoServicioDTO>> GetByIdAsync(int tipoServicioId);
        Task<ApiResponse<List<TipoServicioDTO>>> GetAllAsync();
        Task<ApiResponse<List<TipoServicioDTO>>> GetByClasificacionAsync(ClasificacionServicio clasificacion);
        Task<ApiResponse<List<TipoServicioDTO>>> GetWalkInAsync();  // Solo los que permiten walk-in
        Task<ApiResponse<TipoServicioDTO>> CreateAsync(CreateTipoServicioDTO dto, string usuarioId);
        Task<ApiResponse<TipoServicioDTO>> UpdateAsync(UpdateTipoServicioDTO dto, string usuarioId);
        Task<ApiResponse<bool>> DeleteAsync(int tipoServicioId, string usuarioId);
    }
}
