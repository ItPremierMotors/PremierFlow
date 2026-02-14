using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Dtos.Taller
{
    public class OrdenServicioDTO
    {
        public int OsId { get; set; }
        public string NumeroOs { get; set; } = null!;
        public int? CitaId { get; set; }
        public int VehiculoId { get; set; }
        public int ClienteId { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public int EstadoId { get; set; }
        public int KilometrajeIngreso { get; set; }
        public decimal? NivelCombustible { get; set; }
        public TipoIngreso TipoIngreso { get; set; }
        public bool EsGarantia { get; set; }
        public string? ObservacionesApertura { get; set; }
        public string? ObservacionesCierre { get; set; }
        public string? AsesorId { get; set; }
        public string? CoordinadorId { get; set; }
        public decimal TotalManoObra { get; set; }
        public decimal TotalRepuestos { get; set; }
        public decimal TotalGeneral { get; set; }
        public int? SucursalId { get; set; }

        // Extras para UI
        public string? CodigoCita { get; set; }
        public string ClienteNombre { get; set; } = null!;
        public string? ClienteTelefono { get; set; }
        public string VehiculoDescripcion { get; set; } = null!;
        public string? VehiculoPlaca { get; set; }
        public string EstadoNombre { get; set; } = null!;
        public string EstadoCodigo { get; set; } = null!;
        public string? SucursalNombre { get; set; }
        public int? NivelCombustiblePorcentaje { get; set; }
        public bool EsWalkIn { get; set; }
        public bool EstaAbierta { get; set; }
        public bool PuedeModificarse { get; set; }
        public string TipoIngresoNombre => TipoIngreso.ToString();
    }
    public class OrdenServicioDetalleDTO
    {
        public int OsId { get; set; }
        public string NumeroOs { get; set; } = null!;
        public int? CitaId { get; set; }
        public int VehiculoId { get; set; }
        public int ClienteId { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public int EstadoId { get; set; }
        public int KilometrajeIngreso { get; set; }
        public decimal? NivelCombustible { get; set; }
        public TipoIngreso TipoIngreso { get; set; }
        public bool EsGarantia { get; set; }
        public string? ObservacionesApertura { get; set; }
        public string? ObservacionesCierre { get; set; }
        public string? AsesorId { get; set; }
        public string? CoordinadorId { get; set; }
        public decimal TotalManoObra { get; set; }
        public decimal TotalRepuestos { get; set; }
        public decimal TotalGeneral { get; set; }
        public int? SucursalId { get; set; }

        // Extras para UI
        public string? CodigoCita { get; set; }
        public string ClienteNombre { get; set; } = null!;
        public string? ClienteTelefono { get; set; }
        public string? ClienteEmail { get; set; }
        public string VehiculoDescripcion { get; set; } = null!;
        public string? VehiculoPlaca { get; set; }
        public string? VehiculoVin { get; set; }
        public int VehiculoKilometrajeAnterior { get; set; }
        public bool VehiculoEnGarantia { get; set; }
        public string EstadoNombre { get; set; } = null!;
        public string EstadoCodigo { get; set; } = null!;
        public string? SucursalNombre { get; set; }
        public int? NivelCombustiblePorcentaje { get; set; }
        public bool EsWalkIn { get; set; }
        public bool EstaAbierta { get; set; }
        public bool PuedeModificarse { get; set; }

        // Colecciones relacionadas
        public int CantidadServicios { get; set; }
        public int CantidadTecnicos { get; set; }
        public int CantidadEvidencias { get; set; }
    }
    public class CreateOsFromCitaDTO
    {
        public int CitaId { get; set; }
        public int KilometrajeIngreso { get; set; }
        public decimal? NivelCombustible { get; set; }
        public bool EsGarantia { get; set; } = false;
        public string? ObservacionesApertura { get; set; }
        public string? AsesorId { get; set; }
        public string? CoordinadorId { get; set; }
    }
    public class CreateOsWalkInDTO
    {
        public int ClienteId { get; set; }
        public int VehiculoId { get; set; }
        public int KilometrajeIngreso { get; set; }
        public decimal? NivelCombustible { get; set; }
        public bool EsGarantia { get; set; } = false;
        public string? ObservacionesApertura { get; set; }
        public string? AsesorId { get; set; }
        public string? CoordinadorId { get; set; }
        public int? SucursalId { get; set; }
    }
    public class UpdateOrdenServicioDTO
    {
        public int OsId { get; set; }
        public int KilometrajeIngreso { get; set; }
        public decimal? NivelCombustible { get; set; }
        public bool EsGarantia { get; set; }
        public string? ObservacionesApertura { get; set; }
        public string? AsesorId { get; set; }
        public string? CoordinadorId { get; set; }
    }
    public class CambiarEstadoOsDTO
    {
        public int OsId { get; set; }
        public int NuevoEstadoId { get; set; }
        public string? Observaciones { get; set; }
    }
    public class CerrarOsDTO
    {
        public int OsId { get; set; }
        public string? ObservacionesCierre { get; set; }
        public string? ProximaRevision { get; set; }
    }
}
