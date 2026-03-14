using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Crm;
using PremierFlow.Application.Interfaces.Crm;
using PremierFlow.Domain.Common;
using PremierFlow.Domain.Entities;
using PremierFlow.Infrastructure.Identity;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.Crm
{
    public class NotaCrmService : INotaCrmService
    {
        private readonly PremierFlowDbContext context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager;

        public NotaCrmService(PremierFlowDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<ApiResponse<NotaCrmDTO>> CreateAsync(CreateNotaCrmDTO dto, string usuarioId)
        {
            if (dto.LeadId == null && dto.OportunidadId == null) return ApiResponse<NotaCrmDTO>.fail(400, null, "Debe asociar la nota a un Lead o una Oportunidad.");
            if (string.IsNullOrEmpty(dto.Contenido)) return ApiResponse<NotaCrmDTO>.fail(400, null, "El contenido de la nota es obligatorio.");

            var nota = new Domain.Entities.NotaCrm
            {
                LeadId = dto.LeadId,
                OportunidadId = dto.OportunidadId,
                Contenido = dto.Contenido,
                AutorId = usuarioId,
                EsPrivada = dto.EsPrivada,
                Activo = true,
                UsuarioCreaId = usuarioId,
                FechaCreacion = TimeHelper.Now
            };
            context.NotasCrm.Add(nota);
            if (await context.SaveChangesAsync() > 0)
            {
                var notaDto = await MapToDto(nota);
                return ApiResponse<NotaCrmDTO>.ok(notaDto, "Nota creada exitosamente ");
            }
            else
            {
                return ApiResponse<NotaCrmDTO>.fail(500, null, "No se pudo crear la nota. Intente nuevamente.");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int notaCrmId, string usuarioId)
        {
            try
            {
                 var nota = await context.NotasCrm.FirstOrDefaultAsync(n => n.NotaCrmId == notaCrmId && n.Activo);

                if (nota == null)
                    return ApiResponse<bool>.fail(404, null, "Nota no encontrada.");

                nota.Activo = false;
                nota.UsuarioModificaId = usuarioId;
                nota.FechaModificacion = TimeHelper.Now;

                if (await context.SaveChangesAsync() > 0)
                    return ApiResponse<bool>.ok(true, "Nota eliminada exitosamente.");

                return ApiResponse<bool>.fail(500, null, "No se pudo eliminar la nota. Intente nuevamente.");
            }
            catch (Exception ex)
            {
                // Loggear ex
                return ApiResponse<bool>.fail(500, null, $"Error inesperado al eliminar la nota. {ex.Message}");
            }
        }
        public async Task<ApiResponse<List<NotaCrmDTO>>> GetByLeadAsync(int leadId)
        {
            var notas= await context.NotasCrm
                .Where(n => n.LeadId == leadId && n.Activo)
                .OrderByDescending(n => n.FechaCreacion)
                .AsNoTracking()
                .ToListAsync();
                var dtos = new List<NotaCrmDTO>();
                foreach(var nota in notas)
                {
                    dtos.Add(await MapToDto(nota));
                }
                if(dtos.Count > 0)
                    return ApiResponse<List<NotaCrmDTO>>.ok(dtos, "Notas obtenidas exitosamente");
               return ApiResponse<List<NotaCrmDTO>>.fail(400,null, "no se encontraron notas para este Lead"); 
        }

        public async Task<ApiResponse<List<NotaCrmDTO>>> GetByOportunidadAsync(int oportunidadId)
        {
           var notas= await context.NotasCrm
                .Where(n => n.OportunidadId == oportunidadId && n.Activo)
                .OrderByDescending(n => n.FechaCreacion)
                .AsNoTracking()
                .ToListAsync();
                var dtos = new List<NotaCrmDTO>();
                foreach(var nota in notas)
                {
                    dtos.Add(await MapToDto(nota));
                }
                if(dtos.Count > 0)
                    return ApiResponse<List<NotaCrmDTO>>.ok(dtos, "Notas obtenidas exitosamente");
               return ApiResponse<List<NotaCrmDTO>>.fail(400,null, "no se encontraron notas para esta oportunidad"); 
        }
        #region Helpers

        private async Task<NotaCrmDTO> MapToDto(NotaCrm n)
        {
            var autor = await userManager.FindByIdAsync(n.AutorId);

            return new NotaCrmDTO
            {
                NotaCrmId = n.NotaCrmId,
                Contenido = n.Contenido,
                EsPrivada = n.EsPrivada,
                FechaCreacion = n.FechaCreacion,
                LeadId = n.LeadId,
                OportunidadId = n.OportunidadId,
                AutorId = n.AutorId,
                AutorNombre = autor?.NombreCompleto ?? ""
            };
        }

        #endregion
    }
}