using PremierFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Infrastructure.Identity
{
    public class UsuarioSucursal
    {
        public string UsuarioID { get; set; } = null!;
        public ApplicationUser Usuario { get; set; } = null!;
        public int SucursalID { get; set; }
        public Sucursal Sucursal { get; set; } = null!;
        public bool EsSucursualPrincipal { get; set; } = false;


    }
}
