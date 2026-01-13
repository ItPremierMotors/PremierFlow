using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PremierFlow.Domain.Entities;
using PremierFlow.Infrastructure.Identity;

namespace PremierFlow.Infrastructure.Persistence
{
    public class PremierFlowDbContext: IdentityDbContext<ApplicationUser>
    {
        public PremierFlowDbContext(DbContextOptions<PremierFlowDbContext> options) 
          : base(options){  }

        public DbSet<Sucursal> Sucursales { get; set; } = null!;
        public DbSet<Ubicacion> Ubicaciones { get; set; } = null!;
        public DbSet<UsuarioSucursal> UsuarioSucursales { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Configurar la relación muchos a muchos entre ApplicationUser y Sucursal

            //clave compuesta para TABLA INTERMEDIA UsuarioSucursal
            builder.Entity<UsuarioSucursal>()
                .HasKey(us => new { us.UsuarioID, us.SucursalID });

            builder.Entity<UsuarioSucursal>()
                 .HasOne(us => us.Usuario)
                 .WithMany(u => u.UsuarioSucursales)
                 .HasForeignKey(us => us.UsuarioID);

            builder.Entity<UsuarioSucursal>()
                .HasOne(us => us.Sucursal)
                .WithMany()
                .HasForeignKey(us => us.SucursalID);
        }
    }
}
