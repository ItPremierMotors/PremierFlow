using PremierFlow.Domain.Enums;

namespace PremierFlow.Application.Dtos.Vehiculos
{
    public class CambiarEstadoDTO
    {
        public EstadoVehiculo NuevoEstado { get; set; }
        public int? ClienteId { get; set; }
        public string? VendedorId { get; set; }
        public int? UbicacionId { get; set; }
    }
}
