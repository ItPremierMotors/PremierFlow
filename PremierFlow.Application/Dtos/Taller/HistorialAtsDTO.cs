using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Dtos.Taller
{
    public class HistorialAtsDTO
    {
        public int AtsId { get; set; }
        public int VehiculoId { get; set; }
        public int OsId { get; set; }
        public DateTime FechaServicio { get; set; }
        public string TipoServicio { get; set; } = null!;
        public int Kilometraje { get; set; }
        public string TrabajosRealizados { get; set; } = null!;
        public decimal MontoTotal { get; set; }
        public string? ObservacionesTecnicas { get; set; }
        public string? ProximaRevision { get; set; }
        public DateTime FechaRegistro { get; set; }

        // Extras para UI
        public string NumeroOs { get; set; } = null!;
        public string VehiculoDescripcion { get; set; } = null!;
        public string? VehiculoPlaca { get; set; }
        public string? VehiculoVin { get; set; }
        public string? ClienteNombre { get; set; }
        public bool TieneRecomendacionPendiente { get; set; }
        public string FechaServicioFormateada => FechaServicio.ToString("dd/MM/yyyy");
    }
    public class CreateHistorialAtsDTO
    {
        public int OsId { get; set; }
        public string TrabajosRealizados { get; set; } = null!;
        public string? ObservacionesTecnicas { get; set; }
        public string? ProximaRevision { get; set; }
    }
}
