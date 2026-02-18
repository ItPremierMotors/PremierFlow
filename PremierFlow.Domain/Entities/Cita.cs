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

        public TipoIngreso TipoIngreso { get; set; } = TipoIngreso.Cita;

        public string MotivoVisita { get; set; } = null!;

        public string? Observaciones { get; set; }

        public string? MotivoCancelacion { get; set; }

        /// <summary>
        /// Número de pre-orden generado.
        /// </summary>
        public string? PreOrdenId { get; set; }
        public int? SucursalId { get; set; }

        // Transferencia
        public int? MinutosTrabajados { get; set; }
        public int? CitaOrigenId { get; set; }

        // Navegación
        public virtual Cliente Cliente { get; set; } = null!;
        public virtual Vehiculo Vehiculo { get; set; } = null!;
        public virtual TipoServicio TipoServicio { get; set; } = null!;
        public virtual OrdenServicio? OrdenServicio { get; set; }
        public virtual Sucursal? Sucursal { get; set; }
        public virtual Cita? CitaOrigen { get; set; }

        // Métodos de dominio
        public bool EsTransferencia => CitaOrigenId.HasValue;
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

        public void Transferir(int minutosTrabajados)
        {
            if (Estado != EstadoCita.EnProceso)
                throw new InvalidOperationException("Solo se pueden transferir citas en proceso.");
            if (minutosTrabajados <= 0)
                throw new InvalidOperationException("Los minutos trabajados deben ser mayor a 0.");

            MinutosTrabajados = minutosTrabajados;
            Estado = EstadoCita.Completada;
        }
    }

}
