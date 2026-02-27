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
    public class OsServicioService: IOsServicioService
    {
        private readonly PremierFlowDbContext context;

        public OsServicioService(PremierFlowDbContext context)
        {
            this.context = context;
        }

        public async Task<ApiResponse<OsServicioDTO>> GetByIdAsync(int osServicioId)
        {
            var servicio = await context.OsServicios
                .Include(s => s.OrdenServicio)
                .Include(s => s.TipoServicio)
                .Include(s => s.Tecnico)
                .Include(s => s.Asignaciones.Where(a => a.Activo))
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.OsServicioId == osServicioId && s.Activo);

            if (servicio == null)
                return ApiResponse<OsServicioDTO>.fail(404, null, "Servicio no encontrado.");

            return ApiResponse<OsServicioDTO>.ok(MapToDto(servicio), "Servicio obtenido.");
        }

        public async Task<ApiResponse<List<OsServicioDTO>>> GetByOsIdAsync(int osId)
        {
            var servicios = await context.OsServicios
                .Include(s => s.OrdenServicio)
                .Include(s => s.TipoServicio)
                .Include(s => s.Tecnico)
                .Include(s => s.Asignaciones.Where(a => a.Activo))
                .AsNoTracking()
                .Where(s => s.OsId == osId && s.Activo)
                .OrderBy(s => s.OsServicioId)
                .ToListAsync();

            var dtos = servicios.Select(MapToDto).ToList();

            return ApiResponse<List<OsServicioDTO>>.ok(dtos, "Servicios obtenidos.");
        }

        public async Task<ApiResponse<OsServicioDTO>> AgregarServicioAsync(AgregarServicioDTO dto, string usuarioId)
        {
            // 1. Validar que existe la OS
            var os = await context.OrdenesServicio
                .Include(o => o.Estado)
                .FirstOrDefaultAsync(o => o.OsId == dto.OsId && o.Activo);

            if (os == null)
                return ApiResponse<OsServicioDTO>.fail(404, null, "Orden de servicio no encontrada.");

            if (os.Estado.Codigo != EstadoOs.Estados.Diagnostico)
                return ApiResponse<OsServicioDTO>.fail(400, null, "Solo se pueden agregar servicios cuando la OS está en Diagnóstico.");

            // 2. Validar tipo de servicio
            var tipoServicio = await context.TiposServicio
                .FirstOrDefaultAsync(t => t.TipoServicioId == dto.TipoServicioId && t.Activo);

            if (tipoServicio == null)
                return ApiResponse<OsServicioDTO>.fail(404, null, "Tipo de servicio no encontrado.");

            // 3. Validar técnico si se proporciona
            Tecnico? tecnico = null;
            if (dto.TecnicoAsignadoId.HasValue)
            {
                tecnico = await context.Tecnicos
                    .FirstOrDefaultAsync(t => t.TecnicoId == dto.TecnicoAsignadoId && t.Activo);

                if (tecnico == null)
                    return ApiResponse<OsServicioDTO>.fail(404, null, "Técnico no encontrado.");
            }

            // 4. Determinar precio
            var precioUnitario = dto.PrecioUnitario ?? tipoServicio.PrecioBase;

            // 5. Crear servicio
            var servicio = new OsServicio
            {
                OsId = dto.OsId,
                TipoServicioId = dto.TipoServicioId,
                DescripcionTrabajo = dto.DescripcionTrabajo ?? tipoServicio.Nombre,
                Estado = EstadoServicioOS.Pendiente,
                PrecioUnitario = precioUnitario,
                Cantidad = dto.Cantidad,
                TecnicoAsignadoId = dto.TecnicoAsignadoId,
                Observaciones = dto.Observaciones,
                Activo = true,
                UsuarioCreaId = usuarioId,
                FechaCreacion = DateTime.UtcNow
            };

            servicio.CalcularSubtotal();

            context.OsServicios.Add(servicio);
            await context.SaveChangesAsync();

            // 6. Recalcular totales de la OS
            os.CalcularTotales();
            await context.SaveChangesAsync();

            // 7. Cargar navegaciones para el DTO
            servicio.OrdenServicio = os;
            servicio.TipoServicio = tipoServicio;
            servicio.Tecnico = tecnico;

            return ApiResponse<OsServicioDTO>.ok(MapToDto(servicio), "Servicio agregado exitosamente.");
        }

        public async Task<ApiResponse<OsServicioDTO>> UpdateAsync(UpdateOsServicioDTO dto, string usuarioId)
        {
            var servicio = await context.OsServicios
                .Include(s => s.OrdenServicio)
                    .ThenInclude(o => o.Estado)
                .Include(s => s.OrdenServicio)
                    .ThenInclude(o => o.Servicios.Where(srv => srv.Activo))
                .Include(s => s.TipoServicio)
                .Include(s => s.Tecnico)
                .Include(s => s.Asignaciones.Where(a => a.Activo))
                .FirstOrDefaultAsync(s => s.OsServicioId == dto.OsServicioId && s.Activo);

            if (servicio == null)
                return ApiResponse<OsServicioDTO>.fail(404, null, "Servicio no encontrado.");

            if (!servicio.OrdenServicio.PuedeModificarse)
                return ApiResponse<OsServicioDTO>.fail(400, null, "La orden de servicio no puede modificarse en su estado actual.");

            if (servicio.EstaCompletado || servicio.EstaCancelado)
                return ApiResponse<OsServicioDTO>.fail(400, null, "No se puede modificar un servicio completado o cancelado.");

            // Validar técnico si se proporciona
            if (dto.TecnicoAsignadoId.HasValue)
            {
                var tecnicoExiste = await context.Tecnicos
                    .AnyAsync(t => t.TecnicoId == dto.TecnicoAsignadoId && t.Activo);

                if (!tecnicoExiste)
                    return ApiResponse<OsServicioDTO>.fail(404, null, "Técnico no encontrado.");
            }

            servicio.DescripcionTrabajo = dto.DescripcionTrabajo;
            servicio.PrecioUnitario = dto.PrecioUnitario;
            servicio.Cantidad = dto.Cantidad;
            servicio.TecnicoAsignadoId = dto.TecnicoAsignadoId;
            servicio.Observaciones = dto.Observaciones;
            servicio.UsuarioModificaId = usuarioId;
            servicio.FechaModificacion = DateTime.UtcNow;

            servicio.CalcularSubtotal();

            // Recalcular totales de la OS
            servicio.OrdenServicio.CalcularTotales();

            await context.SaveChangesAsync();

            return ApiResponse<OsServicioDTO>.ok(MapToDto(servicio), "Servicio actualizado exitosamente.");
        }

        public async Task<ApiResponse<bool>> QuitarServicioAsync(int osServicioId, string usuarioId)
        {
            var servicio = await context.OsServicios
                .Include(s => s.OrdenServicio)
                    .ThenInclude(o => o.Estado)
                .Include(s => s.OrdenServicio)
                    .ThenInclude(o => o.Servicios.Where(srv => srv.Activo))
                .FirstOrDefaultAsync(s => s.OsServicioId == osServicioId && s.Activo);

            if (servicio == null)
                return ApiResponse<bool>.fail(404, null, "Servicio no encontrado.");

            if (!servicio.OrdenServicio.PuedeModificarse)
                return ApiResponse<bool>.fail(400, null, "La orden de servicio no puede modificarse en su estado actual.");

            if (servicio.EstaEnProceso || servicio.EstaCompletado)
                return ApiResponse<bool>.fail(400, null, "No se puede quitar un servicio en proceso o completado.");

            // Soft delete
            servicio.Activo = false;
            servicio.UsuarioModificaId = usuarioId;
            servicio.FechaModificacion = DateTime.UtcNow;

            // Recalcular totales de la OS
            servicio.OrdenServicio.CalcularTotales();

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, "Servicio eliminado exitosamente.");
        }

        public async Task<ApiResponse<bool>> IniciarTrabajoAsync(int osServicioId, string usuarioId)
        {
            var servicio = await context.OsServicios
                .Include(s => s.OrdenServicio)
                    .ThenInclude(o => o.Estado)
                .FirstOrDefaultAsync(s => s.OsServicioId == osServicioId && s.Activo);

            if (servicio == null)
                return ApiResponse<bool>.fail(404, null, "Servicio no encontrado.");

            if (!servicio.OrdenServicio.EstaAbierta)
                return ApiResponse<bool>.fail(400, null, "La orden de servicio no está abierta.");

            if (servicio.Estado != EstadoServicioOS.Pendiente)
                return ApiResponse<bool>.fail(400, null, "Solo se puede iniciar un servicio pendiente.");

            if (!servicio.TecnicoAsignadoId.HasValue)
                return ApiResponse<bool>.fail(400, null, "Debe asignar un técnico antes de iniciar el servicio.");

            servicio.IniciarTrabajo();
            servicio.UsuarioModificaId = usuarioId;
            servicio.FechaModificacion = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, "Trabajo iniciado exitosamente.");
        }

        public async Task<ApiResponse<bool>> CompletarTrabajoAsync(int osServicioId, string usuarioId)
        {
            var servicio = await context.OsServicios
                .Include(s => s.OrdenServicio)
                    .ThenInclude(o => o.Estado)
                .FirstOrDefaultAsync(s => s.OsServicioId == osServicioId && s.Activo);

            if (servicio == null)
                return ApiResponse<bool>.fail(404, null, "Servicio no encontrado.");

            if (!servicio.OrdenServicio.EstaAbierta)
                return ApiResponse<bool>.fail(400, null, "La orden de servicio no está abierta.");

            if (servicio.Estado != EstadoServicioOS.EnProceso)
                return ApiResponse<bool>.fail(400, null, "Solo se puede completar un servicio en proceso.");

            servicio.CompletarTrabajo();
            servicio.UsuarioModificaId = usuarioId;
            servicio.FechaModificacion = DateTime.UtcNow;

            // Verificar si TODOS los servicios activos de la OS están completados/cancelados
            var todosServicios = await context.OsServicios
                .Where(s => s.OsId == servicio.OsId && s.Activo)
                .ToListAsync();

            var todosFinalizados = todosServicios.All(s =>
                s.OsServicioId == osServicioId // este que acabamos de completar
                || s.Estado == EstadoServicioOS.Completado
                || s.Estado == EstadoServicioOS.Cancelado);

            if (todosFinalizados)
            {
                var estadoCompletada = await context.EstadosOs
                    .FirstOrDefaultAsync(e => e.Codigo == EstadoOs.Estados.Completada && e.Activo);

                if (estadoCompletada != null)
                {
                    servicio.OrdenServicio.EstadoId = estadoCompletada.EstadoId;
                    servicio.OrdenServicio.UsuarioModificaId = usuarioId;
                    servicio.OrdenServicio.FechaModificacion = DateTime.UtcNow;
                }
            }

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, todosFinalizados
                ? "Trabajo completado. La orden de servicio se completó automáticamente."
                : "Trabajo completado exitosamente.");
        }

        public async Task<ApiResponse<bool>> CancelarServicioAsync(int osServicioId, string usuarioId)
        {
            var servicio = await context.OsServicios
                .Include(s => s.OrdenServicio)
                    .ThenInclude(o => o.Estado)
                .Include(s => s.OrdenServicio)
                    .ThenInclude(o => o.Servicios.Where(srv => srv.Activo))
                .FirstOrDefaultAsync(s => s.OsServicioId == osServicioId && s.Activo);

            if (servicio == null)
                return ApiResponse<bool>.fail(404, null, "Servicio no encontrado.");

            if (!servicio.OrdenServicio.PuedeModificarse)
                return ApiResponse<bool>.fail(400, null, "La orden de servicio no puede modificarse en su estado actual.");

            if (servicio.EstaCompletado)
                return ApiResponse<bool>.fail(400, null, "No se puede cancelar un servicio completado.");

            servicio.Cancelar();
            servicio.UsuarioModificaId = usuarioId;
            servicio.FechaModificacion = DateTime.UtcNow;

            // Recalcular totales de la OS (excluye cancelados)
            servicio.OrdenServicio.CalcularTotales();

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, "Servicio cancelado exitosamente.");
        }

        #region Helper

        private static OsServicioDTO MapToDto(OsServicio s)
        {
            return new OsServicioDTO
            {
                OsServicioId = s.OsServicioId,
                OsId = s.OsId,
                TipoServicioId = s.TipoServicioId,
                DescripcionTrabajo = s.DescripcionTrabajo,
                Estado = s.Estado,
                PrecioUnitario = s.PrecioUnitario,
                Cantidad = s.Cantidad,
                Subtotal = s.Subtotal,
                FechaInicio = s.FechaInicio,
                FechaFin = s.FechaFin,
                TecnicoAsignadoId = s.TecnicoAsignadoId,
                Observaciones = s.Observaciones,
                NumeroOs = s.OrdenServicio?.NumeroOs ?? "",
                TipoServicioNombre = s.TipoServicio?.Nombre ?? "",
                TipoServicioCodigo = s.TipoServicio?.Codigo,
                TecnicoNombre = s.Tecnico != null
                    ? $"{s.Tecnico.Nombre} {s.Tecnico.Apellidos}"
                    : null,
                EstaCompletado = s.EstaCompletado,
                EstaCancelado = s.EstaCancelado,
                EstaEnProceso = s.EstaEnProceso,
                TiempoTrabajoMinutos = s.TiempoTrabajo.HasValue
                    ? (int)s.TiempoTrabajo.Value.TotalMinutes
                    : null,
                CantidadAsignaciones = s.Asignaciones?.Count ?? 0
            };
        }

        #endregion
    }
   }
//```

//---

//## Resumen de métodos:

//| Método | Descripción |
//|--------|-------------|
//| `GetByIdAsync` | Obtener servicio por ID |
//| `GetByOsIdAsync` | Listar servicios de una OS |
//| `AgregarServicioAsync` | Agregar servicio a la OS |
//| `UpdateAsync` | Actualizar servicio |
//| `QuitarServicioAsync` | Eliminar servicio(soft delete) |
//| `IniciarTrabajoAsync` | Iniciar trabajo(Pendiente → EnProceso) |
//| `CompletarTrabajoAsync` | Completar trabajo(EnProceso → Completado) |
//| `CancelarServicioAsync` | Cancelar servicio |

//---

//## Flujo de estados del servicio:
//```
//Pendiente → EnProceso → Completado
//    ↓          ↓
//    └──────────┴──→ Cancelado
//```

//---

//## Flujo de uso:
//```
//1. Se crea OS
//   ↓
//2. Se agregan servicios(AgregarServicioAsync)
//   - Cambio de aceite: $50
//   - Alineación: $30
//   ↓
//3. Técnico inicia trabajo(IniciarTrabajoAsync)
//   - FechaInicio = ahora
//   ↓
//4. Técnico termina(CompletarTrabajoAsync)
//   - FechaFin = ahora
//   - TiempoTrabajo se calcula
//   ↓
//5. Al cerrar OS, se suman subtotales
