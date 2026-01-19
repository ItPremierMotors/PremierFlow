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

            builder.HasIndex(e => e.Codigo).IsUnique();
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
            builder.Property(e => e.Transmision).HasColumnName("transmision").HasMaxLength(50);
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
            builder.HasKey(e => e.ClienteId);
            builder.Property(e => e.ClienteId).HasColumnName("cliente_id");
            builder.Property(e => e.TipoCliente).HasColumnName("tipo_cliente").HasConversion<string>().HasMaxLength(20);
            builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
            builder.Property(e => e.Apellidos).HasColumnName("apellidos").HasMaxLength(100);
            builder.Property(e => e.DocumentoIdentidad).HasColumnName("documento_identidad").HasMaxLength(50);
            builder.Property(e => e.Telefono).HasColumnName("telefono").HasMaxLength(20).IsRequired();
            builder.Property(e => e.Email).HasColumnName("email").HasMaxLength(100);
            builder.Property(e => e.Direccion).HasColumnName("direccion").HasColumnType("nvarchar(500)");
            builder.Property(e => e.FechaRegistro).HasColumnName("fecha_registro");
            builder.Property(e => e.NoShowCount).HasColumnName("no_show_count").HasDefaultValue(0);
            builder.Property(e => e.UsuarioId).HasColumnName("usuario_id").HasMaxLength(450);
            builder.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);
            builder.Property(e => e.UsuarioCreaId).HasColumnName("usuario_crea_id").HasMaxLength(450);
            builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            builder.Property(e => e.UsuarioModificaId).HasColumnName("usuario_modifica_id").HasMaxLength(450);
            builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");

            builder.HasIndex(e => e.DocumentoIdentidad).IsUnique();
            builder.HasIndex(e => e.Telefono);
            builder.HasIndex(e => e.Email);
            builder.Ignore(e => e.NombreCompleto);
            builder.Ignore(e => e.TieneAlertaNoShow);
        }
    }

    #endregion

}
