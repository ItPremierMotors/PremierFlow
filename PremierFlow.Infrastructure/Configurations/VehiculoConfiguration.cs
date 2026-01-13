using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PremierFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Infrastructure.Configurations
{
public class VehiculoConfiguration : IEntityTypeConfiguration<Vehiculo>
    {
        public void Configure(EntityTypeBuilder<Vehiculo> builder)
        {
            builder.ToTable("vehiculos");
            builder.HasKey(e => e.VehiculoId);

            // Identificación
            builder.Property(e => e.VehiculoId).HasColumnName("vehiculo_id");
            builder.Property(e => e.Vin).HasColumnName("vin").HasMaxLength(17).IsRequired();
            builder.Property(e => e.Placa).HasColumnName("placa").HasMaxLength(20);
            builder.Property(e => e.NumeroMotor).HasColumnName("numero_motor").HasMaxLength(50);
            builder.Property(e => e.NumeroChasis).HasColumnName("numero_chasis").HasMaxLength(50);

            // Catálogo
            builder.Property(e => e.MarcaId).HasColumnName("marca_id").IsRequired();
            builder.Property(e => e.ModeloId).HasColumnName("modelo_id").IsRequired();
            builder.Property(e => e.VersionId).HasColumnName("version_id");
            builder.Property(e => e.Anio).HasColumnName("anio").IsRequired();
            builder.Property(e => e.Color).HasColumnName("color").HasMaxLength(50);

            // Estado y Ubicación
            builder.Property(e => e.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20);
            builder.Property(e => e.UbicacionId).HasColumnName("ubicacion_id");
            builder.Property(e => e.SucursalId).HasColumnName("sucursal_id");

            // Operaciones (Importación)
            builder.Property(e => e.Procedencia).HasColumnName("procedencia").HasConversion<string>().HasMaxLength(20);
            builder.Property(e => e.NumeroImportacion).HasColumnName("numero_importacion").HasMaxLength(50);
            builder.Property(e => e.NumeroPoliza).HasColumnName("numero_poliza").HasMaxLength(50);
            builder.Property(e => e.FechaIngresoPais).HasColumnName("fecha_ingreso_pais");
            builder.Property(e => e.FechaRecepcion).HasColumnName("fecha_recepcion");
            builder.Property(e => e.CostoImportacion).HasColumnName("costo_importacion").HasPrecision(12, 2);

            // Ventas
            builder.Property(e => e.ClienteId).HasColumnName("cliente_id");
            builder.Property(e => e.PrecioLista).HasColumnName("precio_lista").HasPrecision(12, 2);
            builder.Property(e => e.PrecioVenta).HasColumnName("precio_venta").HasPrecision(12, 2);
            builder.Property(e => e.FechaVenta).HasColumnName("fecha_venta");
            builder.Property(e => e.FechaEntrega).HasColumnName("fecha_entrega");
            builder.Property(e => e.VendedorId).HasColumnName("vendedor_id").HasMaxLength(450);

            // Taller / Postventa
            builder.Property(e => e.KilometrajeActual).HasColumnName("kilometraje_actual").HasDefaultValue(0);
            builder.Property(e => e.FechaPrimeraMatricula).HasColumnName("fecha_primera_matricula");
            builder.Property(e => e.GarantiaHasta).HasColumnName("garantia_hasta");

            // General
            builder.Property(e => e.Observaciones).HasColumnName("observaciones").HasColumnType("text");
            builder.Property(e => e.FechaRegistro).HasColumnName("fecha_registro");
            builder.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);
            builder.Property(e => e.UsuarioCreaId).HasColumnName("usuario_crea_id").HasMaxLength(450);
            builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            builder.Property(e => e.UsuarioModificaId).HasColumnName("usuario_modifica_id").HasMaxLength(450);
            builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");

            // Relaciones
            builder.HasOne(e => e.Marca).WithMany(m => m.Vehiculos).HasForeignKey(e => e.MarcaId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Modelo).WithMany(m => m.Vehiculos).HasForeignKey(e => e.ModeloId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Version).WithMany(v => v.Vehiculos).HasForeignKey(e => e.VersionId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Cliente).WithMany(c => c.Vehiculos).HasForeignKey(e => e.ClienteId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Ubicacion).WithMany(u => u.Vehiculos).HasForeignKey(e => e.UbicacionId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Sucursal).WithMany().HasForeignKey(e => e.SucursalId).OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(e => e.Vin).IsUnique();
            builder.HasIndex(e => e.Placa);
            builder.HasIndex(e => e.ClienteId);
            builder.HasIndex(e => e.Estado);
            builder.HasIndex(e => e.SucursalId);
            builder.HasIndex(e => e.UbicacionId);

            // Ignorar propiedades calculadas
            builder.Ignore(e => e.DescripcionCompleta);
            builder.Ignore(e => e.EstaVendido);
            builder.Ignore(e => e.DisponibleParaVenta);
            builder.Ignore(e => e.EnGarantia);
        }
    }

}
