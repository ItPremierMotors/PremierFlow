using Microsoft.AspNetCore.Identity;

namespace PremierFlow.Infrastructure.Identity
{
    /// <summary>
    /// Usuario de PremierFlow (login, roles, etc.).
    /// Extiende IdentityUser para agregar datos propios de la empresa.
    //</summary>
    public class ApplicationUser : IdentityUser
    {
        //perfil
       public string NombreCompleto { get; set; }   =null!;
        public string Departamento { get; set; }= null!;
        public string Cargo { get; set; }= null!;

        //estado
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? UltimoLogin { get; set; }

        //relacion mucho a muchos con sucursales padre es ApplicationUser E hijo UsuarioSucursal
        public ICollection<UsuarioSucursal> UsuarioSucursales { get; set; } = new List<UsuarioSucursal>();
    }
}
