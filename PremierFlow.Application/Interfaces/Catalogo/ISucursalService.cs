using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Catalogo;

namespace PremierFlow.Application.Interfaces.Catalogo
{
    public interface ISucursalService
    {
        Task<ApiResponse<List<SucursalDTO>>> GetAllAsync();
        Task<ApiResponse<SucursalDTO>> GetByIdAsync(int sucursalId);
    }
}
