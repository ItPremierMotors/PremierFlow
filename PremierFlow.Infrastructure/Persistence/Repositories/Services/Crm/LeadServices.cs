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
    public class LeadService : ILeadService
    {
        private readonly PremierFlowDbContext context;

        private readonly UserManager<ApplicationUser> userManager;
        public LeadService(PremierFlowDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
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

            //si tiene cliente asociado lo agremos de un solo
            if (dto.ClienteId.HasValue)
            {
                lead.ClienteId = dto.ClienteId.Value;
            }
            lead.UsuarioModificaId = usuarioId;
            lead.FechaModificacion = TimeHelper.Now;
            if (await context.SaveChangesAsync() > 0) return ApiResponse<bool>.ok(true, "Lead calificado exitosamente");
            return ApiResponse<bool>.fail(500, null, "Error al calificar el lead");

        }

        public async Task<ApiResponse<OportunidadDTO>> ConvertirAOportunidadAsync(int leadId, CreateOportunidadDTO dto, string usuarioId)
        {
            var lead= await context.Leads.FirstOrDefaultAsync(l => l.LeadId == leadId && l.Activo);
            if (lead == null) return ApiResponse<OportunidadDTO>.fail(404, null, "Lead no encontrado");
            try
            {
                lead.Convertir();
            }
            catch (System.Exception ex)
            {
                return ApiResponse<OportunidadDTO>.fail(400, null, ex.Message);
            }
            //gerarr código de oportunidad
            var codigo = await GenerarCodigoOportunidad();
            var oportunidad = new Oportunidad
            {
              CodigoOportunidad = codigo,
                LeadId = leadId,
                ClienteId = dto.ClienteId ?? lead.ClienteId,
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
            if (await context.SaveChangesAsync() > 0)
            {
                // recargar la oportunidad con sus relaciones para mapear al DTO completo
                await context.Entry(oportunidad).Reference(o => o.Lead).LoadAsync();
                await context.Entry(oportunidad).Reference(o => o.Cliente).LoadAsync();
                await context.Entry(oportunidad).Reference(o => o.Vehiculo).LoadAsync();
                await context.Entry(oportunidad).Reference(o => o.Sucursal).LoadAsync();

                return ApiResponse<OportunidadDTO>.ok(await MapOportunidadToDto(oportunidad), "Lead convertido a oportunidad exitosamente");
            }
            return ApiResponse<OportunidadDTO>.fail(500, null, "Error al convertir el lead a oportunidad");

        }

        public async Task<ApiResponse<LeadDTO>> CreateAsync(CreateLeadDTO dto, string usuarioId)
        {
            var codigo = await GenerarCodigoLead();
            var lead = new Lead
            {
                CodigoLead = codigo,
                NombreCompleto = dto.NombreCompleto,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Empresa = dto.Empresa,
                Ciudad = dto.Ciudad,
                Origen = dto.Origen,
                DetalleOrigen = dto.DetalleOrigen,
                VehiculoInteres = dto.VehiculoInteres,
                PresupuestoEstimado = dto.PresupuestoEstimado,
                SucursalId = dto.SucursalId,
                Estado = EstadoLead.Nuevo,
                FechaIngreso = TimeHelper.Now,
                Activo = true,
                UsuarioCreaId = usuarioId,
                FechaCreacion = TimeHelper.Now
            };
            context.Leads.Add(lead);
            await context.SaveChangesAsync();
            // recargar el lead con sus relaciones para mapear al DTO completo
            await context.Entry(lead).Reference(l => l.Sucursal).LoadAsync();

            return ApiResponse<LeadDTO>.ok(await MapToDto(lead), "Lead creado exitosamente");
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

        public async Task<ApiResponse<List<LeadDTO>>> GetAllAsync(int? sucursalId)
        {

            var query = context.Leads
                .Include(l => l.Sucursal)
                .Include(l => l.Oportunidades.Where(o => o.Activo))
                .Include(l => l.Actividades.Where(a => a.Activo))
                .AsNoTracking()
                .Where(l => l.Activo);

            if (sucursalId.HasValue)
                query = query.Where(l => l.SucursalId == sucursalId.Value);
            var leads = await query.OrderByDescending(l => l.FechaIngreso).ToListAsync();
            var dtos = new List<LeadDTO>();
            foreach (var lead in leads)
            {
                dtos.Add(await MapToDto(lead));
            }
            if (dtos.Count == 0) return ApiResponse<List<LeadDTO>>.fail(404, null, "No se encontraron leads");
            return ApiResponse<List<LeadDTO>>.ok(dtos, "Leads obtenidos exitosamente");
        }

        public async Task<ApiResponse<List<LeadDTO>>> GetByEstadoAsync(EstadoLead estado, int? sucursalId)
        {
            var query = context.Leads
                .Include(l => l.Sucursal)
                .AsNoTracking()
                .Where(l => l.Estado == estado && l.Activo);
            if (sucursalId.HasValue)
                query = query.Where(l => l.SucursalId == sucursalId.Value);

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
            lead.PresupuestoEstimado = dto.PresupuestoEstimado;
            lead.ClienteId = dto.ClienteId;
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

        // Implementación de métodos helpers
        #region Helpers

        private async Task<string> GenerarCodigoLead()
        {
            var anio = TimeHelper.Now.Year;
            var prefijo = $"LD-{anio}";

            var ultimoCodigo = await context.Leads
                .Where(l => l.CodigoLead.StartsWith(prefijo))
                .OrderByDescending(l => l.CodigoLead)
                .Select(l => l.CodigoLead)
                .FirstOrDefaultAsync();

            if (ultimoCodigo == null)
                return $"{prefijo}-0001";

            var partes = ultimoCodigo.Split('-');
            var siguiente = int.Parse(partes[2]) + 1;
            return $"{prefijo}-{siguiente:D4}";
        }

        private async Task<string> GenerarCodigoOportunidad()
        {
            var anio = TimeHelper.Now.Year;
            var prefijo = $"OP-{anio}";

            var ultimoCodigo = await context.Oportunidades
                .Where(o => o.CodigoOportunidad.StartsWith(prefijo))
                .OrderByDescending(o => o.CodigoOportunidad)
                .Select(o => o.CodigoOportunidad)
                .FirstOrDefaultAsync();

            if (ultimoCodigo == null)
                return $"{prefijo}-0001";

            var partes = ultimoCodigo.Split('-');
            var siguiente = int.Parse(partes[2]) + 1;
            return $"{prefijo}-{siguiente:D4}";
        }

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
                FechaPrimeraRespuesta = l.FechaPrimeraRespuesta,
                VehiculoInteres = l.VehiculoInteres,
                PresupuestoEstimado = l.PresupuestoEstimado,
                SucursalId = l.SucursalId,
                SucursalNombre = l.Sucursal?.Nombre,
                VendedorAsignadoId = l.VendedorAsignadoId,
                VendedorNombre = vendedorNombre,
                ClienteId = l.ClienteId,
                CantidadOportunidades = l.Oportunidades?.Count(o => o.Activo) ?? 0,
                CantidadActividades = l.Actividades?.Count(a => a.Activo) ?? 0
            };
        }


        private async Task<OportunidadDTO> MapOportunidadToDto(Oportunidad o)
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

        #endregion
    }
}