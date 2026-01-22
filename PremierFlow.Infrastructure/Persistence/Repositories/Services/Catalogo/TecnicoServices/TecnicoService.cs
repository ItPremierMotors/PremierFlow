using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Catalogo;
using PremierFlow.Application.Interfaces.Catalogo;
using PremierFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.Catalogo.TecnicoServices
{
    public class TecnicoService : ITecnicoService
    {
        private readonly PremierFlowDbContext context;

        public TecnicoService(PremierFlowDbContext context) {
        this.context = context;
        }

        public async Task<ApiResponse<TecnicoDTO>> CreateAsync(CreateTecnicoDTO dto, string usuarioId)
        {
            //validar codigo duplicado
            if(await CodigoExiste(dto.Codigo))
                return ApiResponse<TecnicoDTO>.fail(400,null,"El código del técnico ya existe.");
            //validar sucursal existe
            if(dto.SucursalId.HasValue)
            {
                var sucursalExists = await context.Sucursales
                    .AnyAsync(s => s.Id == dto.SucursalId.Value && s.Activa);
                if(!sucursalExists)
                    return ApiResponse<TecnicoDTO>.fail(400,null,"La sucursal especificada no existe.");
            }
            var tecnico = new Tecnico
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                Apellidos = dto.Apellidos,
                Especialidad = dto.Especialidad,
                BahiaAsignada = dto.BahiaAsignada,
                UsuarioId = dto.UsuarioId,
                SucursalId = dto.SucursalId,
                Activo = true,
                FechaCreacion = DateTime.UtcNow,
                UsuarioCreaId = usuarioId
            };
            context.Tecnicos.Add(tecnico);
            await context.SaveChangesAsync();
            // Cargar sucursal para el DTO
            if (tecnico.SucursalId.HasValue)
            {
                tecnico.Sucursal = await context.Sucursales
                    .FirstOrDefaultAsync(s => s.Id == tecnico.SucursalId);
            }
            return ApiResponse<TecnicoDTO>.ok(MapToDto(tecnico), "Técnico creado exitosamente.");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int tecnicoId, string usuarioId)
        {
            // 1. buscar
            var tecnico = await context.Tecnicos
                .FirstOrDefaultAsync(t => t.TecnicoId == tecnicoId && t.Activo);
            if (tecnico == null)
                return ApiResponse<bool>.fail(404, null, "Técnico no encontrado.");
            //2. validar que no tenga asignaciones activas
            var tieneAsignacionesActivas = await context.AsignacionesTecnico
                .AnyAsync(a => a.TecnicoId == tecnicoId &&
                               (a.Estado == Domain.Enums.EstadoAsignacion.Asignado ||
                                a.Estado == Domain.Enums.EstadoAsignacion.EnProceso));
            if (tieneAsignacionesActivas)
                return ApiResponse<bool>.fail(400, null, "No se puede eliminar el técnico porque tiene asignaciones activas.");
            //3. eliminar (desactivar)
            tecnico.Activo = false;
            tecnico.UsuarioModificaId = usuarioId;
            tecnico.FechaModificacion = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return ApiResponse<bool>.ok(true, "Técnico eliminado exitosamente.");
        }

        public async Task<ApiResponse<List<TecnicoDTO>>> GetAllAsync()
        {
            var tecnicos = await context.Tecnicos
                .Include(t => t.Sucursal)
                .AsNoTracking()
                .Where(t => t.Activo)
                .Select(t => new TecnicoDTO
                {
                    TecnicoId = t.TecnicoId,
                    Codigo = t.Codigo,
                    Nombre = t.Nombre,
                    Apellidos = t.Apellidos,
                    Especialidad = t.Especialidad,
                    BahiaAsignada = t.BahiaAsignada,
                    UsuarioId = t.UsuarioId,
                    SucursalId = t.SucursalId,
                    NombreCompleto = t.Nombre + " " + t.Apellidos,
                    SucursalNombre = t.Sucursal != null ? t.Sucursal.Nombre : null
                }).ToListAsync();
            return ApiResponse<List<TecnicoDTO>>.ok(tecnicos, "Técnicos obtenidos.");
        }

        public async Task<ApiResponse<TecnicoDTO>> GetByIdAsync(int tecnicoId)
        {
            var tecnico = await context.Tecnicos
                .Include(t=> t.Sucursal)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TecnicoId == tecnicoId && t.Activo);

            if(tecnico ==null)
                return ApiResponse<TecnicoDTO>.fail(404,null,"Técnico no encontrado");
            return ApiResponse<TecnicoDTO>.ok(MapToDto(tecnico));   
        }

        public async Task<ApiResponse<List<TecnicoDTO>>> GetBySucursalAsync(int sucursalId)
        {
            var tecnicos = await context.Tecnicos
                .Include(t => t.Sucursal)
                .AsNoTracking()
                .Where(t => t.SucursalId == sucursalId && t.Activo)
                .Select(t => new TecnicoDTO
                {
                    TecnicoId = t.TecnicoId,
                    Codigo = t.Codigo,
                    Nombre = t.Nombre,
                    Apellidos = t.Apellidos,
                    Especialidad = t.Especialidad,
                    BahiaAsignada = t.BahiaAsignada,
                    UsuarioId = t.UsuarioId,
                    SucursalId = t.SucursalId,
                    NombreCompleto = t.Nombre + " " + t.Apellidos,
                    SucursalNombre = t.Sucursal != null ? t.Sucursal.Nombre : null
                })
            .ToListAsync();

            return ApiResponse<List<TecnicoDTO>>.ok(tecnicos, "Técnicos obtenidos.");
        }

        public async Task<ApiResponse<List<TecnicoDTO>>> GetDisponiblesAsync(int sucursalId)
        {
            //tecnicos con asiganciones activa
            var tecnicosOcupados = await context.AsignacionesTecnico
                .Where(a => a.Estado == Domain.Enums.EstadoAsignacion.Asignado || a.Estado == Domain.Enums.EstadoAsignacion.EnProceso)
                .Select(a => a.TecnicoId)
                .Distinct()
                .ToListAsync();

            var tecnicos = await context.Tecnicos
                .Include(t => t.Sucursal)
                .AsNoTracking()
                .Where(t => t.SucursalId == sucursalId && t.Activo && !tecnicosOcupados.Contains(t.TecnicoId))
                .Select(t => new TecnicoDTO
                {
                    TecnicoId = t.TecnicoId,
                    Codigo = t.Codigo,
                    Nombre = t.Nombre,
                    Apellidos = t.Apellidos,
                    Especialidad = t.Especialidad,
                    BahiaAsignada = t.BahiaAsignada,
                    UsuarioId = t.UsuarioId,
                    SucursalId = t.SucursalId,
                    NombreCompleto = t.Nombre + " " + t.Apellidos,
                    SucursalNombre = t.Sucursal != null ? t.Sucursal.Nombre : null
                }).ToListAsync();
            return ApiResponse<List<TecnicoDTO>>.ok(tecnicos, "Técnicos disponibles obtenidos.");
        }



        public async Task<ApiResponse<TecnicoDTO>> UpdateAsync(UpdateTecnicoDTO dto, string usuarioId)
        {
            //1. buscar
            var tecnico =await context.Tecnicos
                .Include(t => t.Sucursal)
                .FirstOrDefaultAsync(t => t.TecnicoId == dto.TecnicoId && t.Activo);
            if(tecnico == null)
                return ApiResponse<TecnicoDTO>.fail(404,null,"Técnico no encontrado.");
            //2. validar codigo duplicado
            if(await CodigoExiste(dto.Codigo, dto.TecnicoId))
                return ApiResponse<TecnicoDTO>.fail(400,null,"El código del técnico ya existe.");
            //3. validar sucursal existe
            if(dto.SucursalId.HasValue)
            {
                var sucursalExists = await context.Sucursales
                    .AnyAsync(s => s.Id == dto.SucursalId.Value && s.Activa);
                if(!sucursalExists)
                    return ApiResponse<TecnicoDTO>.fail(400,null,"La sucursal especificada no existe.");
            }
            //4. actualizar
            tecnico.Codigo = dto.Codigo;
            tecnico.Nombre = dto.Nombre;
            tecnico.Apellidos = dto.Apellidos;
            tecnico.Especialidad = dto.Especialidad;
            tecnico.BahiaAsignada = dto.BahiaAsignada;
            tecnico.UsuarioId = dto.UsuarioId;
            tecnico.SucursalId = dto.SucursalId;
            tecnico.UsuarioModificaId = usuarioId;
            tecnico.FechaModificacion = DateTime.UtcNow;

            await context.SaveChangesAsync();

            // Recargar sucursal
            if (tecnico.SucursalId.HasValue)
            {
                tecnico.Sucursal = await context.Sucursales
                    .FirstOrDefaultAsync(s => s.Id == tecnico.SucursalId);
            }

            return ApiResponse<TecnicoDTO>.ok(MapToDto(tecnico), "Técnico actualizado exitosamente.");
        }

        #region Helpers

        private async Task<bool> CodigoExiste(string codigo, int? excludeId = null)
        {
            return await context.Tecnicos
                .AnyAsync(t => t.Codigo == codigo &&
                              (excludeId == null || t.TecnicoId != excludeId) &&
                              t.Activo);
        }

        private TecnicoDTO MapToDto(Tecnico t)
        {
            return new TecnicoDTO
            {
                TecnicoId = t.TecnicoId,
                Codigo = t.Codigo,
                Nombre = t.Nombre,
                Apellidos = t.Apellidos,
                Especialidad = t.Especialidad,
                BahiaAsignada = t.BahiaAsignada,
                UsuarioId = t.UsuarioId,
                SucursalId = t.SucursalId,
                NombreCompleto = $"{t.Nombre} {t.Apellidos}",
                SucursalNombre = t.Sucursal?.Nombre
            };
        }

        #endregion
    }
}
