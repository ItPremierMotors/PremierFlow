using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Domain.Entities
{
    /// <summary>
    /// Sede comercial de Premier (ej: SPS Norte, TGU Blvd X).
    /// </summary>
    public class Sucursal
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!; // "Premier SPS Norte
        public string Codigo { get; set; } = null!; // "SPS-NORTE", "TGU-CENTRO"
        public string Ciudad { get; set; } = null!; // "San Pedro Sula", "Tegucigalpa"
        public string? Direccion { get; set; } 
        public bool Activa { get; set; } = true;
    }
}
