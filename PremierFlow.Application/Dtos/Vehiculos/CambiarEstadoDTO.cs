using PremierFlow.Domain.Enums;

namespace PremierFlow.Application.Dtos.Vehiculos
{
    public class CambiarEstadoDTO
    {
        public EstadoVehiculo NuevoEstado { get; set; }
        public int? ClienteId { get; set; }
    }
}
