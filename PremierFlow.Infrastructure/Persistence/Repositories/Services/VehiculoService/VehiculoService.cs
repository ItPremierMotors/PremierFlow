using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Vehiculos;
using PremierFlow.Application.Interfaces.Vehiculo;
using PremierFlow.Domain.Entities;
using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.VehiculoService
{
    public class VehiculoService : IVehiculoService
    {
        private readonly PremierFlowDbContext context;

        public VehiculoService(PremierFlowDbContext context)
        {
            this.context = context;
        }
        public Task<ApiResponse<bool>> ActualizarKilometrajeAsync(int vehiculoId, int nuevoKm, string usuarioId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> CambiarEstadoAsync(int vehiculoId, EstadoVehiculo nuevoEstado, string usuarioId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<VehiculoDTO>> CreateAsync(CreateVehiculoDTO dto, string usuarioId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> DeleteAsync(int vehiculoId, string usuarioId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<VehiculoDTO>>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<VehiculoDTO>>> GetByClienteAsync(int clienteId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<VehiculoDTO>>> GetByEstadoAsync(EstadoVehiculo estado)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<VehiculoDTO>> GetByIdAsync(int vehiculoId)
        {
            var vehiculo = await context.Vehiculos
            .Include(v => v.Cliente)
            .Include(v => v.Marca)
            .Include(v => v.Modelo)
            .Include(v => v.Version)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.VehiculoId == vehiculoId && v.Activo);

            if (vehiculo == null)
                return ApiResponse<VehiculoDTO>.fail(404, null, "Vehículo no encontrado.");

            return ApiResponse<VehiculoDTO>.ok(MapToDto(vehiculo), "Vehículo obtenido.");
        }

        public Task<ApiResponse<VehiculoDTO>> GetByPlacaAsync(string placa)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<VehiculoDTO>>> GetBySucursalAsync(int sucursalId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<VehiculoDTO>> GetByVinAsync(string vin)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<VehiculoDetalleDTO>> GetDetalleByIdAsync(int vehiculoId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<VehiculoDTO>>> GetDisponiblesParaVentaAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<VehiculoDTO>>> SearchAsync(string term)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<VehiculoDTO>> UpdateAsync(UpdateVehiculoDTO dto, string usuarioId)
        {
            throw new NotImplementedException();
        }

        #region Helpers

        private async Task<bool> VinExiste(string vin, int? excludeId = null)
        {
            return await context.Vehiculos
                .AnyAsync(v => v.Vin == vin &&
                              (excludeId == null || v.VehiculoId != excludeId) &&
                              v.Activo);
        }

        private async Task<bool> PlacaExiste(string placa, int? excludeId = null)
        {
            return await context.Vehiculos
                .AnyAsync(v => v.Placa == placa &&
                              (excludeId == null || v.VehiculoId != excludeId) &&
                              v.Activo);
        }

        private static VehiculoDTO MapToDto(Vehiculo v)
        {
            return new VehiculoDTO
            {
                VehiculoId = v.VehiculoId,
                Vin = v.Vin,
                Placa = v.Placa,
                MarcaId = v.MarcaId,
                ModeloId = v.ModeloId,
                VersionId = v.VersionId,
                Anio = v.Anio,
                Color = v.Color,
                TipoCombustible = v.TipoCombustible,
                Transmision = v.Transmision,
                Estado = v.Estado,
                ClienteId = v.ClienteId,
                KilometrajeActual = v.KilometrajeActual,
                GarantiaHasta = v.GarantiaHasta,
                ClienteNombre = v.Cliente?.NombreCompleto,
                MarcaNombre = v.Marca?.Nombre ?? "",
                ModeloNombre = v.Modelo?.Nombre ?? "",
                VersionNombre = v.Version?.Nombre,
                Identificador = v.Identificador,
                DescripcionCompleta = v.DescripcionCompleta,
                EnGarantia = v.EnGarantia,
                EstaVendido = v.EstaVendido,
                DisponibleParaVenta = v.DisponibleParaVenta
            };
        }

        private static VehiculoDetalleDTO MapToDetalleDto(Vehiculo v)
        {
            return new VehiculoDetalleDTO
            {
                VehiculoId = v.VehiculoId,
                Vin = v.Vin,
                Placa = v.Placa,
                NumeroMotor = v.NumeroMotor,
                NumeroChasis = v.NumeroChasis,
                MarcaId = v.MarcaId,
                ModeloId = v.ModeloId,
                VersionId = v.VersionId,
                Anio = v.Anio,
                Color = v.Color,
                TipoCombustible = v.TipoCombustible,
                Transmision = v.Transmision,
                Estado = v.Estado,
                UbicacionId = v.UbicacionId,
                SucursalId = v.SucursalId,
                Procedencia = v.Procedencia,
                NumeroImportacion = v.NumeroImportacion,
                NumeroPoliza = v.NumeroPoliza,
                FechaIngresoPais = v.FechaIngresoPais,
                FechaRecepcion = v.FechaRecepcion,
                CostoImportacion = v.CostoImportacion,
                ClienteId = v.ClienteId,
                PrecioLista = v.PrecioLista,
                PrecioVenta = v.PrecioVenta,
                FechaVenta = v.FechaVenta,
                FechaEntrega = v.FechaEntrega,
                VendedorId = v.VendedorId,
                KilometrajeActual = v.KilometrajeActual,
                FechaPrimeraMatricula = v.FechaPrimeraMatricula,
                GarantiaHasta = v.GarantiaHasta,
                Observaciones = v.Observaciones,
                FechaRegistro = v.FechaRegistro,
                ClienteNombre = v.Cliente?.NombreCompleto,
                MarcaNombre = v.Marca?.Nombre ?? "",
                ModeloNombre = v.Modelo?.Nombre ?? "",
                VersionNombre = v.Version?.Nombre,
                UbicacionNombre = v.Ubicacion?.Nombre,
                SucursalNombre = v.Sucursal?.Nombre,
                Identificador = v.Identificador,
                DescripcionCompleta = v.DescripcionCompleta,
                EnGarantia = v.EnGarantia,
                EstaVendido = v.EstaVendido,
                DisponibleParaVenta = v.DisponibleParaVenta,
                CantidadServicios = v.HistorialAts?.Count ?? 0
            };
        }

        #endregion
    }


}
