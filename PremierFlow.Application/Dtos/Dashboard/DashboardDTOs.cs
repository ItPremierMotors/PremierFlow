namespace PremierFlow.Application.Dtos.Dashboard
{
    // =============================================
    // Sección 1: Resumen Ejecutivo (KPIs)
    // =============================================
    public class DashboardResumenDTO
    {
        // Ventas del mes
        public int VentasMesCount { get; set; }
        public decimal VentasMesRevenue { get; set; }

        // Órdenes activas
        public int OrdenesActivasCount { get; set; }

        // Citas del día
        public int CitasAgendadas { get; set; }
        public int CitasCompletadas { get; set; }
        public int CitasNoShow { get; set; }
        public int CitasEnProceso { get; set; }

        // Ocupación del taller (hoy)
        public decimal OcupacionTallerPorcentaje { get; set; }
        public int MinutosDisponibles { get; set; }
        public int MinutosReservados { get; set; }

        // Vehículos en inventario por estado
        public List<VehiculosPorEstadoDTO> VehiculosPorEstado { get; set; } = new();
    }

    public class VehiculosPorEstadoDTO
    {
        public string Estado { get; set; } = null!;
        public int EstadoId { get; set; }
        public int Cantidad { get; set; }
    }

    // =============================================
    // Sección 2: Taller y Técnicos
    // =============================================
    public class DashboardTallerDTO
    {
        public List<OrdenesPorEstadoDTO> OrdenesPorEstado { get; set; } = new();
        public List<ProductividadTecnicoDTO> ProductividadTecnicos { get; set; } = new();
        public List<CapacidadDiariaDTO> CapacidadDiaria { get; set; } = new();
        public decimal EficienciaTaller { get; set; }
    }

    public class OrdenesPorEstadoDTO
    {
        public string Estado { get; set; } = null!;
        public string Codigo { get; set; } = null!;
        public int Cantidad { get; set; }
        public int OrdenSecuencial { get; set; }
    }

    public class ProductividadTecnicoDTO
    {
        public int TecnicoId { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public int OrdenesCompletadas { get; set; }
        public int OrdenesEnProceso { get; set; }
    }

    public class CapacidadDiariaDTO
    {
        public DateTime Fecha { get; set; }
        public int MinutosDisponibles { get; set; }
        public int MinutosReservados { get; set; }
        public int MinutosUtilizados { get; set; }
        public decimal PorcentajeOcupacion { get; set; }
        public decimal PorcentajeEficiencia { get; set; }
    }

    // =============================================
    // Sección 3: Ventas e Inventario
    // =============================================
    public class DashboardVentasDTO
    {
        public List<VentasMensualesDTO> VentasPorMes { get; set; } = new();
    }

    public class VentasMensualesDTO
    {
        public int Anio { get; set; }
        public int Mes { get; set; }
        public string MesNombre { get; set; } = null!;
        public int Cantidad { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DashboardInventarioDTO
    {
        public List<VehiculosPorEstadoDTO> Pipeline { get; set; } = new();
        public List<EnvejecimientoDTO> Envejecimiento { get; set; } = new();
    }

    public class EnvejecimientoDTO
    {
        public string Estado { get; set; } = null!;
        public double DiasPromedio { get; set; }
        public int Cantidad { get; set; }
    }
}
