using PremierFlow.Domain.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PremierFlow.Domain.Entities
{
    /// <summary>
    /// Catálogo de marcas de vehículos.
    /// </summary>
    public class Marca : SoftDeletableEntity
    {
        public int MarcaId { get; set; }

        /// <summary>
        /// Código único de la marca (ej: TOYOTA, HONDA).
        /// </summary>
        public string Codigo { get; set; } = null!;

        public string Nombre { get; set; } = null!;

        public string? PaisOrigen { get; set; }

        /// <summary>
        /// Indica si la concesionaria importa/distribuye esta marca.
        /// </summary>
        public bool EsMarcaPropia { get; set; } = false;

        public string? LogoUrl { get; set; }

        public string? Observaciones { get; set; }

        // Navegación
        public virtual ICollection<Modelo> Modelos { get; set; } = new List<Modelo>();
        public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
    }
}