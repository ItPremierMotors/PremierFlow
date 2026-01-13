using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Domain.Entities
{
    public class Ubicacion
    {
        public int id { get; set; }
        public string Nombre { get; set; } = null!; // "Bodega Principal", "Showroom Tegus"
       
        public TipoUbicacion Tipo { get; set; }
        
        public bool Activa { get; set; } = true;

        //Relacion con sucursal
        // Puede estar asociada a una sucursal (showroom, bodega propia)
        // o ser externa (Depósito Fiscal, tránsito, etc.).
        public int? SucursalID { get; set; }
        // Navegación
        public virtual Sucursal? Sucursal { get; set; }
        public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
    }
}
