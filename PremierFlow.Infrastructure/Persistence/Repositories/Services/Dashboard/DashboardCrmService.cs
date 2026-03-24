
using System.Security.AccessControl;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Dashboard;
using PremierFlow.Application.Interfaces.Dashboard;
using PremierFlow.Domain.Entities;
using PremierFlow.Domain.Enums;
using PremierFlow.Infrastructure.Identity;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.Dashboard
{

    public class DashboardCrmService : IDashboardCrmService
    {
        private readonly PremierFlowDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public DashboardCrmService(PremierFlowDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        public async Task<ApiResponse<DashboardActividadDTO>> GetActividadKpisAsync(DateTime desde, DateTime hasta, int? sucursalId)
        {
            // Excluir actividades de leads descartados o convertidos
            var query = context.ActividadesCrm
                .Where(a => a.Activo && a.FechaCreacion >= desde && a.FechaCreacion <= hasta
                    && a.Lead != null && a.Lead.Activo
                    && a.Lead.Estado != EstadoLead.Descartado);

            if (sucursalId.HasValue)
                query = query.Where(a => a.Lead!.SucursalId.HasValue && a.Lead.SucursalId.Value == sucursalId.Value);

            var actividades = await query.ToListAsync();

            var completadas = actividades.Count(a => a.Estado == EstadoActividad.Completada);
            var vencidas = actividades.Count(a => a.EstaVencida);
            var base_tasa = completadas + vencidas;

            // Por vendedor
            var porVendedor = actividades.GroupBy(a => a.RealizadaPorId);
            var vendedoresAct = new List<KpiVendedorActividadDTO>();
            foreach (var grupo in porVendedor)
            {
                var user = await userManager.FindByIdAsync(grupo.Key);
                var comp = grupo.Count(a => a.Estado == EstadoActividad.Completada);
                var venc = grupo.Count(a => a.EstaVencida);
                var baseV = comp + venc;

                vendedoresAct.Add(new KpiVendedorActividadDTO
                {
                    VendedorId = grupo.Key,
                    VendedorNombre = user?.NombreCompleto ?? "",
                    Realizadas = comp,
                    Vencidas = venc,
                    TasaCumplimiento = baseV > 0 ? Math.Round((decimal)comp / baseV * 100, 1) : 0
                });
            }

            var dto = new DashboardActividadDTO
            {
                TotalActividades = actividades.Count,
                Completadas = completadas,
                Vencidas = vencidas,
                TasaCumplimiento = base_tasa > 0 ? Math.Round((decimal)completadas / base_tasa * 100, 1) : 0,
                ActividadesPorTipo = actividades.GroupBy(a => a.Tipo)
                    .Select(g => new KpiAgrupadoDTO { Nombre = g.Key.ToString(), Cantidad = g.Count() })
                    .OrderByDescending(x => x.Cantidad).ToList(),
                ActividadesPorVendedor = vendedoresAct.OrderByDescending(v => v.Realizadas).ToList()
            };

            return ApiResponse<DashboardActividadDTO>.ok(dto, "KPIs de actividad");
        }

        public async Task<ApiResponse<DashboardEquipoDTO>> GetEquipoKpisAsync(DateTime desde, DateTime hasta, int? sucursalId)
        {
            var queryLeads = context.Leads.Where(l => l.Activo && l.FechaIngreso >= desde && l.FechaIngreso <= hasta);
            var queryOps = context.Oportunidades.Include(o=>o.Cotizaciones)
            .Where(o => o.Activo && o.FechaCreacion >= desde && o.FechaCreacion <= hasta);

            if (sucursalId.HasValue)
            {
                queryLeads = queryLeads.Where(l => l.SucursalId == sucursalId.Value);
                queryOps = queryOps.Where(o => o.SucursalId == sucursalId.Value);
            }
            var leads = await queryLeads.ToListAsync();
            var oportunidades = await queryOps.ToListAsync();

            var vendedorIds = leads.Where(l => !string.IsNullOrEmpty(l.VendedorAsignadoId)).Select(l => l.VendedorAsignadoId!).Union(oportunidades.Select(o => o.VendedorId)).Distinct();
            var vendedores = new List<KpiVendedorDTO>();
            foreach (var vendedorId in vendedorIds)
            {
                var user = await userManager.FindByIdAsync(vendedorId);
                var opsVendedor = oportunidades.Where(o => o.VendedorId == vendedorId).ToList();
                var ganadas = opsVendedor.Where(o => o.Resultado == ResultadoOportunidad.Ganada).ToList();
                vendedores.Add(new KpiVendedorDTO
                {
                    VendedorId = vendedorId,
                    VendedorNombre = user?.NombreCompleto ?? "",
                    LeadsAsignados = leads.Count(l => l.VendedorAsignadoId == vendedorId),
                    OportunidadesGanadas = ganadas.Count,
                    ValorVendido = ganadas.SelectMany(o => o.Cotizaciones.Where(c => c.Activo && c.Estado == EstadoCotizacion.Aceptada)).Sum(c => c.PrecioOfertado),
                    CargaActual = leads.Count(l => l.VendedorAsignadoId == vendedorId && l.Estado != EstadoLead.Descartado && l.Estado != EstadoLead.ConvertidoAOportunidad)
               + opsVendedor.Count(o => o.Resultado == null)
                });
            }
            return ApiResponse<DashboardEquipoDTO>.ok(new DashboardEquipoDTO { Vendedores = vendedores.OrderByDescending(v => v.ValorVendido).ToList() }, "KPIs de equipo");
        }

        public async Task<ApiResponse<DashboardLeadsDTO>> GetLeadsKpisAsync(DateTime desde, DateTime hasta, int? sucursalId)
        {
            var query = context.Leads.Where(l => l.Activo && l.FechaIngreso >= desde && l.FechaIngreso <= hasta);
            if (sucursalId.HasValue)
                query = query.Where(l => l.SucursalId == sucursalId.Value);

            var leads = await query.ToListAsync();
            var descartados=leads.Count(d=>d.Estado==EstadoLead.Descartado);
            var convertidos = leads.Count(l => l.Estado == EstadoLead.ConvertidoAOportunidad);
            var total = leads.Count;

            var conRespuesta = leads.Where(l => l.FechaPrimeraRespuesta.HasValue).ToList();
            var tiempoPromedio = conRespuesta.Count > 0
                ? conRespuesta.Average(l => (l.FechaPrimeraRespuesta!.Value - l.FechaIngreso).TotalHours)
                : 0;

            var dto = new DashboardLeadsDTO
            {
                LeadsNuevos = total,
                LeadsConvertidos = convertidos,
                LeadsDescartados=descartados,
                TasaConversion = total > 0 ? Math.Round((decimal)convertidos / total * 100, 1) : 0,
                TiempoPromedioRespuestaHoras = Math.Round(tiempoPromedio, 1),
                LeadsSinContactar = leads.Count(l => l.FechaPrimeraRespuesta == null),
                LeadsPorOrigen = leads.GroupBy(l => l.Origen)
                    .Select(g => new KpiAgrupadoDTO { Nombre = g.Key.ToString(), Cantidad = g.Count() })
                    .OrderByDescending(x => x.Cantidad)
                    .ToList()
            };

            return ApiResponse<DashboardLeadsDTO>.ok(dto, "KPIs de leads");
        }

        public async Task<ApiResponse<DashboardOrigenDTO>> GetOrigenKpisAsync(DateTime desde, DateTime hasta, int? sucursalId)
        {
            var queryLeads = context.Leads.Where(l => l.Activo && l.FechaIngreso >= desde && l.FechaIngreso <= hasta);
            var queryOps = context.Oportunidades.Include(o => o.Lead).Where(o => o.Activo && o.FechaCreacion >= desde && o.FechaCreacion <= hasta);

            if (sucursalId.HasValue)
            {
                queryLeads = queryLeads.Where(l => l.SucursalId == sucursalId.Value);
                queryOps = queryOps.Where(o => o.SucursalId == sucursalId.Value);
            }

            var leads = await queryLeads.ToListAsync();
            var oportunidades = await queryOps.ToListAsync();

            var origenes = leads.GroupBy(l => l.Origen).Select(g =>
            {
                var total = g.Count();
                var convertidos = g.Count(l => l.Estado == EstadoLead.ConvertidoAOportunidad);
                var opsOrigen = oportunidades.Where(o => o.Lead?.Origen == g.Key).ToList();
                var ganadas = opsOrigen.Count(o => o.Resultado == ResultadoOportunidad.Ganada);
                var cerradas = opsOrigen.Count(o => o.Resultado != null);

                return new KpiOrigenDTO
                {
                    Origen = g.Key.ToString(),
                    TotalLeads = total,
                    Convertidos = convertidos,
                    TasaConversion = total > 0 ? Math.Round((decimal)convertidos / total * 100, 1) : 0,
                    OportunidadesGanadas = ganadas,
                    WinRate = cerradas > 0 ? Math.Round((decimal)ganadas / cerradas * 100, 1) : 0
                };
            }).OrderByDescending(x => x.TotalLeads).ToList();

            return ApiResponse<DashboardOrigenDTO>.ok(new DashboardOrigenDTO { Origenes = origenes }, "KPIs por origen");

        }
        public async Task<ApiResponse<DashboardPipelineDTO>> GetPipelineKpisAsync(DateTime desde, DateTime hasta, int? sucursalId)
        {
            var query = context.Oportunidades.Where(o => o.Activo && o.FechaCreacion >= desde && o.FechaCreacion <= hasta);
            if (sucursalId.HasValue)
                query = query.Where(o => o.SucursalId == sucursalId);

            var oportunidades = await query.Include(o => o.Cotizaciones).ToListAsync();

            var abiertas = oportunidades.Where(o => o.Resultado == null).ToList();
            var cerradas = oportunidades.Where(o => o.Resultado != null).ToList();
            var ganadas = cerradas.Where(o => o.Resultado == ResultadoOportunidad.Ganada).ToList();

            var valorPipeline = abiertas.SelectMany(o => o.Cotizaciones.Where(c => c.Activo && c.Estado == EstadoCotizacion.Vigente)).Sum(c => c.PrecioOfertado);
            var valorVendido = ganadas.SelectMany(o => o.Cotizaciones.Where(c => c.Activo && c.Estado == EstadoCotizacion.Aceptada)).Sum(c => c.PrecioOfertado);

            var dto = new DashboardPipelineDTO
            {
                OportunidadesAbiertas = abiertas.Count,
                ValorTotalPipeline = valorPipeline,
                WinRate = cerradas.Count > 0 ? Math.Round((decimal)ganadas.Count / cerradas.Count * 100, 1) : 0,
                ValorPromedioOportunidad = ganadas.Count > 0 ? Math.Round((decimal)valorVendido / ganadas.Count, 2) : 0,
                OportunidadesPorEtapa = abiertas.GroupBy(e => e.Etapa).Select(g => new KpiAgrupadoDTO { Nombre = g.Key.ToString(), Cantidad = g.Count() }).ToList()
            };
            return ApiResponse<DashboardPipelineDTO>.ok(dto, "KPIS DE PIPELINE");
        }

        public async Task<ApiResponse<DashboardRetencionDTO>> GetRetencionKpisAsync(DateTime desde, DateTime hasta, int? sucursalId)
        {
            var queryLeads = context.Leads.Where(l => l.Activo && l.Estado == EstadoLead.Descartado && l.FechaDescarte >= desde && l.FechaDescarte <= hasta);
            var queryOps = context.Oportunidades.Where(o => o.Activo && o.Resultado != null && o.FechaCierre >= desde && o.FechaCierre <= hasta);

            if (sucursalId.HasValue)
            {
                queryLeads = queryLeads.Where(l => l.SucursalId == sucursalId.Value);
                queryOps = queryOps.Where(o => o.SucursalId == sucursalId.Value);
            }

            var descartados = await queryLeads.ToListAsync();
            var opsCerradas = await queryOps.ToListAsync();

            var dto = new DashboardRetencionDTO
            {
                LeadsDescartados = descartados.Count,
                OportunidadesPerdidas = opsCerradas.Count(o => o.Resultado == ResultadoOportunidad.Perdida),
                OportunidadesCanceladas = opsCerradas.Count(o => o.Resultado == ResultadoOportunidad.Cancelada),
                MotivoDescarte = descartados.Where(l => !string.IsNullOrEmpty(l.MotivoDescarte))
                    .GroupBy(l => l.MotivoDescarte!)
                    .Select(g => new KpiAgrupadoDTO { Nombre = g.Key, Cantidad = g.Count() })
                    .OrderByDescending(x => x.Cantidad).ToList(),
                MotivoPerdida = opsCerradas.Where(o => o.Resultado == ResultadoOportunidad.Perdida && !string.IsNullOrEmpty(o.MotivoResultado))
                    .GroupBy(o => o.MotivoResultado!)
                    .Select(g => new KpiAgrupadoDTO { Nombre = g.Key, Cantidad = g.Count() })
                    .OrderByDescending(x => x.Cantidad).ToList()
            };

            return ApiResponse<DashboardRetencionDTO>.ok(dto, "KPIs de retención");
        }

        public async Task<ApiResponse<DashboardTiempoDTO>> GetTiempoKpisAsync(DateTime desde, DateTime hasta, int? sucursalId)
        {
            var queryLeads = context.Leads.Where(l => l.Activo && l.FechaIngreso >= desde && l.FechaIngreso <= hasta && l.FechaPrimeraRespuesta.HasValue);
            var queryOps = context.Oportunidades.Include(o => o.Lead).Where(o => o.Activo && o.Resultado == ResultadoOportunidad.Ganada && o.FechaCierre >= desde && o.FechaCierre <= hasta);

            if (sucursalId.HasValue)
            {
                queryLeads = queryLeads.Where(l => l.SucursalId == sucursalId.Value);
                queryOps = queryOps.Where(o => o.SucursalId == sucursalId.Value);
            }

            var leads = await queryLeads.ToListAsync();
            var ganadas = await queryOps.ToListAsync();

            var tiempoRespuesta = leads.Count > 0
                ? leads.Average(l => (l.FechaPrimeraRespuesta!.Value - l.FechaIngreso).TotalHours)
                : 0;

            var cicloVenta = ganadas.Count > 0
                ? ganadas.Average(o => (o.FechaCierre!.Value - o.Lead.FechaIngreso).TotalDays)
                : 0;

            var dto = new DashboardTiempoDTO
            {
                TiempoPromedioRespuestaHoras = Math.Round(tiempoRespuesta, 1),
                CicloVentaPromedioDias = Math.Round(cicloVenta, 1)
            };

            return ApiResponse<DashboardTiempoDTO>.ok(dto, "KPIs de tiempo");
        }
    }
}