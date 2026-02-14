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
    public class BloqueHorarioService : IBloqueHorarioService
    {
        private readonly PremierFlowDbContext context; 
        public BloqueHorarioService(PremierFlowDbContext context)
        {
                this.context = context;
        }
        public async Task<ApiResponse<bool>> AgendarVehiculoAsync(int bloqueId)
        {
           var bloque= await context.BloquesHorario
                .FirstOrDefaultAsync(b=>b.BloqueId==bloqueId && b.Activo);
            if(bloque==null)
                return ApiResponse<bool>.fail(404, null, "Bloque horario no encontrado.");
            if(!bloque.TieneEspacioDisponible)
                return ApiResponse<bool>.fail(400, null, "No hay espacio disponible en este bloque horario.");
            bloque.AgendarVehiculo();
            await context.SaveChangesAsync();
            return ApiResponse<bool>.ok(true, "Vehículo agendado exitosamente.");
        }

        public async Task<ApiResponse<BloqueHorarioDTO>> CreateAsync(CreateBloqueHorarioDTO dto, string usuarioId)
        {
            // 1. Validar que existe la capacidad
            var capacidad = await context.CapacidadTaller
                .Include(c => c.Sucursal)
                .FirstOrDefaultAsync(c => c.CapacidadId == dto.CapacidadId && c.Activo);

            if (capacidad == null)
                return ApiResponse<BloqueHorarioDTO>.fail(404, null, "Capacidad de taller no encontrada.");

            // 2. Validar horario
            if (dto.HoraInicio >= dto.HoraFin)
                return ApiResponse<BloqueHorarioDTO>.fail(400, null, "La hora de inicio debe ser menor a la hora de fin.");

            // 3. Validar que no se traslape con otros bloques
            var hayTraslape = await context.BloquesHorario
                .AnyAsync(b => b.CapacidadId == dto.CapacidadId &&
                              b.Activo &&
                              ((dto.HoraInicio >= b.HoraInicio && dto.HoraInicio < b.HoraFin) ||
                               (dto.HoraFin > b.HoraInicio && dto.HoraFin <= b.HoraFin) ||
                               (dto.HoraInicio <= b.HoraInicio && dto.HoraFin >= b.HoraFin)));

            if (hayTraslape)
                return ApiResponse<BloqueHorarioDTO>.fail(400, null, "El bloque se traslapa con otro existente.");

            // 4. Crear
            var bloque = new BloqueHorario
            {
                CapacidadId = dto.CapacidadId,
                HoraInicio = dto.HoraInicio,
                HoraFin = dto.HoraFin,
                CapacidadMaximaVehiculos = dto.CapacidadMaximaVehiculos,
                VehiculosAgendados = 0,
                TipoBloque = dto.TipoBloque,
                Activo = true,
                UsuarioCreaId = usuarioId,
                FechaCreacion = DateTime.UtcNow
            };

            context.BloquesHorario.Add(bloque);
            await context.SaveChangesAsync();

            // Cargar navegación
            bloque.Capacidad = capacidad;

            return ApiResponse<BloqueHorarioDTO>.ok(MapToDto(bloque), "Bloque horario creado exitosamente.");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int bloqueId, string usuarioId)
        {
            var bloque = await context.BloquesHorario
            .FirstOrDefaultAsync(b => b.BloqueId == bloqueId && b.Activo);

            if (bloque == null)
                return ApiResponse<bool>.fail(404, null, "Bloque horario no encontrado.");

            // No eliminar si tiene vehículos agendados
            if (bloque.VehiculosAgendados > 0)
                return ApiResponse<bool>.fail(400, null,
                    "No se puede eliminar, hay vehículos agendados en este bloque.");

            bloque.Activo = false;
            bloque.UsuarioModificaId = usuarioId;
            bloque.FechaModificacion = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, "Bloque horario eliminado exitosamente.");
        }

        public async Task<ApiResponse<List<BloqueHorarioDTO>>> GenerarBloquesAutomaticosAsync(GenerarBloquesDTO dto, string usuarioId)
        {
            // 1. Validar que existe la capacidad
            var capacidad = await context.CapacidadTaller
                .Include(c => c.Sucursal)
                .FirstOrDefaultAsync(c => c.CapacidadId == dto.CapacidadId && c.Activo);

            if (capacidad == null)
                return ApiResponse<List<BloqueHorarioDTO>>.fail(404, null, "Capacidad de taller no encontrada.");

            // 2. Validar que no existan bloques para esa capacidad
            var existenBloques = await context.BloquesHorario
                .AnyAsync(b => b.CapacidadId == dto.CapacidadId && b.Activo);

            if (existenBloques)
                return ApiResponse<List<BloqueHorarioDTO>>.fail(400, null,
                    "Ya existen bloques para esta capacidad. Elimínelos primero.");

            // 3. Validar horarios
            if (dto.HoraInicioJornada >= dto.HoraFinJornada)
                return ApiResponse<List<BloqueHorarioDTO>>.fail(400, null,
                    "La hora de inicio debe ser menor a la hora de fin de jornada.");

            // 4. Generar bloques
            var bloquesCreados = new List<BloqueHorario>();
            var horaActual = dto.HoraInicioJornada;

            while (horaActual.Add(TimeSpan.FromMinutes(dto.DuracionBloqueMinutos)) <= dto.HoraFinJornada)
            {
                var bloque = new BloqueHorario
                {
                    CapacidadId = dto.CapacidadId,
                    HoraInicio = horaActual,
                    HoraFin = horaActual.Add(TimeSpan.FromMinutes(dto.DuracionBloqueMinutos)),
                    CapacidadMaximaVehiculos = dto.CapacidadPorBloque,
                    VehiculosAgendados = 0,
                    TipoBloque = dto.TipoBloque,
                    Activo = true,
                    UsuarioCreaId = usuarioId,
                    FechaCreacion = DateTime.UtcNow
                };

                context.BloquesHorario.Add(bloque);
                bloquesCreados.Add(bloque);

                horaActual = bloque.HoraFin;
            }

            await context.SaveChangesAsync();

            // Asignar capacidad para el mapeo
            foreach (var bloque in bloquesCreados)
            {
                bloque.Capacidad = capacidad;
            }

            var dtos = bloquesCreados.Select(MapToDto).ToList();

            return ApiResponse<List<BloqueHorarioDTO>>.ok(dtos, $"{dtos.Count} bloques horarios creados.");
        }
        public async Task<ApiResponse<List<BloqueHorarioDTO>>> GetByCapacidadIdAsync(int capacidadId)
        {
            var bloques = await context.BloquesHorario
                .Include(b => b.Capacidad)
                    .ThenInclude(c => c.Sucursal)
                .AsNoTracking()
                .Where(b => b.CapacidadId == capacidadId && b.Activo)
                .OrderBy(b => b.HoraInicio)
                .ToListAsync();
            var bloquesDto = bloques.Select(MapToDto).ToList();
            return ApiResponse<List<BloqueHorarioDTO>>.ok(bloquesDto, "Bloques horarios obtenidos");
        }

        public async Task<ApiResponse<List<BloqueHorarioDTO>>> GetByFechaAsync(DateTime fecha, int? sucursalId = null)
        {
            var query = context.BloquesHorario
                .Include(b => b.Capacidad)
                    .ThenInclude(c => c.Sucursal)
                .AsNoTracking()
                .Where(b => b.Capacidad.Fecha.Date == fecha.Date && b.Activo);
            if (sucursalId.HasValue)
                query = query.Where(b => b.Capacidad.SucursalId == sucursalId);
            var bloques=await query
                .OrderBy(b => b.HoraInicio)
                .ToListAsync();
            var bloquesDto = bloques.Select(MapToDto).ToList();
            return ApiResponse<List<BloqueHorarioDTO>>.ok(bloquesDto, "Bloques horarios obtenidos");

        }

        public async Task<ApiResponse<BloqueHorarioDTO>> GetByIdAsync(int bloqueId)
        {
            var bloque=await context.BloquesHorario
                .Include(b=>b.Capacidad)
                    .ThenInclude(c=>c.Sucursal)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(b => b.BloqueId == bloqueId);
            if(bloque==null)
                return ApiResponse<BloqueHorarioDTO>.fail(404, message: "Bloque horario no encontrado");
            return ApiResponse<BloqueHorarioDTO>.ok(MapToDto(bloque), "Bloque horario obtenido");

        }

        public async Task<ApiResponse<List<BloqueHorarioDTO>>> GetDisponiblesByFechaAsync(DateTime fecha, int? sucursalId = null)
        {
            var query = context.BloquesHorario
               .Include(b => b.Capacidad)
                   .ThenInclude(c => c.Sucursal)
               .AsNoTracking()
               .Where(b => b.Capacidad.Fecha.Date == fecha.Date &&
                          b.Activo &&
                          b.Capacidad.PermiteAgendamiento &&
                          b.VehiculosAgendados < b.CapacidadMaximaVehiculos);

            if (sucursalId.HasValue)
                query = query.Where(b => b.Capacidad.SucursalId == sucursalId);

            var bloques = await query
                .OrderBy(b => b.HoraInicio)
                .ToListAsync();

            var dtos = bloques.Select(MapToDto).ToList();

            return ApiResponse<List<BloqueHorarioDTO>>.ok(dtos, "Bloques disponibles obtenidos.");
        }

        public async Task<ApiResponse<List<BloqueHorarioDTO>>> GetDisponiblesByFechaYTipoAsync(DateTime fecha, TipoBloqueHorario tipo, int? sucursalId = null)
        {
            var query = context.BloquesHorario
                   .Include(b => b.Capacidad)
                       .ThenInclude(c => c.Sucursal)
                   .AsNoTracking()
                   .Where(b => b.Capacidad.Fecha.Date == fecha.Date &&
                              b.TipoBloque == tipo &&
                              b.Activo &&
                              b.Capacidad.PermiteAgendamiento &&
                              b.VehiculosAgendados < b.CapacidadMaximaVehiculos);

            if (sucursalId.HasValue)
                query = query.Where(b => b.Capacidad.SucursalId == sucursalId);

            var bloques = await query
                .OrderBy(b => b.HoraInicio)
                .ToListAsync();

            var dtos = bloques.Select(MapToDto).ToList();

            return ApiResponse<List<BloqueHorarioDTO>>.ok(dtos, "Bloques disponibles obtenidos.");
        }

        public async Task<ApiResponse<bool>> LiberarEspacioAsync(int bloqueId)
        {
           var bloque= await context.BloquesHorario
                .FirstOrDefaultAsync(b=>b.BloqueId==bloqueId && b.Activo);
            if(bloque==null)
                return ApiResponse<bool>.fail(404, null, "Bloque horario no encontrado.");
            bloque.LiberarEspacio();
            await context.SaveChangesAsync();
            return ApiResponse<bool>.ok(true, "Espacio liberado exitosamente.");
        }

        public async Task<ApiResponse<BloqueHorarioDTO>> UpdateAsync(UpdateBloqueHorarioDTO dto, string usuarioId)
        {
            var bloque = await context.BloquesHorario
                .Include(b => b.Capacidad)
                    .ThenInclude(c => c.Sucursal)
                .FirstOrDefaultAsync(b => b.BloqueId == dto.BloqueId && b.Activo);

            if (bloque == null)
                return ApiResponse<BloqueHorarioDTO>.fail(404, null, "Bloque horario no encontrado.");

            // Validar horario
            if (dto.HoraInicio >= dto.HoraFin)
                return ApiResponse<BloqueHorarioDTO>.fail(400, null, "La hora de inicio debe ser menor a la hora de fin.");

            // No permitir reducir capacidad por debajo de lo agendado
            if (dto.CapacidadMaximaVehiculos < bloque.VehiculosAgendados)
                return ApiResponse<BloqueHorarioDTO>.fail(400, null,
                    $"No puede reducir la capacidad por debajo de lo agendado ({bloque.VehiculosAgendados} vehículos).");

            // Validar traslape (excluyendo el bloque actual)
            var hayTraslape = await context.BloquesHorario
                .AnyAsync(b => b.CapacidadId == bloque.CapacidadId &&
                              b.BloqueId != dto.BloqueId &&
                              b.Activo &&
                              ((dto.HoraInicio >= b.HoraInicio && dto.HoraInicio < b.HoraFin) ||
                               (dto.HoraFin > b.HoraInicio && dto.HoraFin <= b.HoraFin) ||
                               (dto.HoraInicio <= b.HoraInicio && dto.HoraFin >= b.HoraFin)));

            if (hayTraslape)
                return ApiResponse<BloqueHorarioDTO>.fail(400, null, "El bloque se traslapa con otro existente.");

            bloque.HoraInicio = dto.HoraInicio;
            bloque.HoraFin = dto.HoraFin;
            bloque.CapacidadMaximaVehiculos = dto.CapacidadMaximaVehiculos;
            bloque.TipoBloque = dto.TipoBloque;
            bloque.UsuarioModificaId = usuarioId;
            bloque.FechaModificacion = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return ApiResponse<BloqueHorarioDTO>.ok(MapToDto(bloque), "Bloque horario actualizado exitosamente.");
        }
        
        #region Helper

        private static BloqueHorarioDTO MapToDto(BloqueHorario b)
        {
            return new BloqueHorarioDTO
            {
                BloqueId = b.BloqueId,
                CapacidadId = b.CapacidadId,
                HoraInicio = b.HoraInicio,
                HoraFin = b.HoraFin,
                CapacidadMaximaVehiculos = b.CapacidadMaximaVehiculos,
                VehiculosAgendados = b.VehiculosAgendados,
                TipoBloque = b.TipoBloque,
                EspaciosDisponibles = b.EspaciosDisponibles,
                TieneEspacioDisponible = b.TieneEspacioDisponible,
                DuracionMinutos = (int)b.Duracion.TotalMinutes,
                Fecha = b.Capacidad?.Fecha,
                SucursalNombre = b.Capacidad?.Sucursal?.Nombre
            };
        }

        #endregion
    }
}
