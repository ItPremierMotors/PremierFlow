using PremierFlow.Domain.Enums;

namespace PremierFlow.Application.Dtos.Crm
{
    public class OportunidadDTO
    {
        public int OportunidadId { get; set; }
        public string CodigoOportunidad { get; set; } = null!;
        public EtapaOportunidad Etapa { get; set; }
        public int ProbabilidadCierre { get; set; }
        public DateTime? FechaCierreEstimada { get; set; }
        public ResultadoOportunidad? Resultado { get; set; }
        public string? MotivoResultado { get; set; }
        public DateTime? FechaCierre { get; set; }
        public DateTime? FechaUltimaActividad { get; set; }

        // Datos relacionados
        public int LeadId { get; set; }
        public string LeadNombre { get; set; } = null!;
        public int? ClienteId { get; set; }
        public string? ClienteNombre { get; set; }
        public int? VehiculoId { get; set; }
        public string? VehiculoDescripcion { get; set; }
        public string VendedorId { get; set; } = null!;
        public string VendedorNombre { get; set; } = null!;
        public int SucursalId { get; set; }
        public string SucursalNombre { get; set; } = null!;

        // Extras para UI
        public string EtapaNombre => Etapa.ToString();
        public string? ResultadoNombre => Resultado?.ToString();
        public bool EstaAbierta => Resultado == null;
        public int CantidadActividades { get; set; }
        public int CantidadCotizaciones { get; set; }
    }

    public class OportunidadDetalleDTO : OportunidadDTO
    {
        public List<ActividadCrmDTO> Actividades { get; set; } = [];
        public List<NotaCrmDTO> Notas { get; set; } = [];
        public List<CotizacionVehiculoDTO> Cotizaciones { get; set; } = [];
    }

    public class CreateOportunidadDTO
    {
        public int LeadId { get; set; }
        public int? ClienteId { get; set; }
        public int? VehiculoId { get; set; }
        public string VendedorId { get; set; } = null!;
        public int SucursalId { get; set; }
        public int ProbabilidadCierre { get; set; }
        public DateTime? FechaCierreEstimada { get; set; }
    }

    public class UpdateOportunidadDTO
    {
        public int OportunidadId { get; set; }
        public int? ClienteId { get; set; }
        public int? VehiculoId { get; set; }
        public int ProbabilidadCierre { get; set; }
        public DateTime? FechaCierreEstimada { get; set; }
    }

    public class CerrarOportunidadDTO
    {
        public int OportunidadId { get; set; }
        public string? MotivoResultado { get; set; }
    }

    /// <summary>
    /// Pipeline agrupado por etapa para el dashboard.
    /// </summary>
    public class PipelineResumenDTO
    {
        public EtapaOportunidad Etapa { get; set; }
        public string EtapaNombre => Etapa.ToString();
        public int Cantidad { get; set; }
    }
}
