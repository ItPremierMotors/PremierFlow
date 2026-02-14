using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Taller;
using PremierFlow.Application.Interfaces.Taller;
using PremierFlow.Domain.Entities;
using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.Taller
{
    public class HistorialAtsService: IHistorialAtsService
    {
        private readonly PremierFlowDbContext context;

        public HistorialAtsService(PremierFlowDbContext context)
        {
            this.context = context;
        }

        public async Task<ApiResponse<HistorialAtsDTO>> GetByIdAsync(int atsId)
        {
            var historial = await context.HistorialAts
                .Include(h => h.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(h => h.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(h => h.Vehiculo)
                    .ThenInclude(v => v.Cliente)
                .Include(h => h.OrdenServicio)
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.AtsId == atsId && h.Activo);

            if (historial == null)
                return ApiResponse<HistorialAtsDTO>.fail(404, null, "Registro de historial no encontrado.");

            return ApiResponse<HistorialAtsDTO>.ok(MapToDto(historial), "Historial obtenido.");
        }

        public async Task<ApiResponse<HistorialAtsDTO>> GetByOsIdAsync(int osId)
        {
            var historial = await context.HistorialAts
                .Include(h => h.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(h => h.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(h => h.Vehiculo)
                    .ThenInclude(v => v.Cliente)
                .Include(h => h.OrdenServicio)
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.OsId == osId && h.Activo);

            if (historial == null)
                return ApiResponse<HistorialAtsDTO>.fail(404, null, "No existe historial para esta orden de servicio.");

            return ApiResponse<HistorialAtsDTO>.ok(MapToDto(historial), "Historial obtenido.");
        }

        public async Task<ApiResponse<List<HistorialAtsDTO>>> GetByVehiculoIdAsync(int vehiculoId)
        {
            var historiales = await context.HistorialAts
                .Include(h => h.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(h => h.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(h => h.Vehiculo)
                    .ThenInclude(v => v.Cliente)
                .Include(h => h.OrdenServicio)
                .AsNoTracking()
                .Where(h => h.VehiculoId == vehiculoId && h.Activo)
                .OrderByDescending(h => h.FechaServicio)
                .ToListAsync();

            var dtos = historiales.Select(MapToDto).ToList();

            return ApiResponse<List<HistorialAtsDTO>>.ok(dtos, "Historial del vehículo obtenido.");
        }

        public async Task<ApiResponse<List<HistorialAtsDTO>>> GetByVinAsync(string vin)
        {
            var vehiculo = await context.Vehiculos
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Vin == vin && v.Activo);

            if (vehiculo == null)
                return ApiResponse<List<HistorialAtsDTO>>.fail(404, null, "Vehículo no encontrado con ese VIN.");

            return await GetByVehiculoIdAsync(vehiculo.VehiculoId);
        }

        public async Task<ApiResponse<List<HistorialAtsDTO>>> GetByPlacaAsync(string placa)
        {
            var vehiculo = await context.Vehiculos
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Placa == placa && v.Activo);

            if (vehiculo == null)
                return ApiResponse<List<HistorialAtsDTO>>.fail(404, null, "Vehículo no encontrado con esa placa.");

            return await GetByVehiculoIdAsync(vehiculo.VehiculoId);
        }

        public async Task<ApiResponse<List<HistorialAtsDTO>>> GetConRecomendacionPendienteAsync()
        {
            var historiales = await context.HistorialAts
                .Include(h => h.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(h => h.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(h => h.Vehiculo)
                    .ThenInclude(v => v.Cliente)
                .Include(h => h.OrdenServicio)
                .AsNoTracking()
                .Where(h => h.Activo &&
                           h.ProximaRevision != null &&
                           h.ProximaRevision != "")
                .OrderByDescending(h => h.FechaServicio)
                .ToListAsync();

            // Filtrar solo el último registro por vehículo (más reciente)
            var ultimosPorVehiculo = historiales
                .GroupBy(h => h.VehiculoId)
                .Select(g => g.First())
                .ToList();

            var dtos = ultimosPorVehiculo.Select(MapToDto).ToList();

            return ApiResponse<List<HistorialAtsDTO>>.ok(dtos, "Vehículos con recomendación pendiente obtenidos.");
        }

        public async Task<ApiResponse<HistorialAtsDTO>> CrearDesdeOsAsync(CreateHistorialAtsDTO dto, string usuarioId)
        {
            // 1. Validar que existe la OS
            var os = await context.OrdenesServicio
                .Include(o => o.Estado)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Cliente)
                .Include(o => o.Servicios.Where(s => s.Activo))
                    .ThenInclude(s => s.TipoServicio)
                .FirstOrDefaultAsync(o => o.OsId == dto.OsId && o.Activo);

            if (os == null)
                return ApiResponse<HistorialAtsDTO>.fail(404, null, "Orden de servicio no encontrada.");

            // 2. Validar que la OS está cerrada
            if (os.Estado.Codigo != EstadoOs.Estados.Cerrada)
                return ApiResponse<HistorialAtsDTO>.fail(400, null, "Solo se puede crear historial de una OS cerrada.");

            // 3. Validar que no exista historial para esta OS
            var existeHistorial = await context.HistorialAts
                .AnyAsync(h => h.OsId == dto.OsId && h.Activo);

            if (existeHistorial)
                return ApiResponse<HistorialAtsDTO>.fail(400, null, "Ya existe un registro de historial para esta OS.");

            // 4. Generar tipos de servicio si no se proporcionó
            var tipoServicio = string.Join(", ", os.Servicios
                .Where(s => s.Estado == EstadoServicioOS.Completado)
                .Select(s => s.TipoServicio?.Nombre ?? "Servicio"));

            if (string.IsNullOrEmpty(tipoServicio))
            {
                tipoServicio = "Servicio general";
            }

            // 5. Crear historial
            var historial = new HistorialAts
            {
                VehiculoId = os.VehiculoId,
                OsId = os.OsId,
                FechaServicio = os.FechaCierre ?? DateTime.UtcNow,
                TipoServicio = tipoServicio,
                Kilometraje = os.KilometrajeIngreso,
                TrabajosRealizados = dto.TrabajosRealizados,
                MontoTotal = os.TotalGeneral,
                ObservacionesTecnicas = dto.ObservacionesTecnicas ?? os.ObservacionesCierre,
                ProximaRevision = dto.ProximaRevision,
                FechaRegistro = DateTime.UtcNow,
                Activo = true,
                UsuarioCreaId = usuarioId,
                FechaCreacion = DateTime.UtcNow
            };

            context.HistorialAts.Add(historial);
            await context.SaveChangesAsync();

            // 6. Cargar navegaciones para el DTO
            historial.OrdenServicio = os;
            historial.Vehiculo = os.Vehiculo;

            return ApiResponse<HistorialAtsDTO>.ok(MapToDto(historial), "Historial creado exitosamente.");
        }

        #region Helper

        private static HistorialAtsDTO MapToDto(HistorialAts h)
        {
            return new HistorialAtsDTO
            {
                AtsId = h.AtsId,
                VehiculoId = h.VehiculoId,
                OsId = h.OsId,
                FechaServicio = h.FechaServicio,
                TipoServicio = h.TipoServicio,
                Kilometraje = h.Kilometraje,
                TrabajosRealizados = h.TrabajosRealizados,
                MontoTotal = h.MontoTotal,
                ObservacionesTecnicas = h.ObservacionesTecnicas,
                ProximaRevision = h.ProximaRevision,
                FechaRegistro = h.FechaRegistro,
                NumeroOs = h.OrdenServicio?.NumeroOs ?? "",
                VehiculoDescripcion = h.Vehiculo?.DescripcionCompleta ?? "",
                VehiculoPlaca = h.Vehiculo?.Placa,
                VehiculoVin = h.Vehiculo?.Vin,
                ClienteNombre = h.Vehiculo?.Cliente?.NombreCompleto,
                TieneRecomendacionPendiente = h.TieneRecomendacionPendiente
            };
        }

        #endregion
    }
}
