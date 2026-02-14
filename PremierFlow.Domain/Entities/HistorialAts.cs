using PremierFlow.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Domain.Entities
{
    /// <summary>
    /// Historial permanente de servicio por VIN (ATS - After-Sales Tracking System).
    /// </summary>
    public class HistorialAts : SoftDeletableEntity
    {
        public int AtsId { get; set; }

        public int VehiculoId { get; set; }

        public int OsId { get; set; }

        public DateTime FechaServicio { get; set; }

        public string TipoServicio { get; set; } = null!;

        public int Kilometraje { get; set; }

        public string TrabajosRealizados { get; set; } = null!;

        public decimal MontoTotal { get; set; }

        public string? ObservacionesTecnicas { get; set; }

        /// <summary>
        /// Recomendación de próximo servicio.
        /// </summary>
        public string? ProximaRevision { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Navegación
        public virtual Vehiculo Vehiculo { get; set; } = null!;
        public virtual OrdenServicio OrdenServicio { get; set; } = null!;

        // Métodos de dominio
        public bool TieneRecomendacionPendiente => !string.IsNullOrEmpty(ProximaRevision);

        /// <summary>
        /// Crea un registro de historial a partir de una OS cerrada.
        /// </summary>
        public static HistorialAts CrearDesdeOs(OrdenServicio os, string trabajosRealizados, string? proximaRevision = null)
        {
            if (os.Vehiculo == null)
                throw new ArgumentException("La OS debe tener un vehículo asociado");

            return new HistorialAts
            {
                VehiculoId = os.VehiculoId,
                OsId = os.OsId,
                FechaServicio = os.FechaCierre ?? os.FechaApertura,
                TipoServicio = string.Join(", ", os.Servicios.Select(s => s.TipoServicio?.Nombre ?? "Servicio")),
                Kilometraje = os.KilometrajeIngreso,
                TrabajosRealizados = trabajosRealizados,
                MontoTotal = os.TotalGeneral,
                ObservacionesTecnicas = os.ObservacionesCierre,
                ProximaRevision = proximaRevision
            };
        }
    }
}
