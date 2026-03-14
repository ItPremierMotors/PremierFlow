using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PremierFlow.Domain.Entities;

namespace PremierFlow.Infrastructure.Configurations
{
    #region Lead

    public class LeadConfiguration : IEntityTypeConfiguration<Lead>
    {
        public void Configure(EntityTypeBuilder<Lead> builder)
        {
            builder.ToTable("leads");
            builder.HasKey(e => e.LeadId);

            builder.Property(e => e.LeadId).HasColumnName("lead_id");
            builder.Property(e => e.CodigoLead).HasColumnName("codigo_lead").HasMaxLength(20).IsRequired();
            builder.Property(e => e.NombreCompleto).HasColumnName("nombre_completo").HasMaxLength(200).IsRequired();
            builder.Property(e => e.Telefono).HasColumnName("telefono").HasMaxLength(20);
            builder.Property(e => e.Email).HasColumnName("email").HasMaxLength(100);
            builder.Property(e => e.Empresa).HasColumnName("empresa").HasMaxLength(200);
            builder.Property(e => e.Ciudad).HasColumnName("ciudad").HasMaxLength(100);

            builder.Property(e => e.Origen).HasColumnName("origen").HasConversion<string>().HasMaxLength(30);
            builder.Property(e => e.DetalleOrigen).HasColumnName("detalle_origen").HasMaxLength(500);
            builder.Property(e => e.SucursalId).HasColumnName("sucursal_id");
            builder.Property(e => e.VendedorAsignadoId).HasColumnName("vendedor_asignado_id").HasMaxLength(450);

            builder.Property(e => e.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(30);
            builder.Property(e => e.FechaIngreso).HasColumnName("fecha_ingreso");
            builder.Property(e => e.FechaPrimeraRespuesta).HasColumnName("fecha_primera_respuesta");
            builder.Property(e => e.FechaConversion).HasColumnName("fecha_conversion");
            builder.Property(e => e.FechaDescarte).HasColumnName("fecha_descarte");
            builder.Property(e => e.MotivoDescarte).HasColumnName("motivo_descarte").HasMaxLength(500);

            builder.Property(e => e.VehiculoInteres).HasColumnName("vehiculo_interes").HasMaxLength(200);
            builder.Property(e => e.PresupuestoEstimado).HasColumnName("presupuesto_estimado").HasPrecision(12, 2);

            builder.Property(e => e.ClienteId).HasColumnName("cliente_id");

            // Auditoría (de SoftDeletableEntity)
            builder.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);
            builder.Property(e => e.UsuarioCreaId).HasColumnName("usuario_crea_id").HasMaxLength(450);
            builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            builder.Property(e => e.UsuarioModificaId).HasColumnName("usuario_modifica_id").HasMaxLength(450);
            builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");

            // Relaciones
            builder.HasOne(e => e.Sucursal).WithMany().HasForeignKey(e => e.SucursalId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Cliente).WithMany().HasForeignKey(e => e.ClienteId).OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(e => e.CodigoLead).IsUnique().HasDatabaseName("ix_leads_codigo");
            builder.HasIndex(e => e.Estado).HasDatabaseName("ix_leads_estado");
            builder.HasIndex(e => e.SucursalId).HasDatabaseName("ix_leads_sucursal");
            builder.HasIndex(e => e.VendedorAsignadoId).HasDatabaseName("ix_leads_vendedor");

            // Ignorar propiedades calculadas
            builder.Ignore(e => e.SinContactar);
            builder.Ignore(e => e.FueConvertido);
        }
    }

    #endregion

    #region Oportunidad

    public class OportunidadConfiguration : IEntityTypeConfiguration<Oportunidad>
    {
        public void Configure(EntityTypeBuilder<Oportunidad> builder)
        {
            builder.ToTable("oportunidades");
            builder.HasKey(e => e.OportunidadId);

            builder.Property(e => e.OportunidadId).HasColumnName("oportunidad_id");
            builder.Property(e => e.CodigoOportunidad).HasColumnName("codigo_oportunidad").HasMaxLength(20).IsRequired();

            builder.Property(e => e.LeadId).HasColumnName("lead_id");
            builder.Property(e => e.ClienteId).HasColumnName("cliente_id");
            builder.Property(e => e.VehiculoId).HasColumnName("vehiculo_id");
            builder.Property(e => e.VendedorId).HasColumnName("vendedor_id").HasMaxLength(450).IsRequired();
            builder.Property(e => e.SucursalId).HasColumnName("sucursal_id");

            builder.Property(e => e.Etapa).HasColumnName("etapa").HasConversion<string>().HasMaxLength(30);
            builder.Property(e => e.ProbabilidadCierre).HasColumnName("probabilidad_cierre");
            builder.Property(e => e.FechaCierreEstimada).HasColumnName("fecha_cierre_estimada");

            builder.Property(e => e.Resultado).HasColumnName("resultado").HasConversion<string>().HasMaxLength(20);
            builder.Property(e => e.MotivoResultado).HasColumnName("motivo_resultado").HasMaxLength(500);
            builder.Property(e => e.FechaCierre).HasColumnName("fecha_cierre");
            builder.Property(e => e.FechaUltimaActividad).HasColumnName("fecha_ultima_actividad");

            // Auditoría
            builder.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);
            builder.Property(e => e.UsuarioCreaId).HasColumnName("usuario_crea_id").HasMaxLength(450);
            builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            builder.Property(e => e.UsuarioModificaId).HasColumnName("usuario_modifica_id").HasMaxLength(450);
            builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");

            // Relaciones
            builder.HasOne(e => e.Lead).WithMany(l => l.Oportunidades).HasForeignKey(e => e.LeadId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Cliente).WithMany().HasForeignKey(e => e.ClienteId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Vehiculo).WithMany().HasForeignKey(e => e.VehiculoId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Sucursal).WithMany().HasForeignKey(e => e.SucursalId).OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(e => e.CodigoOportunidad).IsUnique().HasDatabaseName("ix_oportunidades_codigo");
            builder.HasIndex(e => e.Etapa).HasDatabaseName("ix_oportunidades_etapa");
            builder.HasIndex(e => e.VendedorId).HasDatabaseName("ix_oportunidades_vendedor");
            builder.HasIndex(e => e.SucursalId).HasDatabaseName("ix_oportunidades_sucursal");

            // Ignorar propiedades calculadas
            builder.Ignore(e => e.EstaAbierta);
            builder.Ignore(e => e.FueGanada);
        }
    }

    #endregion

    #region ActividadCrm

    public class ActividadCrmConfiguration : IEntityTypeConfiguration<ActividadCrm>
    {
        public void Configure(EntityTypeBuilder<ActividadCrm> builder)
        {
            builder.ToTable("actividades_crm");
            builder.HasKey(e => e.ActividadCrmId);

            builder.Property(e => e.ActividadCrmId).HasColumnName("actividad_crm_id");
            builder.Property(e => e.LeadId).HasColumnName("lead_id");
            builder.Property(e => e.OportunidadId).HasColumnName("oportunidad_id");
            builder.Property(e => e.RealizadaPorId).HasColumnName("realizada_por_id").HasMaxLength(450).IsRequired();

            builder.Property(e => e.Tipo).HasColumnName("tipo").HasConversion<string>().HasMaxLength(30);
            builder.Property(e => e.Direccion).HasColumnName("direccion").HasConversion<string>().HasMaxLength(20);
            builder.Property(e => e.Asunto).HasColumnName("asunto").HasMaxLength(200).IsRequired();
            builder.Property(e => e.Descripcion).HasColumnName("descripcion").HasColumnType("nvarchar(2000)");
            builder.Property(e => e.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20);

            builder.Property(e => e.FechaProgramada).HasColumnName("fecha_programada");
            builder.Property(e => e.FechaRealizacion).HasColumnName("fecha_realizacion");
            builder.Property(e => e.DuracionMinutos).HasColumnName("duracion_minutos");

            builder.Property(e => e.Resultado).HasColumnName("resultado").HasMaxLength(500);
            builder.Property(e => e.ProximoContacto).HasColumnName("proximo_contacto");

            // Auditoría
            builder.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);
            builder.Property(e => e.UsuarioCreaId).HasColumnName("usuario_crea_id").HasMaxLength(450);
            builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            builder.Property(e => e.UsuarioModificaId).HasColumnName("usuario_modifica_id").HasMaxLength(450);
            builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");

            // Relaciones
            builder.HasOne(e => e.Lead).WithMany(l => l.Actividades).HasForeignKey(e => e.LeadId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Oportunidad).WithMany(o => o.Actividades).HasForeignKey(e => e.OportunidadId).OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(e => e.LeadId).HasDatabaseName("ix_actividades_crm_lead");
            builder.HasIndex(e => e.OportunidadId).HasDatabaseName("ix_actividades_crm_oportunidad");
            builder.HasIndex(e => e.RealizadaPorId).HasDatabaseName("ix_actividades_crm_vendedor");
            builder.HasIndex(e => e.Estado).HasDatabaseName("ix_actividades_crm_estado");

            // Ignorar propiedades calculadas
            builder.Ignore(e => e.EstaVencida);
        }
    }

    #endregion

    #region NotaCrm

    public class NotaCrmConfiguration : IEntityTypeConfiguration<NotaCrm>
    {
        public void Configure(EntityTypeBuilder<NotaCrm> builder)
        {
            builder.ToTable("notas_crm");
            builder.HasKey(e => e.NotaCrmId);

            builder.Property(e => e.NotaCrmId).HasColumnName("nota_crm_id");
            builder.Property(e => e.LeadId).HasColumnName("lead_id");
            builder.Property(e => e.OportunidadId).HasColumnName("oportunidad_id");
            builder.Property(e => e.AutorId).HasColumnName("autor_id").HasMaxLength(450).IsRequired();
            builder.Property(e => e.Contenido).HasColumnName("contenido").HasColumnType("nvarchar(4000)").IsRequired();
            builder.Property(e => e.EsPrivada).HasColumnName("es_privada").HasDefaultValue(false);

            // Auditoría
            builder.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);
            builder.Property(e => e.UsuarioCreaId).HasColumnName("usuario_crea_id").HasMaxLength(450);
            builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            builder.Property(e => e.UsuarioModificaId).HasColumnName("usuario_modifica_id").HasMaxLength(450);
            builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");

            // Relaciones
            builder.HasOne(e => e.Lead).WithMany(l => l.Notas).HasForeignKey(e => e.LeadId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Oportunidad).WithMany(o => o.Notas).HasForeignKey(e => e.OportunidadId).OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(e => e.LeadId).HasDatabaseName("ix_notas_crm_lead");
            builder.HasIndex(e => e.OportunidadId).HasDatabaseName("ix_notas_crm_oportunidad");
        }
    }

    #endregion

    #region CotizacionVehiculo

    public class CotizacionVehiculoConfiguration : IEntityTypeConfiguration<CotizacionVehiculo>
    {
        public void Configure(EntityTypeBuilder<CotizacionVehiculo> builder)
        {
            builder.ToTable("cotizaciones_vehiculo");
            builder.HasKey(e => e.CotizacionVehiculoId);

            builder.Property(e => e.CotizacionVehiculoId).HasColumnName("cotizacion_vehiculo_id");
            builder.Property(e => e.CodigoCotizacion).HasColumnName("codigo_cotizacion").HasMaxLength(20).IsRequired();
            builder.Property(e => e.OportunidadId).HasColumnName("oportunidad_id");
            builder.Property(e => e.VehiculoId).HasColumnName("vehiculo_id");

            builder.Property(e => e.Descuento).HasColumnName("descuento").HasPrecision(12, 2);
            builder.Property(e => e.PrecioOfertado).HasColumnName("precio_ofertado").HasPrecision(12, 2);
            builder.Property(e => e.CondicionesPago).HasColumnName("condiciones_pago").HasMaxLength(500);
            builder.Property(e => e.Observaciones).HasColumnName("observaciones").HasColumnType("nvarchar(2000)");

            builder.Property(e => e.FechaEmision).HasColumnName("fecha_emision");
            builder.Property(e => e.FechaVencimiento).HasColumnName("fecha_vencimiento");
            builder.Property(e => e.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20);

            // Auditoría
            builder.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);
            builder.Property(e => e.UsuarioCreaId).HasColumnName("usuario_crea_id").HasMaxLength(450);
            builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            builder.Property(e => e.UsuarioModificaId).HasColumnName("usuario_modifica_id").HasMaxLength(450);
            builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");

            // Relaciones
            builder.HasOne(e => e.Oportunidad).WithMany(o => o.Cotizaciones).HasForeignKey(e => e.OportunidadId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Vehiculo).WithMany().HasForeignKey(e => e.VehiculoId).OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(e => e.CodigoCotizacion).IsUnique().HasDatabaseName("ix_cotizaciones_codigo");
            builder.HasIndex(e => e.OportunidadId).HasDatabaseName("ix_cotizaciones_oportunidad");
        }
    }

    #endregion
}
