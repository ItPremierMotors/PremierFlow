using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Catalogo;

namespace PremierFlow.Application.Interfaces.Catalogo
{
    public interface IUbicacionService
    {
        Task<ApiResponse<List<UbicacionDTO>>> GetAllAsync();
        Task<ApiResponse<List<UbicacionDTO>>> GetActivasAsync();
    }
}
