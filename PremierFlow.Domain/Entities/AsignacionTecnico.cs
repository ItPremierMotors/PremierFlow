using PremierFlow.Domain.Common;
using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Domain.Entities
{
    /// <summary>
    /// Registra las asignaciones de técnicos a Órdenes de Servicio.
    /// </summary>
    public class AsignacionTecnico : AuditableEntity
    {
        public int AsignacionId { get; set; }

        public int OsId { get; set; }

        public int TecnicoId { get; set; }

        public int? OsServicioId { get; set; }

        public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public EstadoAsignacion Estado { get; set; } = EstadoAsignacion.Asignado;

        public string? Observaciones { get; set; }

        // Navegación
        public virtual OrdenServicio OrdenServicio { get; set; } = null!;
        public virtual Tecnico Tecnico { get; set; } = null!;
        public virtual OsServicio? OsServicio { get; set; }

        // Métodos de dominio
        public bool EstaActiva => Estado == EstadoAsignacion.Asignado || Estado == EstadoAsignacion.EnProceso;

        public TimeSpan? TiempoTrabajado => FechaInicio.HasValue && FechaFin.HasValue
            ? FechaFin.Value - FechaInicio.Value
            : FechaInicio.HasValue
                ? DateTime.UtcNow - FechaInicio.Value
                : null;

        public void IniciarTrabajo()
        {
            if (Estado != EstadoAsignacion.Asignado)
                throw new InvalidOperationException("Solo se puede iniciar una asignación pendiente");

            Estado = EstadoAsignacion.EnProceso;
            FechaInicio = DateTime.UtcNow;
        }

        public void Pausar()
        {
            if (Estado != EstadoAsignacion.EnProceso)
                throw new InvalidOperationException("Solo se puede pausar un trabajo en proceso");

            Estado = EstadoAsignacion.Pausado;
        }

        public void Reanudar()
        {
            if (Estado != EstadoAsignacion.Pausado)
                throw new InvalidOperationException("Solo se puede reanudar un trabajo pausado");

            Estado = EstadoAsignacion.EnProceso;
        }

        public void Completar()
        {
            if (Estado != EstadoAsignacion.EnProceso)
                throw new InvalidOperationException("Solo se puede completar un trabajo en proceso");

            Estado = EstadoAsignacion.Completado;
            FechaFin = DateTime.UtcNow;
        }
    }

}
