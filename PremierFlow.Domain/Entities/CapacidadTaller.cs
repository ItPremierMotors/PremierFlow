using PremierFlow.Domain.Common;
using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Domain.Entities
{
    public class CapacidadTaller : SoftDeletableEntity
    {
        public int CapacidadId { get; set; }
        public DateTime Fecha { get; set; }
        public TurnoTaller Turno { get; set; } = TurnoTaller.Completo;
        public int TecnicosDisponibles { get; set; } = 0;
        public int BahiasDisponibles { get; set; } = 0;

        /// <summary>
        /// Minutos de jornada laboral por técnico (default 480 = 8 horas).
        /// Configurable para medios días (240), turnos especiales, etc.
        /// </summary>
        public int MinutosPorTecnico { get; set; } = 480;

        // Planificación
        /// <summary>
        /// Capacidad total del día = TecnicosDisponibles × MinutosPorTecnico.
        /// Se calcula automáticamente al crear/actualizar.
        /// </summary>
        public int MinutosDisponibles { get; set; } = 0;
        public int MinutosReservados { get; set; } = 0;

        // Ejecución real
        public int MinutosUtilizados { get; set; } = 0;
        public int MinutosSobretiempo { get; set; } = 0;        // ← NUEVO
        public bool PermiteSobretiempo { get; set; } = true;    // ← NUEVO

        public bool PermiteAgendamiento { get; set; } = true;
        public string? Observaciones { get; set; }
        public int? SucursalId { get; set; }

        // Navegación
        public virtual Sucursal? Sucursal { get; set; }
        public virtual ICollection<BloqueHorario> BloquesHorario { get; set; } = new List<BloqueHorario>();

        // Métodos de dominio - PLANIFICACIÓN
        public int MinutosLibres => MinutosDisponibles - MinutosReservados;

        public decimal PorcentajeOcupacion => MinutosDisponibles > 0
            ? (decimal)MinutosReservados / MinutosDisponibles * 100
            : 0;

        public bool TieneCapacidadPara(int minutosRequeridos)
            => PermiteAgendamiento && MinutosLibres >= minutosRequeridos;

        public void ReservarMinutos(int minutos)
        {
            if (!TieneCapacidadPara(minutos))
                throw new InvalidOperationException("No hay capacidad disponible para agendar.");
            MinutosReservados += minutos;
        }

        public void LiberarMinutos(int minutos)
        {
            MinutosReservados = Math.Max(0, MinutosReservados - minutos);
        }

        // Métodos de dominio - EJECUCIÓN REAL
        public void RegistrarTiempoTrabajado(int minutos)
        {
            MinutosUtilizados += minutos;

            if (MinutosUtilizados > MinutosDisponibles)
            {
                MinutosSobretiempo = MinutosUtilizados - MinutosDisponibles;
            }
        }

        public decimal PorcentajeEficiencia => MinutosReservados > 0
            ? (decimal)MinutosUtilizados / MinutosReservados * 100
            : 0;

        public bool TuvoSobretiempo => MinutosSobretiempo > 0;

        /// <summary>
        /// Recalcula MinutosDisponibles basado en técnicos × minutos por técnico.
        /// </summary>
        public void RecalcularCapacidad()
        {
            MinutosDisponibles = TecnicosDisponibles * MinutosPorTecnico;
        }
    }
}