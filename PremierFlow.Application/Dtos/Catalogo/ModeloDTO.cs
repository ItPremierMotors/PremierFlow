using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Dtos.Catalogo
{
    using global::PremierFlow.Domain.Enums;
    
    
    public class ModeloDTO
    {
        public int ModeloId { get; set; }
        public int MarcaId { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public SegmentoVehiculo Segmento { get; set; }
        public int? AnioInicio { get; set; }
        public int? AnioFin { get; set; }
        public string? Descripcion { get; set; }
        public string? ImagenUrl { get; set; }

        // Datos extras para mostrar en UI
        public string? MarcaNombre { get; set; }      // "Toyota"
        public bool EstaEnProduccion { get; set; }    // true/false
    }

    public class UpdateModeloDTO
    {
        public int ModeloId { get; set; }             // Requerido para saber cuál actualizar
        public int MarcaId { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public SegmentoVehiculo Segmento { get; set; }
        public int? AnioInicio { get; set; }
        public int? AnioFin { get; set; }
        public string? Descripcion { get; set; }
        public string? ImagenUrl { get; set; }
    }
}
