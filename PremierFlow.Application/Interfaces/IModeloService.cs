using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Catalogo;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Interfaces
{
    public interface IModeloService
    {
        Task<ApiResponse<ModeloDTO>> GetByIdAsync(int modeloId);
        Task<ApiResponse<List<ModeloDTO>>> GetAllAsync();
        Task<ApiResponse<List<ModeloDTO>>> GetByMarcaAsync(int marcaId);  // ← Este es para obtener modelos por marca
        Task<ApiResponse<ModeloDTO>> CreateAsync(ModeloDTO dto, string usuarioId);
        Task<ApiResponse<ModeloDTO>> UpdateAsync(UpdateModeloDTO dto, string usuarioId);
        Task<ApiResponse<bool>> DeleteAsync(int modeloId, string usuarioId);
    }
}
