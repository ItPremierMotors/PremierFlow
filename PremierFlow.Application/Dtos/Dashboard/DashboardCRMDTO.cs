

namespace PremierFlow.Application.Dtos.Dashboard
{

  public class DashboardLeadsDTO
{
    public int LeadsNuevos { get; set; }
    public int LeadsConvertidos { get; set; }
    public decimal TasaConversion { get; set; }              // (convertidos / total) * 100
    public double TiempoPromedioRespuestaHoras { get; set; } // promedio FechaPrimeraRespuesta - FechaIngreso
    public int LeadsSinContactar { get; set; }
    public int LeadsDescartados {get; set;}
    public List<KpiAgrupadoDTO> LeadsPorOrigen { get; set; } = new();
}

    public class DashboardPipelineDTO
    {
        public int OportunidadesAbiertas { get; set; }
        public decimal ValorTotalPipeline { get; set; } //suma precio ofertados de cotizaciones vigentes
        public decimal WinRate { get; set; } //ganadas/total cerradas * 100
        public decimal ValorPromedioOportunidad { get; set; } //valor vendido / ganadas
        public List<KpiAgrupadoDTO> OportunidadesPorEtapa { get; set; } = new();
    }
    public class DashboardEquipoDTO
    {
        public List<KpiVendedorDTO> Vendedores { get; set; } = new();
    }

    public class KpiVendedorDTO
    {
        public string VendedorId { get; set; } = null!;
        public string VendedorNombre { get; set; } = null!;
        public int LeadsAsignados { get; set; }
        public int OportunidadesGanadas { get; set; }
        public decimal ValorVendido { get; set; }
        public int CargaActual { get; set; }                     // leads activos + oportunidades abiertas
    }
    public class DashboardRetencionDTO
    {
        public int LeadsDescartados { get; set; }
        public int OportunidadesPerdidas { get; set; }
        public int OportunidadesCanceladas { get; set; }
        public List<KpiAgrupadoDTO> MotivoDescarte { get; set; } = new();
        public List<KpiAgrupadoDTO> MotivoPerdida { get; set; } = new();
    }
    public class DashboardActividadDTO
    {
        public int TotalActividades { get; set; }
        public int Completadas { get; set; }
        public int Vencidas { get; set; }
        public decimal TasaCumplimiento { get; set; }            // (completadas / (completadas + vencidas)) * 100
        public List<KpiAgrupadoDTO> ActividadesPorTipo { get; set; } = new();
        public List<KpiVendedorActividadDTO> ActividadesPorVendedor { get; set; } = new();
    }
    public class KpiVendedorActividadDTO
    {
        public string VendedorId { get; set; } = null!;
        public string VendedorNombre { get; set; } = null!;
        public int Realizadas { get; set; }
        public int Vencidas { get; set; }
        public decimal TasaCumplimiento { get; set; }
    }
    public class DashboardOrigenDTO
    {
        public List<KpiOrigenDTO> Origenes { get; set; } = new();
    }
    public class KpiOrigenDTO
    {
        public string Origen { get; set; } = null!;
        public int TotalLeads { get; set; }
        public int Convertidos { get; set; }
        public decimal TasaConversion { get; set; }
        public int OportunidadesGanadas { get; set; }
        public decimal WinRate { get; set; }
    }
    public class DashboardTiempoDTO
    {
        public double TiempoPromedioRespuestaHoras { get; set; }
        public double CicloVentaPromedioDias { get; set; }       // FechaCierre(ganada) - Lead.FechaIngreso
    }
    public class KpiAgrupadoDTO
    {
        public string Nombre { get; set; } = null!;
        public int Cantidad { get; set; }
    }
}
