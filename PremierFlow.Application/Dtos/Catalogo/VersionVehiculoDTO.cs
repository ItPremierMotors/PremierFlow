using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Dtos.Catalogo
{
    public class VersionVehiculoDTO
    {
        public int VersionId { get; set; }
        public int ModeloId { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? Motor { get; set; }
        public string? Transmision { get; set; }
        public TipoTraccion? Traccion { get; set; }
        public int? NumPuertas { get; set; }
        public int? NumPasajeros { get; set; }
        public TipoCombustible TipoCombustible { get; set; }
        public decimal? Cilindraje { get; set; }
        public int? PotenciaHp { get; set; }
        public int? TorqueNm { get; set; }
        public decimal? PrecioBase { get; set; }
        public int? AnioVersion { get; set; }
        public string? CaracteristicasPrincipales { get; set; }

        // Datos extras para UI
        public string? ModeloNombre { get; set; }      // "Civic"
        public string? MarcaNombre { get; set; }       // "Honda"
        public string DescripcionCompleta { get; set; } = null!;  // "EX - 2.0L Turbo CVT"
    }

    public class CreateVersionVehiculoDTO
    {
        public int ModeloId { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? Motor { get; set; }
        public string? Transmision { get; set; }
        public TipoTraccion? Traccion { get; set; }
        public int? NumPuertas { get; set; }
        public int? NumPasajeros { get; set; }
        public TipoCombustible TipoCombustible { get; set; } = TipoCombustible.Gasolina;
        public decimal? Cilindraje { get; set; }
        public int? PotenciaHp { get; set; }
        public int? TorqueNm { get; set; }
        public decimal? PrecioBase { get; set; }
        public int? AnioVersion { get; set; }
        public string? CaracteristicasPrincipales { get; set; }
    }
    public class UpdateVersionVehiculoDTO
    {
        public int VersionId { get; set; }  // Requerido
        public int ModeloId { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? Motor { get; set; }
        public string? Transmision { get; set; }
        public TipoTraccion? Traccion { get; set; }
        public int? NumPuertas { get; set; }
        public int? NumPasajeros { get; set; }
        public TipoCombustible TipoCombustible { get; set; }
        public decimal? Cilindraje { get; set; }
        public int? PotenciaHp { get; set; }
        public int? TorqueNm { get; set; }
        public decimal? PrecioBase { get; set; }
        public int? AnioVersion { get; set; }
        public string? CaracteristicasPrincipales { get; set; }
    }
}
