using PremierFlow.Domain.Enums;

namespace PremierFlow.Application.Dtos.Crm
{
    public class ActividadCrmDTO
    {
        public int ActividadCrmId { get; set; }
        public TipoActividadCrm Tipo { get; set; }
        public DireccionActividad Direccion { get; set; }
        public string Asunto { get; set; } = null!;
        public string? Descripcion { get; set; }
        public EstadoActividad Estado { get; set; }
        public DateTime? FechaProgramada { get; set; }
        public DateTime? FechaRealizacion { get; set; }
        public int? DuracionMinutos { get; set; }
        public string? Resultado { get; set; }
        public DateTime? ProximoContacto { get; set; }

        // Datos relacionados
        public int? LeadId { get; set; }
        public string? LeadNombre { get; set; }
        public int? OportunidadId { get; set; }
        public string? OportunidadCodigo { get; set; }
        public string RealizadaPorId { get; set; } = null!;
        public string RealizadaPorNombre { get; set; } = null!;

        // Extras para UI
        public string TipoNombre => Tipo.ToString();
        public string DireccionNombre => Direccion.ToString();
        public string EstadoNombre => Estado.ToString();
        public bool EstaVencida => Estado == EstadoActividad.Pendiente
                                   && FechaProgramada.HasValue
                                   && FechaProgramada.Value < DateTime.Now;
    }

    public class CreateActividadCrmDTO
    {
        public int? LeadId { get; set; }
        public int? OportunidadId { get; set; }
        public TipoActividadCrm Tipo { get; set; }
        public DireccionActividad Direccion { get; set; }
        public string Asunto { get; set; } = null!;
        public string? Descripcion { get; set; }
        public DateTime? FechaProgramada { get; set; }
    }

    public class CompletarActividadDTO
    {
        public int ActividadCrmId { get; set; }
        public string Resultado { get; set; } = null!;
        public int? DuracionMinutos { get; set; }
        public DateTime? ProximoContacto { get; set; }
    }
}
