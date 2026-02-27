using PremierFlow.Domain.Common;
using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Domain.Entities
{
    // <summary>
    /// Representa una cita agendada (Pre-Orden).
    /// </summary>
    public class Cita : SoftDeletableEntity
    {
        public int CitaId { get; set; }

        /// <summary>
        /// Código visible al cliente.
        /// </summary>
        public string CodigoCita { get; set; } = null!;

        public int ClienteId { get; set; }

        public int VehiculoId { get; set; }

        public int TipoServicioId { get; set; }

        public DateTime FechaHoraInicio { get; set; }

        public DateTime FechaHoraFin { get; set; }

        public EstadoCita Estado { get; set; } = EstadoCita.Agendada;

        public string MotivoVisita { get; set; } = null!;

        public string? Observaciones { get; set; }

        public string? MotivoCancelacion { get; set; }

        /// <summary>
        /// Número de pre-orden generado.
        /// </summary>
        public string? PreOrdenId { get; set; }
        public int? SucursalId { get; set; }

        /// <summary>
        /// Fecha original de recepción. Se establece al agendar y NUNCA cambia,
        /// incluso si la cita es transferida a otro día.
        /// </summary>
        public DateTime FechaRecepcion { get; set; }

        // Acumulado de minutos trabajados (se suma en cada transferencia)
        public int? MinutosTrabajados { get; set; }

        // Relación con Capacidad/Bloque
        public int? CapacidadId { get; set; }
        public int? BloqueHorarioId { get; set; }

        // Navegación
        public virtual Cliente Cliente { get; set; } = null!;
        public virtual Vehiculo Vehiculo { get; set; } = null!;
        public virtual TipoServicio TipoServicio { get; set; } = null!;
        public virtual OrdenServicio? OrdenServicio { get; set; }
        public virtual Sucursal? Sucursal { get; set; }
        public virtual CapacidadTaller? Capacidad { get; set; }
        public virtual BloqueHorario? BloqueHorario { get; set; }

        // Métodos de dominio
        public TimeSpan Duracion => FechaHoraFin - FechaHoraInicio;

        public bool EstaActiva => Estado == EstadoCita.Agendada || Estado == EstadoCita.Confirmada;

        public bool PuedeConvertirseEnOs => Estado == EstadoCita.Agendada || Estado == EstadoCita.Confirmada;

        public void Confirmar()
        {
            if (Estado != EstadoCita.Agendada)
                throw new InvalidOperationException("Solo se pueden confirmar citas agendadas");

            Estado = EstadoCita.Confirmada;
        }

        public void Cancelar(string motivo)
        {
            if (!EstaActiva)
                throw new InvalidOperationException("Solo se pueden cancelar citas activas");

            Estado = EstadoCita.Cancelada;
            MotivoCancelacion = motivo;
        }

        public void MarcarNoShow()
        {
            if (!EstaActiva)
                throw new InvalidOperationException("Solo se pueden marcar no-show citas activas");

            Estado = EstadoCita.NoShow;
        }

        public void IniciarProceso()
        {
            if (!PuedeConvertirseEnOs)
                throw new InvalidOperationException("La cita no está en estado válido para iniciar proceso");

            Estado = EstadoCita.EnProceso;
        }

        public void Completar()
        {
            if (Estado != EstadoCita.EnProceso)
                throw new InvalidOperationException("Solo se pueden completar citas en proceso");
            Estado = EstadoCita.Completada;
        }

    }

}
