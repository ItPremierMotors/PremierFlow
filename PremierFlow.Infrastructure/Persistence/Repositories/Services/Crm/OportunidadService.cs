
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Crm;
using PremierFlow.Application.Interfaces.Crm;
using PremierFlow.Domain.Common;
using PremierFlow.Domain.Entities;
using PremierFlow.Domain.Enums;
using PremierFlow.Infrastructure.Identity;
using PremierFlow.Infrastructure.Persistence.Helpers;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.Crm
{
    public class OportunidadService : IOportunidadService
    {
        private readonly PremierFlowDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly GeneradorCodigos generadorCodigos;

        public OportunidadService(PremierFlowDbContext context, UserManager<ApplicationUser> userManager, GeneradorCodigos generadorCodigos)
        {
            this.context = context;
            this.userManager = userManager;
            this.generadorCodigos = generadorCodigos;
        }



        public async Task<ApiResponse<bool>> CancelarAsync(CerrarOportunidadDTO dto, string usuarioId)
        {
            var op = await context.Oportunidades.FirstOrDefaultAsync(o => o.OportunidadId == dto.OportunidadId && o.Activo);
            if (op == null)
                return ApiResponse<bool>.fail(404, null, "Oportunidad no encontrada");
            try
            {
                op.Cancelar(dto.MotivoResultado);
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<bool>.fail(400, null, ex.Message);
            }
            op.UsuarioModificaId = usuarioId;
            op.FechaModificacion = TimeHelper.Now;
            await context.SaveChangesAsync();
            return ApiResponse<bool>.ok(true, "Oportunidad cancelada");
        }

        public async Task<ApiResponse<bool>> CerrarGanadaAsync(CerrarGanadaDTO dto, string usuarioId)
        {
            var op = await context.Oportunidades
                .Include(o => o.Cotizaciones)
                .FirstOrDefaultAsync(o => o.OportunidadId == dto.OportunidadId && o.Activo);
            if (op == null)
                return ApiResponse<bool>.fail(404, null, "Oportunidad no encontrada");
            if (!op.Cotizaciones.Any(c => c.Estado == EstadoCotizacion.Aceptada))
                return ApiResponse<bool>.fail(400, null, "Debe tener al menos una cotización aceptada para cerrar como ganada");
            try
            {
                op.CerrarGanada();
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<bool>.fail(400, null, ex.Message);
            }
            op.UsuarioModificaId = usuarioId;
            op.FechaModificacion = TimeHelper.Now;
            await context.SaveChangesAsync();
            return ApiResponse<bool>.ok(true, "Oportunidad cerrada como ganada");
        }

        public async Task<ApiResponse<bool>> CerrarPerdidaAsync(CerrarOportunidadDTO dto, string usuarioId)
        {
            var op = await context.Oportunidades.FirstOrDefaultAsync(o => o.OportunidadId == dto.OportunidadId && o.Activo);
            if (op == null)
                return ApiResponse<bool>.fail(404, null, "Oportunidad no encontrada");
            try
            {
                op.CerrarPerdida(dto.MotivoResultado);
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<bool>.fail(400, null, ex.Message);
            }
            op.UsuarioModificaId = usuarioId;
            op.FechaModificacion = TimeHelper.Now;
            await context.SaveChangesAsync();
            return ApiResponse<bool>.ok(true, "Oportunidad cerrada como perdida");
        }

        public async Task<ApiResponse<OportunidadDTO>> CreateAsync(CreateOportunidadDTO dto, string usuarioId)
        {
            // Validar que el lead exista y esté calificado
            var lead = await context.Leads.FirstOrDefaultAsync(l => l.LeadId == dto.LeadId && l.Activo);
            if (lead == null)
                return ApiResponse<OportunidadDTO>.fail(404, null, "Lead no encontrado");

            try
            {
                lead.Convertir();
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<OportunidadDTO>.fail(400, null, ex.Message);
            }

            lead.UsuarioModificaId = usuarioId;
            lead.FechaModificacion = TimeHelper.Now;

            var oportunidad = await generadorCodigos.EjecutarConReintento(
                async (codigo) =>
                {
                    var nuevaOp = new Oportunidad
                    {
                        CodigoOportunidad = codigo,
                        LeadId = dto.LeadId,
                        ModeloId = dto.ModeloId,
                        VendedorId = dto.VendedorId,
                        SucursalId = dto.SucursalId,
                        ProbabilidadCierre = dto.ProbabilidadCierre,
                        FechaCierreEstimada = dto.FechaCierreEstimada,
                        Activo = true,
                        UsuarioCreaId = usuarioId,
                        FechaCreacion = TimeHelper.Now
                    };
                    context.Oportunidades.Add(nuevaOp);
                    await context.SaveChangesAsync();
                    return nuevaOp;
                },
                generadorCodigos.GenerarCodigoOportunidadAsync
            );

            var created = await GetOportunidadQuery().FirstOrDefaultAsync(o => o.OportunidadId == oportunidad.OportunidadId);
            return ApiResponse<OportunidadDTO>.ok(await MapToDto(created!), "Oportunidad creada exitosamente");
        }

        public async Task<ApiResponse<List<OportunidadDTO>>> GetAllAsync(int? sucursalId, string? VendedorId)
        {
            var query = GetOportunidadQuery().Where(o => o.Activo);

            if (sucursalId.HasValue)
                query = query.Where(o => o.SucursalId == sucursalId.Value);

            if (!string.IsNullOrEmpty(VendedorId))
                query = query.Where(o => o.VendedorId == VendedorId);
        
            var ops=await query.OrderByDescending(o=>o.FechaCreacion).ToListAsync();
            var dtos = new List<OportunidadDTO>();
            foreach(var op in ops)
            {
                dtos.Add(await MapToDto(op));   
            }
            if(dtos.Count==0)
                return ApiResponse<List<OportunidadDTO>>.fail(404,null,"No se encontraron oportunidades");
            return ApiResponse<List<OportunidadDTO>>.ok(dtos,"Oportunidades encontradas");
        }

        public async Task<ApiResponse<List<OportunidadDTO>>> GetByEtapaAsync(EtapaOportunidad etapa, int? sucursalId, string? VendedorId)
        {
            var query =  GetOportunidadQuery().Where(o => o.Etapa == etapa && o.Resultado == null && o.Activo);
            if (sucursalId.HasValue)
                query = query.Where(o => o.SucursalId == sucursalId.Value);

            if (!string.IsNullOrEmpty(VendedorId))
                query = query.Where(o => o.VendedorId == VendedorId);
            
            var ops=await query.OrderByDescending(o=>o.FechaCreacion).ToListAsync();
            var dtos = new List<OportunidadDTO>();
            foreach(var op in ops)
            {
                dtos.Add(await MapToDto(op));   
            }
            if(dtos.Count==0)
                return ApiResponse<List<OportunidadDTO>>.fail(404,null,"No se encontraron oportunidades en esta etapa");
            return ApiResponse<List<OportunidadDTO>>.ok(dtos,"Oportunidades encontradas");
            
        }

        public async Task<ApiResponse<OportunidadDTO>> GetByIdAsync(int oportunidadId)
        {
            // Este método traería la información básica de la oportunidad para listados o vistas rápidas.
           var op= await GetOportunidadQuery()
                .FirstOrDefaultAsync(o => o.OportunidadId == oportunidadId);

            if (op == null)
                return ApiResponse<OportunidadDTO>.fail(404,null,"Oportunidad no encontrada");

            var dto = await MapToDto(op);
            return ApiResponse<OportunidadDTO>.ok(dto,"Detalle de oportunidad");
   

        }

        public async Task<ApiResponse<List<OportunidadDTO>>> GetByVendedorAsync(string vendedorId)
        {
            var query = await GetOportunidadQuery().Where(o => o.VendedorId == vendedorId && o.Activo).OrderByDescending( o => o.FechaCreacion).ToListAsync();
            var dtos = new List<OportunidadDTO>();
            foreach(var op in query)
            {
                dtos.Add(await MapToDto(op));   
            }
            if(dtos.Count==0)
                return ApiResponse<List<OportunidadDTO>>.fail(404,null,"No se encontraron oportunidades para este vendedor");
            return ApiResponse<List<OportunidadDTO>>.ok(dtos,"Oportunidades encontradas");
        }

        public async Task<ApiResponse<OportunidadDetalleDTO>> GetDetalleAsync(int oportunidadId)
        {
            // Este método traería toda la información detallada de la oportunidad, incluyendo actividades, notas, cotizaciones, etc.
            var op=await context.Oportunidades
                .Include(o => o.Lead)
                .Include(o => o.Sucursal)
                .Include(o => o.Modelo)
                    .ThenInclude(m => m!.Marca)
                .Include(o => o.Actividades.Where(a => a.Activo))
                .Include(o => o.Notas.Where(n => n.Activo))
                .Include(o => o.Cotizaciones.Where(c => c.Activo))
                    .ThenInclude(c => c.Vehiculo)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OportunidadId == oportunidadId);

            if (op == null) return ApiResponse<OportunidadDetalleDTO>.fail(404,null,"Oportunidad no encontrada");    
            
            var vendedor = await userManager.FindByIdAsync(op.VendedorId);
            var dto=new OportunidadDetalleDTO
            {
                OportunidadId = op.OportunidadId,
                CodigoOportunidad = op.CodigoOportunidad,
                Etapa = op.Etapa,
                ProbabilidadCierre = op.ProbabilidadCierre,
                FechaCierreEstimada = op.FechaCierreEstimada,
                Resultado = op.Resultado,
                MotivoResultado = op.MotivoResultado,
                FechaCierre = op.FechaCierre,
                FechaUltimaActividad = op.FechaUltimaActividad,
                LeadId = op.LeadId,
                LeadNombre = op.Lead?.NombreCompleto ?? "",
                VendedorId = op.VendedorId,
                VendedorNombre = vendedor?.NombreCompleto ?? "",
                SucursalId = op.SucursalId,
                SucursalNombre = op.Sucursal?.Nombre ?? "",
                ModeloId = op.ModeloId,
                ModeloNombre = op.Modelo != null ? $"{op.Modelo.Marca?.Nombre} {op.Modelo.Nombre}" : null,
                CantidadActividades = op.Actividades?.Count(a => a.Activo) ?? 0,
                CantidadCotizaciones = op.Cotizaciones?.Count(c => c.Activo) ?? 0,
                Actividades = op.Actividades?.Where(a => a.Activo).Select(MapActividadToDto).ToList() ?? [],
                Notas = op.Notas?.Where(n => n.Activo).Select(MapNotaToDto).ToList() ?? [],
                Cotizaciones = op.Cotizaciones?.Where(c => c.Activo).Select(MapCotizacionToDto).ToList() ?? []
            };
            return ApiResponse<OportunidadDetalleDTO>.ok(dto, "Detalle de oportunidad");
        }

        public async Task<ApiResponse<List<PipelineResumenDTO>>> GetPipelineAsync(int? sucursalId)
        {
           var query= context.Oportunidades.Where(o => o.Activo && o.Resultado == null);
              if(sucursalId.HasValue)
                query=query.Where(o => o.SucursalId == sucursalId.Value);
            var pipeline = await query.GroupBy(o => o.Etapa)
                .Select(g => new PipelineResumenDTO
                {
                    Etapa = g.Key,
                    Cantidad = g.Count(),
                    // ValorTotalEstimado = g.Sum(o => o.Vehiculo != null ? o.Vehiculo.PrecioLista : 0) // Esto es un ejemplo, podrías usar otro campo para el valor estimado
                })
                .ToListAsync();
                return ApiResponse<List<PipelineResumenDTO>>.ok(pipeline,"Resumen del pipeline de ventas");
        }

        public async Task<ApiResponse<List<OportunidadDTO>>> GetVencidasAsync(int? sucursalId, int diasSinActividad = 7)
        {
            var fechalimite=TimeHelper.Now.AddDays(-diasSinActividad);
            var query = GetOportunidadQuery()
                .Where(o => o.Activo && o.Resultado == null &&
                    (o.FechaUltimaActividad == null || o.FechaUltimaActividad < fechalimite));

            if(sucursalId.HasValue)
                query=query.Where(o => o.SucursalId == sucursalId.Value);
            
            var ops= await query.OrderBy(o => o.FechaUltimaActividad).ToListAsync();
            var dtos = new List<OportunidadDTO>();
            foreach(var op in ops)
            {
                dtos.Add(await MapToDto(op));   
            }
            if(dtos.Count==0)
                return ApiResponse<List<OportunidadDTO>>.fail(404,null,"No se encontraron oportunidades vencidas");
            return ApiResponse<List<OportunidadDTO>>.ok(dtos, "Oportunidades vencidas");
        }

        public async Task<ApiResponse<bool>> CambiarEtapaAsync(CambiarEtapaDTO dto, string usuarioId)
        {
            var op = await context.Oportunidades.FirstOrDefaultAsync(o => o.OportunidadId == dto.OportunidadId && o.Activo);
            if (op == null)
                return ApiResponse<bool>.fail(404, null, "Oportunidad no encontrada");
            try
            {
                op.CambiarEtapa(dto.NuevaEtapa);
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<bool>.fail(400, null, ex.Message);
            }
            op.UsuarioModificaId = usuarioId;
            op.FechaModificacion = TimeHelper.Now;
            if (await context.SaveChangesAsync() > 0)
                return ApiResponse<bool>.ok(true, "Etapa cambiada exitosamente");
            return ApiResponse<bool>.fail(500, null, "No se pudo cambiar la etapa");
        }


        public async Task<ApiResponse<OportunidadDTO>> UpdateAsync(UpdateOportunidadDTO dto, string usuarioId)
        {
            var op = await context.Oportunidades
                .FirstOrDefaultAsync(o => o.OportunidadId == dto.OportunidadId && o.Activo);

            if (op == null)
                return ApiResponse<OportunidadDTO>.fail(404, null, "Oportunidad no encontrada");

            if (!op.EstaAbierta)
                return ApiResponse<OportunidadDTO>.fail(400, null, "No se puede editar una oportunidad cerrada");

            op.ProbabilidadCierre = dto.ProbabilidadCierre;
            op.FechaCierreEstimada = dto.FechaCierreEstimada;
            op.UsuarioModificaId = usuarioId;
            op.FechaModificacion = TimeHelper.Now;

            await context.SaveChangesAsync();

            var updated = await GetOportunidadQuery().FirstOrDefaultAsync(o => o.OportunidadId == op.OportunidadId);
            return ApiResponse<OportunidadDTO>.ok(await MapToDto(updated!), "Oportunidad actualizada exitosamente");
        }

        #region Helpers

        private IQueryable<Oportunidad> GetOportunidadQuery()
        {
            return context.Oportunidades
                .Include(o => o.Lead)
                .Include(o => o.Sucursal)
                .Include(o => o.Actividades.Where(a => a.Activo))
                .Include(o => o.Cotizaciones.Where(c => c.Activo))
                .Include(o => o.Modelo)
                .AsNoTracking();
        }

        private async Task<OportunidadDTO> MapToDto(Oportunidad o)
        {
            var vendedor = await userManager.FindByIdAsync(o.VendedorId);

            return new OportunidadDTO
            {
                OportunidadId = o.OportunidadId,
                CodigoOportunidad = o.CodigoOportunidad,
                Etapa = o.Etapa,
                ProbabilidadCierre = o.ProbabilidadCierre,
                FechaCierreEstimada = o.FechaCierreEstimada,
                Resultado = o.Resultado,
                MotivoResultado = o.MotivoResultado,
                FechaCierre = o.FechaCierre,
                FechaUltimaActividad = o.FechaUltimaActividad,
                LeadId = o.LeadId,
                LeadNombre = o.Lead?.NombreCompleto ?? "",
                VendedorId = o.VendedorId,
                VendedorNombre = vendedor?.NombreCompleto ?? "",
                SucursalId = o.SucursalId,
                SucursalNombre = o.Sucursal?.Nombre ?? "",
                CantidadActividades = o.Actividades?.Count(a => a.Activo) ?? 0,
                CantidadCotizaciones = o.Cotizaciones?.Count(c => c.Activo) ?? 0,
                ModeloId = o.ModeloId,
                ModeloNombre = o.Modelo != null ? $"{o.Modelo.Marca?.Nombre} {o.Modelo.Nombre}" : null
            };
        }

        private static ActividadCrmDTO MapActividadToDto(ActividadCrm a)
        {
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
                OportunidadId = a.OportunidadId,
                RealizadaPorId = a.RealizadaPorId,
                RealizadaPorNombre = "" // se llena en el servicio dedicado
            };
        }

        private static NotaCrmDTO MapNotaToDto(NotaCrm n)
        {
            return new NotaCrmDTO
            {
                NotaCrmId = n.NotaCrmId,
                Contenido = n.Contenido,
                EsPrivada = n.EsPrivada,
                FechaCreacion = n.FechaCreacion,
                LeadId = n.LeadId,
                OportunidadId = n.OportunidadId,
                AutorId = n.AutorId,
                AutorNombre = "" // se llena en el servicio dedicado
            };
        }

        private static CotizacionVehiculoDTO MapCotizacionToDto(CotizacionVehiculo c)
        {
            return new CotizacionVehiculoDTO
            {
                CotizacionVehiculoId = c.CotizacionVehiculoId,
                CodigoCotizacion = c.CodigoCotizacion,
                Descuento = c.Descuento,
                PrecioOfertado = c.PrecioOfertado,
                CondicionesPago = c.CondicionesPago,
                Observaciones = c.Observaciones,
                FechaEmision = c.FechaEmision,
                FechaVencimiento = c.FechaVencimiento,
                Estado = c.Estado,
                OportunidadId = c.OportunidadId,
                VehiculoId = c.VehiculoId,
                VehiculoDescripcion = c.Vehiculo?.DescripcionCompleta ?? "",
                VehiculoPrecioLista = c.Vehiculo?.PrecioLista ?? 0
            };
        }

        #endregion
    }
}