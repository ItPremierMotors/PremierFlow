using PremierFlow.Domain.Common;
using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Domain.Entities
{

    /// <summary>
    /// Representa un vehículo individual identificado por VIN.
    /// </summary>
    public class Vehiculo : SoftDeletableEntity
    {
        #region Identificación
        public int VehiculoId { get; private set; }
        /// <summary>
        /// VIN - Vehicle Identification Number (17 caracteres, único).
        /// </summary>
        public string Vin { get; set; } = null!;

        /// <summary>
        /// Placa del vehículo (se asigna después de matricular).
        /// </summary>
        public string? Placa { get; set; }

        public string? NumeroMotor { get; set; }
        public string? NumeroChasis { get; set; }
        #endregion

        #region Catálogo (Marca/Modelo/Versión)

        public int MarcaId { get; set; }
        public int ModeloId { get; set; }
        public int? VersionId { get; set; }
        public int Anio { get; set; }
        public string? Color { get; set; }
        public TipoCombustible TipoCombustible { get; set; } = TipoCombustible.Gasolina;  // ← Agregado
        public TipoTransmision? Transmision { get; set; }  // ← Agregado

        #endregion

        #region Estado y Ubicación

        /// <summary>
        /// Estado actual en el ciclo de vida (EnTransito, EnBodega, Vendido, etc.).
        /// </summary>
        public EstadoVehiculo Estado { get; set; } = EstadoVehiculo.EnTransito;

        /// <summary>
        /// Ubicación física actual del vehículo.
        /// </summary>
        public int? UbicacionId { get; set; }

        /// <summary>
        /// Sucursal responsable del vehículo (para filtros y permisos).
        /// </summary>
        public int? SucursalId { get; set; }

        #endregion

        #region Operaciones (Importación)

        public ProcedenciaVehiculo Procedencia { get; set; } = ProcedenciaVehiculo.Importado;

        /// <summary>
        /// Número de documento de importación.
        /// </summary>
        public string? NumeroImportacion { get; set; }

        /// <summary>
        /// Número de póliza de importación.
        /// </summary>
        public string? NumeroPoliza { get; set; }

        /// <summary>
        /// Fecha de llegada al país.
        /// </summary>
        public DateTime? FechaIngresoPais { get; set; }

        /// <summary>
        /// Fecha de recepción en bodega Premier.
        /// </summary>
        public DateTime? FechaRecepcion { get; set; }

        /// <summary>
        /// Costo total de importación (CIF + impuestos + gastos).
        /// </summary>
        public decimal? CostoImportacion { get; set; }

        #endregion
        #region Ventas

        /// <summary>
        /// Cliente propietario (NULL si aún no se ha vendido).
        /// </summary>
        public int? ClienteId { get; set; }

        /// <summary>
        /// Precio de lista para venta.
        /// </summary>
        public decimal? PrecioLista { get; set; }

        /// <summary>
        /// Precio final de venta (puede incluir descuentos).
        /// </summary>
        public decimal? PrecioVenta { get; set; }

        /// <summary>
        /// Fecha de facturación/venta.
        /// </summary>
        public DateTime? FechaVenta { get; set; }

        /// <summary>
        /// Fecha de entrega al cliente.
        /// </summary>
        public DateTime? FechaEntrega { get; set; }

        /// <summary>
        /// Vendedor que cerró la venta.
        /// </summary>
        public string? VendedorId { get; set; }

        #endregion

        #region Reserva

        /// <summary>
        /// Usuario que realizó la reserva.
        /// </summary>
        public string? ReservadoPorId { get; set; }

        /// <summary>
        /// Fecha en que se realizó la reserva.
        /// </summary>
        public DateTime? FechaReserva { get; set; }

        /// <summary>
        /// Fecha límite para concretar la venta (10 días desde reserva).
        /// </summary>
        public DateTime? FechaLimiteReserva { get; set; }

        #endregion

        #region Taller / Postventa

        /// <summary>
        /// Kilometraje actual (se actualiza en cada servicio).
        /// </summary>
        public int KilometrajeActual { get; set; } = 0;

        /// <summary>
        /// Fecha de primera matrícula.
        /// </summary>
        public DateTime? FechaPrimeraMatricula { get; set; }

        /// <summary>
        /// Fecha de vencimiento de garantía.
        /// </summary>
        public DateTime? GarantiaHasta { get; set; }

        #endregion

        #region General

        public string? Observaciones { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        #endregion

        #region Navegación

        public virtual Marca Marca { get; set; } = null!;
        public virtual Modelo Modelo { get; set; } = null!;
        public virtual VersionVehiculo? Version { get; set; }
        public virtual Cliente? Cliente { get; set; }
        public virtual Ubicacion? Ubicacion { get; set; }
        public virtual Sucursal? Sucursal { get; set; }

        // Relaciones con Taller
        public virtual ICollection<Cita> Citas { get; set; } = new List<Cita>();
        public virtual ICollection<OrdenServicio> OrdenesServicio { get; set; } = new List<OrdenServicio>();
        public virtual ICollection<HistorialAts> HistorialAts { get; set; } = new List<HistorialAts>();

        #endregion

        #region Métodos de Dominio

        /// <summary>
        /// Identificador principal (Placa o VIN).
        /// </summary>
        public string Identificador => !string.IsNullOrEmpty(Placa) ? Placa : Vin;  // ← Agregado
        /// <summary>
        /// Descripción completa del vehículo para mostrar en UI.
        /// </summary>
        public string DescripcionCompleta => $"{Marca?.Nombre} {Modelo?.Nombre} {Version?.Nombre} {Anio}".Trim();

        /// <summary>
        /// Indica si el vehículo ya fue vendido.
        /// </summary>
        public bool EstaVendido => Estado == EstadoVehiculo.Vendido ||
                                   Estado == EstadoVehiculo.Entregado;

        /// <summary>
        /// Indica si el vehículo está disponible para venta.
        /// </summary>
        public bool DisponibleParaVenta => Estado == EstadoVehiculo.EnBodega ||
                                            Estado == EstadoVehiculo.EnExhibicion;

        /// <summary>
        /// Indica si está en garantía vigente.
        /// </summary>
        public bool EnGarantia => GarantiaHasta.HasValue && GarantiaHasta.Value > DateTime.UtcNow;

        /// <summary>
        /// Indica si la reserva ha expirado (pasó la fecha límite).
        /// </summary>
        public bool ReservaExpirada => Estado == EstadoVehiculo.Reservado &&
                                        FechaLimiteReserva.HasValue &&
                                        FechaLimiteReserva.Value < DateTime.UtcNow;



        /// <summary>
        /// Actualiza el kilometraje si el nuevo valor es mayor.
        /// </summary>
        public void ActualizarKilometraje(int nuevoKilometraje)
        {
            if (nuevoKilometraje > KilometrajeActual)
                KilometrajeActual = nuevoKilometraje;
        }

        /// <summary>
        /// Verifica si tiene una OS activa.
        /// </summary>
        //public bool TieneOsActiva(IEnumerable<OrdenServicio> ordenes)
        //{
        //    return ordenes.Any(os =>
        //        os.VehiculoId == VehiculoId &&
        //        os.Estado?.Codigo != EstadoOs.Estados.Cerrada &&
        //        os.Estado?.Codigo != EstadoOs.Estados.Cancelada);
        //}

        /// <summary>
        /// Marca el vehículo como vendido.
        /// </summary>
        public void MarcarComoVendido(int clienteId, decimal precioVenta, string vendedorId)
        {
            if (!DisponibleParaVenta)
                throw new InvalidOperationException("El vehículo no está disponible para venta");

            ClienteId = clienteId;
            PrecioVenta = precioVenta;
            VendedorId = vendedorId;
            FechaVenta = DateTime.UtcNow;
            Estado = EstadoVehiculo.Vendido;
        }

        /// <summary>
        /// Marca el vehículo como entregado al cliente.
        /// </summary>
        public void MarcarComoEntregado()
        {
            if (Estado != EstadoVehiculo.Vendido)
                throw new InvalidOperationException("El vehículo debe estar vendido para ser entregado");

            FechaEntrega = DateTime.UtcNow;
            Estado = EstadoVehiculo.Entregado;
        }

        #endregion

    }

}
