using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Dtos.Marcas
{
    // / <summary> 
    // DTOS para listar y detallar marcas de vehículos.
    public class MarcaDTO
    {
        public int MarcaId { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? PaisOrigen { get; set; }
        public bool EsMarcaPropia { get; set; } = true;
        public string? LogoUrl { get; set; }
        public string? Observaciones { get; set; }  
    }

    // <summary> para actualizar una marca de vehículo.
    public class UpdateMarcaDTO 
    {
        public int MarcaId { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? PaisOrigen { get; set; }
        public bool EsMarcaPropia { get; set; }
        public string? LogoUrl { get; set; }
        public string? Observaciones { get; set; }

    }






}
