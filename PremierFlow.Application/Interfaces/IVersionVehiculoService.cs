using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Catalogo;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Interfaces
{
    public interface IVersionVehiculoService
    {
        Task<ApiResponse<VersionVehiculoDTO>> GetByIdAsync(int versionId);
        Task<ApiResponse<List<VersionVehiculoDTO>>> GetAllAsync();
        Task<ApiResponse<List<VersionVehiculoDTO>>> GetByModeloAsync(int modeloId);  // Filtrar por modelo
        Task<ApiResponse<VersionVehiculoDTO>> CreateAsync(CreateVersionVehiculoDTO dto, string usuarioId);
        Task<ApiResponse<VersionVehiculoDTO>> UpdateAsync(UpdateVersionVehiculoDTO dto, string usuarioId);
        Task<ApiResponse<bool>> DeleteAsync(int versionId, string usuarioId);
    }
}
