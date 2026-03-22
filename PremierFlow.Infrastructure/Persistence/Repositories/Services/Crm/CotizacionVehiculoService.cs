using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Crm;
using PremierFlow.Application.Interfaces.Crm;
using PremierFlow.Domain.Common;
using PremierFlow.Domain.Entities;
using PremierFlow.Domain.Enums;
using PremierFlow.Infrastructure.Persistence.Helpers;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.Crm
{
    public class CotizacionVehiculoService : ICotizacionVehiculoService
    {
        private readonly PremierFlowDbContext context;
        private readonly GeneradorCodigos generadorCodigos;

        public CotizacionVehiculoService(PremierFlowDbContext context, GeneradorCodigos generadorCodigos)
        {
            this.context = context;
            this.generadorCodigos = generadorCodigos;
        }

        public async Task<ApiResponse<bool>> AceptarAsync(int cotizacionVehiculoId, string usuarioId)
        {
            var cotizacion = await context.CotizacionesVehiculo.FirstOrDefaultAsync(c => c.CotizacionVehiculoId == cotizacionVehiculoId && c.Activo);
            if (cotizacion == null)
                return ApiResponse<bool>.fail(404, null, "Cotización no encontrada.");
            if (cotizacion.EstaVencida)
                return ApiResponse<bool>.fail(400, null, "La cotización ya venció");

            try
            {
                cotizacion.Aceptar();
                //debemos invalidar las demas cotizaciones vigente a la misma oportunidad
                var otrasCotizaciones = await context.CotizacionesVehiculo
                                    .Where(c => c.OportunidadId == cotizacion.OportunidadId
                                    && c.CotizacionVehiculoId != cotizacionVehiculoId
                                    && c.Estado == EstadoCotizacion.Vigente
                                    && c.Activo)
                            .ToListAsync();
                foreach(var otrasCotizacion in otrasCotizaciones)
                {
                    otrasCotizacion.Cancelar();
                }

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
            if (oportunidad.Etapa == EtapaOportunidad.Necesidades)
            {
                oportunidad.CambiarEtapa(EtapaOportunidad.Cotizacion);
                oportunidad.UsuarioModificaId = usuarioId;
                oportunidad.FechaModificacion = TimeHelper.Now;
            }
            var vehiculo = await context.Vehiculos.FirstOrDefaultAsync(v => v.VehiculoId == dto.VehiculoId && v.Activo);
            if (vehiculo == null)
                return ApiResponse<CotizacionVehiculoDTO>.fail(404, null, "Vehículo no encontrado.");
            if (!vehiculo.DisponibleParaVenta && vehiculo.Estado != EstadoVehiculo.Reservado)
                return ApiResponse<CotizacionVehiculoDTO>.fail(400, null, "El vehículo no está disponible para venta.");

            var cotizacion = await generadorCodigos.EjecutarConReintento(
                async (codigo) =>
                {
                    var nuevaCot = new CotizacionVehiculo
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
                    context.CotizacionesVehiculo.Add(nuevaCot);
                    await context.SaveChangesAsync();
                    return nuevaCot;
                },
                generadorCodigos.GenerarCodigoCotizacionAsync
            );

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
            var cotizacion = await context.CotizacionesVehiculo.FirstOrDefaultAsync(c => c.CotizacionVehiculoId == cotizacionVehiculoId && c.Activo);
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

        public async Task<ApiResponse<bool>> CancelarAsync(int cotizacionVehiculoId, string usuarioId)
        {
            var cotizacion = await context.CotizacionesVehiculo.FirstOrDefaultAsync(c => c.CotizacionVehiculoId == cotizacionVehiculoId && c.Activo);
            if (cotizacion == null)
                return ApiResponse<bool>.fail(404, null, "Cotizacion no encontrada");
            try
            {
                cotizacion.Cancelar();
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<bool>.fail(400, null, ex.Message);
            }
            cotizacion.FechaModificacion = TimeHelper.Now;
            cotizacion.UsuarioModificaId = usuarioId;
            await context.SaveChangesAsync();
            return ApiResponse<bool>.ok(true, "Cotizacion Cancelada correctamente");
        }

        #endregion
    }
}