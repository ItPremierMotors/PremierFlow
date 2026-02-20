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

            builder.HasKey(v => v.VehiculoId);

            #region Identificación

            builder.Property(v => v.VehiculoId)
                .HasColumnName("vehiculo_id");

            builder.Property(v => v.Vin)
                .HasColumnName("vin")
                .HasMaxLength(17)
                .IsRequired();

            builder.Property(v => v.Placa)
                .HasColumnName("placa")
                .HasMaxLength(15);

            builder.Property(v => v.NumeroMotor)
                .HasColumnName("numero_motor")
                .HasMaxLength(50);

            builder.Property(v => v.NumeroChasis)
                .HasColumnName("numero_chasis")
                .HasMaxLength(50);

            #endregion

            #region Catálogo

            builder.Property(v => v.MarcaId)
                .HasColumnName("marca_id")
                .IsRequired();

            builder.Property(v => v.ModeloId)
                .HasColumnName("modelo_id")
                .IsRequired();

            builder.Property(v => v.VersionId)
                .HasColumnName("version_id");

            builder.Property(v => v.Anio)
                .HasColumnName("anio")
                .IsRequired();

            builder.Property(v => v.Color)
                .HasColumnName("color")
                .HasMaxLength(50);

            #endregion

            #region Estado y Ubicación

            builder.Property(v => v.Estado)
                .HasColumnName("estado")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(v => v.UbicacionId)
                .HasColumnName("ubicacion_id");

            builder.Property(v => v.SucursalId)
                .HasColumnName("sucursal_id");

            #endregion

            #region Operaciones (Importación)

            builder.Property(v => v.Procedencia)
                .HasColumnName("procedencia")
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(v => v.NumeroImportacion)
                .HasColumnName("numero_importacion")
                .HasMaxLength(50);

            builder.Property(v => v.NumeroPoliza)
                .HasColumnName("numero_poliza")
                .HasMaxLength(50);

            builder.Property(v => v.FechaIngresoPais)
                .HasColumnName("fecha_ingreso_pais");

            builder.Property(v => v.FechaRecepcion)
                .HasColumnName("fecha_recepcion");

            builder.Property(v => v.CostoImportacion)
                .HasColumnName("costo_importacion")
                .HasPrecision(18, 2);

            #endregion

            #region Ventas

            builder.Property(v => v.ClienteId)
                .HasColumnName("cliente_id");

            builder.Property(v => v.PrecioLista)
                .HasColumnName("precio_lista")
                .HasPrecision(18, 2);

            builder.Property(v => v.PrecioVenta)
                .HasColumnName("precio_venta")
                .HasPrecision(18, 2);

            builder.Property(v => v.FechaVenta)
                .HasColumnName("fecha_venta");

            builder.Property(v => v.FechaEntrega)
                .HasColumnName("fecha_entrega");

            builder.Property(v => v.VendedorId)
                .HasColumnName("vendedor_id")
                .HasMaxLength(450);

            #endregion

            #region Reserva

            builder.Property(v => v.ReservadoPorId)
                .HasColumnName("reservado_por_id")
                .HasMaxLength(450);

            builder.Property(v => v.FechaReserva)
                .HasColumnName("fecha_reserva");

            builder.Property(v => v.FechaLimiteReserva)
                .HasColumnName("fecha_limite_reserva");

            #endregion

            #region Taller / Postventa

            builder.Property(v => v.KilometrajeActual)
                .HasColumnName("kilometraje_actual")
                .HasDefaultValue(0);

            builder.Property(v => v.FechaPrimeraMatricula)
                .HasColumnName("fecha_primera_matricula");

            builder.Property(v => v.GarantiaHasta)
                .HasColumnName("garantia_hasta");

            #endregion

            #region General

            builder.Property(v => v.Observaciones)
                .HasColumnName("observaciones")
                .HasMaxLength(500);

            builder.Property(v => v.FechaRegistro)
                .HasColumnName("fecha_registro");

            #endregion

            #region Auditoría

            builder.Property(v => v.Activo)
                .HasColumnName("activo")
                .HasDefaultValue(true);

            builder.Property(v => v.FechaCreacion)
                .HasColumnName("fecha_creacion");

            builder.Property(v => v.FechaModificacion)
                .HasColumnName("fecha_modificacion");

            builder.Property(v => v.UsuarioCreaId)
                .HasColumnName("usuario_crea_id")
                .HasMaxLength(450);

            builder.Property(v => v.UsuarioModificaId)
                .HasColumnName("usuario_modifica_id")
                .HasMaxLength(450);

            #endregion

            #region Relaciones

            builder.HasOne(v => v.Cliente)
                .WithMany(c => c.Vehiculos)
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Marca)
                .WithMany(m => m.Vehiculos)
                .HasForeignKey(v => v.MarcaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Modelo)
                .WithMany(m => m.Vehiculos)
                .HasForeignKey(v => v.ModeloId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Version)
                .WithMany(ver => ver.Vehiculos)
                .HasForeignKey(v => v.VersionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Ubicacion)
                .WithMany(u => u.Vehiculos)
                .HasForeignKey(v => v.UbicacionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Sucursal)
                .WithMany()
                .HasForeignKey(v => v.SucursalId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Índices

            builder.HasIndex(v => v.Vin)
                .IsUnique()
                .HasDatabaseName("ix_vehiculos_vin");

            builder.HasIndex(v => v.Placa)
                .HasDatabaseName("ix_vehiculos_placa");

            builder.HasIndex(v => v.ClienteId)
                .HasDatabaseName("ix_vehiculos_cliente_id");

            builder.HasIndex(v => v.Estado)
                .HasDatabaseName("ix_vehiculos_estado");

            builder.HasIndex(v => v.MarcaId)
                .HasDatabaseName("ix_vehiculos_marca_id");

            builder.HasIndex(v => v.SucursalId)
                .HasDatabaseName("ix_vehiculos_sucursal_id");

            #endregion

            // Ignorar propiedades calculadas
            builder.Ignore(e => e.DescripcionCompleta);
            builder.Ignore(e => e.EstaVendido);
            builder.Ignore(e => e.DisponibleParaVenta);
            builder.Ignore(e => e.EnGarantia);
            builder.Ignore(e => e.ReservaExpirada);
        }
    }

}
