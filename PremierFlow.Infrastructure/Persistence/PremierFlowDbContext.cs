using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PremierFlow.Domain.Entities;
using PremierFlow.Infrastructure.Identity;

namespace PremierFlow.Infrastructure.Persistence
{
    public class PremierFlowDbContext : IdentityDbContext<ApplicationUser>
    {
        public PremierFlowDbContext(DbContextOptions<PremierFlowDbContext> options)
          : base(options) { }

        #region Entidades Base / Identity

        public DbSet<Sucursal> Sucursales { get; set; } = null!;
        public DbSet<Ubicacion> Ubicaciones { get; set; } = null!;
        #endregion

        #region Catálogos de Vehículos

        public DbSet<Marca> Marcas { get; set; } = null!;
        public DbSet<Modelo> Modelos { get; set; } = null!;
        public DbSet<VersionVehiculo> Versiones { get; set; } = null!;

        #endregion

        #region Entidades Principales

        public DbSet<Cliente> Clientes { get; set; } = null!;
        public DbSet<Vehiculo> Vehiculos { get; set; } = null!;

        #endregion

        #region Catálogos de Taller

        public DbSet<TipoServicio> TiposServicio { get; set; } = null!;
        public DbSet<EstadoOs> EstadosOs { get; set; } = null!;
        public DbSet<Tecnico> Tecnicos { get; set; } = null!;

        #endregion
        #region Agenda

        public DbSet<CapacidadTaller> CapacidadTaller { get; set; } = null!;
        public DbSet<BloqueHorario> BloquesHorario { get; set; } = null!;
        public DbSet<Cita> Citas { get; set; } = null!;

        #endregion

        #region Orden de Servicio

        public DbSet<OrdenServicio> OrdenesServicio { get; set; } = null!;
        public DbSet<Recepcion> Recepciones { get; set; } = null!;
        public DbSet<Evidencia> Evidencias { get; set; } = null!;
        public DbSet<OsServicio> OsServicios { get; set; } = null!;
        public DbSet<AsignacionTecnico> AsignacionesTecnico { get; set; } = null!;

        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // ==========================================
            // Aplicar todas las configuraciones del ensamblado
            // ==========================================
            builder.ApplyConfigurationsFromAssembly(typeof(PremierFlowDbContext).Assembly);
        }
    }
}
