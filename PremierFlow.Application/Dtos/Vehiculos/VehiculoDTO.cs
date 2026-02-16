using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Dtos.Vehiculos
{
    public class VehiculoDTO
    {
            public int VehiculoId { get; set; }
            public string Vin { get; set; } = null!;
            public string? Placa { get; set; }
            public int MarcaId { get; set; }
            public int ModeloId { get; set; }
            public int? VersionId { get; set; }
           #region Estado y Ubicación
            public EstadoVehiculo Estado { get; set; }
            public int? UbicacionId { get; set; }
            public int? SucursalId { get; set; }
         #endregion
            public int Anio { get; set; }
            public string? Color { get; set; }
            public TipoCombustible TipoCombustible { get; set; }
            public TipoTransmision? Transmision { get; set; }
           
            public int? ClienteId { get; set; }
            public decimal? PrecioLista { get; set; }
            public int KilometrajeActual { get; set; }
            public DateTime? GarantiaHasta { get; set; }

            // Extras para UI
            public string? UbicacionNombre { get; set; }
            public string? SucursalNombre { get; set; }
            public string? ClienteNombre { get; set; }
            public string MarcaNombre { get; set; } = null!;
            public string ModeloNombre { get; set; } = null!;
            public string? VersionNombre { get; set; }
            public string Identificador { get; set; } = null!;
            public string DescripcionCompleta { get; set; } = null!;
            public bool EnGarantia { get; set; }
            public bool EstaVendido { get; set; }
            public bool DisponibleParaVenta { get; set; }
            public DateTime? FechaLimiteReserva { get; set; }
    }

    public class VehiculoDetalleDTO
    {
        #region Identificación
        public int VehiculoId { get; set; }
        public string Vin { get; set; } = null!;
        public string? Placa { get; set; }
        public string? NumeroMotor { get; set; }
        public string? NumeroChasis { get; set; }
        #endregion

        #region Catálogo
        public int MarcaId { get; set; }
        public int ModeloId { get; set; }
        public int? VersionId { get; set; }
        public int Anio { get; set; }
        public string? Color { get; set; }
        public TipoCombustible TipoCombustible { get; set; }
        public TipoTransmision? Transmision { get; set; }
        #endregion

        #region Estado y Ubicación
        public EstadoVehiculo Estado { get; set; }
        public int? UbicacionId { get; set; }
        public int? SucursalId { get; set; }
        #endregion

        #region Operaciones
        public ProcedenciaVehiculo Procedencia { get; set; }
        public string? NumeroImportacion { get; set; }
        public string? NumeroPoliza { get; set; }
        public DateTime? FechaIngresoPais { get; set; }
        public DateTime? FechaRecepcion { get; set; }
        public decimal? CostoImportacion { get; set; }
        #endregion

        #region Ventas
        public int? ClienteId { get; set; }
        public decimal? PrecioLista { get; set; }
        public decimal? PrecioVenta { get; set; }
        public DateTime? FechaVenta { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string? VendedorId { get; set; }
        #endregion

        #region Taller
        public int KilometrajeActual { get; set; }
        public DateTime? FechaPrimeraMatricula { get; set; }
        public DateTime? GarantiaHasta { get; set; }
        #endregion

        #region General
        public string? Observaciones { get; set; }
        public DateTime FechaRegistro { get; set; }
        #endregion

        #region Extras para UI
        public string? ClienteNombre { get; set; }
        public string MarcaNombre { get; set; } = null!;
        public string ModeloNombre { get; set; } = null!;
        public string? VersionNombre { get; set; }
        public string? UbicacionNombre { get; set; }
        public string? SucursalNombre { get; set; }
        public string Identificador { get; set; } = null!;
        public string DescripcionCompleta { get; set; } = null!;
        public bool EnGarantia { get; set; }
        public bool EstaVendido { get; set; }
        public bool DisponibleParaVenta { get; set; }
        public int CantidadServicios { get; set; }
        public string? ReservadoPorId { get; set; }
        public string? ReservadoPorNombre { get; set; }
        public DateTime? FechaReserva { get; set; }
        public DateTime? FechaLimiteReserva { get; set; }
        #endregion
    }
    public class CreateVehiculoDTO
    {
        #region Identificación
        public string Vin { get; set; } = null!;
        public string? Placa { get; set; }
        public string? NumeroMotor { get; set; }
        public string? NumeroChasis { get; set; }
        #endregion

        #region Catálogo
        public int MarcaId { get; set; }
        public int ModeloId { get; set; }
        public int? VersionId { get; set; }
        public int Anio { get; set; }
        public string? Color { get; set; }
        public TipoCombustible TipoCombustible { get; set; } = TipoCombustible.Gasolina;
        public TipoTransmision? Transmision { get; set; }
        #endregion

        #region Estado y Ubicación
        public EstadoVehiculo Estado { get; set; } = EstadoVehiculo.EnTransito;
        public int? UbicacionId { get; set; }
        public int? SucursalId { get; set; }
        #endregion

        #region Operaciones
        public ProcedenciaVehiculo Procedencia { get; set; } = ProcedenciaVehiculo.Importado;
        public string? NumeroImportacion { get; set; }
        public string? NumeroPoliza { get; set; }
        public DateTime? FechaIngresoPais { get; set; }
        public DateTime? FechaRecepcion { get; set; }
        public decimal? CostoImportacion { get; set; }
        #endregion

        #region Ventas
        public int? ClienteId { get; set; }
        public decimal? PrecioLista { get; set; }
        #endregion

        #region Taller
        public int KilometrajeActual { get; set; } = 0;
        public DateTime? FechaPrimeraMatricula { get; set; }
        public DateTime? GarantiaHasta { get; set; }
        #endregion

        #region General
        public string? Observaciones { get; set; }
        #endregion
    }
    public class UpdateVehiculoDTO
    {
        public int VehiculoId { get; set; }

        #region Identificación
        public string Vin { get; set; } = null!;
        public string? Placa { get; set; }
        public string? NumeroMotor { get; set; }
        public string? NumeroChasis { get; set; }
        #endregion

        #region Catálogo
        public int MarcaId { get; set; }
        public int ModeloId { get; set; }
        public int? VersionId { get; set; }
        public int Anio { get; set; }
        public string? Color { get; set; }
        public TipoCombustible TipoCombustible { get; set; }
        public TipoTransmision? Transmision { get; set; }
        #endregion

        #region Estado y Ubicación
        public EstadoVehiculo Estado { get; set; }
        public int? UbicacionId { get; set; }
        public int? SucursalId { get; set; }
        #endregion

        #region Operaciones
        public ProcedenciaVehiculo Procedencia { get; set; }
        public string? NumeroImportacion { get; set; }
        public string? NumeroPoliza { get; set; }
        public DateTime? FechaIngresoPais { get; set; }
        public DateTime? FechaRecepcion { get; set; }
        public decimal? CostoImportacion { get; set; }
        #endregion

        #region Ventas
        public int? ClienteId { get; set; }
        public decimal? PrecioLista { get; set; }
        public decimal? PrecioVenta { get; set; }
        public DateTime? FechaVenta { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string? VendedorId { get; set; }
        #endregion

        #region Taller
        public int KilometrajeActual { get; set; }
        public DateTime? FechaPrimeraMatricula { get; set; }
        public DateTime? GarantiaHasta { get; set; }
        #endregion

        #region General
        public string? Observaciones { get; set; }
        #endregion
    }

}
