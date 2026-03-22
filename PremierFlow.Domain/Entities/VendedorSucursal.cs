using PremierFlow.Domain.Common;
using PremierFlow.Domain.Enums;
using System;

namespace PremierFlow.Domain.Entities
{
    public class VendedorSucursal : SoftDeletableEntity
    {
        public int VendedorSucursalId { get; set; }
        public string VendedorId { get; set; } = string.Empty;
        public int SucursalId { get; set; }
        public DateTime? UltimaAsignacion { get; set; }
        public bool EstaActivo { get; set; } = true;

        // Navegación
        public virtual Sucursal Sucursal { get; set; } = null!;
    }
}