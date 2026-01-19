using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Marcas;
using PremierFlow.Application.Interfaces;
using PremierFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.MarcaServices
{
    public class MarcaService : IMarcaService
    {
        private readonly PremierFlowDbContext context;
        public MarcaService(PremierFlowDbContext context)
        {
            this.context = context;
        }

        public async Task<ApiResponse<MarcaDTO>> CreateAsync(MarcaDTO marcaDto, string UsuarioID)
        {
            //.1 Validar que no exista una marca con el mismo codigo activa
            if(await MarcaCodigoExists(marcaDto.Codigo))
            {
                return ApiResponse<MarcaDTO>.fail(400, null, "El código de la marca ya existe.");
            }
            var nuevaMarca = new Marca
            {
                Codigo = marcaDto.Codigo,
                Nombre = marcaDto.Nombre,
                PaisOrigen = marcaDto.PaisOrigen,
                EsMarcaPropia = marcaDto.EsMarcaPropia,
                LogoUrl = marcaDto.LogoUrl,
                Observaciones = marcaDto.Observaciones,
                Activo = true,
                FechaCreacion = DateTime.UtcNow,
                UsuarioCreaId = UsuarioID // TODO: Reemplazar con el usuario actual
            };

            //3. Guardar en la base de datos
            context.Marcas.Add(nuevaMarca);
            await context.SaveChangesAsync();
            return ApiResponse<MarcaDTO>.ok(
                MapToDto(nuevaMarca),
                "Marca creada exitosamente."
            );
        }

        public async Task<ApiResponse<List<MarcaDTO>>> GetAllAsync()
        {
            // devolveremos todas las marcas activas
            var marcas = await context.Marcas.Where(m => m.Activo)
                .Select(m => new MarcaDTO
                {
                    MarcaId = m.MarcaId,
                    Codigo = m.Codigo,
                    Nombre = m.Nombre,
                    PaisOrigen = m.PaisOrigen,
                    EsMarcaPropia = m.EsMarcaPropia,
                    LogoUrl = m.LogoUrl,
                    Observaciones = m.Observaciones
                })
                .ToListAsync();
    
            return ApiResponse<List<MarcaDTO>>.ok(
                marcas,
                marcas.Count == 0 ? "No se encontraron marcas activas." : "Marcas Obtendidas exitosamente.");
        }

        public async Task<ApiResponse<MarcaDTO>> GetByIdAsync(int marcaId)
        {
            //devolveremos una marca por su id si esta activa
            var marca = await context.Marcas
                .Where(m => m.MarcaId == marcaId && m.Activo)
                .Select(m => new MarcaDTO
                {
                    MarcaId = m.MarcaId,
                    Codigo = m.Codigo,
                    Nombre = m.Nombre,
                    PaisOrigen = m.PaisOrigen,
                    EsMarcaPropia = m.EsMarcaPropia,
                    LogoUrl = m.LogoUrl,
                    Observaciones = m.Observaciones
                })
                .FirstOrDefaultAsync();
            if (marca == null)
                return ApiResponse<MarcaDTO>.fail(404, null, "Marca no encontrada.");

            return ApiResponse<MarcaDTO>.ok(marca, "Marca obtenida exitosamente.");
        }
        public async Task<ApiResponse<MarcaDTO>> UpdateAsync(UpdateMarcaDTO marcaDto, string UsuarioID)
        {

            //VALIDACIONES
            var marca = await context.Marcas
                .FirstOrDefaultAsync(m => m.MarcaId == marcaDto.MarcaId && m.Activo);
            if (marca == null)
                return ApiResponse<MarcaDTO>.fail(404, null, "Marca no encontrada.");
            if (await MarcaCodigoExists(marcaDto.Codigo, marcaDto.MarcaId))
                return ApiResponse<MarcaDTO>.fail(400, null, "El código de la marca ya existe.");


            //3. Actualizar los campos de la marca
            marca.Codigo = marcaDto.Codigo;
            marca.Nombre = marcaDto.Nombre;
            marca.PaisOrigen = marcaDto.PaisOrigen;
            marca.EsMarcaPropia = marcaDto.EsMarcaPropia;
            marca.LogoUrl = marcaDto.LogoUrl;
            marca.Observaciones = marcaDto.Observaciones;

            marca.FechaModificacion = DateTime.UtcNow;
            marca.UsuarioModificaId = UsuarioID; // TODO: Reemplazar con el usuario actual

            //4. Guardar los cambios en la base de datos
            await context.SaveChangesAsync();
            return ApiResponse<MarcaDTO>.ok(
                MapToDto(marca),
                "Marca actualizada exitosamente."
            );
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int marcaId, string UsuarioID)
        {
            var marca = await context.Marcas
                .FirstOrDefaultAsync(m => m.MarcaId == marcaId && m.Activo);
            if (marca == null)
                return ApiResponse<bool>.fail(404, null, "Marca no encontrada.");
            //validar modelos asociados 
            var tieneModelosAsociados = await context.Modelos
                .AnyAsync(md => md.MarcaId == marcaId && md.Activo);
            if (tieneModelosAsociados)
                return ApiResponse<bool>.fail(400, null, "No se puede eliminar la marca porque tiene modelos asociados.");
            marca.Activo = false;
            marca.FechaModificacion = DateTime.UtcNow;
            marca.UsuarioModificaId = UsuarioID; // TODO: Reemplazar con el usuario actual
            await context.SaveChangesAsync();
            return ApiResponse<bool>.ok(true, "Marca eliminada exitosamente.");

        }

        //helper para validar duplicados
        private async Task<bool> MarcaCodigoExists(string codigo, int? excludeMarcaId = null)
        {
            return await context.Marcas
                .AnyAsync(m => m.Codigo == codigo
                      && (excludeMarcaId == null || m.MarcaId != excludeMarcaId)
                      && m.Activo);
        }
        //helper para pasar de entidad a dto
        private MarcaDTO MapToDto(Marca marca)
        {
            return new MarcaDTO
            {
                MarcaId = marca.MarcaId,
                Codigo = marca.Codigo,
                Nombre = marca.Nombre,
                PaisOrigen = marca.PaisOrigen,
                EsMarcaPropia = marca.EsMarcaPropia,
                LogoUrl = marca.LogoUrl,
                Observaciones = marca.Observaciones
            };
        }
    }
}
