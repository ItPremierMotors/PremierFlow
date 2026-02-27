using PremierFlow.Domain.Common;
using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;

namespace PremierFlow.Domain.Entities
{// <summary>
    /// Documento operativo principal del taller (Orden de Servicio).
    /// </summary>
    public class OrdenServicio : SoftDeletableEntity
    {
        public int OsId { get; set; }

        /// <summary>
        /// Número visible de la OS.
        /// </summary>
        public string NumeroOs { get; set; } = null!;

        /// <summary>
        /// NULL para walk-in.
        /// </summary>
        public int? CitaId { get; set; }

        public int VehiculoId { get; set; }

        public int ClienteId { get; set; }

        public DateTime FechaApertura { get; set; } = DateTime.UtcNow;

        public DateTime? FechaCierre { get; set; }

        public int EstadoId { get; set; }

        public int KilometrajeIngreso { get; set; }

        /// <summary>
        /// Nivel de combustible de 0.00 a 1.00 (0% a 100%).
        /// </summary>
        public decimal? NivelCombustible { get; set; }

        public TipoIngreso TipoIngreso { get; set; }

        public bool EsGarantia { get; set; } = false;

        public string? ObservacionesApertura { get; set; }

        public string? ObservacionesCierre { get; set; }

        /// <summary>
        /// FK a ApplicationUser - Asesor de servicio responsable.
        /// </summary>
        public string? AsesorId { get; set; }

        /// <summary>
        /// FK a ApplicationUser - Coordinador de taller.
        /// </summary>
        public string? CoordinadorId { get; set; }
        public decimal TotalManoObra { get; set; } = 0;

        public decimal TotalRepuestos { get; set; } = 0;

        public decimal TotalGeneral { get; set; } = 0;
        public int? SucursalId { get; set; }

        /// <summary>
        /// Recomendación de próxima revisión (se establece al cerrar la OS).
        /// </summary>
        public string? ProximaRevision { get; set; }

        // Navegación
        public virtual Cita? Cita { get; set; }
        public virtual Vehiculo Vehiculo { get; set; } = null!;
        public virtual Cliente Cliente { get; set; } = null!;
        public virtual EstadoOs Estado { get; set; } = null!;
        public virtual Sucursal? Sucursal { get; set; }
        public virtual Recepcion? Recepcion { get; set; }
        public virtual ICollection<OsServicio> Servicios { get; set; } = new List<OsServicio>();
        public virtual ICollection<Evidencia> Evidencias { get; set; } = new List<Evidencia>();
        public virtual ICollection<AsignacionTecnico> AsignacionesTecnico { get; set; } = new List<AsignacionTecnico>();

        // Métodos de dominio
        public bool EsWalkIn => TipoIngreso == TipoIngreso.WalkIn;

        public bool EstaAbierta => Estado?.Codigo != EstadoOs.Estados.Cerrada &&
                                   Estado?.Codigo != EstadoOs.Estados.Cancelada;

        public bool PuedeModificarse => EstaAbierta && Estado?.Codigo != EstadoOs.Estados.Facturada;

        public int? NivelCombustiblePorcentaje => NivelCombustible.HasValue
            ? (int)(NivelCombustible.Value * 100)
            : null;

        public void CalcularTotales()
        {
            TotalManoObra = Servicios.Where(s => s.Estado != EstadoServicioOS.Cancelado)
                                     .Sum(s => s.Subtotal);
            // TotalRepuestos se calcularía desde otra tabla de repuestos
            TotalGeneral = TotalManoObra + TotalRepuestos;
        }

        public void Cerrar(string? observaciones = null)
        {
            if (!EstaAbierta)
                throw new InvalidOperationException("La OS ya está cerrada o cancelada");

            FechaCierre = DateTime.UtcNow;
            ObservacionesCierre = observaciones;
        }
    }

}
