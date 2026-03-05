using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Dashboard;
using PremierFlow.Application.Interfaces.Dashboard;
using PremierFlow.Domain.Common;
using PremierFlow.Domain.Entities;
using PremierFlow.Domain.Enums;
using PremierFlow.Infrastructure.Persistence;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly PremierFlowDbContext context;

        public DashboardService(PremierFlowDbContext context)
        {
            this.context = context;
        }

        public async Task<ApiResponse<DashboardResumenDTO>> GetResumenAsync()
        {
            var hoy = TimeHelper.Today;
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
            var finMes = inicioMes.AddMonths(1);

            // Ventas del mes
            var ventasMes = await context.Vehiculos.AsNoTracking()
                .Where(v => v.Activo && v.FechaVenta.HasValue
                    && v.FechaVenta.Value >= inicioMes
                    && v.FechaVenta.Value < finMes)
                .GroupBy(v => 1)
                .Select(g => new
                {
                    Count = g.Count(),
                    Revenue = g.Sum(v => v.PrecioVenta ?? 0)
                })
                .FirstOrDefaultAsync();

            // Órdenes activas (no cerrada ni cancelada)
            var ordenesActivas = await context.OrdenesServicio.AsNoTracking()
                .Include(o => o.Estado)
                .Where(o => o.Activo
                    && o.Estado.Codigo != EstadoOs.Estados.Cerrada
                    && o.Estado.Codigo != EstadoOs.Estados.Cancelada)
                .CountAsync();

            // Citas del día agrupadas por estado
            var citasHoy = await context.Citas.AsNoTracking()
                .Where(c => c.Activo && c.FechaHoraInicio.Date == hoy)
                .GroupBy(c => c.Estado)
                .Select(g => new { Estado = g.Key, Count = g.Count() })
                .ToListAsync();

            // Ocupación del taller (hoy, todas las sucursales)
            var capacidadHoy = await context.CapacidadTaller.AsNoTracking()
                .Where(c => c.Activo && c.Fecha.Date == hoy)
                .GroupBy(c => 1)
                .Select(g => new
                {
                    MinutosDisponibles = g.Sum(c => c.MinutosDisponibles),
                    MinutosReservados = g.Sum(c => c.MinutosReservados)
                })
                .FirstOrDefaultAsync();

            // Vehículos por estado
            var vehiculosRaw = await context.Vehiculos.AsNoTracking()
                .Where(v => v.Activo)
                .GroupBy(v => v.Estado)
                .Select(g => new
                {
                    Estado = g.Key,
                    Cantidad = g.Count()
                })
                .ToListAsync();

            var vehiculosPorEstado = vehiculosRaw
                .Select(g => new VehiculosPorEstadoDTO
                {
                    Estado = g.Estado.ToString(),
                    EstadoId = (int)g.Estado,
                    Cantidad = g.Cantidad
                })
                .OrderBy(v => v.EstadoId)
                .ToList();

            var dto = new DashboardResumenDTO
            {
                VentasMesCount = ventasMes?.Count ?? 0,
                VentasMesRevenue = ventasMes?.Revenue ?? 0,
                OrdenesActivasCount = ordenesActivas,
                CitasAgendadas = citasHoy
                    .Where(c => c.Estado == EstadoCita.Agendada || c.Estado == EstadoCita.Confirmada)
                    .Sum(c => c.Count),
                CitasCompletadas = citasHoy
                    .Where(c => c.Estado == EstadoCita.Completada)
                    .Sum(c => c.Count),
                CitasNoShow = citasHoy
                    .Where(c => c.Estado == EstadoCita.NoShow)
                    .Sum(c => c.Count),
                CitasEnProceso = citasHoy
                    .Where(c => c.Estado == EstadoCita.EnProceso)
                    .Sum(c => c.Count),
                MinutosDisponibles = capacidadHoy?.MinutosDisponibles ?? 0,
                MinutosReservados = capacidadHoy?.MinutosReservados ?? 0,
                OcupacionTallerPorcentaje = capacidadHoy != null && capacidadHoy.MinutosDisponibles > 0
                    ? Math.Round((decimal)capacidadHoy.MinutosReservados / capacidadHoy.MinutosDisponibles * 100, 1)
                    : 0,
                VehiculosPorEstado = vehiculosPorEstado
            };

            return ApiResponse<DashboardResumenDTO>.ok(dto, "Resumen del dashboard obtenido.");
        }

        public async Task<ApiResponse<DashboardTallerDTO>> GetTallerAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            // Órdenes por estado en el rango de fechas
            var ordenesPorEstado = await context.OrdenesServicio.AsNoTracking()
                .Where(o => o.Activo
                    && o.FechaApertura.Date >= fechaInicio.Date
                    && o.FechaApertura.Date <= fechaFin.Date)
                .GroupBy(o => new { o.Estado.Nombre, o.Estado.Codigo, o.Estado.OrdenSecuencial })
                .Select(g => new OrdenesPorEstadoDTO
                {
                    Estado = g.Key.Nombre,
                    Codigo = g.Key.Codigo,
                    Cantidad = g.Count(),
                    OrdenSecuencial = g.Key.OrdenSecuencial
                })
                .OrderBy(x => x.OrdenSecuencial)
                .ToListAsync();

            // Productividad por técnico (asignaciones en el rango)
            var productividad = await context.AsignacionesTecnico.AsNoTracking()
                .Include(a => a.Tecnico)
                .Where(a => a.Activo
                    && a.FechaAsignacion.Date >= fechaInicio.Date
                    && a.FechaAsignacion.Date <= fechaFin.Date)
                .GroupBy(a => new { a.TecnicoId, a.Tecnico.Nombre, a.Tecnico.Apellidos })
                .Select(g => new ProductividadTecnicoDTO
                {
                    TecnicoId = g.Key.TecnicoId,
                    NombreCompleto = g.Key.Nombre + " " + g.Key.Apellidos,
                    OrdenesCompletadas = g.Count(a => a.Estado == EstadoAsignacion.Completado),
                    OrdenesEnProceso = g.Count(a => a.Estado == EstadoAsignacion.EnProceso
                                                 || a.Estado == EstadoAsignacion.Asignado)
                })
                .OrderByDescending(t => t.OrdenesCompletadas)
                .ToListAsync();

            // Capacidad diaria (tendencia)
            var capacidadDiaria = await context.CapacidadTaller.AsNoTracking()
                .Where(c => c.Activo && c.Fecha.Date >= fechaInicio.Date && c.Fecha.Date <= fechaFin.Date)
                .GroupBy(c => c.Fecha.Date)
                .Select(g => new CapacidadDiariaDTO
                {
                    Fecha = g.Key,
                    MinutosDisponibles = g.Sum(c => c.MinutosDisponibles),
                    MinutosReservados = g.Sum(c => c.MinutosReservados),
                    MinutosUtilizados = g.Sum(c => c.MinutosUtilizados)
                })
                .OrderBy(c => c.Fecha)
                .ToListAsync();

            // Eficiencia global del taller en el rango
            var totales = await context.CapacidadTaller.AsNoTracking()
                .Where(c => c.Activo && c.Fecha.Date >= fechaInicio.Date && c.Fecha.Date <= fechaFin.Date)
                .GroupBy(c => 1)
                .Select(g => new
                {
                    Reservados = g.Sum(c => c.MinutosReservados),
                    Utilizados = g.Sum(c => c.MinutosUtilizados)
                })
                .FirstOrDefaultAsync();

            var eficiencia = totales != null && totales.Reservados > 0
                ? Math.Round((decimal)totales.Utilizados / totales.Reservados * 100, 1)
                : 0;

            return ApiResponse<DashboardTallerDTO>.ok(new DashboardTallerDTO
            {
                OrdenesPorEstado = ordenesPorEstado,
                ProductividadTecnicos = productividad,
                CapacidadDiaria = capacidadDiaria,
                EficienciaTaller = eficiencia
            }, "Datos del taller obtenidos.");
        }

        public async Task<ApiResponse<DashboardVentasDTO>> GetVentasAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var ventasPorMes = await context.Vehiculos.AsNoTracking()
                .Where(v => v.Activo
                    && v.FechaVenta.HasValue
                    && v.FechaVenta.Value >= fechaInicio
                    && v.FechaVenta.Value <= fechaFin)
                .GroupBy(v => new { v.FechaVenta!.Value.Year, v.FechaVenta!.Value.Month })
                .Select(g => new VentasMensualesDTO
                {
                    Anio = g.Key.Year,
                    Mes = g.Key.Month,
                    MesNombre = "",
                    Cantidad = g.Count(),
                    Revenue = g.Sum(v => v.PrecioVenta ?? 0)
                })
                .OrderBy(v => v.Anio).ThenBy(v => v.Mes)
                .ToListAsync();

            var meses = new[] { "", "Ene", "Feb", "Mar", "Abr", "May", "Jun",
                               "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };
            foreach (var v in ventasPorMes)
                v.MesNombre = meses[v.Mes];

            return ApiResponse<DashboardVentasDTO>.ok(new DashboardVentasDTO
            {
                VentasPorMes = ventasPorMes
            }, "Datos de ventas obtenidos.");
        }

        public async Task<ApiResponse<DashboardInventarioDTO>> GetInventarioAsync()
        {
            var hoy = TimeHelper.Today;

            // Pipeline: vehículos por estado
            // Materializamos con enum raw y convertimos a string en memoria
            var pipelineRaw = await context.Vehiculos.AsNoTracking()
                .Where(v => v.Activo)
                .GroupBy(v => v.Estado)
                .Select(g => new
                {
                    Estado = g.Key,
                    Cantidad = g.Count()
                })
                .ToListAsync();

            var pipeline = pipelineRaw
                .Select(g => new VehiculosPorEstadoDTO
                {
                    Estado = g.Estado.ToString(),
                    EstadoId = (int)g.Estado,
                    Cantidad = g.Cantidad
                })
                .OrderBy(v => v.EstadoId)
                .ToList();

            // Envejecimiento: días promedio en cada estado (solo no vendidos/entregados)
            // Materializamos primero para evitar problemas de traducción LINQ
            var vehiculosActivos = await context.Vehiculos.AsNoTracking()
                .Where(v => v.Activo
                    && v.Estado != EstadoVehiculo.Vendido
                    && v.Estado != EstadoVehiculo.Entregado)
                .Select(v => new { v.Estado, v.FechaRegistro })
                .ToListAsync();

            var envejecimiento = vehiculosActivos
                .GroupBy(v => v.Estado)
                .Select(g => new EnvejecimientoDTO
                {
                    Estado = g.Key.ToString(),
                    DiasPromedio = Math.Round(g.Average(v => (hoy - v.FechaRegistro).TotalDays), 1),
                    Cantidad = g.Count()
                })
                .OrderBy(e => (int)Enum.Parse<EstadoVehiculo>(e.Estado))
                .ToList();

            return ApiResponse<DashboardInventarioDTO>.ok(new DashboardInventarioDTO
            {
                Pipeline = pipeline,
                Envejecimiento = envejecimiento
            }, "Datos de inventario obtenidos.");
        }
    }
}
