using PremierFlow.Domain.Enums;

namespace PremierFlow.Application.Dtos.Crm
{
    public class LeadDTO
    {
        public int LeadId { get; set; }
        public string CodigoLead { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Empresa { get; set; }
        public string? Ciudad { get; set; }
        public OrigenLead Origen { get; set; }
        public string? DetalleOrigen { get; set; }
        public EstadoLead Estado { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaPrimeraRespuesta { get; set; }
        public string? VehiculoInteres { get; set; }
        public decimal? PresupuestoEstimado { get; set; }

        // Datos relacionados
        public int SucursalId { get; set; }
        public string? SucursalNombre { get; set; }
        public string? VendedorAsignadoId { get; set; }
        public string? VendedorNombre { get; set; }
        public int? ClienteId { get; set; }

        // Extras para UI
        public string EstadoNombre => Estado.ToString();
        public string OrigenNombre => Origen.ToString();
        public bool SinContactar => Estado == EstadoLead.Nuevo && FechaPrimeraRespuesta == null;
        public int CantidadOportunidades { get; set; }
        public int CantidadActividades { get; set; }
    }

    public class CreateLeadDTO
    {
        public string NombreCompleto { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Empresa { get; set; }
        public string? Ciudad { get; set; }
        public OrigenLead Origen { get; set; }
        public string? DetalleOrigen { get; set; }
        public int SucursalId { get; set; }
        public string? VendedorAsignadoId { get; set; }
        public string? VehiculoInteres { get; set; }
        public decimal? PresupuestoEstimado { get; set; }
        public int? ClienteId { get; set; }
    }

    public class UpdateLeadDTO
    {
        public int LeadId { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Empresa { get; set; }
        public string? Ciudad { get; set; }
        public string? DetalleOrigen { get; set; }
        public string? VehiculoInteres { get; set; }
        public decimal? PresupuestoEstimado { get; set; }
        public int? ClienteId { get; set; }
    }

    public class CalificarLeadDTO
    {
        public int LeadId { get; set; }
        public int? ClienteId { get; set; }
    }

    public class DescartarLeadDTO
    {
        public int LeadId { get; set; }
        public string Motivo { get; set; } = null!;
    }

    public class AsignarVendedorLeadDTO
    {
        public int LeadId { get; set; }
        public string VendedorId { get; set; } = null!;
    }
}
