using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Crm;
using PremierFlow.Application.Interfaces.Crm;
using PremierFlow.Domain.Common;
using PremierFlow.Domain.Entities;
using PremierFlow.Domain.Enums;
using PremierFlow.Infrastructure.Identity;


namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.Crm
{
     public class ActividadCrmService : IActividadCrmService
     {

          private readonly PremierFlowDbContext context;
          private readonly UserManager<ApplicationUser> userManager;
          public ActividadCrmService(PremierFlowDbContext context, UserManager<ApplicationUser> userManager)
          {
               this.context = context;
               this.userManager = userManager;
          }
          public async Task<ApiResponse<bool>> CancelarAsync(int actividadCrmId, string usuarioId)
          {
              var actividad = await context.ActividadesCrm.FirstOrDefaultAsync(a => a.ActividadCrmId == actividadCrmId && a.Activo);
              if(actividad == null) return ApiResponse<bool>.fail(404, null, "Actividad no encontrada");
              try
              {
               actividad.CancelarActividad();
              }
              catch (System.InvalidOperationException ex)
              {
                   return ApiResponse<bool>.fail(400, null, ex.Message);
              }
              actividad.UsuarioModificaId = usuarioId;
              actividad.FechaModificacion = TimeHelper.Now;
               await context.SaveChangesAsync();
            return ApiResponse<bool>.ok(true, "Actividad cancelada"); 
          }

          public async Task<ApiResponse<bool>> CompletarAsync(CompletarActividadDTO dto, string usuarioId)
          {
              var actividad = await context.ActividadesCrm.FirstOrDefaultAsync(a => a.ActividadCrmId == dto.ActividadCrmId && a.Activo);
              if(actividad == null) return ApiResponse<bool>.fail(404, null, "Actividad no encontrada");
              try
              {
               actividad.Completar(dto.Resultado, dto.ProximoContacto);
              }
              catch (System.InvalidOperationException ex)
              {
                   return ApiResponse<bool>.fail(400, null, ex.Message);
              }
              actividad.DuracionMinutos= dto.DuracionMinutos;
              actividad.UsuarioModificaId = usuarioId;
              actividad.FechaModificacion = TimeHelper.Now;

              //actualizar la fecha de ultima oportunidad si aplica
              if(actividad.OportunidadId.HasValue)
              {
                   var op = await context.Oportunidades.FirstOrDefaultAsync(o => o.OportunidadId == actividad.OportunidadId && o.Activo);
                   op?.RegistrarActividad();
              }
               await context.SaveChangesAsync();
            return ApiResponse<bool>.ok(true, "Actividad completada");
              
          }

          public async Task<ApiResponse<ActividadCrmDTO>> CreateAsync(CreateActividadCrmDTO dto, string usuarioId)
          {
               if (dto.LeadId == null && dto.OportunidadId == null) return ApiResponse<ActividadCrmDTO>.fail(400, null, "Debe asociar la actividad a un Lead o una Oportunidad.");
               var actividad = new ActividadCrm
               {
                    LeadId = dto.LeadId,
                    OportunidadId = dto.OportunidadId,
                    RealizadaPorId = usuarioId,
                    Tipo = dto.Tipo,
                    Direccion = dto.Direccion,
                    Asunto = dto.Asunto,
                    Descripcion = dto.Descripcion,
                    FechaProgramada = dto.FechaProgramada,
                    Estado = EstadoActividad.Pendiente,
                    Activo = true,
                    UsuarioCreaId = usuarioId,
                    FechaCreacion = TimeHelper.Now
               };
               context.ActividadesCrm.Add(actividad);
               if (dto.OportunidadId.HasValue)
               {
                    var op = await context.Oportunidades.FirstOrDefaultAsync(o => o.OportunidadId == dto.OportunidadId && o.Activo);
                    op?.RegistrarActividad();
               }
               await context.SaveChangesAsync();
               var created = await GetActividadQuery().FirstOrDefaultAsync(a => a.ActividadCrmId == actividad.ActividadCrmId);
               return ApiResponse<ActividadCrmDTO>.ok(await MapToDto(created!), "Actividad creada exitosamente");
          }

          public async Task<ApiResponse<List<ActividadCrmDTO>>> GetByLeadAsync(int leadId)
          {
               var actividades = await GetActividadQuery().Where(a => a.LeadId == leadId && a.Activo).OrderByDescending(a => a.FechaCreacion).ToListAsync();
               var dtos = new List<ActividadCrmDTO>();
               foreach (var a in actividades)
               {
                    dtos.Add(await MapToDto(a));
               }
               return ApiResponse<List<ActividadCrmDTO>>.ok(dtos, "Actividades de leads");
          }

          public async Task<ApiResponse<List<ActividadCrmDTO>>> GetByOportunidadAsync(int oportunidadId)
          {
               var actividades = await GetActividadQuery().Where(a => a.OportunidadId == oportunidadId && a.Activo).OrderByDescending(a => a.FechaCreacion).ToListAsync();
               var dtos = new List<ActividadCrmDTO>();
               foreach (var a in actividades)
               {
                    dtos.Add(await MapToDto(a));
               }
               return ApiResponse<List<ActividadCrmDTO>>.ok(dtos, "Actividades de oportunidades");
          }

          public async Task<ApiResponse<List<ActividadCrmDTO>>> GetPendientesVendedorAsync(string vendedorId)
          {
                var actividades = await GetActividadQuery()
                .Where(a => a.RealizadaPorId == vendedorId && a.Estado == EstadoActividad.Pendiente && a.Activo)
                       .OrderBy(a => a.FechaProgramada).ToListAsync();
               var dtos =new List<ActividadCrmDTO>();
               foreach(var a in actividades){
                    dtos.Add(await MapToDto(a));
               }
               return ApiResponse<List<ActividadCrmDTO>>.ok(dtos, "Activades Pendientes del vendedor");
          }

          public async Task<ApiResponse<List<ActividadCrmDTO>>> GetProximosContactosAsync(string vendedorId, DateTime fecha)
          {
               var actividades = await GetActividadQuery()
                .Where(a => a.RealizadaPorId == vendedorId && a.Estado == EstadoActividad.Pendiente &&
                       a.FechaProgramada.HasValue && a.FechaProgramada.Value.Date == fecha.Date)
                       .OrderBy(a => a.FechaProgramada).ToListAsync();
               var dtos =new List<ActividadCrmDTO>();
               foreach(var a in actividades){
                    dtos.Add(await MapToDto(a));
               }
               return ApiResponse<List<ActividadCrmDTO>>.ok(dtos, "Próximos contactos");
          }

          public async Task<ApiResponse<List<ActividadCrmDTO>>> GetVencidasAsync(int? sucursalId)
          {
               var query = GetActividadQuery()
                .Where(a => a.Activo && a.Estado == EstadoActividad.Pendiente &&
                       a.FechaProgramada.HasValue && a.FechaProgramada < TimeHelper.Now);

            if (sucursalId.HasValue)
                query = query.Where(a =>
                    (a.Lead != null && a.Lead.SucursalId == sucursalId.Value) ||
                    (a.Oportunidad != null && a.Oportunidad.SucursalId == sucursalId.Value));

            var actividades = await query.OrderBy(a => a.FechaProgramada).ToListAsync();

            var dtos = new List<ActividadCrmDTO>();
            foreach (var a in actividades)
                dtos.Add(await MapToDto(a));

            return ApiResponse<List<ActividadCrmDTO>>.ok(dtos, "Actividades vencidas");
          }

          #region Helpers

          private IQueryable<ActividadCrm> GetActividadQuery()
          {
               return context.ActividadesCrm
                   .Include(a => a.Lead)
                   .Include(a => a.Oportunidad)
                   .AsNoTracking();
          }

          private async Task<ActividadCrmDTO> MapToDto(ActividadCrm a)
          {
               var vendedor = await userManager.FindByIdAsync(a.RealizadaPorId);

               return new ActividadCrmDTO
               {
                    ActividadCrmId = a.ActividadCrmId,
                    Tipo = a.Tipo,
                    Direccion = a.Direccion,
                    Asunto = a.Asunto,
                    Descripcion = a.Descripcion,
                    Estado = a.Estado,
                    FechaProgramada = a.FechaProgramada,
                    FechaRealizacion = a.FechaRealizacion,
                    DuracionMinutos = a.DuracionMinutos,
                    Resultado = a.Resultado,
                    ProximoContacto = a.ProximoContacto,
                    LeadId = a.LeadId,
                    LeadNombre = a.Lead?.NombreCompleto,
                    OportunidadId = a.OportunidadId,
                    OportunidadCodigo = a.Oportunidad?.CodigoOportunidad,
                    RealizadaPorId = a.RealizadaPorId,
                    RealizadaPorNombre = vendedor?.NombreCompleto ?? ""
               };
          }

          #endregion
     }

}