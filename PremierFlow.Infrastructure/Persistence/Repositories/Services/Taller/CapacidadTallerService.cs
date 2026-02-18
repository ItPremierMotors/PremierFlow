using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Taller;
using PremierFlow.Application.Interfaces.Taller;
using PremierFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.Taller
{
    public class CapacidadTallerService : ICapacidadTallerService
    {
        private readonly PremierFlowDbContext context;
        public CapacidadTallerService(PremierFlowDbContext context)
        {
            this.context = context;
        }
        public async Task<ApiResponse<bool>> BloquearDiaAsync(int capacidadId, string motivo, string usuarioId)
        {
            var capacidad = await context.CapacidadTaller
                .FirstOrDefaultAsync(c => c.CapacidadId == capacidadId && c.Activo);
            //verificar que no sea null
            if(capacidad==null)
                return ApiResponse<bool>.fail(404, message: "Capacidad de taller no encontrada.");
            //verificar que no tenga una cita agendada
            if(capacidad.MinutosReservados>0)
                return ApiResponse<bool>.fail(400, message: "No se puede bloquear, hay citas agendadas. Cancele las citas primero.");
            capacidad.PermiteAgendamiento = false;
            capacidad.Observaciones = motivo;
            capacidad.UsuarioModificaId = usuarioId;
            capacidad.FechaModificacion = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return ApiResponse<bool>.ok(true, "Día bloqueado exitosamente.");
        }

        public async Task<ApiResponse<CapacidadTallerDTO>> CreateAsync(CreateCapacidadTallerDTO dto, string usuarioId)
        {
            //0. No permitir crear capacidad para fechas pasadas
            if (dto.Fecha.Date < DateTime.UtcNow.Date)
                return ApiResponse<CapacidadTallerDTO>.fail(400, message: "No se puede crear capacidad para fechas anteriores a hoy.");

            //1. Validar si ya existe una capacidad para la misma fecha, turno y sucursal
            var existe=await context.CapacidadTaller
                .AnyAsync(CapacidadTaller=> CapacidadTaller.Fecha.Date == dto.Fecha.Date 
                    && CapacidadTaller.Turno == dto.Turno 
                    && CapacidadTaller.Activo
                    && CapacidadTaller.SucursalId == dto.SucursalId);
            if (existe)
                return ApiResponse<CapacidadTallerDTO>.fail(400, message: "Ya existe una capacidad de taller para la fecha, turno y sucursal indicada.");
            //2. validar sucurisal si se proporciona
            if (dto.SucursalId.HasValue)
            {
                var sucursalExiste = await context.Sucursales
                    .AnyAsync(s => s.Id == dto.SucursalId && s.Activa);

                if (!sucursalExiste)
                    return ApiResponse<CapacidadTallerDTO>.fail(400, null, "La sucursal no existe o está inactiva.");
            }
            //3. Crear la capacidad
            var capacidad = new CapacidadTaller
            {
                Fecha = dto.Fecha.Date,
                Turno = dto.Turno,
                TecnicosDisponibles = dto.TecnicosDisponibles,
                BahiasDisponibles = dto.BahiasDisponibles,
                MinutosDisponibles = dto.MinutosDisponibles,
                MinutosReservados = 0,
                MinutosUtilizados = 0,
                MinutosSobretiempo = 0,
                PermiteAgendamiento = dto.PermiteAgendamiento,
                PermiteSobretiempo = dto.PermiteSobretiempo,
                Observaciones = dto.Observaciones,
                SucursalId = dto.SucursalId,
                Activo = true,
                UsuarioCreaId = usuarioId,
                FechaCreacion = DateTime.UtcNow
            };
            context.CapacidadTaller.Add(capacidad);
            await context.SaveChangesAsync();
            //cargar sucursales para dto
            if(capacidad.SucursalId.HasValue)
            {
                await context.Entry(capacidad).Reference(v => v.Sucursal).LoadAsync();
            }
            return ApiResponse<CapacidadTallerDTO>.ok(MapToDto(capacidad), "Capacidad de taller creada exitosamente.");
        }

        public async Task<ApiResponse<bool>> DesbloquearDiaAsync(int capacidadId, string usuarioId)
        {
           var capacidad = context.CapacidadTaller
                .FirstOrDefault(c => c.CapacidadId == capacidadId && c.Activo);
            //verificar que no sea null
            if (capacidad == null)
                return ApiResponse<bool>.fail(404, message: "Capacidad de taller no encontrada.");
            capacidad.PermiteAgendamiento = true;
            capacidad.UsuarioModificaId = usuarioId;
            capacidad.FechaModificacion = DateTime.UtcNow;
            context.SaveChanges();
            return ApiResponse<bool>.ok(true, "Día desbloqueado exitosamente.");   

        }

        public async Task<ApiResponse<List<CapacidadTallerDTO>>> GenerarCapacidadSemanalAsync(DateTime fechaInicio, CreateCapacidadTallerDTO plantilla, string usuarioId)
        {
            //No permitir generar semana para fechas pasadas
            if (fechaInicio.Date < DateTime.UtcNow.Date)
                return ApiResponse<List<CapacidadTallerDTO>>.fail(400, message: "No se puede generar capacidad para fechas anteriores a hoy.");

            var capacidades = new List<CapacidadTaller>();
            for (int i = 0; i < 7; i++)
            {
                var fecha = fechaInicio.Date.AddDays(i);
                //verificar si ya existe una capacidad para la fecha, turno y sucursal
                var existe = context.CapacidadTaller
                    .Any(c => c.Fecha.Date == fecha && c.Turno == plantilla.Turno 
                        && c.Activo
                        && c.SucursalId == plantilla.SucursalId);
                if (existe)
                    continue; //si ya existe, saltar a la siguiente fecha
                // Saltar domingo(opcional)
               if (fecha.DayOfWeek == DayOfWeek.Sunday)
                    continue;
                var capacidad = new CapacidadTaller
                {
                    Fecha = fecha,
                    Turno = plantilla.Turno,
                    TecnicosDisponibles = plantilla.TecnicosDisponibles,
                    BahiasDisponibles = plantilla.BahiasDisponibles,
                    MinutosDisponibles = plantilla.MinutosDisponibles,
                    MinutosReservados = 0,
                    MinutosUtilizados = 0,
                    MinutosSobretiempo = 0,
                    PermiteAgendamiento = plantilla.PermiteAgendamiento,
                    PermiteSobretiempo = plantilla.PermiteSobretiempo,
                    Observaciones = plantilla.Observaciones,
                    SucursalId = plantilla.SucursalId,
                    Activo = true,
                    UsuarioCreaId = usuarioId,
                    FechaCreacion = DateTime.UtcNow
                };
                context.CapacidadTaller.Add(capacidad);//agregar a la base de datos
                capacidades.Add(capacidad);//agregar a la lista
            }
            await context.SaveChangesAsync();
            var dtos = capacidades.Select(MapToDto).ToList();
            return ApiResponse<List<CapacidadTallerDTO>>.ok(dtos, $"{dtos.Count} días de capacidad creados.");
        }

        public async Task<ApiResponse<CapacidadTallerDTO>> GetByFechaAsync(DateTime fecha, int? sucursalId = null)
        {
            //Buscamos la capacidad del taller para la fecha y sucursal indicada
            var query = context.CapacidadTaller
                .Include(c => c.Sucursal)
                .AsNoTracking()
                .Where(c => c.Fecha.Date == fecha.Date && c.Activo);

            if (sucursalId.HasValue)
                query = query.Where(c => c.SucursalId == sucursalId.Value);

            var capacidades = await query.FirstOrDefaultAsync();//Asumimos que solo hay un turno por dia por sucursal
            if (capacidades == null)
                return ApiResponse<CapacidadTallerDTO>.fail(404, message: "Capacidad de taller no encontrada para la fecha indicada.");
            return ApiResponse<CapacidadTallerDTO>.ok(MapToDto(capacidades), "Capacidad obtenida.");
        }

        public async Task<ApiResponse<CapacidadTallerDTO>> GetByIdAsync(int capacidadId)
        {
            var capacidad = await context.CapacidadTaller
                .Include(c=>c.Sucursal)
                .AsNoTracking()
                .FirstOrDefaultAsync(c=>c.CapacidadId==capacidadId);
            if (capacidad == null)
                return ApiResponse<CapacidadTallerDTO>.fail(404, message: "Capacidad de taller no encontrada");
            return ApiResponse<CapacidadTallerDTO>.ok(MapToDto(capacidad), "Capacidad obtenida.");
        }

        public async Task<ApiResponse<List<CapacidadTallerDTO>>> GetByRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin, int? sucursalId = null)
        {
           var query=context.CapacidadTaller
                .Include(c=>c.Sucursal)
                .AsNoTracking()
                .Where(c=>c.Fecha.Date>=fechaInicio.Date && c.Fecha.Date<=fechaFin.Date && c.Activo);
            if(sucursalId.HasValue)
                query=query.Where(c=>c.SucursalId==sucursalId.Value);
            
            var capacidades=await query.OrderBy(c=>c.Fecha).ToListAsync();
            var dtos = capacidades.Select(MapToDto).ToList();
            return ApiResponse<List<CapacidadTallerDTO>>.ok(dtos, "Capacidades obtenidas.");
        }

        public async Task<ApiResponse<List<CapacidadTallerDTO>>> GetBySucursalAsync(int sucursalId)
        {
            var capacidad =await context.CapacidadTaller
                .Include(c => c.Sucursal)
                .AsNoTracking()
                .Where(c => c.SucursalId == sucursalId && c.Activo)
                .OrderBy(c => c.Fecha)
                .ToListAsync();
            var dtos = capacidad.Select(MapToDto).ToList();
            return ApiResponse<List<CapacidadTallerDTO>>.ok(dtos, "Capacidades obtenidas.");
        }

        public async Task<ApiResponse<bool>> LiberarMinutosAsync(int capacidadId, int minutos)
        {
            var capacidad = await context.CapacidadTaller
                .FirstOrDefaultAsync(c => c.CapacidadId == capacidadId && c.Activo);
             if(capacidad == null)
                return ApiResponse<bool>.fail(404, null, "Capacidad no encontrada.");
             capacidad.LiberarMinutos(minutos);
            await context.SaveChangesAsync();
            return ApiResponse<bool>.ok(true, $"{minutos} minutos liberados.");

        }

        public async Task<ApiResponse<bool>> RegistrarTiempoTrabajadoAsync(int capacidadId, int minutos)
        {
            var capacidad = context.CapacidadTaller
                .FirstOrDefault(c => c.CapacidadId == capacidadId && c.Activo);
            if(capacidad == null)
            return ApiResponse<bool>.fail(404, null, "Capacidad no encontrada.");

            capacidad.RegistrarTiempoTrabajado(minutos);
            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, $"{minutos} minutos registrados.");
        }

        public async Task<ApiResponse<bool>> ReservarMinutosAsync(int capacidadId, int minutos)
        {
            var capacidad = await context.CapacidadTaller
             .FirstOrDefaultAsync(c => c.CapacidadId == capacidadId && c.Activo);

            if (capacidad == null)
                return ApiResponse<bool>.fail(404, null, "Capacidad no encontrada.");

            if (!capacidad.TieneCapacidadPara(minutos))
                return ApiResponse<bool>.fail(400, null, "No hay capacidad disponible.");

            capacidad.ReservarMinutos(minutos);
            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, $"{minutos} minutos reservados.");
        }

        public async Task<ApiResponse<bool>> TieneCapacidadAsync(DateTime fecha, int minutosRequeridos, int? sucursalId = null)
        {
           var query = context.CapacidadTaller
                .AsNoTracking()
                .Where(c => c.Fecha.Date == fecha.Date && c.Activo);

            if (sucursalId.HasValue)
                query = query.Where(c => c.SucursalId == sucursalId.Value);
            var capacidad = await query.FirstOrDefaultAsync();
            if (capacidad == null)
                return ApiResponse<bool>.fail(404, message: "Capacidad de taller no encontrada para la fecha indicada.");
            var tieneCapacidad = capacidad.TieneCapacidadPara(minutosRequeridos);

            return ApiResponse<bool>.ok(tieneCapacidad,
           tieneCapacidad ? "Hay capacidad disponible." : "No hay capacidad disponible.");
        }

        public async Task<ApiResponse<CapacidadTallerDTO>> UpdateAsync(UpdateCapacidadTallerDTO dto, string usuarioId)
        {
            var capacidad= await context.CapacidadTaller
                .FirstOrDefaultAsync(c=>c.CapacidadId==dto.CapacidadId && c.Activo);
            if(capacidad==null)
                return ApiResponse<CapacidadTallerDTO>.fail(404, message: "Capacidad de taller no encontrada.");

            //validar que no se reduza la capacidad por debajo de lo ya reservado o utilizado
            if (dto.MinutosDisponibles < capacidad.MinutosReservados)
                return ApiResponse<CapacidadTallerDTO>.fail(400, null,
                    $"No puede reducir la capacidad por debajo de lo reservado ({capacidad.MinutosReservados} min).");
            
            capacidad.Turno = dto.Turno;
            capacidad.TecnicosDisponibles = dto.TecnicosDisponibles;
            capacidad.BahiasDisponibles = dto.BahiasDisponibles;
            capacidad.MinutosDisponibles = dto.MinutosDisponibles;
            capacidad.PermiteAgendamiento = dto.PermiteAgendamiento;
            capacidad.PermiteSobretiempo = dto.PermiteSobretiempo;
            capacidad.Observaciones = dto.Observaciones;
            capacidad.UsuarioModificaId = usuarioId;
            capacidad.FechaModificacion = DateTime.UtcNow;
            await context.SaveChangesAsync();

            return ApiResponse<CapacidadTallerDTO>.ok(MapToDto(capacidad), "Capacidad actualizada exitosamente.");
        }
        #region Helper

        private static CapacidadTallerDTO MapToDto(CapacidadTaller c)
        {
            return new CapacidadTallerDTO
            {
                CapacidadId = c.CapacidadId,
                Fecha = c.Fecha,
                Turno = c.Turno,
                TecnicosDisponibles = c.TecnicosDisponibles,
                BahiasDisponibles = c.BahiasDisponibles,
                MinutosDisponibles = c.MinutosDisponibles,
                MinutosReservados = c.MinutosReservados,
                MinutosUtilizados = c.MinutosUtilizados,
                MinutosSobretiempo = c.MinutosSobretiempo,
                PermiteSobretiempo = c.PermiteSobretiempo,
                PermiteAgendamiento = c.PermiteAgendamiento,
                Observaciones = c.Observaciones,
                SucursalId = c.SucursalId,
                SucursalNombre = c.Sucursal?.Nombre,
                MinutosLibres = c.MinutosLibres,
                PorcentajeOcupacion = c.PorcentajeOcupacion,
                PorcentajeEficiencia = c.PorcentajeEficiencia,
                TuvoSobretiempo = c.TuvoSobretiempo
            };
        }

        #endregion
    }
}
