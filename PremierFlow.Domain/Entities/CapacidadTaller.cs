using PremierFlow.Domain.Common;
using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Domain.Entities
{
    /// <summary>
    /// Gestiona la capacidad diaria del taller.
    /// </summary>
    public class CapacidadTaller : AuditableEntity
    {
        public int CapacidadId { get; set; }

        public DateTime Fecha { get; set; }

        public TurnoTaller Turno { get; set; } = TurnoTaller.Completo;

        public int TecnicosDisponibles { get; set; } = 0;

        public int BahiasDisponibles { get; set; } = 0;

        /// <summary>
        /// Capacidad total en minutos.
        /// </summary>
        public int MinutosDisponibles { get; set; } = 0;

        /// <summary>
        /// Minutos ya agendados.
        /// </summary>
        public int MinutosReservados { get; set; } = 0;

        /// <summary>
        /// Minutos realmente utilizados.
        /// </summary>
        public int MinutosUtilizados { get; set; } = 0;

        public bool PermiteAgendamiento { get; set; } = true;

        public string? Observaciones { get; set; }

        // Navegación
        public virtual ICollection<BloqueHorario> BloquesHorario { get; set; } = new List<BloqueHorario>();

        // Métodos de dominio
        public int MinutosLibres => MinutosDisponibles - MinutosReservados;

        public decimal PorcentajeOcupacion => MinutosDisponibles > 0
            ? (decimal)MinutosReservados / MinutosDisponibles * 100
            : 0;

        public bool TieneCapacidadPara(int minutosRequeridos)
            => PermiteAgendamiento && MinutosLibres >= minutosRequeridos;

        public void ReservarMinutos(int minutos)
        {
            if (!TieneCapacidadPara(minutos))
                throw new InvalidOperationException("No hay capacidad disponible");

            MinutosReservados += minutos;
        }

        public void LiberarMinutos(int minutos)
        {
            MinutosReservados = Math.Max(0, MinutosReservados - minutos);
        }
    }
}