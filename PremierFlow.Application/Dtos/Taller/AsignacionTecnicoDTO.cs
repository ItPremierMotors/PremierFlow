using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Dtos.Taller
{
    public class AsignacionTecnicoDTO
    {
        public int AsignacionId { get; set; }
        public int OsId { get; set; }
        public int TecnicoId { get; set; }
        public int? OsServicioId { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public EstadoAsignacion Estado { get; set; }
        public string? Observaciones { get; set; }

        // Extras para UI
        public string NumeroOs { get; set; } = null!;
        public string TecnicoNombre { get; set; } = null!;
        public string? TecnicoCodigo { get; set; }
        public string? ServicioDescripcion { get; set; }
        public string EstadoNombre => Estado.ToString();
        public bool EstaActiva { get; set; }
        public int? TiempoTrabajadoMinutos { get; set; }
    }
    public class CreateAsignacionDTO
    {
        public int OsId { get; set; }
        public int TecnicoId { get; set; }
        public int? OsServicioId { get; set; }
        public string? Observaciones { get; set; }
    }
    public class ReasignarDTO
    {
        public int AsignacionId { get; set; }
        public int NuevoTecnicoId { get; set; }
        public string? Observaciones { get; set; }
    }
}
