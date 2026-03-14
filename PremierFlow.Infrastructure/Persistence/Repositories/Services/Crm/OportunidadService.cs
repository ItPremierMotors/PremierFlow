
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
    public class OportunidadService : IOportunidadService
    {
        private readonly PremierFlowDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public OportunidadService(PremierFlowDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<ApiResponse<bool>> AvanzarEtapaAsync(int oportunidadId, string usuarioId)
        {
            // Este método avanzaría la oportunidad a la siguiente etapa del pipeline.
            var op=await context.Oportunidades.FirstOrDefaultAsync(o=>o.OportunidadId == oportunidadId && o.Activo);
            if(op==null)
                return ApiResponse<bool>.fail(404,null,"Oportunidad no encontrada");
            try
            {
                op.AvanzarEtapa();
            }
            catch (System.InvalidOperationException ex)
            {
                return ApiResponse<bool>.fail(400,null,ex.Message);
            }
            op.UsuarioModificaId=usuarioId;
            op.FechaModificacion=TimeHelper.Now;
            if(await context.SaveChangesAsync()>0) 
                return ApiResponse<bool>.ok(true,"Oportunidad avanzada a la siguiente etapa");
            return ApiResponse<bool>.fail(500,null,"No se pudo avanzar la oportunidad");
        }

        public async Task<ApiResponse<bool>> CancelarAsync(CerrarOportunidadDTO dto, string usuarioId)
        {
            var op = await context.Oportunidades.FirstOrDefaultAsync(o => o.OportunidadId == dto.OportunidadId && o.Activo);
            if (op == null)
                return ApiResponse<bool>.fail(404, null, "Oportunidad no encontrada");
            try
            {
                op.Cancelar(dto.MotivoResultado ?? "Sin motivo especificado");
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

        public async Task<ApiResponse<bool>> CerrarGanadaAsync(CerrarOportunidadDTO dto, string usuarioId)
        {
            var op = await context.Oportunidades.FirstOrDefaultAsync(o => o.OportunidadId == dto.OportunidadId && o.Activo);
            if (op == null)
                return ApiResponse<bool>.fail(404, null, "Oportunidad no encontrada");
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
                op.CerrarPerdida(dto.MotivoResultado ?? "Sin motivo especificado");
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
            var anio = TimeHelper.Now.Year;
            var prefijo = $"OP-{anio}";
            var ultimoCodigo = await context.Oportunidades
                .Where(o => o.CodigoOportunidad.StartsWith(prefijo))
                .OrderByDescending(o => o.CodigoOportunidad)
                .Select(o => o.CodigoOportunidad)
                .FirstOrDefaultAsync();

            var siguiente = 1;
            if (ultimoCodigo != null)
            {
                var partes = ultimoCodigo.Split('-');
                siguiente = int.Parse(partes[2]) + 1;
            }
            var codigo = $"{prefijo}-{siguiente:D4}";
            var oportunidad = new Oportunidad
            {
                CodigoOportunidad = codigo,
                LeadId = dto.LeadId,
                ClienteId = dto.ClienteId,
                VehiculoId = dto.VehiculoId,
                VendedorId = dto.VendedorId,
                SucursalId = dto.SucursalId,
                ProbabilidadCierre = dto.ProbabilidadCierre,
                FechaCierreEstimada = dto.FechaCierreEstimada,
                Etapa = EtapaOportunidad.Prospeccion,
                Activo = true,
                UsuarioCreaId = usuarioId,
                FechaCreacion = TimeHelper.Now
            };

            context.Oportunidades.Add(oportunidad);
            await context.SaveChangesAsync();
            var created = await GetOportunidadQuery().FirstOrDefaultAsync(o => o.OportunidadId == oportunidad.OportunidadId);
            return ApiResponse<OportunidadDTO>.ok(await MapToDto(created!), "Oportunidad creada exitosamente");
        }

        public async Task<ApiResponse<List<OportunidadDTO>>> GetAllAsync(int? sucursalId)
        {
            var query = GetOportunidadQuery().Where(o => o.Activo);

            if (sucursalId.HasValue)
                query = query.Where(o => o.SucursalId == sucursalId.Value);
        
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

        public async Task<ApiResponse<List<OportunidadDTO>>> GetByEtapaAsync(EtapaOportunidad etapa, int? sucursalId)
        {
            var query =  GetOportunidadQuery().Where(o => o.Etapa == etapa && o.Resultado == null && o.Activo);
            if (sucursalId.HasValue)
                query = query.Where(o => o.SucursalId == sucursalId.Value);
            
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
                .Include(o => o.Cliente)
                .Include(o => o.Vehiculo)
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
                ClienteId = op.ClienteId,
                ClienteNombre = op.Cliente?.NombreCompleto,
                VehiculoId = op.VehiculoId,
                VehiculoDescripcion = op.Vehiculo?.DescripcionCompleta,
                VendedorId = op.VendedorId,
                VendedorNombre = vendedor?.NombreCompleto ?? "",
                SucursalId = op.SucursalId,
                SucursalNombre = op.Sucursal?.Nombre ?? "",
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

        public async Task<ApiResponse<bool>> RetrocederEtapaAsync(int oportunidadId, string usuarioId)
        {
            var op=await context.Oportunidades.FirstOrDefaultAsync(o => o.OportunidadId == oportunidadId && o.Activo); 
            if(op==null)
                return ApiResponse<bool>.fail(404,null,"Oportunidad no encontrada");
            try
            {
                op.RetrocederEtapa();
            }
            catch (System.InvalidOperationException ex)
            {
                return ApiResponse<bool>.fail(400,null,ex.Message);
            }
            op.UsuarioModificaId=usuarioId;
            op.FechaModificacion=TimeHelper.Now;
            if(await context.SaveChangesAsync()>0) 
                return ApiResponse<bool>.ok(true,"Oportunidad retrocedida a la etapa anterior");
            return ApiResponse<bool>.fail(500,null,"No se pudo retroceder la oportunidad");

        }

        public async Task<ApiResponse<OportunidadDTO>> UpdateAsync(UpdateOportunidadDTO dto, string usuarioId)
        {
            var op = await context.Oportunidades
                .FirstOrDefaultAsync(o => o.OportunidadId == dto.OportunidadId && o.Activo);

            if (op == null)
                return ApiResponse<OportunidadDTO>.fail(404, null, "Oportunidad no encontrada");

            if (!op.EstaAbierta)
                return ApiResponse<OportunidadDTO>.fail(400, null, "No se puede editar una oportunidad cerrada");

            op.ClienteId = dto.ClienteId;
            op.VehiculoId = dto.VehiculoId;
            op.ProbabilidadCierre = dto.ProbabilidadCierre;
            op.FechaCierreEstimada = dto.FechaCierreEstimada;
            op.UsuarioModificaId = usuarioId;
            op.FechaModificacion = TimeHelper.Now;

            await context.SaveChangesAsync();

            var updated = await GetOportunidadQuery().FirstOrDefaultAsync(o => o.OportunidadId == op.OportunidadId);
            return ApiResponse<OportunidadDTO>.ok(await MapToDto(updated!), "Oportunidad actualizada exitosamente");
        }

        public async Task<ApiResponse<bool>> VincularVehiculoAsync(int oportunidadId, int vehiculoId, string usuarioId)
        {
            var op = await context.Oportunidades.FirstOrDefaultAsync(o => o.OportunidadId == oportunidadId && o.Activo);
            if (op == null)
                return ApiResponse<bool>.fail(404, null, "Oportunidad no encontrada");

            if (!op.EstaAbierta)
                return ApiResponse<bool>.fail(400, null, "No se puede modificar una oportunidad cerrada");

            var vehiculo = await context.Vehiculos.FirstOrDefaultAsync(v => v.VehiculoId == vehiculoId && v.Activo);
            if (vehiculo == null)
                return ApiResponse<bool>.fail(404, null, "Vehículo no encontrado");

            if (!vehiculo.DisponibleParaVenta && vehiculo.Estado != EstadoVehiculo.Reservado)
                return ApiResponse<bool>.fail(400, null, $"El vehículo no está disponible para venta (Estado: {vehiculo.Estado})");

            op.VehiculoId = vehiculoId;
            op.UsuarioModificaId = usuarioId;
            op.FechaModificacion = TimeHelper.Now;

            await context.SaveChangesAsync();
            return ApiResponse<bool>.ok(true, "Vehículo vinculado a la oportunidad");
        }

        // Aquí irían los métodos para manejar la lógica de negocio relacionada con las oportunidades,
        // como crear, actualizar, cerrar oportunidades, etc.
        #region Helpers

        private IQueryable<Oportunidad> GetOportunidadQuery()
        {
            return context.Oportunidades
                .Include(o => o.Lead)
                .Include(o => o.Sucursal)
                .Include(o => o.Cliente)
                .Include(o => o.Vehiculo)
                .Include(o => o.Actividades.Where(a => a.Activo))
                .Include(o => o.Cotizaciones.Where(c => c.Activo))
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
                ClienteId = o.ClienteId,
                ClienteNombre = o.Cliente?.NombreCompleto,
                VehiculoId = o.VehiculoId,
                VehiculoDescripcion = o.Vehiculo?.DescripcionCompleta,
                VendedorId = o.VendedorId,
                VendedorNombre = vendedor?.NombreCompleto ?? "",
                SucursalId = o.SucursalId,
                SucursalNombre = o.Sucursal?.Nombre ?? "",
                CantidadActividades = o.Actividades?.Count(a => a.Activo) ?? 0,
                CantidadCotizaciones = o.Cotizaciones?.Count(c => c.Activo) ?? 0
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