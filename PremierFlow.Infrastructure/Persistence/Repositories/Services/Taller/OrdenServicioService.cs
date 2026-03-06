using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Catalogo;
using PremierFlow.Application.Dtos.Taller;
using PremierFlow.Application.Interfaces.Taller;
using PremierFlow.Domain.Common;
using PremierFlow.Domain.Entities;
using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.Taller
{
    public class OrdenServicioService: IOrdenServicioService
    {
        private readonly PremierFlowDbContext context;

        public OrdenServicioService(PremierFlowDbContext context)
        {
            this.context = context;
        }

        #region Consultas

        public async Task<ApiResponse<OrdenServicioDTO>> GetByIdAsync(int osId)
        {
            var os = await context.OrdenesServicio
                .Include(o => o.Cita)
                .Include(o => o.Cliente)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(o => o.Estado)
                .Include(o => o.Sucursal)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OsId == osId && o.Activo);

            if (os == null)
                return ApiResponse<OrdenServicioDTO>.fail(404, null, "Orden de servicio no encontrada.");

            return ApiResponse<OrdenServicioDTO>.ok(MapToDto(os), "Orden de servicio obtenida.");
        }

        public async Task<ApiResponse<OrdenServicioDTO>> GetByNumeroAsync(string numeroOs)
        {
            var os = await context.OrdenesServicio
                .Include(o => o.Cita)
                .Include(o => o.Cliente)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(o => o.Estado)
                .Include(o => o.Sucursal)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.NumeroOs == numeroOs && o.Activo);

            if (os == null)
                return ApiResponse<OrdenServicioDTO>.fail(404, null, "Orden de servicio no encontrada.");

            return ApiResponse<OrdenServicioDTO>.ok(MapToDto(os), "Orden de servicio obtenida.");
        }

        public async Task<ApiResponse<OrdenServicioDetalleDTO>> GetDetalleByIdAsync(int osId)
        {
            var os = await context.OrdenesServicio
                .Include(o => o.Cita)
                .Include(o => o.Cliente)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(o => o.Estado)
                .Include(o => o.Sucursal)
                .Include(o => o.Servicios.Where(s => s.Activo))
                .Include(o => o.AsignacionesTecnico.Where(a => a.Activo))
                .Include(o => o.Evidencias.Where(e => e.Activo))
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OsId == osId && o.Activo);

            if (os == null)
                return ApiResponse<OrdenServicioDetalleDTO>.fail(404, null, "Orden de servicio no encontrada.");

            return ApiResponse<OrdenServicioDetalleDTO>.ok(MapToDetalleDto(os), "Orden de servicio obtenida.");
        }

        public async Task<ApiResponse<List<OrdenServicioDTO>>> GetAllAsync()
        {
            var ordenes = await context.OrdenesServicio
                .Include(o => o.Cita)
                .Include(o => o.Cliente)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(o => o.Estado)
                .Include(o => o.Sucursal)
                .AsNoTracking()
                .Where(o => o.Activo)
                .OrderByDescending(o => o.FechaApertura)
                .ToListAsync();

            var dtos = ordenes.Select(MapToDto).ToList();

            return ApiResponse<List<OrdenServicioDTO>>.ok(dtos, "Órdenes de servicio obtenidas.");
        }

        public async Task<ApiResponse<List<OrdenServicioDTO>>> GetAbiertasAsync(int? sucursalId = null)
        {
            var query = context.OrdenesServicio
                .Include(o => o.Cita)
                .Include(o => o.Cliente)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(o => o.Estado)
                .Include(o => o.Sucursal)
                .AsNoTracking()
                .Where(o => o.Activo &&
                           o.Estado.Codigo != EstadoOs.Estados.Cerrada &&
                           o.Estado.Codigo != EstadoOs.Estados.Cancelada);

            if (sucursalId.HasValue)
                query = query.Where(o => o.SucursalId == sucursalId);

            var ordenes = await query
                .OrderBy(o => o.FechaApertura)
                .ToListAsync();

            var dtos = ordenes.Select(MapToDto).ToList();

            return ApiResponse<List<OrdenServicioDTO>>.ok(dtos, "Órdenes abiertas obtenidas.");
        }

        public async Task<ApiResponse<List<OrdenServicioDTO>>> GetByFechaAsync(DateTime fecha, int? sucursalId = null)
        {
            var query = context.OrdenesServicio
                .Include(o => o.Cita)
                .Include(o => o.Cliente)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(o => o.Estado)
                .Include(o => o.Sucursal)
                .AsNoTracking()
                .Where(o => o.FechaApertura.Date == fecha.Date && o.Activo);

            if (sucursalId.HasValue)
                query = query.Where(o => o.SucursalId == sucursalId);

            var ordenes = await query
                .OrderBy(o => o.FechaApertura)
                .ToListAsync();

            var dtos = ordenes.Select(MapToDto).ToList();

            return ApiResponse<List<OrdenServicioDTO>>.ok(dtos, "Órdenes de servicio obtenidas.");
        }

        public async Task<ApiResponse<List<OrdenServicioDTO>>> GetByRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin, int? sucursalId = null)
        {
            var query = context.OrdenesServicio
                .Include(o => o.Cita)
                .Include(o => o.Cliente)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(o => o.Estado)
                .Include(o => o.Sucursal)
                .AsNoTracking()
                .Where(o => o.FechaApertura.Date >= fechaInicio.Date &&
                           o.FechaApertura.Date <= fechaFin.Date &&
                           o.Activo);

            if (sucursalId.HasValue)
                query = query.Where(o => o.SucursalId == sucursalId);

            var ordenes = await query
                .OrderBy(o => o.FechaApertura)
                .ToListAsync();

            var dtos = ordenes.Select(MapToDto).ToList();

            return ApiResponse<List<OrdenServicioDTO>>.ok(dtos, "Órdenes de servicio obtenidas.");
        }

        public async Task<ApiResponse<List<OrdenServicioDTO>>> GetByClienteAsync(int clienteId)
        {
            var ordenes = await context.OrdenesServicio
                .Include(o => o.Cita)
                .Include(o => o.Cliente)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(o => o.Estado)
                .Include(o => o.Sucursal)
                .AsNoTracking()
                .Where(o => o.ClienteId == clienteId && o.Activo)
                .OrderByDescending(o => o.FechaApertura)
                .ToListAsync();

            var dtos = ordenes.Select(MapToDto).ToList();

            return ApiResponse<List<OrdenServicioDTO>>.ok(dtos, "Órdenes de servicio obtenidas.");
        }

        public async Task<ApiResponse<List<OrdenServicioDTO>>> GetByVehiculoAsync(int vehiculoId)
        {
            var ordenes = await context.OrdenesServicio
                .Include(o => o.Cita)
                .Include(o => o.Cliente)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(o => o.Estado)
                .Include(o => o.Sucursal)
                .AsNoTracking()
                .Where(o => o.VehiculoId == vehiculoId && o.Activo)
                .OrderByDescending(o => o.FechaApertura)
                .ToListAsync();

            var dtos = ordenes.Select(MapToDto).ToList();

            return ApiResponse<List<OrdenServicioDTO>>.ok(dtos, "Órdenes de servicio obtenidas.");
        }

        public async Task<ApiResponse<HistorialServicioVehiculoDTO>> GetHistorialServicioVehiculoAsync(int vehiculoId)
        {
            var vehiculo = await context.Vehiculos
                .Include(v => v.Marca)
                .Include(v => v.Modelo)
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.VehiculoId == vehiculoId && v.Activo);

            if (vehiculo == null)
                return ApiResponse<HistorialServicioVehiculoDTO>.fail(404, null, "Vehículo no encontrado.");

            var ordenes = await context.OrdenesServicio
                .Include(o => o.Estado)
                .Include(o => o.Sucursal)
                .Include(o => o.Servicios.Where(s => s.Activo))
                    .ThenInclude(s => s.TipoServicio)
                .Include(o => o.Servicios.Where(s => s.Activo))
                    .ThenInclude(s => s.Tecnico)
                .AsNoTracking()
                .Where(o => o.VehiculoId == vehiculoId && o.Activo)
                .OrderByDescending(o => o.FechaApertura)
                .ToListAsync();

            var ultimaCerrada = ordenes.FirstOrDefault(o => o.FechaCierre != null);

            var dto = new HistorialServicioVehiculoDTO
            {
                VehiculoId = vehiculo.VehiculoId,
                VehiculoDescripcion = $"{vehiculo.Marca?.Nombre} {vehiculo.Modelo?.Nombre} {vehiculo.Anio}".Trim(),
                Placa = vehiculo.Placa,
                KilometrajeActual = vehiculo.KilometrajeActual,
                TotalVisitas = ordenes.Count,
                UltimaVisita = ordenes.FirstOrDefault()?.FechaApertura,
                UltimaRecomendacion = ultimaCerrada?.ProximaRevision,
                Ordenes = ordenes.Select(o => new HistorialServicioItemDTO
                {
                    OsId = o.OsId,
                    NumeroOs = o.NumeroOs,
                    FechaApertura = o.FechaApertura,
                    FechaCierre = o.FechaCierre,
                    EstadoNombre = o.Estado?.Nombre ?? "Desconocido",
                    EstadoCodigo = o.Estado?.Codigo ?? "",
                    KilometrajeIngreso = o.KilometrajeIngreso,
                    TipoIngreso = o.TipoIngreso.ToString(),
                    EsGarantia = o.EsGarantia,
                    TotalGeneral = o.TotalGeneral,
                    ProximaRevision = o.ProximaRevision,
                    ObservacionesCierre = o.ObservacionesCierre,
                    SucursalNombre = o.Sucursal?.Nombre,
                    Servicios = o.Servicios.Select(s => new HistorialServicioLineaDTO
                    {
                        TipoServicioNombre = s.TipoServicio?.Nombre ?? "N/A",
                        DescripcionTrabajo = s.DescripcionTrabajo,
                        EstadoNombre = s.Estado.ToString(),
                        Subtotal = s.Subtotal,
                        TecnicoNombre = s.Tecnico?.NombreCompleto
                    }).ToList()
                }).ToList()
            };

            return ApiResponse<HistorialServicioVehiculoDTO>.ok(dto, "Historial de servicio obtenido.");
        }

        public async Task<ApiResponse<List<OrdenServicioDTO>>> GetByEstadoAsync(int estadoId, int? sucursalId = null)
        {
            var estadoExiste = await context.EstadosOs.AnyAsync(e => e.EstadoId == estadoId && e.Activo);
            if (!estadoExiste)
                return ApiResponse<List<OrdenServicioDTO>>.fail(404, null, "Estado no encontrado.");
            var query = context.OrdenesServicio
                .Include(o => o.Cita)
                .Include(o => o.Cliente)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(o => o.Estado)
                .Include(o => o.Sucursal)
                .AsNoTracking()
                .Where(o => o.EstadoId == estadoId && o.Activo);

            if (sucursalId.HasValue)
                query = query.Where(o => o.SucursalId == sucursalId);

            var ordenes = await query
                .OrderBy(o => o.FechaApertura)
                .ToListAsync();

            var dtos = ordenes.Select(MapToDto).ToList();

            return ApiResponse<List<OrdenServicioDTO>>.ok(dtos, "Órdenes de servicio obtenidas.");
        }

        #endregion

        #region Acciones

        public async Task<ApiResponse<OrdenServicioDTO>> CreateFromCitaAsync(CreateOsFromCitaDTO dto, string usuarioId)
        {
            // 1. Validar cita
            var cita = await context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(c => c.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(c => c.TipoServicio)
                .FirstOrDefaultAsync(c => c.CitaId == dto.CitaId && c.Activo);

            if (cita == null)
                return ApiResponse<OrdenServicioDTO>.fail(404, null, "Cita no encontrada.");

            if (!cita.PuedeConvertirseEnOs)
                return ApiResponse<OrdenServicioDTO>.fail(400, null, "La cita no está en estado válido para crear OS.");

            // 2. Validar que no exista OS para esta cita
            var existeOs = await context.OrdenesServicio
                .AnyAsync(o => o.CitaId == dto.CitaId && o.Activo);

            if (existeOs)
                return ApiResponse<OrdenServicioDTO>.fail(400, null, "Ya existe una orden de servicio para esta cita.");

            // 3. Validar kilometraje
            if (dto.KilometrajeIngreso < cita.Vehiculo.KilometrajeActual)
                return ApiResponse<OrdenServicioDTO>.fail(400, null,
                    $"El kilometraje debe ser mayor o igual al actual ({cita.Vehiculo.KilometrajeActual} km).");

            // 4. Obtener estado inicial (ABIERTA)
            var estadoAbierta = await context.EstadosOs
                .FirstOrDefaultAsync(e => e.Codigo == EstadoOs.Estados.Abierta && e.Activo);

            if (estadoAbierta == null)
                return ApiResponse<OrdenServicioDTO>.fail(500, null, "Estado ABIERTA no configurado en el sistema.");

            // 5. Generar número de OS
            var numeroOs = await GenerarNumeroOsAsync();

            // 6. Crear OS
            var os = new OrdenServicio
            {
                NumeroOs = numeroOs,
                CitaId = dto.CitaId,
                VehiculoId = cita.VehiculoId,
                ClienteId = cita.ClienteId,
                FechaApertura = TimeHelper.Now,
                EstadoId = estadoAbierta.EstadoId,
                KilometrajeIngreso = dto.KilometrajeIngreso,
                NivelCombustible = dto.NivelCombustible,
                TipoIngreso = TipoIngreso.Cita,
                EsGarantia = dto.EsGarantia,
                ObservacionesApertura = dto.ObservacionesApertura,

                SucursalId = cita.SucursalId,
                Activo = true,
                UsuarioCreaId = usuarioId,
                FechaCreacion = TimeHelper.Now
            };

            context.OrdenesServicio.Add(os);

            // 7. Agregar servicio de la cita automaticamente
            var servicioCita = new OsServicio
            {
                TipoServicioId = cita.TipoServicioId,
                DescripcionTrabajo = cita.TipoServicio.Nombre,
                Estado = EstadoServicioOS.Pendiente,
                PrecioUnitario = cita.TipoServicio.PrecioBase,
                Cantidad = 1,
                Observaciones = cita.MotivoVisita,
                Activo = true,
                UsuarioCreaId = usuarioId,
                FechaCreacion = TimeHelper.Now
            };
            servicioCita.CalcularSubtotal();
            os.Servicios.Add(servicioCita);

            // 8. Actualizar kilometraje del vehículo
            cita.Vehiculo.ActualizarKilometraje(dto.KilometrajeIngreso);

            // 9. Actualizar estado de la cita
            cita.IniciarProceso();

            await context.SaveChangesAsync();

            // 9. Cargar navegaciones para el DTO
            os.Cita = cita;
            os.Cliente = cita.Cliente;
            os.Vehiculo = cita.Vehiculo;
            os.Estado = estadoAbierta;

            return ApiResponse<OrdenServicioDTO>.ok(MapToDto(os), "Orden de servicio creada exitosamente.");
        }

        public async Task<ApiResponse<OrdenServicioDTO>> CreateWalkInAsync(CreateOsWalkInDTO dto, string usuarioId)
        {
            // 1. Validar cliente
            var cliente = await context.Clientes
                .FirstOrDefaultAsync(c => c.ClienteId == dto.ClienteId && c.Activo);

            if (cliente == null)
                return ApiResponse<OrdenServicioDTO>.fail(404, null, "Cliente no encontrado.");

            // 2. Validar vehículo
            var vehiculo = await context.Vehiculos
                .Include(v => v.Marca)
                .Include(v => v.Modelo)
                .FirstOrDefaultAsync(v => v.VehiculoId == dto.VehiculoId && v.Activo);
            if (dto.SucursalId.HasValue)
            {
                var sucursalExiste = await context.Sucursales.AnyAsync(s => s.Id == dto.SucursalId && s.Activa);
                if (!sucursalExiste)
                    return ApiResponse<OrdenServicioDTO>.fail(404, null, "Sucursal no encontrada.");
            }
            if (vehiculo == null)
                return ApiResponse<OrdenServicioDTO>.fail(404, null, "Vehículo no encontrado.");

            if (vehiculo.ClienteId != dto.ClienteId)
                return ApiResponse<OrdenServicioDTO>.fail(400, null, "El vehículo no pertenece al cliente.");

            // 3. Validar que no tenga OS abierta
            var tieneOsAbierta = await context.OrdenesServicio
                .Include(o => o.Estado)
                .AnyAsync(o => o.VehiculoId == dto.VehiculoId &&
                              o.Estado.Codigo != EstadoOs.Estados.Cerrada &&
                              o.Estado.Codigo != EstadoOs.Estados.Cancelada &&
                              o.Activo);

            if (tieneOsAbierta)
                return ApiResponse<OrdenServicioDTO>.fail(400, null, "El vehículo ya tiene una orden de servicio abierta.");

            // 4. Validar kilometraje
            if (dto.KilometrajeIngreso < vehiculo.KilometrajeActual)
                return ApiResponse<OrdenServicioDTO>.fail(400, null,
                    $"El kilometraje debe ser mayor o igual al actual ({vehiculo.KilometrajeActual} km).");

            // 5. Obtener estado inicial
            var estadoAbierta = await context.EstadosOs
                .FirstOrDefaultAsync(e => e.Codigo == EstadoOs.Estados.Abierta && e.Activo);

            if (estadoAbierta == null)
                return ApiResponse<OrdenServicioDTO>.fail(500, null, "Estado ABIERTA no configurado en el sistema.");

            // 6. Generar número de OS
            var numeroOs = await GenerarNumeroOsAsync();

            // 7. Crear OS
            var os = new OrdenServicio
            {
                NumeroOs = numeroOs,
                CitaId = null,  // Walk-in no tiene cita
                VehiculoId = dto.VehiculoId,
                ClienteId = dto.ClienteId,
                FechaApertura = TimeHelper.Now,
                EstadoId = estadoAbierta.EstadoId,
                KilometrajeIngreso = dto.KilometrajeIngreso,
                NivelCombustible = dto.NivelCombustible,
                TipoIngreso = TipoIngreso.WalkIn,
                EsGarantia = dto.EsGarantia,
                ObservacionesApertura = dto.ObservacionesApertura,

                SucursalId = dto.SucursalId,
                Activo = true,
                UsuarioCreaId = usuarioId,
                FechaCreacion = TimeHelper.Now
            };

            context.OrdenesServicio.Add(os);

            // 8. Actualizar kilometraje del vehículo
            vehiculo.ActualizarKilometraje(dto.KilometrajeIngreso);

            await context.SaveChangesAsync();

            // 9. Cargar navegaciones para el DTO
            os.Cliente = cliente;
            os.Vehiculo = vehiculo;
            os.Estado = estadoAbierta;

            return ApiResponse<OrdenServicioDTO>.ok(MapToDto(os), "Orden de servicio (Walk-In) creada exitosamente.");
        }

        public async Task<ApiResponse<OrdenServicioDTO>> UpdateAsync(UpdateOrdenServicioDTO dto, string usuarioId)
        {
            var os = await context.OrdenesServicio
                .Include(o => o.Cita)
                .Include(o => o.Cliente)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(o => o.Estado)
                .Include(o => o.Sucursal)
                .FirstOrDefaultAsync(o => o.OsId == dto.OsId && o.Activo);

            if (os == null)
                return ApiResponse<OrdenServicioDTO>.fail(404, null, "Orden de servicio no encontrada.");

            if (!os.PuedeModificarse)
                return ApiResponse<OrdenServicioDTO>.fail(400, null, "La orden de servicio no puede modificarse en su estado actual.");

            os.KilometrajeIngreso = dto.KilometrajeIngreso;
            os.NivelCombustible = dto.NivelCombustible;
            os.EsGarantia = dto.EsGarantia;
            os.ObservacionesApertura = dto.ObservacionesApertura;
            os.UsuarioModificaId = usuarioId;
            os.FechaModificacion = TimeHelper.Now;

            await context.SaveChangesAsync();

            return ApiResponse<OrdenServicioDTO>.ok(MapToDto(os), "Orden de servicio actualizada exitosamente.");
        }

        public async Task<ApiResponse<bool>> CambiarEstadoAsync(CambiarEstadoOsDTO dto, string usuarioId)
        {
            var os = await context.OrdenesServicio
                .Include(o => o.Estado)
                .Include(o => o.Cita)
                .FirstOrDefaultAsync(o => o.OsId == dto.OsId && o.Activo);

            if (os == null)
                return ApiResponse<bool>.fail(404, null, "Orden de servicio no encontrada.");

            // Validar nuevo estado
            var nuevoEstado = await context.EstadosOs
                .FirstOrDefaultAsync(e => e.EstadoId == dto.NuevoEstadoId && e.Activo);

            if (nuevoEstado == null)
                return ApiResponse<bool>.fail(404, null, "Estado no encontrado.");

            // Validar transición
            var transicionesValidas = GetTransicionesValidas(os.Estado.Codigo);
            if (!transicionesValidas.Contains(nuevoEstado.Codigo))
                return ApiResponse<bool>.fail(400, null,
                    $"No se puede cambiar de {os.Estado.Nombre} a {nuevoEstado.Nombre}.");

            os.EstadoId = dto.NuevoEstadoId;
            os.UsuarioModificaId = usuarioId;
            os.FechaModificacion = TimeHelper.Now;

            if (!string.IsNullOrEmpty(dto.Observaciones))
            {
                os.ObservacionesApertura = string.IsNullOrEmpty(os.ObservacionesApertura)
                    ? dto.Observaciones
                    : $"{os.ObservacionesApertura}\n{dto.Observaciones}";
            }

            // Si se está cerrando, sincronizar Cita
            if (nuevoEstado.Codigo == EstadoOs.Estados.Cerrada)
            {
                os.Cerrar();
                if (os.Cita != null && os.Cita.Estado == EstadoCita.EnProceso)
                    os.Cita.Completar();
            }

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, $"Estado cambiado a {nuevoEstado.Nombre}.");
        }

        public async Task<ApiResponse<int?>> CerrarAsync(CerrarOsDTO dto, string usuarioId)
        {
            var os = await context.OrdenesServicio
                .Include(o => o.Estado)
                .Include(o => o.Vehiculo)
                .Include(o => o.Servicios.Where(s => s.Activo))
                    .ThenInclude(s => s.TipoServicio)
                .Include(o => o.Cita)
                .FirstOrDefaultAsync(o => o.OsId == dto.OsId && o.Activo);

            if (os == null)
                return ApiResponse<int?>.fail(404, null, "Orden de servicio no encontrada.");

            if (!os.EstaAbierta)
                return ApiResponse<int?>.fail(400, null, "La orden de servicio ya está cerrada o cancelada.");

            // Solo se puede cerrar desde FACTURADA
            if (os.Estado.Codigo != EstadoOs.Estados.Facturada)
                return ApiResponse<int?>.fail(400, null, "Solo se puede cerrar una orden facturada.");

            // Obtener estado CERRADA
            var estadoCerrada = await context.EstadosOs
                .FirstOrDefaultAsync(e => e.Codigo == EstadoOs.Estados.Cerrada && e.Activo);

            if (estadoCerrada == null)
                return ApiResponse<int?>.fail(500, null, "Estado CERRADA no configurado en el sistema.");

            // Cerrar OS
            os.Cerrar(dto.ObservacionesCierre);
            os.EstadoId = estadoCerrada.EstadoId;
            os.UsuarioModificaId = usuarioId;
            os.FechaModificacion = TimeHelper.Now;

            // Recalcular totales
            os.CalcularTotales();

            // Completar cita si existe
            if (os.Cita != null && os.Cita.Estado == EstadoCita.EnProceso)
            {
                os.Cita.Completar();
            }

            // Guardar próxima revisión en la OS
            os.ProximaRevision = dto.ProximaRevision;

            await context.SaveChangesAsync();

            // Retornar CitaId para que el frontend pueda redirigir a fotos de salida
            return ApiResponse<int?>.ok(os.CitaId, "Orden de servicio cerrada exitosamente.");
        }

        public async Task<ApiResponse<bool>> CancelarAsync(int osId, string motivo, string usuarioId)
        {
            var os = await context.OrdenesServicio
                .Include(o => o.Estado)
                .Include(o => o.Cita)
                .FirstOrDefaultAsync(o => o.OsId == osId && o.Activo);

            if (os == null)
                return ApiResponse<bool>.fail(404, null, "Orden de servicio no encontrada.");

            if (!os.EstaAbierta)
                return ApiResponse<bool>.fail(400, null, "La orden de servicio ya está cerrada o cancelada.");

            // Validar que se puede cancelar desde el estado actual
            var estadosCancelables = new[]
            {
                EstadoOs.Estados.Abierta,
                EstadoOs.Estados.Diagnostico,
                EstadoOs.Estados.Cotizada,
                EstadoOs.Estados.Aprobada,
                EstadoOs.Estados.Pausada
            };

            if (!estadosCancelables.Contains(os.Estado.Codigo))
                return ApiResponse<bool>.fail(400, null,
                    $"No se puede cancelar una orden en estado {os.Estado.Nombre}. Solo se pueden cancelar órdenes que no estén en trabajo, completadas o facturadas.");

            // Obtener estado CANCELADA
            var estadoCancelada = await context.EstadosOs
                .FirstOrDefaultAsync(e => e.Codigo == EstadoOs.Estados.Cancelada && e.Activo);

            if (estadoCancelada == null)
                return ApiResponse<bool>.fail(500, null, "Estado CANCELADA no configurado en el sistema.");

            os.EstadoId = estadoCancelada.EstadoId;
            os.ObservacionesCierre = motivo;
            os.FechaCierre = TimeHelper.Now;
            os.UsuarioModificaId = usuarioId;
            os.FechaModificacion = TimeHelper.Now;

            // Cancelar cita si existe
            if (os.Cita != null && os.Cita.EstaActiva)
            {
                os.Cita.Cancelar(motivo);
            }

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, "Orden de servicio cancelada.");
        }

        public async Task<ApiResponse<bool>> RecalcularTotalesAsync(int osId)
        {
            var os = await context.OrdenesServicio
                .Include(o => o.Servicios.Where(s => s.Activo))
                .FirstOrDefaultAsync(o => o.OsId == osId && o.Activo);

            if (os == null)
                return ApiResponse<bool>.fail(404, null, "Orden de servicio no encontrada.");

            os.CalcularTotales();
            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, $"Totales recalculados. Total: {os.TotalGeneral:C}");
        }

        #endregion

        public async Task<ApiResponse<List<EstadoOsDTO>>> GetTransicionesValidasAsync(int osId)
        {
            var os = await context.OrdenesServicio
                .Include(o => o.Estado)
                .FirstOrDefaultAsync(o => o.OsId == osId && o.Activo);

            if (os == null)
                return ApiResponse<List<EstadoOsDTO>>.fail(404, null, "Orden de servicio no encontrada.");

            var codigosValidos = GetTransicionesValidas(os.Estado.Codigo);

            if (codigosValidos.Length == 0)
                return ApiResponse<List<EstadoOsDTO>>.ok(new List<EstadoOsDTO>(), "No hay transiciones disponibles.");

            var estados = await context.EstadosOs
                .Where(e => codigosValidos.Contains(e.Codigo) && e.Activo)
                .OrderBy(e => e.OrdenSecuencial)
                .Select(e => new EstadoOsDTO
                {
                    EstadoId = e.EstadoId,
                    Codigo = e.Codigo,
                    Nombre = e.Nombre,
                    Descripcion = e.Descripcion,
                    OrdenSecuencial = e.OrdenSecuencial,
                    EsEstadoFinal = e.EsEstadoFinal,
                    PermiteModificacion = e.PermiteModificacion
                })
                .ToListAsync();

            return ApiResponse<List<EstadoOsDTO>>.ok(estados);
        }

        #region Helpers

        private async Task<string> GenerarNumeroOsAsync()
        {
            var fecha = TimeHelper.Now;
            var prefijo = $"OS-{fecha:yyyyMMdd}-";

            var ultimaOs = await context.OrdenesServicio
                .Where(o => o.NumeroOs.StartsWith(prefijo))
                .OrderByDescending(o => o.NumeroOs)
                .FirstOrDefaultAsync();

            int siguiente = 1;
            if (ultimaOs != null)
            {
                var ultimoNumero = ultimaOs.NumeroOs.Replace(prefijo, "");
                if (int.TryParse(ultimoNumero, out int numero))
                    siguiente = numero + 1;
            }

            return $"{prefijo}{siguiente:D4}";
        }

        private static string[] GetTransicionesValidas(string estadoActual)
        {
            var transiciones = new Dictionary<string, string[]>
        {
            { EstadoOs.Estados.Abierta, new[] { EstadoOs.Estados.Diagnostico, EstadoOs.Estados.Cancelada } },
            { EstadoOs.Estados.Diagnostico, new[] { EstadoOs.Estados.Cotizada, EstadoOs.Estados.Cancelada } },
            { EstadoOs.Estados.Cotizada, new[] { EstadoOs.Estados.Aprobada, EstadoOs.Estados.Cancelada } },
            { EstadoOs.Estados.Aprobada, new[] { EstadoOs.Estados.EnTrabajo, EstadoOs.Estados.Cancelada } },
            { EstadoOs.Estados.EnTrabajo, new[] { EstadoOs.Estados.Pausada, EstadoOs.Estados.Completada } },
            { EstadoOs.Estados.Pausada, new[] { EstadoOs.Estados.EnTrabajo, EstadoOs.Estados.Cancelada } },
            { EstadoOs.Estados.Completada, new[] { EstadoOs.Estados.Facturada } },
            { EstadoOs.Estados.Facturada, new[] { EstadoOs.Estados.Cerrada } },
            { EstadoOs.Estados.Cerrada, Array.Empty<string>() },
            { EstadoOs.Estados.Cancelada, Array.Empty<string>() }
        };

            return transiciones.TryGetValue(estadoActual, out var validas) ? validas : Array.Empty<string>();
        }

        private static OrdenServicioDTO MapToDto(OrdenServicio o)
        {
            return new OrdenServicioDTO
            {
                OsId = o.OsId,
                NumeroOs = o.NumeroOs,
                CitaId = o.CitaId,
                VehiculoId = o.VehiculoId,
                ClienteId = o.ClienteId,
                FechaApertura = o.FechaApertura,
                FechaCierre = o.FechaCierre,
                EstadoId = o.EstadoId,
                KilometrajeIngreso = o.KilometrajeIngreso,
                NivelCombustible = o.NivelCombustible,
                TipoIngreso = o.TipoIngreso,
                EsGarantia = o.EsGarantia,
                ObservacionesApertura = o.ObservacionesApertura,
                ObservacionesCierre = o.ObservacionesCierre,

                TotalManoObra = o.TotalManoObra,
                TotalRepuestos = o.TotalRepuestos,
                TotalGeneral = o.TotalGeneral,
                SucursalId = o.SucursalId,
                CodigoCita = o.Cita?.CodigoCita,
                ClienteNombre = o.Cliente?.NombreCompleto ?? "",
                ClienteTelefono = o.Cliente?.Telefono,
                VehiculoDescripcion = o.Vehiculo?.DescripcionCompleta ?? "",
                VehiculoPlaca = o.Vehiculo?.Placa,
                EstadoNombre = o.Estado?.Nombre ?? "",
                EstadoCodigo = o.Estado?.Codigo ?? "",
                SucursalNombre = o.Sucursal?.Nombre,
                NivelCombustiblePorcentaje = o.NivelCombustiblePorcentaje,
                EsWalkIn = o.EsWalkIn,
                EstaAbierta = o.EstaAbierta,
                PuedeModificarse = o.PuedeModificarse
            };
        }

        private static OrdenServicioDetalleDTO MapToDetalleDto(OrdenServicio o)
        {
            return new OrdenServicioDetalleDTO
            {
                OsId = o.OsId,
                NumeroOs = o.NumeroOs,
                CitaId = o.CitaId,
                VehiculoId = o.VehiculoId,
                ClienteId = o.ClienteId,
                FechaApertura = o.FechaApertura,
                FechaCierre = o.FechaCierre,
                EstadoId = o.EstadoId,
                KilometrajeIngreso = o.KilometrajeIngreso,
                NivelCombustible = o.NivelCombustible,
                TipoIngreso = o.TipoIngreso,
                EsGarantia = o.EsGarantia,
                ObservacionesApertura = o.ObservacionesApertura,
                ObservacionesCierre = o.ObservacionesCierre,

                TotalManoObra = o.TotalManoObra,
                TotalRepuestos = o.TotalRepuestos,
                TotalGeneral = o.TotalGeneral,
                SucursalId = o.SucursalId,
                CodigoCita = o.Cita?.CodigoCita,
                ClienteNombre = o.Cliente?.NombreCompleto ?? "",
                ClienteTelefono = o.Cliente?.Telefono,
                ClienteEmail = o.Cliente?.Email,
                VehiculoDescripcion = o.Vehiculo?.DescripcionCompleta ?? "",
                VehiculoPlaca = o.Vehiculo?.Placa,
                VehiculoVin = o.Vehiculo?.Vin,
                VehiculoKilometrajeAnterior = o.Vehiculo?.KilometrajeActual ?? 0,
                VehiculoEnGarantia = o.Vehiculo?.EnGarantia ?? false,
                EstadoNombre = o.Estado?.Nombre ?? "",
                EstadoCodigo = o.Estado?.Codigo ?? "",
                SucursalNombre = o.Sucursal?.Nombre,
                NivelCombustiblePorcentaje = o.NivelCombustiblePorcentaje,
                EsWalkIn = o.EsWalkIn,
                EstaAbierta = o.EstaAbierta,
                PuedeModificarse = o.PuedeModificarse,
                CantidadServicios = o.Servicios?.Count ?? 0,
                CantidadTecnicos = o.AsignacionesTecnico?.Count ?? 0,
                CantidadEvidencias = o.Evidencias?.Count ?? 0
            };
        }

        #endregion
    }
}
