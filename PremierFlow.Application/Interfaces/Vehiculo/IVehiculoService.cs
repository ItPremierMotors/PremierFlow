using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Vehiculos;
using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Interfaces.Vehiculo
{
    public interface IVehiculoService
    {
        Task<ApiResponse<VehiculoDTO>> GetByIdAsync(int vehiculoId);
        Task<ApiResponse<VehiculoDetalleDTO>> GetDetalleByIdAsync(int vehiculoId);
        Task<ApiResponse<List<VehiculoDTO>>> GetAllAsync();
        Task<ApiResponse<List<VehiculoDTO>>> GetByClienteAsync(int clienteId);
        Task<ApiResponse<List<VehiculoDTO>>> GetByEstadoAsync(EstadoVehiculo estado);
        Task<ApiResponse<List<VehiculoDTO>>> GetBySucursalAsync(int sucursalId);
        Task<ApiResponse<VehiculoDTO>> GetByVinAsync(string vin);
        Task<ApiResponse<VehiculoDTO>> GetByPlacaAsync(string placa);
        Task<ApiResponse<List<VehiculoDTO>>> SearchAsync(string term);
        Task<ApiResponse<List<VehiculoDTO>>> GetDisponiblesParaVentaAsync();
        Task<ApiResponse<VehiculoDTO>> CreateAsync(CreateVehiculoDTO dto, string usuarioId);
        Task<ApiResponse<VehiculoDTO>> UpdateAsync(UpdateVehiculoDTO dto, string usuarioId);
        Task<ApiResponse<bool>> DeleteAsync(int vehiculoId, string usuarioId);
        Task<ApiResponse<bool>> ActualizarKilometrajeAsync(int vehiculoId, int nuevoKm, string usuarioId);
        Task<ApiResponse<bool>> CambiarEstadoAsync(int vehiculoId, EstadoVehiculo nuevoEstado, string usuarioId, int? clienteId = null);
        Task<ApiResponse<int>> CancelarReservasVencidasAsync(string usuarioId);
    }
}
