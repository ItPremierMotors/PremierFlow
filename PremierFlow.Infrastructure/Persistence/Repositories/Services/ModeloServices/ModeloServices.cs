using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Catalogo;
using PremierFlow.Application.Interfaces;
using PremierFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.ModeloServices
{
   
    public class ModeloServices : IModeloService
    {
        private readonly PremierFlowDbContext context;
        public ModeloServices(PremierFlowDbContext context) {
            this.context = context;
        }
        public async Task<ApiResponse<ModeloDTO>> CreateAsync(ModeloDTO dto, string usuarioId)
        {
            //validar si el codigo ya existe,
            if (await ModeloCodigoExists(dto.Codigo))
            {
                return ApiResponse<ModeloDTO>.fail(400, null, "El código del modelo ya existe."));
            }
            var modelo = new Modelo
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                Segmento = dto.Segmento,
                AnioInicio = dto.AnioInicio,
                AnioFin = dto.AnioFin,
                Descripcion = dto.Descripcion,
                ImagenUrl = dto.ImagenUrl,
                Activo = true,
                FechaCreacion = DateTime.UtcNow,
                UsuarioCreaId = usuarioId
            };

            //Agregar el nuevo modelo al contexto
            context.Modelos.Add(modelo);
            await context.SaveChangesAsync();
            return ApiResponse<ModeloDTO>.ok(MapToDto(modelo), "Modelo creado exitosamente.");
        }

        public  async Task<ApiResponse<bool>> DeleteAsync(int modeloId, string usuarioId)
        {
            var modelo =await context.Modelos.FirstOrDefaultAsync(m => m.ModeloId == modeloId && m.Activo);
            //inactivar el modelo
            if (modelo == null)
                return ApiResponse<bool>.fail(404, null, "Modelo no encontrado.");
            
            //validar no tenga vehiculos asociados
            var tieneVehiculosAsociados = await context.Vehiculos.AnyAsync(v => v.ModeloId == modeloId && v.Activo);
            if (tieneVehiculosAsociados)
                return ApiResponse<bool>.fail(400, null, "No se puede eliminar el modelo porque tiene vehículos asociados.");
            
            modelo.Activo = false;
            modelo.FechaModificacion = DateTime.UtcNow;
            modelo.UsuarioModificaId = usuarioId;
            await context.SaveChangesAsync();
            return ApiResponse<bool>.ok(true, "Modelo eliminado exitosamente.");
        }

        public async Task<ApiResponse<List<ModeloDTO>>> GetAllAsync()
        {
            var modelos =await  context.Modelos
                .AsNoTracking()
                .Where(m => m.Activo)
                .Select(m => new ModeloDTO
                {
                    ModeloId = m.ModeloId,
                    MarcaId = m.MarcaId,
                    Codigo = m.Codigo,
                    Nombre = m.Nombre,
                    Segmento = m.Segmento,
                    AnioInicio = m.AnioInicio,
                    AnioFin = m.AnioFin,
                    Descripcion = m.Descripcion,
                    ImagenUrl = m.ImagenUrl,
                    MarcaNombre = m.Marca.Nombre,
                    EstaEnProduccion = m.AnioFin == null
                }).ToListAsync();


            return ApiResponse<List<ModeloDTO>>.ok(modelos,
                modelos.Count == 0 ? "No se encontraron modelos." : "Modelos obtenidos exitosamente."
                );
        }

        public async Task<ApiResponse<ModeloDTO>> GetByIdAsync(int modeloId)
        {
            var modelo = await context.Modelos
                .AsNoTracking()
                .Where(m=>m.ModeloId==modeloId && m.Activo).
                Select(m => new ModeloDTO
                {
                    ModeloId = m.ModeloId,
                    MarcaId = m.MarcaId,
                    Codigo = m.Codigo,
                    Nombre = m.Nombre,
                    Segmento = m.Segmento,
                    AnioInicio = m.AnioInicio,
                    AnioFin = m.AnioFin,
                    Descripcion = m.Descripcion,
                    ImagenUrl = m.ImagenUrl,
                    MarcaNombre = m.Marca.Nombre,
                    EstaEnProduccion = m.AnioFin == null
                }).FirstOrDefaultAsync();
            if (modelo == null)
                return ApiResponse<ModeloDTO>.fail(404, null, "Modelo no encontrado.");
            
            return ApiResponse<ModeloDTO>.ok(modelo, "Modelo obtenido exitosamente.");
        }

        public async Task<ApiResponse<List<ModeloDTO>>> GetByMarcaAsync(int marcaId)
        {
            var modelos = await context.Modelos
                .Include(m => m.Marca)
                .Where(m => m.MarcaId == marcaId && m.Activo)
                .Select(m => new ModeloDTO
                {
                    ModeloId = m.ModeloId,
                    MarcaId = m.MarcaId,
                    Codigo = m.Codigo,
                    Nombre = m.Nombre,
                    Segmento = m.Segmento,
                    AnioInicio = m.AnioInicio,
                    AnioFin = m.AnioFin,
                    Descripcion = m.Descripcion,
                    ImagenUrl = m.ImagenUrl,
                    MarcaNombre = m.Marca.Nombre,
                    EstaEnProduccion = m.AnioFin == null
                }).ToListAsync();
            return ApiResponse<List<ModeloDTO>>.ok(modelos);
        }

        public async Task<ApiResponse<ModeloDTO>> UpdateAsync(UpdateModeloDTO dto, string usuarioId)
        {
            //validacion si el modelo existe
            var marca= await context.Modelos.FirstOrDefaultAsync(m => m.ModeloId == dto.ModeloId && m.Activo);
            if(marca==null)
                return ApiResponse<ModeloDTO>.fail(404, null, "Modelo no encontrado.");
            if (await ModeloCodigoExists(dto.Codigo, dto.ModeloId))
                return ApiResponse<ModeloDTO>.fail(400, null, "El código del modelo ya existe.");
            //actualizar los campos
            marca.MarcaId = dto.MarcaId;
            marca.Codigo = dto.Codigo;
            marca.Nombre = dto.Nombre;
            marca.Segmento = dto.Segmento;
            marca.AnioInicio = dto.AnioInicio;
            marca.AnioFin = dto.AnioFin;
            marca.Descripcion = dto.Descripcion;
            marca.ImagenUrl = dto.ImagenUrl;
            marca.FechaModificacion = DateTime.UtcNow;
            marca.UsuarioModificaId = usuarioId;
            await context.SaveChangesAsync();

            return ApiResponse<ModeloDTO>.ok(MapToDto(marca), "Modelo actualizado exitosamente.");

        }
        private ModeloDTO MapToDto(Modelo modelo)
        {
            return new ModeloDTO
            {
                ModeloId = modelo.ModeloId,
                MarcaId = modelo.MarcaId,
                Codigo = modelo.Codigo,
                Nombre = modelo.Nombre,
                Segmento = modelo.Segmento,
                AnioInicio = modelo.AnioInicio,
                AnioFin = modelo.AnioFin,
                Descripcion = modelo.Descripcion,
                ImagenUrl = modelo.ImagenUrl,
                MarcaNombre = modelo.Marca?.Nombre,       // ← Incluir nombre de marca
                EstaEnProduccion = modelo.AnioFin == null // ← Calcular aquí
            };
        }

        private async Task<bool> ModeloCodigoExists(string codigo, int? excludeModeloId=null)
        {
            return await context.Modelos
                .AnyAsync(m=>m.Codigo==codigo
                && (excludeModeloId==null || m.ModeloId!=excludeModeloId)
                && m.Activo);
        }
    }
}
