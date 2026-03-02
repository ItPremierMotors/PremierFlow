namespace PremierFlow.Application.Dtos.Taller
{
    /// <summary>
    /// Resumen consolidado del historial de servicio de un vehículo.
    /// </summary>
    public class HistorialServicioVehiculoDTO
    {
        public int VehiculoId { get; set; }
        public string VehiculoDescripcion { get; set; } = null!;
        public string? Placa { get; set; }
        public int KilometrajeActual { get; set; }
        public int TotalVisitas { get; set; }
        public DateTime? UltimaVisita { get; set; }
        public string? UltimaRecomendacion { get; set; }
        public List<HistorialServicioItemDTO> Ordenes { get; set; } = new();
    }

    /// <summary>
    /// Una orden de servicio dentro del historial, con sus servicios realizados.
    /// </summary>
    public class HistorialServicioItemDTO
    {
        public int OsId { get; set; }
        public string NumeroOs { get; set; } = null!;
        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string EstadoNombre { get; set; } = null!;
        public string EstadoCodigo { get; set; } = null!;
        public int KilometrajeIngreso { get; set; }
        public string? TipoIngreso { get; set; }
        public bool EsGarantia { get; set; }
        public decimal TotalGeneral { get; set; }
        public string? ProximaRevision { get; set; }
        public string? ObservacionesCierre { get; set; }
        public string? SucursalNombre { get; set; }
        public List<HistorialServicioLineaDTO> Servicios { get; set; } = new();
    }

    /// <summary>
    /// Línea de servicio realizado dentro de una OS.
    /// </summary>
    public class HistorialServicioLineaDTO
    {
        public string TipoServicioNombre { get; set; } = null!;
        public string DescripcionTrabajo { get; set; } = null!;
        public string EstadoNombre { get; set; } = null!;
        public decimal Subtotal { get; set; }
        public string? TecnicoNombre { get; set; }
    }
}
