using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Dtos.Taller
{
    public class BloqueHorarioDTO
    {

        public int BloqueId { get; set; }
        public int CapacidadId { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public int CapacidadMaximaVehiculos { get; set; }
        public int VehiculosAgendados { get; set; }
        public TipoBloqueHorario TipoBloque { get; set; }

        // Extras para UI
        public int EspaciosDisponibles { get; set; }
        public bool TieneEspacioDisponible { get; set; }
        public int DuracionMinutos { get; set; }
        public string HoraInicioFormateada => HoraInicio.ToString(@"hh\:mm");
        public string HoraFinFormateada => HoraFin.ToString(@"hh\:mm");
        public string RangoHorario => $"{HoraInicioFormateada} - {HoraFinFormateada}";
        public string TipoBloqueNombre => TipoBloque.ToString();

        // Datos de CapacidadTaller
        public DateTime? Fecha { get; set; }
        public string? SucursalNombre { get; set; }
    }
    public class CreateBloqueHorarioDTO
    {
        public int CapacidadId { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public int CapacidadMaximaVehiculos { get; set; } = 1;
        public TipoBloqueHorario TipoBloque { get; set; } = TipoBloqueHorario.Estandar;
    }
    public class UpdateBloqueHorarioDTO
    {
        public int BloqueId { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public int CapacidadMaximaVehiculos { get; set; }
        public TipoBloqueHorario TipoBloque { get; set; }
    }
    public class GenerarBloquesDTO
    {
        public int CapacidadId { get; set; }
        public TimeSpan HoraInicioJornada { get; set; } = new TimeSpan(8, 0, 0);   // 08:00
        public TimeSpan HoraFinJornada { get; set; } = new TimeSpan(17, 0, 0);     // 17:00
        public int DuracionBloqueMinutos { get; set; } = 60;                        // 1 hora
        public int CapacidadPorBloque { get; set; } = 2;                            // 2 vehículos por bloque
        public TipoBloqueHorario TipoBloque { get; set; } = TipoBloqueHorario.Estandar;
    }
}
