using Microsoft.AspNetCore.Identity;
using PremierFlow.Domain.Common;

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
        public DateTime FechaCreacion { get; set; } = TimeHelper.Now;
        public DateTime? UltimoLogin { get; set; }

    }
}
