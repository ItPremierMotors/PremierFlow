using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Taller;
using PremierFlow.Application.Interfaces.Taller;
using PremierFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.Taller
{
    public class RecepcionService : IRecepcionService
    {
        private readonly PremierFlowDbContext context;

        public RecepcionService(PremierFlowDbContext context)
        {
            this.context = context;
        }

        public async Task<ApiResponse<RecepcionDTO>> GetByIdAsync(int recepcionId)
        {
            var recepcion = await context.Recepciones
                .Include(r => r.OrdenServicio)
                    .ThenInclude(o => o.Vehiculo)
                        .ThenInclude(v => v.Marca)
                .Include(r => r.OrdenServicio)
                    .ThenInclude(o => o.Vehiculo)
                        .ThenInclude(v => v.Modelo)
                .Include(r => r.OrdenServicio)
                    .ThenInclude(o => o.Cliente)
                .Include(r => r.Evidencias.Where(e => e.Activo))
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RecepcionId == recepcionId && r.Activo);

            if (recepcion == null)
                return ApiResponse<RecepcionDTO>.fail(404, null, "Recepción no encontrada.");

            return ApiResponse<RecepcionDTO>.ok(MapToDto(recepcion), "Recepción obtenida.");
        }

        public async Task<ApiResponse<RecepcionDTO>> GetByOsIdAsync(int osId)
        {
            var recepcion = await context.Recepciones
                .Include(r => r.OrdenServicio)
                    .ThenInclude(o => o.Vehiculo)
                        .ThenInclude(v => v.Marca)
                .Include(r => r.OrdenServicio)
                    .ThenInclude(o => o.Vehiculo)
                        .ThenInclude(v => v.Modelo)
                .Include(r => r.OrdenServicio)
                    .ThenInclude(o => o.Cliente)
                .Include(r => r.Evidencias.Where(e => e.Activo))
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.OsId == osId && r.Activo);

            if (recepcion == null)
                return ApiResponse<RecepcionDTO>.fail(404, null, "Recepción no encontrada para esta orden de servicio.");

            return ApiResponse<RecepcionDTO>.ok(MapToDto(recepcion), "Recepción obtenida.");
        }

        public async Task<ApiResponse<List<RecepcionDTO>>> GetPendientesFirmaAsync(int? sucursalId = null)
        {
            var query = context.Recepciones
                .Include(r => r.OrdenServicio)
                    .ThenInclude(o => o.Vehiculo)
                        .ThenInclude(v => v.Marca)
                .Include(r => r.OrdenServicio)
                    .ThenInclude(o => o.Vehiculo)
                        .ThenInclude(v => v.Modelo)
                .Include(r => r.OrdenServicio)
                    .ThenInclude(o => o.Cliente)
                .Include(r => r.Evidencias.Where(e => e.Activo))
                .AsNoTracking()
                .Where(r => r.Activo &&
                           (r.FirmaClienteBase64 == null || r.FirmaClienteBase64 == ""));

            if (sucursalId.HasValue)
                query = query.Where(r => r.OrdenServicio.SucursalId == sucursalId);

            var recepciones = await query
                .OrderBy(r => r.FechaHoraRecepcion)
                .ToListAsync();

            var dtos = recepciones.Select(MapToDto).ToList();

            return ApiResponse<List<RecepcionDTO>>.ok(dtos, "Recepciones pendientes de firma obtenidas.");
        }

        public async Task<ApiResponse<RecepcionDTO>> CreateAsync(CreateRecepcionDTO dto, string usuarioId)
        {
            // 1. Validar que existe la OS
            var os = await context.OrdenesServicio
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Marca)
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(o => o.Cliente)
                .Include(o => o.Estado)
                .FirstOrDefaultAsync(o => o.OsId == dto.OsId && o.Activo);

            if (os == null)
                return ApiResponse<RecepcionDTO>.fail(404, null, "Orden de servicio no encontrada.");

            if (!os.EstaAbierta)
                return ApiResponse<RecepcionDTO>.fail(400, null, "La orden de servicio no está abierta.");

            // 2. Validar que no exista recepción para esta OS
            var existeRecepcion = await context.Recepciones
                .AnyAsync(r => r.OsId == dto.OsId && r.Activo);

            if (existeRecepcion)
                return ApiResponse<RecepcionDTO>.fail(400, null, "Ya existe una recepción para esta orden de servicio.");

            // 3. Crear recepción
            var recepcion = new Recepcion
            {
                OsId = dto.OsId,
                FechaHoraRecepcion = DateTime.UtcNow,
                RecibidoPorId = usuarioId,
                EntregadoPor = dto.EntregadoPor,
                EstadoCarroceria = dto.EstadoCarroceria,
                AccesoriosRecibidos = dto.AccesoriosRecibidos,
                LlantaRepuesto = dto.LlantaRepuesto,
                Gato = dto.Gato,
                Triangulos = dto.Triangulos,
                Extintor = dto.Extintor,
                Herramientas = dto.Herramientas,
                Radio = dto.Radio,
                Tapetes = dto.Tapetes,
                ObservacionesGenerales = dto.ObservacionesGenerales,
                ChecklistCompletado = false,
                Activo = true,
                UsuarioCreaId = usuarioId,
                FechaCreacion = DateTime.UtcNow
            };

            context.Recepciones.Add(recepcion);
            await context.SaveChangesAsync();

            // 4. Cargar navegación para el DTO
            recepcion.OrdenServicio = os;

            return ApiResponse<RecepcionDTO>.ok(MapToDto(recepcion), "Recepción creada exitosamente.");
        }

        public async Task<ApiResponse<RecepcionDTO>> UpdateAsync(UpdateRecepcionDTO dto, string usuarioId)
        {
            var recepcion = await context.Recepciones
                .Include(r => r.OrdenServicio)
                    .ThenInclude(o => o.Vehiculo)
                        .ThenInclude(v => v.Marca)
                .Include(r => r.OrdenServicio)
                    .ThenInclude(o => o.Vehiculo)
                        .ThenInclude(v => v.Modelo)
                .Include(r => r.OrdenServicio)
                    .ThenInclude(o => o.Cliente)
                .Include(r => r.Evidencias.Where(e => e.Activo))
                .FirstOrDefaultAsync(r => r.RecepcionId == dto.RecepcionId && r.Activo);

            if (recepcion == null)
                return ApiResponse<RecepcionDTO>.fail(404, null, "Recepción no encontrada.");

            if (recepcion.EstaCompleta)
                return ApiResponse<RecepcionDTO>.fail(400, null, "No se puede modificar una recepción completada.");

            recepcion.EntregadoPor = dto.EntregadoPor;
            recepcion.EstadoCarroceria = dto.EstadoCarroceria;
            recepcion.AccesoriosRecibidos = dto.AccesoriosRecibidos;
            recepcion.LlantaRepuesto = dto.LlantaRepuesto;
            recepcion.Gato = dto.Gato;
            recepcion.Triangulos = dto.Triangulos;
            recepcion.Extintor = dto.Extintor;
            recepcion.Herramientas = dto.Herramientas;
            recepcion.Radio = dto.Radio;
            recepcion.Tapetes = dto.Tapetes;
            recepcion.ObservacionesGenerales = dto.ObservacionesGenerales;
            recepcion.UsuarioModificaId = usuarioId;
            recepcion.FechaModificacion = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return ApiResponse<RecepcionDTO>.ok(MapToDto(recepcion), "Recepción actualizada exitosamente.");
        }

        public async Task<ApiResponse<bool>> CompletarChecklistAsync(int recepcionId, string usuarioId)
        {
            var recepcion = await context.Recepciones
                .FirstOrDefaultAsync(r => r.RecepcionId == recepcionId && r.Activo);

            if (recepcion == null)
                return ApiResponse<bool>.fail(404, null, "Recepción no encontrada.");

            if (recepcion.ChecklistCompletado)
                return ApiResponse<bool>.fail(400, null, "El checklist ya está completado.");

            recepcion.CompletarChecklist();
            recepcion.UsuarioModificaId = usuarioId;
            recepcion.FechaModificacion = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, "Checklist completado exitosamente.");
        }

        public async Task<ApiResponse<bool>> RegistrarFirmaAsync(RegistrarFirmaDTO dto, string usuarioId)
        {
            var recepcion = await context.Recepciones
                .FirstOrDefaultAsync(r => r.RecepcionId == dto.RecepcionId && r.Activo);

            if (recepcion == null)
                return ApiResponse<bool>.fail(404, null, "Recepción no encontrada.");

            if (recepcion.TieneFirmaCliente)
                return ApiResponse<bool>.fail(400, null, "La recepción ya tiene firma del cliente.");

            if (string.IsNullOrWhiteSpace(dto.FirmaClienteBase64))
                return ApiResponse<bool>.fail(400, null, "La firma no puede estar vacía.");

            recepcion.RegistrarFirma(dto.FirmaClienteBase64);
            recepcion.UsuarioModificaId = usuarioId;
            recepcion.FechaModificacion = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, "Firma registrada exitosamente.");
        }

        #region Helper

        private static RecepcionDTO MapToDto(Recepcion r)
        {
            return new RecepcionDTO
            {
                RecepcionId = r.RecepcionId,
                OsId = r.OsId,
                FechaHoraRecepcion = r.FechaHoraRecepcion,
                RecibidoPorId = r.RecibidoPorId,
                EntregadoPor = r.EntregadoPor,
                EstadoCarroceria = r.EstadoCarroceria,
                AccesoriosRecibidos = r.AccesoriosRecibidos,
                LlantaRepuesto = r.LlantaRepuesto,
                Gato = r.Gato,
                Triangulos = r.Triangulos,
                Extintor = r.Extintor,
                Herramientas = r.Herramientas,
                Radio = r.Radio,
                Tapetes = r.Tapetes,
                ObservacionesGenerales = r.ObservacionesGenerales,
                FirmaClienteBase64 = r.FirmaClienteBase64,
                ChecklistCompletado = r.ChecklistCompletado,
                NumeroOs = r.OrdenServicio?.NumeroOs ?? "",
                VehiculoDescripcion = r.OrdenServicio?.Vehiculo?.DescripcionCompleta,
                VehiculoPlaca = r.OrdenServicio?.Vehiculo?.Placa,
                ClienteNombre = r.OrdenServicio?.Cliente?.NombreCompleto,
                TieneFirmaCliente = r.TieneFirmaCliente,
                CantidadAccesoriosVerificados = r.CantidadAccesoriosVerificados,
                EstaCompleta = r.EstaCompleta,
                CantidadEvidencias = r.Evidencias?.Count ?? 0
            };
        }

        #endregion
    }


//---

//## Resumen de métodos:

//| Método | Descripción |
//|--------|-------------|
//| `GetByIdAsync` | Obtener por ID |
//| `GetByOsIdAsync` | Obtener recepción de una OS |
//| `GetPendientesFirmaAsync` | Recepciones sin firma del cliente |
//| `CreateAsync` | Crear recepción |
//| `UpdateAsync` | Actualizar checklist |
//| `CompletarChecklistAsync` | Marcar checklist como completado |
//| `RegistrarFirmaAsync` | Registrar firma digital del cliente |

//---

//## Flujo de uso:
//```
//1. Cliente llega con su vehículo
//   ↓
//2. Recepcionista crea Recepción(CreateAsync)
//   - Registra quién entrega
//   - Marca accesorios presentes
//   - Describe estado de carrocería
//   ↓
//3. Toma fotos(Evidencias - siguiente servicio)
//   ↓
//4. Completa checklist(CompletarChecklistAsync)
//   ↓
//5. Cliente firma en tablet(RegistrarFirmaAsync)
//   ↓
//6. Recepción completa(EstaCompleta = true)
}
