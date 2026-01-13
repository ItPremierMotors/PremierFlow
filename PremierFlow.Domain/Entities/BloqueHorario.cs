using PremierFlow.Domain.Common;
using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Domain.Entities
{

    /// <summary>
    /// Define bloques de tiempo específicos dentro de la capacidad del taller.
    /// </summary>
    public class BloqueHorario : AuditableEntity
    {
        public int BloqueId { get; set; }

        public int CapacidadId { get; set; }

        public TimeSpan HoraInicio { get; set; }

        public TimeSpan HoraFin { get; set; }

        public int CapacidadMaximaVehiculos { get; set; } = 1;

        public int VehiculosAgendados { get; set; } = 0;

        public TipoBloqueHorario TipoBloque { get; set; } = TipoBloqueHorario.Estandar;

        public bool Activo { get; set; } = true;

        // Navegación
        public virtual CapacidadTaller Capacidad { get; set; } = null!;

        // Métodos de dominio
        public int EspaciosDisponibles => CapacidadMaximaVehiculos - VehiculosAgendados;

        public bool TieneEspacioDisponible => Activo && EspaciosDisponibles > 0;

        public TimeSpan Duracion => HoraFin - HoraInicio;

        public bool ContieneHora(TimeSpan hora) => hora >= HoraInicio && hora < HoraFin;

        public void AgendarVehiculo()
        {
            if (!TieneEspacioDisponible)
                throw new InvalidOperationException("No hay espacios disponibles en este bloque");

            VehiculosAgendados++;
        }

        public void LiberarEspacio()
        {
            VehiculosAgendados = Math.Max(0, VehiculosAgendados - 1);
        }
    }

}
