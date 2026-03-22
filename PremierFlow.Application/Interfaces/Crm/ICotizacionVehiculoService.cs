using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Crm;

namespace PremierFlow.Application.Interfaces.Crm
{
    public interface ICotizacionVehiculoService
    {
        Task<ApiResponse<List<CotizacionVehiculoDTO>>> GetByOportunidadAsync(int oportunidadId); //obtenemos las cotizaciones de vehículos relacionadas a una oportunidad específica    
        Task<ApiResponse<CotizacionVehiculoDTO>> CreateAsync(CreateCotizacionVehiculoDTO dto, string usuarioId); //creamos una nueva cotización de vehículo a partir de un DTO con la información de la cotización y el ID del usuario que la crea
        Task<ApiResponse<bool>> AceptarAsync(int cotizacionVehiculoId, string usuarioId); //aceptamos una cotización de vehículo
        Task<ApiResponse<bool>> RechazarAsync(int cotizacionVehiculoId, string usuarioId); //rechazamos una cotización de vehículo
        Task<ApiResponse<bool>> CancelarAsync(int cotizacionVehiculoId, string usuarioId); 

    }
}
