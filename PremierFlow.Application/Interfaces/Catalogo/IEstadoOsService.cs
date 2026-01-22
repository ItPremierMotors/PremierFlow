using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Catalogo;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Interfaces.Catalogo
{
    public interface IEstadoOsService
    {
        Task<ApiResponse<EstadoOsDTO>> GetByIdAsync(int estadoId);
        Task<ApiResponse<EstadoOsDTO>> GetByCodigoAsync(string codigo);
        Task<ApiResponse<List<EstadoOsDTO>>> GetAllAsync();
        Task<ApiResponse<List<EstadoOsDTO>>> GetTransicionesValidasAsync(int estadoActualId);
    }
}
