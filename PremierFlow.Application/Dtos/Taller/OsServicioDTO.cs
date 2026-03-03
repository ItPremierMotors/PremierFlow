using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Dtos.Taller
{
    public class OsServicioDTO
    {
        public int OsServicioId { get; set; }
        public int OsId { get; set; }
        public int TipoServicioId { get; set; }
        public string DescripcionTrabajo { get; set; } = null!;
        public EstadoServicioOS Estado { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? TecnicoAsignadoId { get; set; }
        public string? Observaciones { get; set; }

        // Extras para UI
        public string NumeroOs { get; set; } = null!;
        public string TipoServicioNombre { get; set; } = null!;
        public string? TipoServicioCodigo { get; set; }
        public string? TecnicoNombre { get; set; }
        public string EstadoNombre => Estado.ToString();
        public bool EstaCompletado { get; set; }
        public bool EstaCancelado { get; set; }
        public bool EstaEnProceso { get; set; }
        public int? TiempoTrabajoMinutos { get; set; }
        public int CantidadAsignaciones { get; set; }
        public bool TieneAsignacionActiva { get; set; }
    }
    public class AgregarServicioDTO
    {
        public int OsId { get; set; }
        public int TipoServicioId { get; set; }
        public string? DescripcionTrabajo { get; set; }
        public decimal? PrecioUnitario { get; set; }  // Si es null, usa el precio del TipoServicio
        public int Cantidad { get; set; } = 1;
        public int? TecnicoAsignadoId { get; set; }
        public string? Observaciones { get; set; }
    }
    public class UpdateOsServicioDTO
    {
        public int OsServicioId { get; set; }
        public string DescripcionTrabajo { get; set; } = null!;
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public int? TecnicoAsignadoId { get; set; }
        public string? Observaciones { get; set; }
    }
}
