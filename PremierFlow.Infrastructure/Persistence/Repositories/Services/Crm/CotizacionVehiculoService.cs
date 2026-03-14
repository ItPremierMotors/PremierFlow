using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Crm;
using PremierFlow.Application.Interfaces.Crm;
using PremierFlow.Domain.Common;
using PremierFlow.Domain.Entities;
using PremierFlow.Domain.Enums;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.Crm
{
    public class CotizacionVehiculoService : ICotizacionVehiculoService
    {
        private readonly PremierFlowDbContext context;

        public CotizacionVehiculoService(PremierFlowDbContext context)
        {
            this.context = context;
        }

        public async Task<ApiResponse<bool>> AceptarAsync(int cotizacionVehiculoId, string usuarioId)
        {
           var cotizacion=await context.CotizacionesVehiculo.FirstOrDefaultAsync(c => c.CotizacionVehiculoId == cotizacionVehiculoId && c.Activo);
            if (cotizacion == null)
                return ApiResponse<bool>.fail(404, null, "Cotización no encontrada.");
             if (cotizacion.EstaVencida)
                return ApiResponse<bool>.fail(400, null, "La cotización ya venció");
        
            try
            {
                cotizacion.Aceptar();
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<bool>.fail(400, null, ex.Message);
            }
            cotizacion.UsuarioModificaId = usuarioId;
            cotizacion.FechaModificacion = TimeHelper.Now;
            await context.SaveChangesAsync();
            return ApiResponse<bool>.ok(true, "Cotización aceptada");
        }

        public async Task<ApiResponse<CotizacionVehiculoDTO>> CreateAsync(CreateCotizacionVehiculoDTO dto, string usuarioId)
        {
            var oportunidad = await context.Oportunidades.FirstOrDefaultAsync(o => o.OportunidadId == dto.OportunidadId && o.Activo);
            if (oportunidad == null)
                return ApiResponse<CotizacionVehiculoDTO>.fail(404, null, "Oportunidad no encontrada.");
            if (!oportunidad.EstaAbierta)
                return ApiResponse<CotizacionVehiculoDTO>.fail(400, null, "La oportunidad no está abierta.");
            var vehiculo = await context.Vehiculos.FirstOrDefaultAsync(v => v.VehiculoId == dto.VehiculoId && v.Activo);
            if (vehiculo == null)
                return ApiResponse<CotizacionVehiculoDTO>.fail(404, null, "Vehículo no encontrado.");
            if (!vehiculo.DisponibleParaVenta && vehiculo.Estado != EstadoVehiculo.Reservado)
                return ApiResponse<CotizacionVehiculoDTO>.fail(400, null, "El vehículo no está disponible para venta.");

            // Generar código
            var anio = TimeHelper.Now.Year;
            var prefijo = $"COT-{anio}";
            var ultimoCodigo = await context.CotizacionesVehiculo
                .Where(c => c.CodigoCotizacion.StartsWith(prefijo))
                .OrderByDescending(c => c.CodigoCotizacion)
                .Select(c => c.CodigoCotizacion)
                .FirstOrDefaultAsync();

            var siguiente = 1;
            if (ultimoCodigo != null)
            {
                var partes = ultimoCodigo.Split('-');
                siguiente = int.Parse(partes[2]) + 1;
            }
            var codigo = $"{prefijo}-{siguiente:D4}";

              var cotizacion = new CotizacionVehiculo
            {
                CodigoCotizacion = codigo,
                OportunidadId = dto.OportunidadId,
                VehiculoId = dto.VehiculoId,
                Descuento = dto.Descuento,
                PrecioOfertado = dto.PrecioOfertado,
                CondicionesPago = dto.CondicionesPago,
                Observaciones = dto.Observaciones,
                FechaEmision = TimeHelper.Now,
                FechaVencimiento = dto.FechaVencimiento,
                Estado = EstadoCotizacion.Vigente,
                Activo = true,
                UsuarioCreaId = usuarioId,
                FechaCreacion = TimeHelper.Now
            };

            context.CotizacionesVehiculo.Add(cotizacion);
            await context.SaveChangesAsync();

            var created = await GetCotizacionQuery().FirstOrDefaultAsync(c => c.CotizacionVehiculoId == cotizacion.CotizacionVehiculoId);
            return ApiResponse<CotizacionVehiculoDTO>.ok(MapToDto(created!), "Cotización creada exitosamente");
        
        }

        public async Task<ApiResponse<List<CotizacionVehiculoDTO>>> GetByOportunidadAsync(int oportunidadId)
        {
            var cotizaciones = await GetCotizacionQuery()
                .Where(c => c.OportunidadId == oportunidadId && c.Activo)
                .OrderByDescending(c => c.FechaEmision)
                .ToListAsync();

            var dtos = cotizaciones.Select(MapToDto).ToList();
            return ApiResponse<List<CotizacionVehiculoDTO>>.ok(dtos, "Cotizaciones de la oportunidad");
        }

        public async Task<ApiResponse<bool>> RechazarAsync(int cotizacionVehiculoId, string usuarioId)
        {
            var cotizacion= await context.CotizacionesVehiculo.FirstOrDefaultAsync(c => c.CotizacionVehiculoId == cotizacionVehiculoId && c.Activo);
            if (cotizacion == null)
                return ApiResponse<bool>.fail(404, null, "Cotización no encontrada.");
             if (cotizacion.EstaVencida)
                return ApiResponse<bool>.fail(400, null, "La cotización ya venció");
        
            try
            {
                cotizacion.Rechazar();
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<bool>.fail(400, null, ex.Message);
            }
            cotizacion.UsuarioModificaId = usuarioId;
            cotizacion.FechaModificacion = TimeHelper.Now;
            await context.SaveChangesAsync();
            return ApiResponse<bool>.ok(true, "Cotización Rechazada");
        }

        // Implementación de métodos para gestionar cotizaciones de vehículos
        #region Helpers

        private IQueryable<CotizacionVehiculo> GetCotizacionQuery()
        {
            return context.CotizacionesVehiculo
                .Include(c => c.Vehiculo)
                .AsNoTracking();
        }

        private static CotizacionVehiculoDTO MapToDto(CotizacionVehiculo c)
        {
            return new CotizacionVehiculoDTO
            {
                CotizacionVehiculoId = c.CotizacionVehiculoId,
                CodigoCotizacion = c.CodigoCotizacion,
                Descuento = c.Descuento,
                PrecioOfertado = c.PrecioOfertado,
                CondicionesPago = c.CondicionesPago,
                Observaciones = c.Observaciones,
                FechaEmision = c.FechaEmision,
                FechaVencimiento = c.FechaVencimiento,
                Estado = c.Estado,
                OportunidadId = c.OportunidadId,
                VehiculoId = c.VehiculoId,
                VehiculoDescripcion = c.Vehiculo?.DescripcionCompleta ?? "",
                VehiculoPrecioLista = c.Vehiculo?.PrecioLista ?? 0
            };
        }

        #endregion
    }
}