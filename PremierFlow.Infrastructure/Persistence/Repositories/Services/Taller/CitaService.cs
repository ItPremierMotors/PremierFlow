using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Validation;
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
    public class CitaService : ICitaService
    {
        private readonly PremierFlowDbContext context;
        public CitaService(PremierFlowDbContext context)
        {
            this.context = context;
        }
        #region consultas
        public async Task<ApiResponse<CitaDTO>> GetByIdAsync(int citaId)
        {
            var cita = await context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(c => c.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(c => c.TipoServicio)
                .Include(c => c.Sucursal)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CitaId == citaId && c.Activo);

            if (cita == null)
                return ApiResponse<CitaDTO>.fail(404, null, "Cita no encontrada");
            return ApiResponse<CitaDTO>.ok(MapToDto(cita));

        }
        public async Task<ApiResponse<CitaDTO>> GetByCodigoAsync(string codigoCita)
        {
            var cita = await context.Citas
               .Include(c => c.Cliente)
               .Include(c => c.Vehiculo)
                   .ThenInclude(v => v.Marca)
               .Include(c => c.Vehiculo)
                   .ThenInclude(v => v.Modelo)
               .Include(c => c.TipoServicio)
               .Include(c => c.Sucursal)
               .AsNoTracking()
               .FirstOrDefaultAsync(c => c.CodigoCita == codigoCita && c.Activo);
            if (cita == null)
                return ApiResponse<CitaDTO>.fail(404, null, "Cita no encontrada.");

            return ApiResponse<CitaDTO>.ok(MapToDto(cita), "Cita obtenida.");
        }

        public async Task<ApiResponse<List<CitaDTO>>> GetAllAsync()
        {
            var cita = await context.Citas
               .Include(c => c.Cliente)
               .Include(c => c.Vehiculo)
                   .ThenInclude(v => v.Marca)
               .Include(c => c.Vehiculo)
                   .ThenInclude(v => v.Modelo)
               .Include(c => c.TipoServicio)
               .Include(c => c.Sucursal)
               .AsNoTracking()
               .Where(c => c.Activo)
               .OrderByDescending(c => c.FechaHoraInicio)
               .ToListAsync();
            var dtos = cita.Select(MapToDto).ToList();
            return ApiResponse<List<CitaDTO>>.ok(dtos, "Citas Obtenidas");
        }
        public async Task<ApiResponse<List<CitaDTO>>> GetByFechaAsync(DateTime fecha, int? sucursalId = null)
        {
            var query = context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Vehiculo)
                   .ThenInclude(V => V.Marca)
                .Include(v => v.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(c => c.TipoServicio)
                .Include(c => c.Sucursal)
                .AsNoTracking()
                .Where(c => c.FechaHoraInicio.Date == fecha.Date && c.Activo);

            if (sucursalId.HasValue)
                query = query.Where(c => c.SucursalId == sucursalId);
            var citas = await query.OrderBy(c => c.FechaHoraInicio).ToListAsync();
            var dtos = citas.Select(MapToDto).ToList();
            return ApiResponse<List<CitaDTO>>.ok(dtos, "citas obtenidas");
        }
        public async Task<ApiResponse<List<CitaDTO>>> GetByClienteAsync(int clienteId)
        {
            var citas = await context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(c => c.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(c => c.TipoServicio)
                .Include(c => c.Sucursal)
                .AsNoTracking()
                .Where(c => c.ClienteId == clienteId && c.Activo)
                .OrderByDescending(c => c.FechaHoraInicio)
                .ToListAsync();

            var dtos = citas.Select(MapToDto).ToList();

            return ApiResponse<List<CitaDTO>>.ok(dtos, "Citas obtenidas.");
        }

        public async Task<ApiResponse<List<CitaDTO>>> GetByVehiculoAsync(int vehiculoId)
        {
            var citas = await context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(c => c.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(c => c.TipoServicio)
                .Include(c => c.Sucursal)
                .AsNoTracking()
                .Where(c => c.VehiculoId == vehiculoId && c.Activo)
                .OrderByDescending(c => c.FechaHoraInicio)
                .ToListAsync();

            var dtos = citas.Select(MapToDto).ToList();

            return ApiResponse<List<CitaDTO>>.ok(dtos, "Citas obtenidas.");
        }
        public async Task<ApiResponse<List<CitaDTO>>> GetByEstadoAsync(EstadoCita estado, int? sucursalId = null)
        {
            var query = context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(c => c.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(c => c.TipoServicio)
                .Include(c => c.Sucursal)
                .AsNoTracking()
                .Where(c => c.Estado == estado && c.Activo);

            if (sucursalId.HasValue)
                query = query.Where(c => c.SucursalId == sucursalId);

            var citas = await query
                .OrderBy(c => c.FechaHoraInicio)
                .ToListAsync();

            var dtos = citas.Select(MapToDto).ToList();

            return ApiResponse<List<CitaDTO>>.ok(dtos, "Citas obtenidas.");
        }
        public async Task<ApiResponse<List<CitaDTO>>> GetActivasDelDiaAsync(DateTime fecha, int? sucursalId = null)
        {
            var query = context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(c => c.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(c => c.TipoServicio)
                .Include(c => c.Sucursal)
                .AsNoTracking()
                .Where(c => c.FechaHoraInicio.Date == fecha.Date &&
                           (c.Estado == EstadoCita.Agendada || c.Estado == EstadoCita.Confirmada) &&
                           c.Activo);

            if (sucursalId.HasValue)
                query = query.Where(c => c.SucursalId == sucursalId);

            var citas = await query
                .OrderBy(c => c.FechaHoraInicio)
                .ToListAsync();

            var dtos = citas.Select(MapToDto).ToList();

            return ApiResponse<List<CitaDTO>>.ok(dtos, "Citas activas obtenidas.");
        }
        #endregion

        #region Acciones
         public async Task<ApiResponse<CitaDTO>> AgendarAsync(CreateCitaDTO dto, string usuarioId)
        {
            //1. validar ciente
            var cliente = await context.Clientes
             .FirstOrDefaultAsync(c => c.ClienteId == dto.ClienteId && c.Activo);
            if (cliente == null)
                return ApiResponse<CitaDTO>.fail(404, null, "cliente no encontrado");

            //2. validar que el vehiculo pertenezca al cliente
            var vehiculo = await context.Vehiculos
                .Include(v => v.Marca)
                .Include(v => v.Modelo)
                .FirstOrDefaultAsync(v => v.VehiculoId == dto.VehiculoId && v.Activo);

            if (vehiculo == null)
            return ApiResponse<CitaDTO>.fail(404, null, "Vehículo no encontrado.");

            if (vehiculo.ClienteId != dto.ClienteId)
                return ApiResponse<CitaDTO>.fail(400, null, "El vehículo no pertenece al cliente.");
            // 3. Validar tipo de servicio
            var tipoServicio = await context.TiposServicio
                .FirstOrDefaultAsync(t => t.TipoServicioId == dto.TipoServicioId && t.Activo);

            if (tipoServicio == null)
                return ApiResponse<CitaDTO>.fail(404, null, "Tipo de servicio no encontrado.");

            // 3.1 Validar compatibilidad servicio ↔ TipoIngreso
            if (dto.TipoIngreso == TipoIngreso.WalkIn && !tipoServicio.PermiteWalkIn)
                return ApiResponse<CitaDTO>.fail(400, null, "Este tipo de servicio no permite Walk-In.");

            // 4. Validar que no tenga cita activa para el mismo vehículo
            var tieneCitaActiva = await context.Citas
                .AnyAsync(c => c.VehiculoId == dto.VehiculoId &&
                              (c.Estado == EstadoCita.Agendada || c.Estado == EstadoCita.Confirmada) &&
                              c.Activo);

            if (tieneCitaActiva)
                return ApiResponse<CitaDTO>.fail(400, null, "El vehículo ya tiene una cita activa.");

            // 5. Validar fecha
            if (dto.TipoIngreso == TipoIngreso.WalkIn)
            {
                // Walk-In: el cliente está presente, solo validar que sea hoy o futuro
                if (dto.FechaHoraInicio.Date < DateTime.Now.Date)
                    return ApiResponse<CitaDTO>.fail(400, null, "La fecha del Walk-In no puede ser anterior a hoy.");
            }
            else
            {
                // Cita y Garantía: debe ser fecha/hora futura
                if (dto.FechaHoraInicio <= DateTime.Now)
                    return ApiResponse<CitaDTO>.fail(400, null, "La fecha de la cita debe ser futura.");
            }

            // 6. Calcular hora fin
            var fechaHoraFin = dto.FechaHoraInicio.AddMinutes(tipoServicio.DuracionEstimadaMin);

            // 7. Validar capacidad del día (si existe)
            var capacidad = await context.CapacidadTaller
                .FirstOrDefaultAsync(c => c.Fecha.Date == dto.FechaHoraInicio.Date &&
                                         c.SucursalId == dto.SucursalId &&
                                         c.Activo);

            if (capacidad != null)
            {
                if (!capacidad.PermiteAgendamiento)
                    return ApiResponse<CitaDTO>.fail(400, null, "No se permite agendar citas para esta fecha.");

                if (!capacidad.TieneCapacidadPara(tipoServicio.DuracionEstimadaMin))
                    return ApiResponse<CitaDTO>.fail(400, null, "No hay capacidad disponible para esta fecha.");
            }

            // 8. Validar bloque horario
            BloqueHorario? bloque = null;
            if (dto.BloqueHorarioId.HasValue)
            {
                bloque = await context.BloquesHorario
                    .FirstOrDefaultAsync(b => b.BloqueId == dto.BloqueHorarioId && b.Activo);

                if (bloque == null)
                    return ApiResponse<CitaDTO>.fail(404, null, "Bloque horario no encontrado.");

                if (!bloque.TieneEspacioDisponible)
                    return ApiResponse<CitaDTO>.fail(400, null, "No hay espacio disponible en el bloque seleccionado.");
            }
            else if (capacidad != null)
            {
                // Sin bloque seleccionado — verificar si hay bloques activos
                var hayBloques = await context.BloquesHorario
                    .AnyAsync(b => b.CapacidadId == capacidad.CapacidadId && b.Activo);

                if (hayBloques)
                    return ApiResponse<CitaDTO>.fail(400, null,
                        "Debe seleccionar un bloque horario. Hay bloques configurados para esta fecha.");

                // Sin bloques configurados — Cita y Garantia requieren bloques, solo Walk-In permite hora manual
                if (dto.TipoIngreso != TipoIngreso.WalkIn)
                    return ApiResponse<CitaDTO>.fail(400, null,
                        "No hay bloques horarios configurados para esta fecha. Configure bloques o use Walk-In.");
            }

            // 9. Generar código de cita
            var codigoCita = await GenerarCodigoCitaAsync();
            var preOrdenId = await GenerarPreOrdenIdAsync();
            // 10. Crear cita
            var cita = new Cita
            {
                CodigoCita = codigoCita,
                PreOrdenId = preOrdenId,
                ClienteId = dto.ClienteId,
                VehiculoId = dto.VehiculoId,
                TipoServicioId = dto.TipoServicioId,
                FechaHoraInicio = dto.FechaHoraInicio,
                FechaHoraFin = fechaHoraFin,
                FechaRecepcion = dto.FechaHoraInicio.Date,
                Estado = EstadoCita.Agendada,
                TipoIngreso = dto.TipoIngreso,
                MotivoVisita = dto.MotivoVisita,
                Observaciones = dto.Observaciones,
                SucursalId = dto.SucursalId,
                CapacidadId = capacidad?.CapacidadId,
                BloqueHorarioId = bloque?.BloqueId,
                Activo = true,
                UsuarioCreaId = usuarioId,
                FechaCreacion = DateTime.UtcNow
            };

            context.Citas.Add(cita);

            // 11. Reservar capacidad y bloque
            if (capacidad != null)
            {
                capacidad.ReservarMinutos(tipoServicio.DuracionEstimadaMin);
            }

            if (bloque != null)
            {
                bloque.AgendarVehiculo();
            }

            await context.SaveChangesAsync();

            // 12. Cargar navegaciones para el DTO
            cita.Cliente = cliente;
            cita.Vehiculo = vehiculo;
            cita.TipoServicio = tipoServicio;

            if (dto.SucursalId.HasValue)
            {
                cita.Sucursal = await context.Sucursales
                    .FirstOrDefaultAsync(s => s.Id == dto.SucursalId);
            }

            return ApiResponse<CitaDTO>.ok(MapToDto(cita), "Cita agendada exitosamente.");
        }

        public async Task<ApiResponse<CitaDTO>> UpdateAsync(UpdateCitaDTO dto, string usuarioId)
        {
            var cita = await context.Citas
            .Include(c => c.Cliente)
            .Include(c => c.Vehiculo)
                .ThenInclude(v => v.Marca)
            .Include(c => c.Vehiculo)
                .ThenInclude(v => v.Modelo)
            .Include(c => c.TipoServicio)
            .Include(c => c.Sucursal)
            .FirstOrDefaultAsync(c => c.CitaId == dto.CitaId && c.Activo);

            if (cita == null)
                return ApiResponse<CitaDTO>.fail(404, null, "Cita no encontrada");
            if (!cita.EstaActiva)
                return ApiResponse<CitaDTO>.fail(400, null, "Solo pueden modificarse citas activas");

            //validar fechas futuras
            if (dto.FechaHoraInicio <= DateTime.Now)
                return ApiResponse<CitaDTO>.fail(400, null, "la fecha de la cita debe ser futura");

            //calcular nueva hora de fin
            var nuevaFechaFin = dto.FechaHoraInicio.AddMinutes(cita.TipoServicio.DuracionEstimadaMin);

            cita.FechaHoraInicio = dto.FechaHoraInicio;
            cita.FechaHoraFin = nuevaFechaFin;
            cita.MotivoVisita = dto.MotivoVisita;
            cita.UsuarioModificaId = usuarioId;
            cita.FechaModificacion = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return ApiResponse<CitaDTO>.ok(MapToDto(cita), "cita actualiada exictosamente");
        }
        public async Task<ApiResponse<bool>> ConfirmarAsync(int citaId, string usuarioId)
        {
            var cita = await context.Citas
            .FirstOrDefaultAsync(c => c.CitaId == citaId && c.Activo);

            if (cita == null)
                return ApiResponse<bool>.fail(404, null, "Cita no encontrada.");

            if (cita.Estado != EstadoCita.Agendada)
                return ApiResponse<bool>.fail(400, null, "Solo se pueden confirmar citas agendadas.");

            cita.Confirmar();
            cita.UsuarioModificaId = usuarioId;
            cita.FechaModificacion = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, "Cita confirmada exitosamente.");
        }
        public async Task<ApiResponse<bool>> CancelarAsync(CancelarCitaDTO dto, string usuarioId)
        {
            var cita = await context.Citas
                .Include(c => c.TipoServicio)
                .FirstOrDefaultAsync(c => c.CitaId == dto.CitaId && c.Activo);

            if (cita == null)
                return ApiResponse<bool>.fail(404, null, "Cita no encontrada.");

            if (!cita.EstaActiva)
                return ApiResponse<bool>.fail(400, null, "Solo se pueden cancelar citas activas.");

            // Liberar capacidad usando FK directa
            if (cita.CapacidadId.HasValue)
            {
                var capacidad = await context.CapacidadTaller.FindAsync(cita.CapacidadId.Value);
                if (capacidad != null)
                    capacidad.LiberarMinutos(cita.TipoServicio.DuracionEstimadaMin);
            }

            // Liberar bloque usando FK directa
            if (cita.BloqueHorarioId.HasValue)
            {
                var bloque = await context.BloquesHorario.FindAsync(cita.BloqueHorarioId.Value);
                if (bloque != null)
                    bloque.LiberarEspacio();
            }

            cita.Cancelar(dto.MotivoCancelacion);
            cita.UsuarioModificaId = usuarioId;
            cita.FechaModificacion = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, "Cita cancelada exitosamente.");
        }
        public async Task<ApiResponse<bool>> MarcarNoShowAsync(int citaId, string usuarioId) {
            var cita = await context.Citas
          .Include(c => c.Cliente)
          .Include(c => c.TipoServicio)
          .FirstOrDefaultAsync(c => c.CitaId == citaId && c.Activo);

            if (cita == null)
                return ApiResponse<bool>.fail(404, null, "Cita no encontrada.");

            if (!cita.EstaActiva)
                return ApiResponse<bool>.fail(400, null, "Solo se pueden marcar no-show citas activas.");

            // Incrementar contador de no-show del cliente
            cita.Cliente.IncrementarNoShow();

            // Liberar capacidad usando FK directa
            if (cita.CapacidadId.HasValue)
            {
                var capacidad = await context.CapacidadTaller.FindAsync(cita.CapacidadId.Value);
                if (capacidad != null)
                    capacidad.LiberarMinutos(cita.TipoServicio.DuracionEstimadaMin);
            }

            // Liberar bloque usando FK directa
            if (cita.BloqueHorarioId.HasValue)
            {
                var bloque = await context.BloquesHorario.FindAsync(cita.BloqueHorarioId.Value);
                if (bloque != null)
                    bloque.LiberarEspacio();
            }

            cita.MarcarNoShow();
            cita.UsuarioModificaId = usuarioId;
            cita.FechaModificacion = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, $"No-show registrado. El cliente tiene {cita.Cliente.NoShowCount} inasistencias.");
        }
       public async Task<ApiResponse<bool>> IniciarAtencionAsync(int citaId, string usuarioId)
        {
            var cita = await context.Citas
           .FirstOrDefaultAsync(c => c.CitaId == citaId && c.Activo);

            if (cita == null)
                return ApiResponse<bool>.fail(404, null, "Cita no encontrada.");

            if (!cita.PuedeConvertirseEnOs)
                return ApiResponse<bool>.fail(400, null, "La cita no está en estado válido para iniciar atención.");

            cita.IniciarProceso();
            cita.UsuarioModificaId = usuarioId;
            cita.FechaModificacion = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, "Atención iniciada. Puede crear la Orden de Servicio.");
        
        }
        public async Task<ApiResponse<bool>> CompletarAsync(int citaId, string usuarioId)
        {
            var cita = await context.Citas
                .Include(c => c.TipoServicio)
                .FirstOrDefaultAsync(c => c.CitaId == citaId && c.Activo);

            if (cita == null)
                return ApiResponse<bool>.fail(404, null, "Cita no encontrada.");

            if (cita.Estado != EstadoCita.EnProceso)
                return ApiResponse<bool>.fail(400, null, "Solo se pueden completar citas en proceso.");

            cita.Completar();
            cita.UsuarioModificaId = usuarioId;
            cita.FechaModificacion = DateTime.UtcNow;

            // Registrar minutos trabajados usando FK directa
            if (cita.CapacidadId.HasValue)
            {
                var capacidad = await context.CapacidadTaller.FindAsync(cita.CapacidadId.Value);
                if (capacidad != null)
                    capacidad.RegistrarTiempoTrabajado(cita.TipoServicio.DuracionEstimadaMin);
            }

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, "Cita completada exitosamente.");
        }

        public async Task<ApiResponse<CitaDTO>> TransferirAsync(TransferirCitaDTO dto, string usuarioId)
        {
            // 1. Cargar la cita con sus relaciones
            var cita = await context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(c => c.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(c => c.TipoServicio)
                .Include(c => c.Sucursal)
                .FirstOrDefaultAsync(c => c.CitaId == dto.CitaId && c.Activo);

            if (cita == null)
                return ApiResponse<CitaDTO>.fail(404, null, "Cita no encontrada.");

            if (cita.Estado != EstadoCita.EnProceso)
                return ApiResponse<CitaDTO>.fail(400, null, "Solo se pueden transferir citas en proceso.");

            // 2. Calcular minutos restantes basado en la duración ACTUAL de la cita
            var minutosActuales = (int)cita.Duracion.TotalMinutes;

            if (dto.MinutosTrabajadosHoy <= 0)
                return ApiResponse<CitaDTO>.fail(400, null, "Los minutos trabajados deben ser mayor a 0.");

            if (dto.MinutosTrabajadosHoy >= minutosActuales)
                return ApiResponse<CitaDTO>.fail(400, null, "Si se trabajaron todos los minutos, use Completar en vez de Transferir.");

            var minutosRestantes = minutosActuales - dto.MinutosTrabajadosHoy;

            // 3. Validar capacidad de mañana ANTES de modificar nada
            var manana = cita.FechaHoraInicio.Date.AddDays(1);

            var capacidadManana = await context.CapacidadTaller
                .FirstOrDefaultAsync(c => c.Fecha.Date == manana &&
                                         c.SucursalId == cita.SucursalId &&
                                         c.Activo);

            if (capacidadManana == null)
                return ApiResponse<CitaDTO>.fail(400, null,
                    $"No hay capacidad de taller configurada para {manana:dd/MM/yyyy}. Configure la capacidad antes de transferir.");

            if (!capacidadManana.TieneCapacidadPara(minutosRestantes))
                return ApiResponse<CitaDTO>.fail(400, null,
                    $"No hay capacidad suficiente para {manana:dd/MM/yyyy}. Se requieren {minutosRestantes} min.");

            // 3.1 Buscar primer bloque disponible para mañana
            var bloqueManana = await context.BloquesHorario
                .Where(b => b.CapacidadId == capacidadManana.CapacidadId &&
                            b.Activo && b.VehiculosAgendados < b.CapacidadMaximaVehiculos)
                .OrderBy(b => b.HoraInicio)
                .FirstOrDefaultAsync();

            // 4. Liberar capacidad y bloque de HOY
            if (cita.CapacidadId.HasValue)
            {
                var capacidadHoy = await context.CapacidadTaller.FindAsync(cita.CapacidadId.Value);
                if (capacidadHoy != null)
                {
                    capacidadHoy.RegistrarTiempoTrabajado(dto.MinutosTrabajadosHoy);
                    capacidadHoy.LiberarMinutos(minutosRestantes);
                }
            }

            if (cita.BloqueHorarioId.HasValue)
            {
                var bloqueHoy = await context.BloquesHorario.FindAsync(cita.BloqueHorarioId.Value);
                if (bloqueHoy != null)
                    bloqueHoy.LiberarEspacio();
            }

            // 5. Actualizar la MISMA cita para mañana (no se crea fila nueva)
            var horaInicio = new DateTime(manana.Year, manana.Month, manana.Day, 8, 0, 0);
            if (bloqueManana != null)
            {
                horaInicio = new DateTime(manana.Year, manana.Month, manana.Day,
                    bloqueManana.HoraInicio.Hours, bloqueManana.HoraInicio.Minutes, 0);
            }

            cita.MinutosTrabajados = (cita.MinutosTrabajados ?? 0) + dto.MinutosTrabajadosHoy;
            cita.FechaHoraInicio = horaInicio;
            cita.FechaHoraFin = horaInicio.AddMinutes(minutosRestantes);
            cita.CapacidadId = capacidadManana.CapacidadId;
            cita.BloqueHorarioId = bloqueManana?.BloqueId;
            cita.Observaciones = (cita.Observaciones ?? "") +
                $"\n[Transferida {DateTime.Now:dd/MM}] {dto.MinutosTrabajadosHoy} min trabajados, {minutosRestantes} min pendientes.";
            cita.UsuarioModificaId = usuarioId;
            cita.FechaModificacion = DateTime.UtcNow;
            // Estado sigue siendo EnProceso — no cambia

            // 6. Reservar capacidad y bloque de mañana
            capacidadManana.ReservarMinutos(minutosRestantes);

            if (bloqueManana != null)
            {
                bloqueManana.AgendarVehiculo();
            }

            await context.SaveChangesAsync();

            // La OS no se toca — sigue apuntando al mismo CitaId

            return ApiResponse<CitaDTO>.ok(MapToDto(cita),
                $"Cita transferida a {manana:dd/MM/yyyy}. {dto.MinutosTrabajadosHoy} min registrados, {minutosRestantes} min pendientes.");
        }

        #endregion

        #region Helpers

        private async Task<string> GenerarCodigoCitaAsync()
        {
            var fecha = DateTime.Now;
            var prefijo = $"CIT-{fecha:yyyyMMdd}-";

            var ultimaCita = await context.Citas
                .Where(c => c.CodigoCita.StartsWith(prefijo))
                .OrderByDescending(c => c.CodigoCita)
                .FirstOrDefaultAsync();

            int siguiente = 1;
            if (ultimaCita != null)
            {
                var ultimoNumero = ultimaCita.CodigoCita.Replace(prefijo, "");
                if (int.TryParse(ultimoNumero, out int numero))
                {
                    siguiente = numero + 1;
                }
            }

            return $"{prefijo}{siguiente:D4}";
        }
        private async Task<string> GenerarPreOrdenIdAsync()
        {
            var fecha = DateTime.Now;
            var prefijo = $"PRE-{fecha:yyyyMMdd}-";

            var ultimaPreOrden = await context.Citas
                .Where(c => c.PreOrdenId != null && c.PreOrdenId.StartsWith(prefijo))
                .OrderByDescending(c => c.PreOrdenId)
                .FirstOrDefaultAsync();

            int siguiente = 1;
            if (ultimaPreOrden != null)
            {
                var ultimoNumero = ultimaPreOrden.PreOrdenId!.Replace(prefijo, "");
                if (int.TryParse(ultimoNumero, out int numero))
                    siguiente = numero + 1;
            }

            return $"{prefijo}{siguiente:D4}";
        }
        private static CitaDTO MapToDto(Cita c)
        {
            return new CitaDTO
            {
                CitaId = c.CitaId,
                CodigoCita = c.CodigoCita,
                ClienteId = c.ClienteId,
                VehiculoId = c.VehiculoId,
                TipoServicioId = c.TipoServicioId,
                FechaHoraInicio = c.FechaHoraInicio,
                FechaHoraFin = c.FechaHoraFin,
                FechaRecepcion = c.FechaRecepcion,
                Estado = c.Estado,
                TipoIngreso = c.TipoIngreso,
                MotivoVisita = c.MotivoVisita,
                Observaciones = c.Observaciones,
                MotivoCancelacion = c.MotivoCancelacion,
                PreOrdenId = c.PreOrdenId,
                SucursalId = c.SucursalId,
                ClienteNombre = c.Cliente?.NombreCompleto ?? "",
                ClienteTelefono = c.Cliente?.Telefono,
                VehiculoDescripcion = c.Vehiculo?.DescripcionCompleta ?? "",
                VehiculoPlaca = c.Vehiculo?.Placa,
                TipoServicioNombre = c.TipoServicio?.Nombre ?? "",
                SucursalNombre = c.Sucursal?.Nombre,
                DuracionMinutos = (int)c.Duracion.TotalMinutes,
                MinutosTrabajados = c.MinutosTrabajados,
                CapacidadId = c.CapacidadId,
                BloqueHorarioId = c.BloqueHorarioId,
                EstaActiva = c.EstaActiva,
                PuedeConvertirseEnOs = c.PuedeConvertirseEnOs
            };
        }

        public async Task<ApiResponse<List<CitaDTO>>> GetByRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin, int? sucursalId = null)
        {
            var query = context.Citas
            .Include(c => c.Cliente)
            .Include(c => c.Vehiculo)
                .ThenInclude(v => v.Marca)
            .Include(c => c.Vehiculo)
                .ThenInclude(v => v.Modelo)
            .Include(c => c.TipoServicio)
            .Include(c => c.Sucursal)
            .AsNoTracking()
            .Where(c => c.FechaHoraInicio.Date >= fechaInicio.Date &&
                       c.FechaHoraInicio.Date <= fechaFin.Date &&
                       c.Activo);

            if (sucursalId.HasValue)
                query = query.Where(c => c.SucursalId == sucursalId);

            var citas = await query
                .OrderBy(c => c.FechaHoraInicio)
                .ToListAsync();

            var dtos = citas.Select(MapToDto).ToList();

            return ApiResponse<List<CitaDTO>>.ok(dtos, "Citas obtenidas.");
        }

        #endregion
    }
}
//## Resumen de métodos:

//| Método | Descripción |
//| --------| -------------|
//| `GetByIdAsync` | Obtener por ID |
//| `GetByCodigoAsync` | Buscar por código (CIT-20260128-0001) |
//| `GetByFechaAsync` | Citas de un día |
//| `GetByRangoFechasAsync` | Citas de varios días (calendario) |
//| `GetByClienteAsync` | Historial de citas del cliente |
//| `GetByVehiculoAsync` | Historial de citas del vehículo |
//| `GetByEstadoAsync` | Filtrar por estado |
//| `GetActivasDelDiaAsync` | Citas pendientes de hoy |
//| `AgendarAsync` | Crear nueva cita |
//| `UpdateAsync` | Modificar cita |
//| `ConfirmarAsync` | Confirmar cita agendada |
//| `CancelarAsync` | Cancelar cita (libera capacidad) |
//| `MarcarNoShowAsync` | Cliente no llegó (incrementa contador) |
//| `IniciarAtencionAsync` | Cambiar a EnProceso |
//| `CompletarAsync` | Marcar como completada |

//---

//## Flujo de estados:
//```
//Agendada → Confirmada → EnProceso → Completada
//    ↓          ↓
//    └──────────┴──→ Cancelada
//    └──────────┴──→ NoShow