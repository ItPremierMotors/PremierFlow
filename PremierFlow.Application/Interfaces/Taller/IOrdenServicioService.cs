using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Catalogo;
using PremierFlow.Application.Dtos.Taller;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Interfaces.Taller
{
    public interface IOrdenServicioService
    {
        // Consultas
        Task<ApiResponse<OrdenServicioDTO>> GetByIdAsync(int osId);
        Task<ApiResponse<OrdenServicioDTO>> GetByNumeroAsync(string numeroOs);
        Task<ApiResponse<OrdenServicioDetalleDTO>> GetDetalleByIdAsync(int osId);
        Task<ApiResponse<List<OrdenServicioDTO>>> GetAllAsync();
        Task<ApiResponse<List<OrdenServicioDTO>>> GetAbiertasAsync(int? sucursalId = null);
        Task<ApiResponse<List<OrdenServicioDTO>>> GetByFechaAsync(DateTime fecha, int? sucursalId = null);
        Task<ApiResponse<List<OrdenServicioDTO>>> GetByRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin, int? sucursalId = null);
        Task<ApiResponse<List<OrdenServicioDTO>>> GetByClienteAsync(int clienteId);
        Task<ApiResponse<List<OrdenServicioDTO>>> GetByVehiculoAsync(int vehiculoId);
        Task<ApiResponse<List<OrdenServicioDTO>>> GetByEstadoAsync(int estadoId, int? sucursalId = null);

        // Acciones
        Task<ApiResponse<OrdenServicioDTO>> CreateFromCitaAsync(CreateOsFromCitaDTO dto, string usuarioId);
        Task<ApiResponse<OrdenServicioDTO>> CreateWalkInAsync(CreateOsWalkInDTO dto, string usuarioId);
        Task<ApiResponse<OrdenServicioDTO>> UpdateAsync(UpdateOrdenServicioDTO dto, string usuarioId);
        Task<ApiResponse<bool>> CambiarEstadoAsync(CambiarEstadoOsDTO dto, string usuarioId);
        Task<ApiResponse<bool>> CerrarAsync(CerrarOsDTO dto, string usuarioId);
        Task<ApiResponse<bool>> CancelarAsync(int osId, string motivo, string usuarioId);
        Task<ApiResponse<bool>> RecalcularTotalesAsync(int osId);
        Task<ApiResponse<List<EstadoOsDTO>>> GetTransicionesValidasAsync(int osId);
    }
}
