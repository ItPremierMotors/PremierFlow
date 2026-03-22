using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PremierFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Infrastructure.Configurations
{

    #region Entidades Base

    public class SucursalConfiguration : IEntityTypeConfiguration<Sucursal>
    {
        public void Configure(EntityTypeBuilder<Sucursal> builder)
        {
            builder.ToTable("sucursales");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
            builder.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
            builder.Property(e => e.Ciudad).HasColumnName("ciudad").HasMaxLength(100).IsRequired();
            builder.Property(e => e.Direccion).HasColumnName("direccion").HasMaxLength(500);
            builder.Property(e => e.Activa).HasColumnName("activa").HasDefaultValue(true);
            builder.Property(e=>e.LineaNegocio).HasConversion<string>().HasMaxLength(30);
            builder.HasIndex(e => e.Codigo).IsUnique();
        }
    }

public class VendedorSucursalConfiguration : IEntityTypeConfiguration<VendedorSucursal>
{
    public void Configure(EntityTypeBuilder<VendedorSucursal> builder)
    {
        builder.ToTable("vendedores_sucursal");
        builder.HasKey(e => e.VendedorSucursalId);

        builder.Property(e => e.VendedorSucursalId).HasColumnName("vendedor_sucursal_id");
        builder.Property(e => e.VendedorId).HasColumnName("vendedor_id").HasMaxLength(450).IsRequired();
        builder.Property(e => e.SucursalId).HasColumnName("sucursal_id");
        builder.Property(e => e.UltimaAsignacion).HasColumnName("ultima_asignacion");
        builder.Property(e => e.EstaActivo).HasColumnName("esta_activo").HasDefaultValue(true);

        // Relaciones
        builder.HasOne(e => e.Sucursal).WithMany().HasForeignKey(e => e.SucursalId).OnDelete(DeleteBehavior.Restrict);

        // Índice único: un vendedor no puede estar duplicado en la misma sucursal
        builder.HasIndex(e => new { e.VendedorId, e.SucursalId }).IsUnique().HasDatabaseName("ix_vendedor_sucursal_unico");
    }
}

    public class UbicacionConfiguration : IEntityTypeConfiguration<Ubicacion>
    {
        public void Configure(EntityTypeBuilder<Ubicacion> builder)
        {
            builder.ToTable("ubicaciones");
            builder.HasKey(e => e.id);
            builder.Property(e => e.id).HasColumnName("id");
            builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
            builder.Property(e => e.Tipo).HasColumnName("tipo").HasConversion<string>().HasMaxLength(30);
            builder.Property(e => e.Activa).HasColumnName("activa").HasDefaultValue(true);
            builder.Property(e => e.SucursalID).HasColumnName("sucursal_id");

            builder.HasOne(e => e.Sucursal)
                .WithMany(s => s.Ubicaciones)
                .HasForeignKey(e => e.SucursalID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    #endregion

    #region Catálogos de Vehículos

    public class MarcaConfiguration : IEntityTypeConfiguration<Marca>
    {
        public void Configure(EntityTypeBuilder<Marca> builder)
        {
            builder.ToTable("marcas");
            builder.HasKey(e => e.MarcaId);
            builder.Property(e => e.MarcaId).HasColumnName("marca_id");
            builder.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
            builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
            builder.Property(e => e.PaisOrigen).HasColumnName("pais_origen").HasMaxLength(50);
            builder.Property(e => e.EsMarcaPropia).HasColumnName("es_marca_propia").HasDefaultValue(false);
            builder.Property(e => e.LogoUrl).HasColumnName("logo_url").HasMaxLength(500);
            builder.Property(e => e.Observaciones).HasColumnName("observaciones").HasColumnType("nvarchar(2000)");
            builder.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);
            builder.Property(e => e.UsuarioCreaId).HasColumnName("usuario_crea_id").HasMaxLength(450);
            builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            builder.Property(e => e.UsuarioModificaId).HasColumnName("usuario_modifica_id").HasMaxLength(450);
            builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");

            builder.HasIndex(e => e.Codigo).IsUnique();
        }
    }

    public class ModeloConfiguration : IEntityTypeConfiguration<Modelo>
    {
        public void Configure(EntityTypeBuilder<Modelo> builder)
        {
            builder.ToTable("modelos");
            builder.HasKey(e => e.ModeloId);
            builder.Property(e => e.ModeloId).HasColumnName("modelo_id");
            builder.Property(e => e.MarcaId).HasColumnName("marca_id").IsRequired();
            builder.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(50).IsRequired();
            builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
            builder.Property(e => e.Segmento).HasColumnName("segmento").HasConversion<string>().HasMaxLength(20);
            builder.Property(e => e.AnioInicio).HasColumnName("anio_inicio");
            builder.Property(e => e.AnioFin).HasColumnName("anio_fin");
            builder.Property(e => e.Descripcion).HasColumnName("descripcion").HasColumnType("nvarchar(2000)");
            builder.Property(e => e.ImagenUrl).HasColumnName("imagen_url").HasMaxLength(500);
            builder.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);
            builder.Property(e => e.UsuarioCreaId).HasColumnName("usuario_crea_id").HasMaxLength(450);
            builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            builder.Property(e => e.UsuarioModificaId).HasColumnName("usuario_modifica_id").HasMaxLength(450);
            builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");

            builder.HasOne(e => e.Marca).WithMany(m => m.Modelos).HasForeignKey(e => e.MarcaId).OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(e => e.Codigo).IsUnique();
            builder.Ignore(e => e.EstaEnProduccion);
        }
    }

    public class VersionVehiculoConfiguration : IEntityTypeConfiguration<VersionVehiculo>
    {
        public void Configure(EntityTypeBuilder<VersionVehiculo> builder)
        {
            builder.ToTable("versiones");
            builder.HasKey(e => e.VersionId);
            builder.Property(e => e.VersionId).HasColumnName("version_id");
            builder.Property(e => e.ModeloId).HasColumnName("modelo_id").IsRequired();
            builder.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(50).IsRequired();
            builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
            builder.Property(e => e.Motor).HasColumnName("motor").HasMaxLength(100);
            builder.Property(e => e.Transmision).HasColumnName("transmision").HasConversion<string>().HasMaxLength(20);
            builder.Property(e => e.Traccion).HasColumnName("traccion").HasConversion<string>().HasMaxLength(10);
            builder.Property(e => e.NumPuertas).HasColumnName("num_puertas");
            builder.Property(e => e.NumPasajeros).HasColumnName("num_pasajeros");
            builder.Property(e => e.TipoCombustible).HasColumnName("tipo_combustible").HasConversion<string>().HasMaxLength(20);
            builder.Property(e => e.Cilindraje).HasColumnName("cilindraje").HasPrecision(4, 2);
            builder.Property(e => e.PotenciaHp).HasColumnName("potencia_hp");
            builder.Property(e => e.TorqueNm).HasColumnName("torque_nm");
            builder.Property(e => e.PrecioBase).HasColumnName("precio_base").HasPrecision(12, 2);
            builder.Property(e => e.AnioVersion).HasColumnName("anio_version");
            builder.Property(e => e.CaracteristicasPrincipales).HasColumnName("caracteristicas_principales").HasColumnType("nvarchar(2000)");
            builder.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);
            builder.Property(e => e.UsuarioCreaId).HasColumnName("usuario_crea_id").HasMaxLength(450);
            builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            builder.Property(e => e.UsuarioModificaId).HasColumnName("usuario_modifica_id").HasMaxLength(450);
            builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");

            builder.HasOne(e => e.Modelo).WithMany(m => m.VersionVehiculos).HasForeignKey(e => e.ModeloId).OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(e => e.Codigo).IsUnique();
            builder.Ignore(e => e.DescripcionCompleta);
        }
    }

    #endregion

    #region Cliente

    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("clientes");

            builder.HasKey(c => c.ClienteId);

            builder.Property(c => c.ClienteId)
                .HasColumnName("cliente_id");

            builder.Property(c => c.TipoCliente)
                .HasColumnName("tipo_cliente")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(c => c.Nombre)
                .HasColumnName("nombre")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.Apellidos)
                .HasColumnName("apellidos")
                .HasMaxLength(100);

            builder.Property(c => c.DNI)
                .HasColumnName("dni")
                .HasMaxLength(15);

            builder.Property(c => c.RTN)
                .HasColumnName("rtn")
                .HasMaxLength(16);

            builder.Property(c => c.Telefono)
                .HasColumnName("telefono")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(c => c.TelefonoSecundario)
                .HasColumnName("telefono_secundario")
                .HasMaxLength(20);

            builder.Property(c => c.Email)
                .HasColumnName("email")
                .HasMaxLength(100);

            builder.Property(c => c.Direccion)
                .HasColumnName("direccion")
                .HasMaxLength(250);

            builder.Property(c => c.Ciudad)
                .HasColumnName("ciudad")
                .HasMaxLength(100);

            builder.Property(c => c.FechaRegistro)
                .HasColumnName("fecha_registro");

            builder.Property(c => c.NoShowCount)
                .HasColumnName("no_show_count")
                .HasDefaultValue(0);

            // Campos de auditoría (de SoftDeletableEntity)
            builder.Property(c => c.Activo)
                .HasColumnName("activo")
                .HasDefaultValue(true);

            builder.Property(c => c.FechaCreacion)
                .HasColumnName("fecha_creacion");

            builder.Property(c => c.FechaModificacion)
                .HasColumnName("fecha_modificacion");

            builder.Property(c => c.UsuarioCreaId)
                .HasColumnName("usuario_crea_id")
                .HasMaxLength(450);

            builder.Property(c => c.UsuarioModificaId)
                .HasColumnName("usuario_modifica_id")
                .HasMaxLength(450);

            // Índices
            builder.HasIndex(c => c.DNI)
                .HasDatabaseName("ix_clientes_dni");

            builder.HasIndex(c => c.RTN)
                .HasDatabaseName("ix_clientes_rtn");

            builder.HasIndex(c => c.Telefono)
                .HasDatabaseName("ix_clientes_telefono");

            builder.HasIndex(c => c.Email)
                .HasDatabaseName("ix_clientes_email");
        }
    }

    #endregion

}
