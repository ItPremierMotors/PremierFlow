using PremierFlow.Domain.Enums;

namespace PremierFlow.Application.Dtos.Vehiculos
{
    public class BulkCambiarEstadoDTO
    {
        public List<int> VehiculoIds { get; set; } = new();
        public EstadoVehiculo NuevoEstado { get; set; }
        public int? UbicacionId { get; set; }
        public int? ClienteId { get; set; }
        public string? VendedorId { get; set; }
        public decimal? PrecioVenta { get; set; }
    }

    public class BulkEditarDTO
    {
        public List<int> VehiculoIds { get; set; } = new();

        // Importacion
        public string? NumeroImportacion { get; set; }
        public string? NumeroPoliza { get; set; }
        public decimal? CostoImportacion { get; set; }
        public DateTime? FechaIngresoPais { get; set; }
        public DateTime? FechaRecepcion { get; set; }

        // Comercial
        public decimal? PrecioLista { get; set; }
        public string? Color { get; set; }

        // Taller
        public int? KilometrajeActual { get; set; }
        public DateTime? FechaPrimeraMatricula { get; set; }
        public DateTime? GarantiaHasta { get; set; }
    }

    public class ResultadoBulkDTO
    {
        public int Total { get; set; }
        public int Exitosos { get; set; }
        public int Fallidos { get; set; }
        public List<FilaBulkResultDTO> Detalle { get; set; } = new();
    }

    public class FilaBulkResultDTO
    {
        public int VehiculoId { get; set; }
        public string? Vin { get; set; }
        public bool Exitoso { get; set; }
        public string? Error { get; set; }
    }
}
