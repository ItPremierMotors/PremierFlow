using PremierFlow.Domain.Common;
using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Domain.Entities
{
    /// <summary>
    /// Versiones/Trim específicos de cada modelo (ej: EX, LX, Sport, Limited).
    /// </summary>
    public class VersionVehiculo : SoftDeletableEntity
    {
        public int VersionId { get; set; }

        public int ModeloId { get; set; }

        /// <summary>
        /// Código único de la versión (ej: CIVIC-EX, COROLLA-XLE).
        /// </summary>
        public string Codigo { get; set; } = null!;

        /// <summary>
        /// Nombre comercial (EX, LX, Touring, Sport, Limited, etc).
        /// </summary>
        public string Nombre { get; set; } = null!;

        /// <summary>
        /// Especificación del motor (ej: 2.0L Turbo, 1.5L Hybrid).
        /// </summary>
        public string? Motor { get; set; }

        /// <summary>
        /// Tipo de transmisión.
        /// </summary>
        public TipoTransmision? Transmision { get; set; }

        public TipoTraccion? Traccion { get; set; }

        public int? NumPuertas { get; set; }

        public int? NumPasajeros { get; set; }

        public TipoCombustible TipoCombustible { get; set; } = TipoCombustible.Gasolina;

        /// <summary>
        /// Cilindraje en litros.
        /// </summary>
        public decimal? Cilindraje { get; set; }

        /// <summary>
        /// Potencia en caballos de fuerza.
        /// </summary>
        public int? PotenciaHp { get; set; }

        /// <summary>
        /// Torque en Newton-metros.
        /// </summary>
        public int? TorqueNm { get; set; }

        public decimal? PrecioBase { get; set; }

        public int? AnioVersion { get; set; }

        /// <summary>
        /// Características principales (puede ser JSON).
        /// </summary>
        public string? CaracteristicasPrincipales { get; set; }

        // Navegación
        public virtual Modelo Modelo { get; set; } = null!;
        public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();

        // Métodos de dominio
        public string DescripcionCompleta => $"{Nombre} - {Motor ?? "N/A"} {Transmision?.ToString() ?? ""}".Trim();
    }
}