using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Dtos.Taller
{
    public class CitaDTO
    {
        public int CitaId { get; set; }
        public string CodigoCita { get; set; } = null!;
        public int ClienteId { get; set; }
        public int VehiculoId { get; set; }
        public int TipoServicioId { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }
        public EstadoCita Estado { get; set; }
        public TipoIngreso TipoIngreso { get; set; }
        public string MotivoVisita { get; set; } = null!;
        public string? Observaciones { get; set; }
        public string? MotivoCancelacion { get; set; }
        public string? PreOrdenId { get; set; }
        public int? SucursalId { get; set; }
        // Extras para UI
        public string ClienteNombre { get; set; } = null!;
        public string? ClienteTelefono { get; set; }
        public string VehiculoDescripcion { get; set; } = null!;
        public string? VehiculoPlaca { get; set; }
        public string TipoServicioNombre { get; set; } = null!;
        public string? SucursalNombre { get; set; }
        public int DuracionMinutos { get; set; }
        public int? MinutosTrabajados { get; set; }
        public int? CitaOrigenId { get; set; }
        public bool EsTransferencia { get; set; }
        public string EstadoNombre => Estado.ToString();
        public bool EstaActiva { get; set; }
        public bool PuedeConvertirseEnOs { get; set; }
        public string HoraInicioFormateada => FechaHoraInicio.ToString("HH:mm");
        public string HoraFinFormateada => FechaHoraFin.ToString("HH:mm");
        public string FechaFormateada => FechaHoraInicio.ToString("dd/MM/yyyy");
    }
    
    public class CreateCitaDTO
    {
        public int ClienteId { get; set; }
        public int VehiculoId { get; set; }
        public int TipoServicioId { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public string MotivoVisita { get; set; } = null!;
        public string? Observaciones { get; set; }
        public int? SucursalId { get; set; }
        public int? BloqueHorarioId { get; set; }  // Opcional: si usa bloques
    }
    public class UpdateCitaDTO
    {
        public int CitaId { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public string MotivoVisita { get; set; } = null!;
        public string? Observaciones { get; set; }
        public int? BloqueHorarioId { get; set; }
    }
    public class CancelarCitaDTO
    {
        public int CitaId { get; set; }
        public string MotivoCancelacion { get; set; } = null!;
    }

    public class TransferirCitaDTO
    {
        public int CitaId { get; set; }
        public int MinutosTrabajadosHoy { get; set; }
    }

}
