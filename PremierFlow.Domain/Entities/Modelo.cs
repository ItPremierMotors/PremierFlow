using PremierFlow.Domain.Common;
using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Domain.Entities
{
    /// <summary>
    /// Catálogo de modelos de vehículos por marca.
    /// </summary>
    public class Modelo : SoftDeletableEntity
    {
        public int ModeloId { get; set; }

        public int MarcaId { get; set; }

        /// <summary>
        /// Código único del modelo (ej: CIVIC, COROLLA).
        /// </summary>
        public string Codigo { get; set; } = null!;

        public string Nombre { get; set; } = null!;

        public SegmentoVehiculo Segmento { get; set; } = SegmentoVehiculo.Sedan;

        /// <summary>
        /// Año en que comenzó la producción.
        /// </summary>
        public int? AnioInicio { get; set; }

        /// <summary>
        /// Año en que finalizó la producción (NULL si aún se produce).
        /// </summary>
        public int? AnioFin { get; set; }

        public string? Descripcion { get; set; }

        public string? ImagenUrl { get; set; }

        // Navegación
        public virtual Marca Marca { get; set; } = null!;
        public virtual ICollection<VersionVehiculo> VersionVehiculos { get; set; } = new List<VersionVehiculo>();
        public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();

        // Métodos de dominio
        public bool EstaEnProduccion => AnioFin == null;
    }

}
