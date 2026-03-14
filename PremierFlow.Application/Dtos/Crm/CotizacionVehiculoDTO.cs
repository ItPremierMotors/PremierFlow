using PremierFlow.Domain.Enums;

namespace PremierFlow.Application.Dtos.Crm
{
    public class CotizacionVehiculoDTO
    {
        public int CotizacionVehiculoId { get; set; }
        public string CodigoCotizacion { get; set; } = null!;
        public decimal Descuento { get; set; }
        public decimal PrecioOfertado { get; set; }
        public string? CondicionesPago { get; set; }
        public string? Observaciones { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public EstadoCotizacion Estado { get; set; }

        // Datos relacionados
        public int OportunidadId { get; set; }
        public int VehiculoId { get; set; }
        public string VehiculoDescripcion { get; set; } = null!;
        public decimal VehiculoPrecioLista { get; set; }

        // Extras para UI
        public string EstadoNombre => Estado.ToString();
        public bool EstaVencida => Estado == EstadoCotizacion.Vigente
                                   && FechaVencimiento < DateTime.Now;
    }

    public class CreateCotizacionVehiculoDTO
    {
        public int OportunidadId { get; set; }
        public int VehiculoId { get; set; }
        public decimal Descuento { get; set; }
        public decimal PrecioOfertado { get; set; }
        public string? CondicionesPago { get; set; }
        public string? Observaciones { get; set; }
        public DateTime FechaVencimiento { get; set; }
    }
}
