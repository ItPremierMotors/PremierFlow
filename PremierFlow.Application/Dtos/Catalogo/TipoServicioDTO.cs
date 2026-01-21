using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Dtos.Catalogo
{
    public class TipoServicioDTO
    {
        public int TipoServicioId { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public int DuracionEstimadaMin { get; set; }
        public ClasificacionServicio Clasificacion { get; set; }
        public bool PermiteWalkIn { get; set; }
        public bool RequiereCita { get; set; }
        public decimal PrecioBase { get; set; }
        public int StockRequerido { get; set; }

        // Extra para UI
        public string DuracionFormateada { get; set; } = null!;  // "2h 30min"
    }

    public class CreateTipoServicioDTO
    {
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public int DuracionEstimadaMin { get; set; }
        public ClasificacionServicio Clasificacion { get; set; } = ClasificacionServicio.Estandar;
        public bool PermiteWalkIn { get; set; } = false;
        public bool RequiereCita { get; set; } = true;
        public decimal PrecioBase { get; set; }
        public int StockRequerido { get; set; } = 0;
    }
    public class UpdateTipoServicioDTO
    {
        public int TipoServicioId { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public int DuracionEstimadaMin { get; set; }
        public ClasificacionServicio Clasificacion { get; set; }
        public bool PermiteWalkIn { get; set; }
        public bool RequiereCita { get; set; }
        public decimal PrecioBase { get; set; }
        public int StockRequerido { get; set; }
    }
}
