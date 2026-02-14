using PremierFlow.Domain.Common;
using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Domain.Entities
{

    /// <summary>
    /// Detalle de servicios incluidos en cada Orden de Servicio.
    /// </summary>
    public class OsServicio : SoftDeletableEntity
    {
        public int OsServicioId { get; set; }

        public int OsId { get; set; }

        public int TipoServicioId { get; set; }

        public string DescripcionTrabajo { get; set; } = null!;

        public EstadoServicioOS Estado { get; set; } = EstadoServicioOS.Pendiente;

        public decimal PrecioUnitario { get; set; }

        public int Cantidad { get; set; } = 1;

        public decimal Subtotal { get; set; }

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public int? TecnicoAsignadoId { get; set; }

        public string? Observaciones { get; set; }

        // Navegación
        public virtual OrdenServicio OrdenServicio { get; set; } = null!;
        public virtual TipoServicio TipoServicio { get; set; } = null!;
        public virtual Tecnico? Tecnico { get; set; }
        public virtual ICollection<AsignacionTecnico> Asignaciones { get; set; } = new List<AsignacionTecnico>();

        // Métodos de dominio
        public bool EstaCompletado => Estado == EstadoServicioOS.Completado;

        public bool EstaCancelado => Estado == EstadoServicioOS.Cancelado;

        public bool EstaEnProceso => Estado == EstadoServicioOS.EnProceso;

        public TimeSpan? TiempoTrabajo => FechaInicio.HasValue && FechaFin.HasValue
            ? FechaFin.Value - FechaInicio.Value
            : null;

        public void CalcularSubtotal()
        {
            Subtotal = PrecioUnitario * Cantidad;
        }

        public void IniciarTrabajo()
        {
            if (Estado != EstadoServicioOS.Pendiente)
                throw new InvalidOperationException("Solo se puede iniciar un trabajo pendiente");

            Estado = EstadoServicioOS.EnProceso;
            FechaInicio = DateTime.UtcNow;
        }

        public void CompletarTrabajo()
        {
            if (Estado != EstadoServicioOS.EnProceso)
                throw new InvalidOperationException("Solo se puede completar un trabajo en proceso");

            Estado = EstadoServicioOS.Completado;
            FechaFin = DateTime.UtcNow;
        }

        public void Cancelar()
        {
            if (EstaCompletado)
                throw new InvalidOperationException("No se puede cancelar un trabajo completado");

            Estado = EstadoServicioOS.Cancelado;
        }
    }
}
