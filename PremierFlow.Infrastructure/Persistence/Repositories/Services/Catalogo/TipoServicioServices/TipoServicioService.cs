using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Catalogo;
using PremierFlow.Application.Interfaces;
using PremierFlow.Domain.Entities;
using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.Catalogo.TipoServicioServices
{
    public class TipoServicioService: ITipoServicioService
    {
        private readonly PremierFlowDbContext context;
        public TipoServicioService(PremierFlowDbContext context)
        {
            this.context = context;
        }

        public Task<ApiResponse<TipoServicioDTO>> CreateAsync(CreateTipoServicioDTO dto, string usuarioId)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int tipoServicioId, string usuarioId)
        {
            // 1. Buscar
            var tipo = await context.TiposServicio
                .FirstOrDefaultAsync(t => t.TipoServicioId == tipoServicioId && t.Activo);

            if (tipo == null)
                return ApiResponse<bool>.fail(404, null, "Tipo de servicio no encontrado.");

            // 2. Validar que no tenga citas activas
            var tieneCitas = await context.Citas
                .AnyAsync(c => c.TipoServicioId == tipoServicioId &&
                              (c.Estado == EstadoCita.Agendada || c.Estado == EstadoCita.Confirmada));

            if (tieneCitas)
                return ApiResponse<bool>.fail(400, null, "No se puede eliminar, tiene citas pendientes.");

            // 3. Soft delete
            tipo.Activo = false;
            tipo.UsuarioModificaId = usuarioId;
            tipo.FechaModificacion = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return ApiResponse<bool>.ok(true, "Tipo de servicio eliminado exitosamente.");
        }


        public async Task<ApiResponse<List<TipoServicioDTO>>> GetAllAsync()
        {
            var tipos = await context.TiposServicio
                .AsNoTracking()
                .Where(t => t.Activo)
                .Select(t => new TipoServicioDTO
                {
                    TipoServicioId = t.TipoServicioId,
                    Codigo = t.Codigo,
                    Nombre = t.Nombre,
                    Descripcion = t.Descripcion,
                    DuracionEstimadaMin = t.DuracionEstimadaMin,
                    Clasificacion = t.Clasificacion,
                    PermiteWalkIn = t.PermiteWalkIn,
                    RequiereCita = t.RequiereCita,
                    PrecioBase = t.PrecioBase,
                    StockRequerido = t.StockRequerido,
                    DuracionFormateada = FormatearDuracion(t.DuracionEstimadaMin)
                })
                .ToListAsync();

            return ApiResponse<List<TipoServicioDTO>>.ok(tipos, "Tipos de servicio obtenidos.");
        }

        public async Task<ApiResponse<List<TipoServicioDTO>>> GetByClasificacionAsync(ClasificacionServicio clasificacion)
        {
            var tipos = await context.TiposServicio
                .AsNoTracking()
                .Where(t => t.Clasificacion == clasificacion && t.Activo)
                .Select(t => new TipoServicioDTO
                {
                    TipoServicioId = t.TipoServicioId,
                    Codigo = t.Codigo,
                    Nombre = t.Nombre,
                    Descripcion = t.Descripcion,
                    DuracionEstimadaMin = t.DuracionEstimadaMin,
                    Clasificacion = t.Clasificacion,
                    PermiteWalkIn = t.PermiteWalkIn,
                    RequiereCita = t.RequiereCita,
                    PrecioBase = t.PrecioBase,
                    StockRequerido = t.StockRequerido,
                    DuracionFormateada = FormatearDuracion(t.DuracionEstimadaMin)
                })
                .ToListAsync();

            return ApiResponse<List<TipoServicioDTO>>.ok(tipos, "Tipos de servicio obtenidos.");
        }

        public async Task<ApiResponse<TipoServicioDTO>> GetByIdAsync(int tipoServicioId)
            {
                var tipo = await context.TiposServicio
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.TipoServicioId == tipoServicioId);

                    if (tipo == null)
                    return ApiResponse<TipoServicioDTO>.fail(404, null, "Tipo de servicio no encontrado.");

                return ApiResponse<TipoServicioDTO>.ok(MapToDto(tipo), "Tipo de servicio obtenido.");
         }

        //GetWalkInAsync - ExplicaciónEs para obtener solo los servicios que permiten atención sin cita prev
        public async Task<ApiResponse<List<TipoServicioDTO>>> GetWalkInAsync()
        {
            var tipos = await context.TiposServicio
                .AsNoTracking()
                .Where(t => t.PermiteWalkIn && t.Activo)
                .Select(t => new TipoServicioDTO
                {
                    TipoServicioId = t.TipoServicioId,
                    Codigo = t.Codigo,
                    Nombre = t.Nombre,
                    Descripcion = t.Descripcion,
                    DuracionEstimadaMin = t.DuracionEstimadaMin,
                    Clasificacion = t.Clasificacion,
                    PermiteWalkIn = t.PermiteWalkIn,
                    RequiereCita = t.RequiereCita,
                    PrecioBase = t.PrecioBase,
                    StockRequerido = t.StockRequerido,
                    DuracionFormateada = FormatearDuracion(t.DuracionEstimadaMin)
                })
                .ToListAsync();

            return ApiResponse<List<TipoServicioDTO>>.ok(tipos, "Tipos de servicio walk-in obtenidos.");
        }

        public async Task<ApiResponse<TipoServicioDTO>> UpdateAsync(UpdateTipoServicioDTO dto, string usuarioId)
        {
            // 1. Buscar
            var tipo = await context.TiposServicio
                .FirstOrDefaultAsync(t => t.TipoServicioId == dto.TipoServicioId && t.Activo);

            if (tipo == null)
                return ApiResponse<TipoServicioDTO>.fail(404, null, "Tipo de servicio no encontrado.");

            // 2. Validar código duplicado
            if (await CodigoExiste(dto.Codigo, dto.TipoServicioId))
                return ApiResponse<TipoServicioDTO>.fail(400, null, "Ya existe un tipo de servicio con ese código.");

            // 3. Actualizar
            tipo.Codigo = dto.Codigo;
            tipo.Nombre = dto.Nombre;
            tipo.Descripcion = dto.Descripcion;
            tipo.DuracionEstimadaMin = dto.DuracionEstimadaMin;
            tipo.Clasificacion = dto.Clasificacion;
            tipo.PermiteWalkIn = dto.PermiteWalkIn;
            tipo.RequiereCita = dto.RequiereCita;
            tipo.PrecioBase = dto.PrecioBase;
            tipo.StockRequerido = dto.StockRequerido;
            tipo.UsuarioModificaId = usuarioId;
            tipo.FechaModificacion = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return ApiResponse<TipoServicioDTO>.ok(MapToDto(tipo), "Tipo de servicio actualizado exitosamente.");
        }
        #region Helpers

        private async Task<bool> CodigoExiste(string codigo, int? excludeId = null)
        {
            return await context.TiposServicio
                .AnyAsync(t => t.Codigo == codigo &&
                              (excludeId == null || t.TipoServicioId != excludeId) &&
                              t.Activo);
        }

        private TipoServicioDTO MapToDto(TipoServicio t)
        {
            return new TipoServicioDTO
            {
                TipoServicioId = t.TipoServicioId,
                Codigo = t.Codigo,
                Nombre = t.Nombre,
                Descripcion = t.Descripcion,
                DuracionEstimadaMin = t.DuracionEstimadaMin,
                Clasificacion = t.Clasificacion,
                PermiteWalkIn = t.PermiteWalkIn,
                RequiereCita = t.RequiereCita,
                PrecioBase = t.PrecioBase,
                StockRequerido = t.StockRequerido,
                DuracionFormateada = FormatearDuracion(t.DuracionEstimadaMin)
            };
        }

        private static string FormatearDuracion(int minutos)
        {
            var horas = minutos / 60;
            var mins = minutos % 60;

            if (horas > 0 && mins > 0)
                return $"{horas}h {mins}min";
            if (horas > 0)
                return $"{horas}h";
            return $"{mins}min";
        }

        #endregion
    }
}
