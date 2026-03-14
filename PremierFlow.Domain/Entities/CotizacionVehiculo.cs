using PremierFlow.Domain.Common;
using PremierFlow.Domain.Enums;


namespace PremierFlow.Domain.Entities
{
    /// <summary>
    /// Propuesta formal de precio para un vehículo dentro de una oportunidad.
    /// Los precios de lista y base se consultan desde Vehiculo y VersionVehiculo.
    /// </summary>
    public class CotizacionVehiculo : SoftDeletableEntity
    {
        public int CotizacionVehiculoId { get; set; }

        /// <summary>
        /// Código único de la cotización (COT-2026-0001).
        /// </summary>
        public string CodigoCotizacion { get; set; } = null!;

        #region Vinculación

        public int OportunidadId { get; set; }

        /// <summary>
        /// Vehículo específico del inventario que se cotiza.
        /// De aquí se jala PrecioLista y Version.PrecioBase.
        /// </summary>
        public int VehiculoId { get; set; }

        #endregion

        #region Negociación

        /// <summary>
        /// Descuento ofrecido al cliente.
        /// </summary>
        public decimal Descuento { get; set; }

        /// <summary>
        /// Precio propuesto al cliente (PrecioLista - Descuento u otro cálculo del vendedor).
        /// </summary>
        public decimal PrecioOfertado { get; set; }

        /// <summary>
        /// Condiciones: "Contado", "Financiamiento 60 meses", etc.
        /// </summary>
        public string? CondicionesPago { get; set; }

        public string? Observaciones { get; set; }

        #endregion

        #region Vigencia

        public DateTime FechaEmision { get; set; } = TimeHelper.Now;
        public DateTime FechaVencimiento { get; set; }
        public EstadoCotizacion Estado { get; set; } = EstadoCotizacion.Vigente;

        #endregion

        #region Navegación

        public virtual Oportunidad Oportunidad { get; set; } = null!;
        public virtual Vehiculo Vehiculo { get; set; } = null!;

        #endregion

        #region Métodos de Dominio

        /// <summary>
        /// Indica si la cotización ha vencido.
        /// </summary>
        public bool EstaVencida => Estado == EstadoCotizacion.Vigente &&
                                    FechaVencimiento < TimeHelper.Now;

        /// <summary>
        /// Acepta la cotización.
        /// </summary>
        public void Aceptar()
        {
            if (Estado != EstadoCotizacion.Vigente)
                throw new InvalidOperationException("Solo se pueden aceptar cotizaciones vigentes");

            Estado = EstadoCotizacion.Aceptada;
        }

        /// <summary>
        /// Rechaza la cotización.
        /// </summary>
        public void Rechazar()
        {
            if (Estado != EstadoCotizacion.Vigente)
                throw new InvalidOperationException("Solo se pueden rechazar cotizaciones vigentes");

            Estado = EstadoCotizacion.Rechazada;
        }

        #endregion
    }
}
