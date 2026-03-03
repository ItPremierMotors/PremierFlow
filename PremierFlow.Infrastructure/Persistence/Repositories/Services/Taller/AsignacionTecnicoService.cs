using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
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
    internal class AsignacionTecnicoService : IAsignacionTecnicoService
    {
        private readonly PremierFlowDbContext context;

        public AsignacionTecnicoService(PremierFlowDbContext context)
        {
            this.context = context;
        }

        public async Task<ApiResponse<AsignacionTecnicoDTO>> GetByIdAsync(int asignacionId)
        {
            var asignacion = await context.AsignacionesTecnico
                .Include(a => a.OrdenServicio).ThenInclude(o => o.Vehiculo).ThenInclude(v => v.Marca)
                .Include(a => a.OrdenServicio).ThenInclude(o => o.Vehiculo).ThenInclude(v => v.Modelo)
                .Include(a => a.Tecnico)
                .Include(a => a.OsServicio)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AsignacionId == asignacionId && a.Activo);

            if (asignacion == null)
                return ApiResponse<AsignacionTecnicoDTO>.fail(404, null, "Asignación no encontrada.");

            return ApiResponse<AsignacionTecnicoDTO>.ok(MapToDto(asignacion), "Asignación obtenida.");
        }

        public async Task<ApiResponse<List<AsignacionTecnicoDTO>>> GetByOsIdAsync(int osId)
        {
            var asignaciones = await context.AsignacionesTecnico
                .Include(a => a.OrdenServicio).ThenInclude(o => o.Vehiculo).ThenInclude(v => v.Marca)
                .Include(a => a.OrdenServicio).ThenInclude(o => o.Vehiculo).ThenInclude(v => v.Modelo)
                .Include(a => a.Tecnico)
                .Include(a => a.OsServicio)
                .AsNoTracking()
                .Where(a => a.OsId == osId && a.Activo)
                .OrderBy(a => a.FechaAsignacion)
                .ToListAsync();

            var dtos = asignaciones.Select(MapToDto).ToList();

            return ApiResponse<List<AsignacionTecnicoDTO>>.ok(dtos, "Asignaciones obtenidas.");
        }

        public async Task<ApiResponse<List<AsignacionTecnicoDTO>>> GetByTecnicoIdAsync(int tecnicoId)
        {
            var asignaciones = await context.AsignacionesTecnico
                .Include(a => a.OrdenServicio).ThenInclude(o => o.Vehiculo).ThenInclude(v => v.Marca)
                .Include(a => a.OrdenServicio).ThenInclude(o => o.Vehiculo).ThenInclude(v => v.Modelo)
                .Include(a => a.Tecnico)
                .Include(a => a.OsServicio)
                .AsNoTracking()
                .Where(a => a.TecnicoId == tecnicoId && a.Activo)
                .OrderByDescending(a => a.FechaAsignacion)
                .ToListAsync();

            var dtos = asignaciones.Select(MapToDto).ToList();

            return ApiResponse<List<AsignacionTecnicoDTO>>.ok(dtos, "Asignaciones obtenidas.");
        }

        public async Task<ApiResponse<List<AsignacionTecnicoDTO>>> GetActivasByTecnicoIdAsync(int tecnicoId)
        {
            var asignaciones = await context.AsignacionesTecnico
                .Include(a => a.OrdenServicio).ThenInclude(o => o.Vehiculo).ThenInclude(v => v.Marca)
                .Include(a => a.OrdenServicio).ThenInclude(o => o.Vehiculo).ThenInclude(v => v.Modelo)
                .Include(a => a.Tecnico)
                .Include(a => a.OsServicio)
                .AsNoTracking()
                .Where(a => a.TecnicoId == tecnicoId &&
                           a.Activo &&
                           (a.Estado == EstadoAsignacion.Asignado || a.Estado == EstadoAsignacion.EnProceso || a.Estado == EstadoAsignacion.Pausado))
                .OrderBy(a => a.FechaAsignacion)
                .ToListAsync();

            var dtos = asignaciones.Select(MapToDto).ToList();

            return ApiResponse<List<AsignacionTecnicoDTO>>.ok(dtos, "Asignaciones activas obtenidas.");
        }

        public async Task<ApiResponse<AsignacionTecnicoDTO>> AsignarAsync(CreateAsignacionDTO dto, string usuarioId)
        {
            // 1. Validar que existe la OS
            var os = await context.OrdenesServicio
                .Include(o => o.Estado)
                .FirstOrDefaultAsync(o => o.OsId == dto.OsId && o.Activo);

            if (os == null)
                return ApiResponse<AsignacionTecnicoDTO>.fail(404, null, "Orden de servicio no encontrada.");

            if (!os.EstaAbierta)
                return ApiResponse<AsignacionTecnicoDTO>.fail(400, null, "La orden de servicio no está abierta.");

            // 2. Validar técnico
            var tecnico = await context.Tecnicos
                .FirstOrDefaultAsync(t => t.TecnicoId == dto.TecnicoId && t.Activo);

            if (tecnico == null)
                return ApiResponse<AsignacionTecnicoDTO>.fail(404, null, "Técnico no encontrado.");

            // 3. Validar servicio (obligatorio)
            if (!dto.OsServicioId.HasValue)
                return ApiResponse<AsignacionTecnicoDTO>.fail(400, null, "Debe seleccionar un servicio.");

            OsServicio? osServicio = null;
            if (dto.OsServicioId.HasValue)
            {
                osServicio = await context.OsServicios
                    .FirstOrDefaultAsync(s => s.OsServicioId == dto.OsServicioId &&
                                             s.OsId == dto.OsId &&
                                             s.Activo);

                if (osServicio == null)
                    return ApiResponse<AsignacionTecnicoDTO>.fail(404, null, "Servicio no encontrado en esta OS.");

                if (osServicio.EstaCompletado || osServicio.EstaCancelado)
                    return ApiResponse<AsignacionTecnicoDTO>.fail(400, null, "El servicio ya está completado o cancelado.");
            }

            // 4. Validar que el servicio no tenga ya una asignación activa (cualquier técnico)
            var servicioYaAsignado = await context.AsignacionesTecnico
                .AnyAsync(a => a.OsId == dto.OsId &&
                              a.OsServicioId == dto.OsServicioId &&
                              a.Activo &&
                              (a.Estado == EstadoAsignacion.Asignado || a.Estado == EstadoAsignacion.EnProceso));

            if (servicioYaAsignado)
                return ApiResponse<AsignacionTecnicoDTO>.fail(400, null, "Este servicio ya tiene un técnico asignado.");

            // 5. Crear asignación
            var asignacion = new AsignacionTecnico
            {
                OsId = dto.OsId,
                TecnicoId = dto.TecnicoId,
                OsServicioId = dto.OsServicioId,
                FechaAsignacion = TimeHelper.Now,
                Estado = EstadoAsignacion.Asignado,
                Observaciones = dto.Observaciones,
                Activo = true,
                UsuarioCreaId = usuarioId,
                FechaCreacion = TimeHelper.Now
            };

            context.AsignacionesTecnico.Add(asignacion);

            // 6. Actualizar TecnicoAsignadoId en el servicio si aplica
            if (osServicio != null)
            {
                osServicio.TecnicoAsignadoId = dto.TecnicoId;
            }

            await context.SaveChangesAsync();

            // 7. Cargar navegaciones para el DTO
            asignacion.OrdenServicio = os;
            asignacion.Tecnico = tecnico;
            asignacion.OsServicio = osServicio;

            return ApiResponse<AsignacionTecnicoDTO>.ok(MapToDto(asignacion), "Técnico asignado exitosamente.");
        }

        public async Task<ApiResponse<bool>> IniciarTrabajoAsync(int asignacionId, string usuarioId)
        {
            var asignacion = await context.AsignacionesTecnico
                .Include(a => a.OrdenServicio)
                    .ThenInclude(o => o.Estado)
                .Include(a => a.OsServicio)
                .FirstOrDefaultAsync(a => a.AsignacionId == asignacionId && a.Activo);

            if (asignacion == null)
                return ApiResponse<bool>.fail(404, null, "Asignación no encontrada.");

            if (!asignacion.OrdenServicio.EstaAbierta)
                return ApiResponse<bool>.fail(400, null, "La orden de servicio no está abierta.");

            // Solo se puede iniciar trabajo si la OS esta en APROBADA o EN_TRABAJO
            var estadoOs = asignacion.OrdenServicio.Estado.Codigo;
            if (estadoOs != EstadoOs.Estados.Aprobada && estadoOs != EstadoOs.Estados.EnTrabajo)
                return ApiResponse<bool>.fail(400, null, "Solo se puede iniciar trabajo cuando la OS está Aprobada o En Trabajo.");

            if (asignacion.Estado != EstadoAsignacion.Asignado)
                return ApiResponse<bool>.fail(400, null, "Solo se puede iniciar una asignación pendiente.");

            asignacion.IniciarTrabajo();
            asignacion.UsuarioModificaId = usuarioId;
            asignacion.FechaModificacion = TimeHelper.Now;

            // Iniciar el servicio si está vinculado y pendiente
            if (asignacion.OsServicio != null && asignacion.OsServicio.Estado == EstadoServicioOS.Pendiente)
            {
                asignacion.OsServicio.IniciarTrabajo();
            }

            // Auto-transicionar la OS a EN_TRABAJO si está en APROBADA
            if (estadoOs == EstadoOs.Estados.Aprobada)
            {
                var estadoEnTrabajo = await context.EstadosOs
                    .FirstOrDefaultAsync(e => e.Codigo == EstadoOs.Estados.EnTrabajo && e.Activo);

                if (estadoEnTrabajo != null)
                {
                    asignacion.OrdenServicio.EstadoId = estadoEnTrabajo.EstadoId;
                }
            }

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, "Trabajo iniciado exitosamente.");
        }

        public async Task<ApiResponse<bool>> PausarAsync(int asignacionId, string usuarioId)
        {
            var asignacion = await context.AsignacionesTecnico
                .FirstOrDefaultAsync(a => a.AsignacionId == asignacionId && a.Activo);

            if (asignacion == null)
                return ApiResponse<bool>.fail(404, null, "Asignación no encontrada.");

            if (asignacion.Estado != EstadoAsignacion.EnProceso)
                return ApiResponse<bool>.fail(400, null, "Solo se puede pausar un trabajo en proceso.");

            asignacion.Pausar();
            asignacion.UsuarioModificaId = usuarioId;
            asignacion.FechaModificacion = TimeHelper.Now;

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, "Trabajo pausado exitosamente.");
        }

        public async Task<ApiResponse<bool>> ReanudarAsync(int asignacionId, string usuarioId)
        {
            var asignacion = await context.AsignacionesTecnico
                .FirstOrDefaultAsync(a => a.AsignacionId == asignacionId && a.Activo);

            if (asignacion == null)
                return ApiResponse<bool>.fail(404, null, "Asignación no encontrada.");

            if (asignacion.Estado != EstadoAsignacion.Pausado)
                return ApiResponse<bool>.fail(400, null, "Solo se puede reanudar un trabajo pausado.");

            asignacion.Reanudar();
            asignacion.UsuarioModificaId = usuarioId;
            asignacion.FechaModificacion = TimeHelper.Now;

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, "Trabajo reanudado exitosamente.");
        }

        public async Task<ApiResponse<bool>> CompletarAsync(int asignacionId, string usuarioId)
        {
            var asignacion = await context.AsignacionesTecnico
                .Include(a => a.OsServicio)
                .FirstOrDefaultAsync(a => a.AsignacionId == asignacionId && a.Activo);

            if (asignacion == null)
                return ApiResponse<bool>.fail(404, null, "Asignación no encontrada.");

            if (asignacion.Estado != EstadoAsignacion.EnProceso)
                return ApiResponse<bool>.fail(400, null, "Solo se puede completar un trabajo en proceso.");

            asignacion.Completar();
            asignacion.UsuarioModificaId = usuarioId;
            asignacion.FechaModificacion = TimeHelper.Now;

            // Completar el servicio si está vinculado y en proceso
            bool osAutoCompletada = false;
            if (asignacion.OsServicio != null && asignacion.OsServicio.EstaEnProceso)
            {
                asignacion.OsServicio.CompletarTrabajo();

                // Verificar si TODOS los servicios activos de la OS están completados/cancelados
                var todosServicios = await context.OsServicios
                    .Where(s => s.OsId == asignacion.OsServicio.OsId && s.Activo)
                    .ToListAsync();

                var todosFinalizados = todosServicios.All(s =>
                    s.OsServicioId == asignacion.OsServicioId
                    || s.Estado == EstadoServicioOS.Completado
                    || s.Estado == EstadoServicioOS.Cancelado);

                if (todosFinalizados)
                {
                    var estadoCompletada = await context.EstadosOs
                        .FirstOrDefaultAsync(e => e.Codigo == EstadoOs.Estados.Completada && e.Activo);

                    if (estadoCompletada != null)
                    {
                        var os = await context.OrdenesServicio.FindAsync(asignacion.OsServicio.OsId);
                        if (os != null)
                        {
                            os.EstadoId = estadoCompletada.EstadoId;
                            os.UsuarioModificaId = usuarioId;
                            os.FechaModificacion = TimeHelper.Now;
                            osAutoCompletada = true;
                        }
                    }
                }
            }

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, osAutoCompletada
                ? "Trabajo completado. La orden de servicio se completó automáticamente."
                : "Trabajo completado exitosamente.");
        }

        public async Task<ApiResponse<AsignacionTecnicoDTO>> ReasignarAsync(ReasignarDTO dto, string usuarioId)
        {
            var asignacion = await context.AsignacionesTecnico
                .Include(a => a.OrdenServicio)
                .Include(a => a.Tecnico)
                .Include(a => a.OsServicio)
                .FirstOrDefaultAsync(a => a.AsignacionId == dto.AsignacionId && a.Activo);

            if (asignacion == null)
                return ApiResponse<AsignacionTecnicoDTO>.fail(404, null, "Asignación no encontrada.");

            if (!asignacion.EstaActiva)
                return ApiResponse<AsignacionTecnicoDTO>.fail(400, null, "Solo se pueden reasignar trabajos activos.");

            // Validar nuevo técnico
            var nuevoTecnico = await context.Tecnicos
                .FirstOrDefaultAsync(t => t.TecnicoId == dto.NuevoTecnicoId && t.Activo);

            if (nuevoTecnico == null)
                return ApiResponse<AsignacionTecnicoDTO>.fail(404, null, "Nuevo técnico no encontrado.");

            if (nuevoTecnico.TecnicoId == asignacion.TecnicoId)
                return ApiResponse<AsignacionTecnicoDTO>.fail(400, null, "El nuevo técnico es el mismo que el actual.");

            // Actualizar asignación
            var observacionReasignacion = $"Reasignado de {asignacion.Tecnico.Nombre} {asignacion.Tecnico.Apellidos} a {nuevoTecnico.Nombre} {nuevoTecnico.Apellidos}";

            asignacion.TecnicoId = dto.NuevoTecnicoId;
            asignacion.Observaciones = string.IsNullOrEmpty(asignacion.Observaciones)
                ? observacionReasignacion
                : $"{asignacion.Observaciones}\n{observacionReasignacion}";

            if (!string.IsNullOrEmpty(dto.Observaciones))
            {
                asignacion.Observaciones += $"\nMotivo: {dto.Observaciones}";
            }

            asignacion.UsuarioModificaId = usuarioId;
            asignacion.FechaModificacion = TimeHelper.Now;

            // Actualizar TecnicoAsignadoId en el servicio si aplica
            if (asignacion.OsServicio != null)
            {
                asignacion.OsServicio.TecnicoAsignadoId = dto.NuevoTecnicoId;
            }

            await context.SaveChangesAsync();

            // Actualizar referencia para el DTO
            asignacion.Tecnico = nuevoTecnico;

            return ApiResponse<AsignacionTecnicoDTO>.ok(MapToDto(asignacion), "Técnico reasignado exitosamente.");
        }

        public async Task<ApiResponse<bool>> CancelarAsync(int asignacionId, string usuarioId)
        {
            var asignacion = await context.AsignacionesTecnico
                .Include(a => a.OsServicio)
                .FirstOrDefaultAsync(a => a.AsignacionId == asignacionId && a.Activo);

            if (asignacion == null)
                return ApiResponse<bool>.fail(404, null, "Asignación no encontrada.");

            if (asignacion.Estado == EstadoAsignacion.Completado)
                return ApiResponse<bool>.fail(400, null, "No se puede cancelar una asignación completada.");

            // Soft delete
            asignacion.Activo = false;
            asignacion.UsuarioModificaId = usuarioId;
            asignacion.FechaModificacion = TimeHelper.Now;

            // Limpiar TecnicoAsignadoId del servicio si aplica
            if (asignacion.OsServicio != null && asignacion.OsServicio.TecnicoAsignadoId == asignacion.TecnicoId)
            {
                asignacion.OsServicio.TecnicoAsignadoId = null;
            }

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, "Asignación cancelada exitosamente.");
        }

        #region Helper

        private static AsignacionTecnicoDTO MapToDto(AsignacionTecnico a)
        {
            return new AsignacionTecnicoDTO
            {
                AsignacionId = a.AsignacionId,
                OsId = a.OsId,
                TecnicoId = a.TecnicoId,
                OsServicioId = a.OsServicioId,
                FechaAsignacion = a.FechaAsignacion,
                FechaInicio = a.FechaInicio,
                FechaFin = a.FechaFin,
                Estado = a.Estado,
                Observaciones = a.Observaciones,
                NumeroOs = a.OrdenServicio?.NumeroOs ?? "",
                VehiculoId = a.OrdenServicio?.VehiculoId ?? 0,
                VehiculoDescripcion = a.OrdenServicio?.Vehiculo != null
                    ? $"{a.OrdenServicio.Vehiculo.Marca?.Nombre} {a.OrdenServicio.Vehiculo.Modelo?.Nombre} {a.OrdenServicio.Vehiculo.Anio}".Trim()
                    : null,
                VehiculoPlaca = a.OrdenServicio?.Vehiculo?.Placa,
                TecnicoNombre = a.Tecnico != null
                    ? $"{a.Tecnico.Nombre} {a.Tecnico.Apellidos}"
                    : "",
                TecnicoCodigo = a.Tecnico?.Codigo,
                ServicioDescripcion = a.OsServicio?.DescripcionTrabajo,
                EstaActiva = a.EstaActiva,
                TiempoTrabajadoMinutos = a.TiempoTrabajado.HasValue
                    ? (int)a.TiempoTrabajado.Value.TotalMinutes
                    : null
            };
        }

        #endregion
    }
}
//## Resumen de métodos:

//| Método | Descripción |
//| --------| -------------|
//| `GetByIdAsync` | Obtener por ID |
//| `GetByOsIdAsync` | Asignaciones de una OS |
//| `GetByTecnicoIdAsync` | Historial de un técnico |
//| `GetActivasByTecnicoIdAsync` | Trabajos activos del técnico |
//| `AsignarAsync` | Asignar técnico a OS/servicio |
//| `IniciarTrabajoAsync` | Técnico inicia trabajo |
//| `PausarAsync` | Pausar trabajo |
//| `ReanudarAsync` | Reanudar trabajo pausado |
//| `CompletarAsync` | Completar trabajo |
//| `ReasignarAsync` | Cambiar a otro técnico |
//| `CancelarAsync` | Cancelar asignación |

//---

//## Flujo de estados:
//```
//Asignado → EnProceso → Completado
//              ↓ ↑
//           Pausado
//```

//---

//## Flujo de uso:
//```
//1. Coordinador asigna técnico a OS (AsignarAsync)
//   ↓
//2. Técnico ve sus trabajos pendientes (GetActivasByTecnicoIdAsync)
//   ↓
//3. Técnico inicia trabajo (IniciarTrabajoAsync)
//   - FechaInicio = ahora
//   - OsServicio también inicia
//   ↓
//4. Si necesita, pausa (PausarAsync) y reanuda (ReanudarAsync)
//   ↓
//5. Técnico termina (CompletarAsync)
//   - FechaFin = ahora
//   - TiempoTrabajado se calcula
//   - OsServicio también completa