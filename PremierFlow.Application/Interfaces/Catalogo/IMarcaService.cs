using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Marcas;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Interfaces.Catalogo
{
    public interface IMarcaService
    {
        Task<ApiResponse<MarcaDTO>  > GetByIdAsync(int marcaId);
        Task<ApiResponse<List<MarcaDTO>>> GetAllAsync();
        Task<ApiResponse<MarcaDTO>> CreateAsync(MarcaDTO marcaDto, string UsuarioID);
        Task<ApiResponse<MarcaDTO>> UpdateAsync(UpdateMarcaDTO marcaDto, string UsuarioID);
        Task<ApiResponse<bool>> DeleteAsync(int marcaId, string UsuarioID);

    }
}
