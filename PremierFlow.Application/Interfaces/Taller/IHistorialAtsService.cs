using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Taller;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PremierFlow.Application.Interfaces.Taller
{
    public interface IHistorialAtsService
    {
        Task<ApiResponse<HistorialAtsDTO>> GetByIdAsync(int atsId);
        Task<ApiResponse<HistorialAtsDTO>> GetByOsIdAsync(int osId);
        Task<ApiResponse<List<HistorialAtsDTO>>> GetByVehiculoIdAsync(int vehiculoId);
        Task<ApiResponse<List<HistorialAtsDTO>>> GetByVinAsync(string vin);
        Task<ApiResponse<List<HistorialAtsDTO>>> GetByPlacaAsync(string placa);
        Task<ApiResponse<List<HistorialAtsDTO>>> GetConRecomendacionPendienteAsync();
        Task<ApiResponse<HistorialAtsDTO>> CrearDesdeOsAsync(CreateHistorialAtsDTO dto, string usuarioId);
    }
}
