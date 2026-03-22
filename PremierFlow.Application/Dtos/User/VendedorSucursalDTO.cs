using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Dtos.User
{
    public class VendedorSucursalDTO
    {
        public int VendedorSucursalId { get; set; }
        public string VendedorId { get; set; } = null!;
        public string? VendedorNombre { get; set; }
        public int SucursalId { get; set; }
        public string? SucursalNombre { get; set; }
        public bool EstaActivo { get; set; }
    }

    public class AsignarVendedorSucursalDTO
    {
        public string VendedorId { get; set; } = null!;
        public int SucursalId { get; set; }
    }

}