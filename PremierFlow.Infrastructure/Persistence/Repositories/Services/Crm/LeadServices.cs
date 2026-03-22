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
    public class LeadService : ILeadService
    {
        private readonly PremierFlowDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly GeneradorCodigos generadorCodigos;

        public LeadService(PremierFlowDbContext context, UserManager<ApplicationUser> userManager, GeneradorCodigos generadorCodigos)
        {
            this.context = context;
            this.userManager = userManager;
            this.generadorCodigos = generadorCodigos;
        }

        public async Task<ApiResponse<bool>> AsignarVendedorAsync(AsignarVendedorLeadDTO dto, string usuarioId)
        {
            var lead = await context.Leads.FirstOrDefaultAsync(l => l.LeadId == dto.LeadId && l.Activo);
            if (lead == null) return ApiResponse<bool>.fail(404, null, "Lead no encontrado");

            //validar que el vendedor exista
            var vendedor = await userManager.FindByIdAsync(dto.VendedorId);
            if (vendedor == null) return ApiResponse<bool>.fail(404, null, "Vendedor no encontrado");
            lead.VendedorAsignadoId = dto.VendedorId;
            lead.UsuarioModificaId = usuarioId;
            lead.FechaModificacion = TimeHelper.Now;
            if (await context.SaveChangesAsync() > 0) return ApiResponse<bool>.ok(true, "Vendedor asignado exitosamente");

            return ApiResponse<bool>.fail(500, null, "Error al asignar el vendedor");
        }

        public async Task<ApiResponse<bool>> CalificarAsync(CalificarLeadDTO dto, string usuarioId)
        {
            var lead = await context.Leads.FirstOrDefaultAsync(l => l.LeadId == dto.LeadId && l.Activo);
            if (lead == null) return ApiResponse<bool>.fail(404, null, "Lead no encontrado");
            try
            {
                lead.Calificar();

            }
            catch (System.Exception ex)
            {
                return ApiResponse<bool>.fail(400, null, ex.Message);
            }

            lead.UsuarioModificaId = usuarioId;
            lead.FechaModificacion = TimeHelper.Now;
            if (await context.SaveChangesAsync() > 0) return ApiResponse<bool>.ok(true, "Lead calificado exitosamente");
            return ApiResponse<bool>.fail(500, null, "Error al calificar el lead");

        }

        public async Task<ApiResponse<LeadDTO>> CreateAsync(CreateLeadDTO dto, string usuarioId)
        {
            //validar que el lead no exista con el mismo email o telefono (si vienen)
            if (!string.IsNullOrEmpty(dto.Email) || !string.IsNullOrEmpty(dto.Telefono))
            {
                var duplicado = await context.Leads.FirstOrDefaultAsync(l => l.Activo &&
                   ((!string.IsNullOrEmpty(dto.Email) && l.Email == dto.Email) ||
                    (!string.IsNullOrEmpty(dto.Telefono) && l.Telefono == dto.Telefono)));


                if (duplicado != null) //está duplicado
                {
                    if (duplicado.Estado == EstadoLead.Descartado)
                    {
                        //reactivar -actualizar el origen y resetrar estado
                        duplicado.Activo = true;
                        duplicado.Origen = dto.Origen;
                        duplicado.DetalleOrigen = dto.DetalleOrigen;
                        duplicado.Estado = EstadoLead.Nuevo;
                        duplicado.UsuarioModificaId = usuarioId;
                        duplicado.FechaModificacion = TimeHelper.Now;
                        duplicado.MotivoDescarte = null;
                        duplicado.FechaDescarte = null;

                        await context.SaveChangesAsync();
                        return ApiResponse<LeadDTO>.ok(await MapToDto(duplicado), "Lead reactivado exitosamente desde un descartado previo");
                    }
                    if (duplicado.Estado == EstadoLead.ConvertidoAOportunidad)
                        return ApiResponse<LeadDTO>.fail(409, null,
                            $"Este contacto ya tiene una oportunidad activa ({duplicado.CodigoLead}). " +
                            $"Si desea iniciar una nueva negociación, cree una oportunidad desde el lead existente.");

                    return ApiResponse<LeadDTO>.fail(400, null, $"Ya existe un lead activo con el mismo email o teléfono({duplicado.CodigoLead})");
                }
            }
            var tieneSucursal = dto.SucursalId.HasValue && dto.SucursalId.Value > 0;
            var tieneContacto = !string.IsNullOrEmpty(dto.Email) || !string.IsNullOrEmpty(dto.Telefono);
            var tieneTipoVehiculo = dto.TipoVehiculoInteres.HasValue;
            var estadoInicial=(tieneSucursal && tieneContacto && tieneTipoVehiculo) ? EstadoLead.Nuevo : EstadoLead.Incompleto;

            //auto asignacion round robin
            string? vendedorAsignado=dto.VendedorAsignadoId; 
            VendedorSucursal? turnoVendedor=null;

            if(dto.SucursalId.HasValue && string.IsNullOrWhiteSpace(vendedorAsignado))
            {
                turnoVendedor=await context.VendedorSucursales.Where(vs=>vs.SucursalId==dto.SucursalId.Value && vs.EstaActivo && vs.Activo)
                .OrderBy(vs=>vs.UltimaAsignacion)
                .FirstOrDefaultAsync();

                if(turnoVendedor != null)
                {
                  vendedorAsignado  =  turnoVendedor.VendedorId;
                }
            }

            var lead = await generadorCodigos.EjecutarConReintento(
                async (codigo) =>
                {
                    var nuevoLead = new Lead
                    {
                        CodigoLead = codigo,
                        NombreCompleto = dto.NombreCompleto,
                        Telefono = dto.Telefono,
                        Email = dto.Email,
                        Empresa = dto.Empresa,
                        Ciudad = dto.Ciudad,
                        Origen = dto.Origen,
                        DetalleOrigen = dto.DetalleOrigen,
                        TipoVehiculoInteres = dto.TipoVehiculoInteres,
                        VehiculoInteres = dto.VehiculoInteres,
                        PresupuestoEstimado = dto.PresupuestoEstimado,
                        SucursalId = dto.SucursalId,
                        VendedorAsignadoId=vendedorAsignado,
                        Estado = estadoInicial,
                        FechaIngreso = TimeHelper.Now,
                        Activo = true,
                        UsuarioCreaId = usuarioId,
                        FechaCreacion = TimeHelper.Now
                    };
                    context.Leads.Add(nuevoLead);

                    //actualizar el turno round-robin
                    if (turnoVendedor != null)
                    {
                        turnoVendedor.UltimaAsignacion=TimeHelper.Now;
                    }
                    await context.SaveChangesAsync();
                    await context.Entry(nuevoLead).Reference(l => l.Sucursal).LoadAsync();
                    return nuevoLead;
                },
                generadorCodigos.GenerarCodigoLeadAsync
            );

            return ApiResponse<LeadDTO>.ok(await MapToDto(lead), "Lead creado exitosamente");
        }

        public async Task<ApiResponse<bool>> MarcarContactadoAsync(int leadId, string usuarioId)
        {
            var lead = await context.Leads.FirstOrDefaultAsync(l => l.LeadId == leadId && l.Activo);
            if (lead == null) return ApiResponse<bool>.fail(404, null, "Lead no encontrado");

            if (lead.Estado != EstadoLead.Nuevo && lead.Estado != EstadoLead.Incompleto)
                return ApiResponse<bool>.fail(400, null, $"El lead ya está en estado {lead.Estado} y no puede marcarse como contactado");

            lead.MarcarContactado();
            lead.UsuarioModificaId = usuarioId;
            lead.FechaModificacion = TimeHelper.Now;
            if (await context.SaveChangesAsync() > 0) return ApiResponse<bool>.ok(true, "Lead marcado como contactado");
            return ApiResponse<bool>.fail(500, null, "Error al marcar el lead como contactado");
        }

        public async Task<ApiResponse<bool>> DescartarAsync(DescartarLeadDTO dto, string usuarioId)
        {
            var lead = await context.Leads.FirstOrDefaultAsync(l => l.LeadId == dto.LeadId && l.Activo);
            if (lead == null) return ApiResponse<bool>.fail(404, null, "Lead no encontrado");
            try
            {
                lead.Descartar(dto.Motivo);
            }
            catch (System.Exception ex)
            {
                return ApiResponse<bool>.fail(400, null, ex.Message);
            }
            lead.UsuarioModificaId = usuarioId;
            lead.FechaModificacion = TimeHelper.Now;
            if (await context.SaveChangesAsync() > 0) return ApiResponse<bool>.ok(true, "Lead descartado exitosamente");
            return ApiResponse<bool>.fail(500, null, "Error al descartar el lead");
        }

        public async Task<ApiResponse<List<LeadDTO>>> GetAllAsync(int? sucursalId, string? vendedorId = null)
        {

            var query = context.Leads
                .Include(l => l.Sucursal)
                .Include(l => l.Oportunidades.Where(o => o.Activo))
                .Include(l => l.Actividades.Where(a => a.Activo))
                .AsNoTracking()
                .Where(l => l.Activo && l.Estado != EstadoLead.ConvertidoAOportunidad && l.Estado != EstadoLead.Descartado);

            if (sucursalId.HasValue)
                query = query.Where(l => l.SucursalId == sucursalId.Value);

            if (!string.IsNullOrEmpty(vendedorId))
                query = query.Where(l => l.VendedorAsignadoId == vendedorId);
            var leads = await query.OrderByDescending(l => l.FechaIngreso).ToListAsync();
            var dtos = new List<LeadDTO>();
            foreach (var lead in leads)
            {
                dtos.Add(await MapToDto(lead));
            }
            if (dtos.Count == 0) return ApiResponse<List<LeadDTO>>.fail(404, null, "No se encontraron leads");
            return ApiResponse<List<LeadDTO>>.ok(dtos, "Leads obtenidos exitosamente");
        }

        public async Task<ApiResponse<List<LeadDTO>>> GetByEstadoAsync(EstadoLead estado, int? sucursalId, string? vendedorId)
        {
            var query = context.Leads
                .Include(l => l.Sucursal)
                .AsNoTracking()
                .Where(l => l.Estado == estado && l.Activo);
            if (sucursalId.HasValue)
                query = query.Where(l => l.SucursalId == sucursalId.Value);
            if (!string.IsNullOrEmpty(vendedorId))
                query = query.Where(l => l.VendedorAsignadoId == vendedorId);

            var leads = await query.OrderByDescending(l => l.FechaIngreso).ToListAsync();
            var dtos = new List<LeadDTO>();
            foreach (var lead in leads)
            {
                dtos.Add(await MapToDto(lead));
            }
            if (dtos.Count == 0) return ApiResponse<List<LeadDTO>>.fail(404, null, $"No se encontraron leads con estado {estado}");
            return ApiResponse<List<LeadDTO>>.ok(dtos, $"Leads con estado {estado} obtenidos exitosamente");
        }

        public async Task<ApiResponse<LeadDTO>> GetByIdAsync(int leadId)
        {
            // Obtener el lead con sus relaciones necesarias para mostrar en el detalle
            var lead = await context.Leads
            .Include(l => l.Sucursal)
            .Include(l => l.Oportunidades.Where(o => o.Activo))
            .Include(l => l.Actividades.Where(a => a.Activo))
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.LeadId == leadId && l.Activo);

            if (lead == null) return ApiResponse<LeadDTO>.fail(404, null, "Lead no encontrado");

            return ApiResponse<LeadDTO>.ok(await MapToDto(lead), "Lead obtenido exitosamente");
        }

        public async Task<ApiResponse<List<LeadDTO>>> GetByVendedorAsync(string vendedorId)
        {
            var leads = await context.Leads
                 .Include(l => l.Sucursal)
                 .Include(l => l.Oportunidades.Where(o => o.Activo))
                 .Include(l => l.Actividades.Where(a => a.Activo))
                 .AsNoTracking()
                 .Where(l => l.VendedorAsignadoId == vendedorId && l.Activo)
                 .OrderByDescending(l => l.FechaIngreso)
                 .ToListAsync();

            var dtos = new List<LeadDTO>();
            foreach (var lead in leads)
            {
                dtos.Add(await MapToDto(lead));
            }
            if (dtos.Count == 0) return ApiResponse<List<LeadDTO>>.fail(404, null, "No se encontraron leads para el vendedor");
            return ApiResponse<List<LeadDTO>>.ok(dtos, "Leads obtenidos exitosamente");
        }

        public async Task<ApiResponse<List<LeadDTO>>> GetSinAsignarAsync(int? sucursalId)
        {
            var query = context.Leads
                .Include(l => l.Sucursal)
                .AsNoTracking()
                .Where(l => l.VendedorAsignadoId == null && l.Activo);

            if (sucursalId.HasValue)
                query = query.Where(l => l.SucursalId == sucursalId.Value);
            var leads = await query.OrderByDescending(l => l.FechaIngreso).ToListAsync();
            var dtos = new List<LeadDTO>();
            foreach (var lead in leads)
            {
                dtos.Add(await MapToDto(lead));
            }
            if (dtos.Count == 0) return ApiResponse<List<LeadDTO>>.fail(404, null, "No se encontraron leads sin asignar");
            return ApiResponse<List<LeadDTO>>.ok(dtos, "Leads sin asignar obtenidos exitosamente");
        }

        public async Task<ApiResponse<List<LeadDTO>>> SearchAsync(string termino)
        {
            var term = termino.ToLower();
            var leads = await context.Leads
                .Include(l => l.Sucursal)
                .AsNoTracking()
                .Where(l => (l.NombreCompleto.ToLower().Contains(term) ||
                            (l.Telefono != null && l.Telefono.Contains(term)) ||
                            (l.Email != null && l.Email.ToLower().Contains(term)) ||
                            (l.Empresa != null && l.Empresa.ToLower().Contains(term)) ||
                            (l.Ciudad != null && l.Ciudad.ToLower().Contains(term)) ||
                            (l.VehiculoInteres != null && l.VehiculoInteres.ToLower().Contains(term))
                            ) && l.Activo
                ).Take(20)
                .OrderByDescending(l => l.FechaIngreso)
                .ToListAsync();

            var dtos = new List<LeadDTO>();
            foreach (var lead in leads)
            {
                dtos.Add(await MapToDto(lead));
            }
            if (dtos.Count == 0) return ApiResponse<List<LeadDTO>>.fail(404, null, "No se encontraron leads que coincidan con el término de búsqueda");
            return ApiResponse<List<LeadDTO>>.ok(dtos, "Leads obtenidos exitosamente");
        }

        public async Task<ApiResponse<LeadDTO>> UpdateAsync(UpdateLeadDTO dto, string usuarioId)
        {
            var lead = await context.Leads.FirstOrDefaultAsync(l => l.LeadId == dto.LeadId && l.Activo);
            if (lead == null) return ApiResponse<LeadDTO>.fail(404, null, "Lead no encontrado");

            lead.NombreCompleto = dto.NombreCompleto;
            lead.Telefono = dto.Telefono;
            lead.Email = dto.Email;
            lead.Empresa = dto.Empresa;
            lead.Ciudad = dto.Ciudad;
            lead.DetalleOrigen = dto.DetalleOrigen;
            lead.VehiculoInteres = dto.VehiculoInteres;
            lead.TipoVehiculoInteres = dto.TipoVehiculoInteres;
            lead.PresupuestoEstimado = dto.PresupuestoEstimado;
            lead.UsuarioModificaId = usuarioId;
            lead.FechaModificacion = TimeHelper.Now;

            if (await context.SaveChangesAsync() > 0)
            {
                // recargar el lead con sus relaciones para mapear al DTO completo
                await context.Entry(lead).Reference(l => l.Sucursal).LoadAsync();
                return ApiResponse<LeadDTO>.ok(await MapToDto(lead), "Lead actualizado exitosamente");
            }
            return ApiResponse<LeadDTO>.fail(500, null, "Error al actualizar el lead");
        }
        public async Task<ApiResponse<AlertasLeadsFriosDTO>> GetAlertasLeadsFriosAsync(int? sucursalId)
        {
            //consulta leads activos (no descartados, no convertidos), calcula días inactivos, se clasifican en los 4 grupos
            // validar que la sucursal venga con valores
           if(!sucursalId.HasValue) return ApiResponse<AlertasLeadsFriosDTO>.fail(400, null, "Se necesita una sucursal");

           //1. generar consulta de leads
            var leads = await context.Leads
                    .Include(l=>l.Sucursal)
                    .Where(l => l.Activo 
                        && l.Estado != EstadoLead.Descartado 
                        && l.Estado != EstadoLead.ConvertidoAOportunidad && l.SucursalId==sucursalId.Value)
                    .ToListAsync();  

            //2. agrupar por clase 
            var alertas=new AlertasLeadsFriosDTO();
            foreach(var lead in leads)
            {
                var dto=await MapToDto(lead); // esto me dara los dias inactivos y fecha ultima actividad
                if(lead.FechaPrimeraRespuesta==null)
                  alertas.SinContactar.Add(dto);
                else if(dto.DiasInactivo>=30)
                  alertas.CandidatosDescarte.Add(dto);
                else if(dto.DiasInactivo>=7)
                  alertas.MuyFrios.Add(dto);
                else if(dto.DiasInactivo>=3)
                 alertas.Frios.Add(dto);
            }   
            return ApiResponse<AlertasLeadsFriosDTO>.ok(alertas, "leads frios cargados");
        }

        #region Helpers

        private async Task<LeadDTO> MapToDto(Lead l)
        {
            string? vendedorNombre = null;
            if (!string.IsNullOrEmpty(l.VendedorAsignadoId))
            {
                var vendedor = await userManager.FindByIdAsync(l.VendedorAsignadoId);
                vendedorNombre = vendedor?.NombreCompleto;
            }

            return new LeadDTO
            {
                LeadId = l.LeadId,
                CodigoLead = l.CodigoLead,
                NombreCompleto = l.NombreCompleto,
                Telefono = l.Telefono,
                Email = l.Email,
                Empresa = l.Empresa,
                Ciudad = l.Ciudad,
                Origen = l.Origen,
                DetalleOrigen = l.DetalleOrigen,
                Estado = l.Estado,
                FechaIngreso = l.FechaIngreso,
                FechaUltimaActividad=l.FechaUltimaActividad,
                FechaPrimeraRespuesta = l.FechaPrimeraRespuesta,
                VehiculoInteres = l.VehiculoInteres,
                TipoVehiculoInteres = l.TipoVehiculoInteres,
                PresupuestoEstimado = l.PresupuestoEstimado,
                SucursalId = l.SucursalId,
                SucursalNombre = l.Sucursal?.Nombre,
                VendedorAsignadoId = l.VendedorAsignadoId,
                VendedorNombre = vendedorNombre,
                CantidadOportunidades = l.Oportunidades?.Count(o => o.Activo) ?? 0,
                CantidadActividades = l.Actividades?.Count(a => a.Activo) ?? 0,
                DiasInactivo = (int)(TimeHelper.Now - (l.FechaUltimaActividad ?? l.FechaIngreso)).TotalDays
                
            };
        }

        


        #endregion
    }
}