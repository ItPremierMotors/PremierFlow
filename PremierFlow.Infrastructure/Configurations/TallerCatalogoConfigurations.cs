using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PremierFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Infrastructure.Configurations
{
    #region Catálogos de Taller

    public class TipoServicioConfiguration : IEntityTypeConfiguration<TipoServicio>
    {
        public void Configure(EntityTypeBuilder<TipoServicio> builder)
        {
            builder.ToTable("tipos_servicio");
            builder.HasKey(e => e.TipoServicioId);
            builder.Property(e => e.TipoServicioId).HasColumnName("tipo_servicio_id");
            builder.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
            builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
            builder.Property(e => e.Descripcion).HasColumnName("descripcion").HasColumnType("text");
            builder.Property(e => e.DuracionEstimadaMin).HasColumnName("duracion_estimada_min").IsRequired();
            builder.Property(e => e.Clasificacion).HasColumnName("clasificacion").HasConversion<string>().HasMaxLength(20);
            builder.Property(e => e.PermiteWalkIn).HasColumnName("permite_walk_in").HasDefaultValue(false);
            builder.Property(e => e.RequiereCita).HasColumnName("requiere_cita").HasDefaultValue(true);
            builder.Property(e => e.PrecioBase).HasColumnName("precio_base").HasPrecision(10, 2);
            builder.Property(e => e.StockRequerido).HasColumnName("stock_requerido").HasDefaultValue(0);
            builder.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);
            builder.Property(e => e.UsuarioCreaId).HasColumnName("usuario_crea_id").HasMaxLength(450);
            builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            builder.Property(e => e.UsuarioModificaId).HasColumnName("usuario_modifica_id").HasMaxLength(450);
            builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");

            builder.HasIndex(e => e.Codigo).IsUnique();
            builder.Ignore(e => e.EsServicioRapido);
            builder.Ignore(e => e.DuracionEstimada);
        }
    }

    public class EstadoOsConfiguration : IEntityTypeConfiguration<EstadoOs>
    {
        public void Configure(EntityTypeBuilder<EstadoOs> builder)
        {
            builder.ToTable("estados_os");
            builder.HasKey(e => e.EstadoId);
            builder.Property(e => e.EstadoId).HasColumnName("estado_id");
            builder.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(30).IsRequired();
            builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
            builder.Property(e => e.Descripcion).HasColumnName("descripcion").HasColumnType("text");
            builder.Property(e => e.OrdenSecuencial).HasColumnName("orden_secuencial").IsRequired();
            builder.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);
            builder.Property(e => e.UsuarioCreaId).HasColumnName("usuario_crea_id").HasMaxLength(450);
            builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            builder.Property(e => e.UsuarioModificaId).HasColumnName("usuario_modifica_id").HasMaxLength(450);
            builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");

            builder.HasIndex(e => e.Codigo).IsUnique();
            builder.Ignore(e => e.EsEstadoFinal);
            builder.Ignore(e => e.PermiteModificacion);

            // Seed data
            builder.HasData(
                new EstadoOs { EstadoId = 1, Codigo = "ABIERTA", Nombre = "Abierta", Descripcion = "OS creada, pendiente de diagnóstico", OrdenSecuencial = 1 },
                new EstadoOs { EstadoId = 2, Codigo = "DIAGNOSTICO", Nombre = "En Diagnóstico", Descripcion = "Técnico evaluando el vehículo", OrdenSecuencial = 2 },
                new EstadoOs { EstadoId = 3, Codigo = "COTIZADA", Nombre = "Cotizada", Descripcion = "Presupuesto generado", OrdenSecuencial = 3 },
                new EstadoOs { EstadoId = 4, Codigo = "APROBADA", Nombre = "Aprobada", Descripcion = "Cliente aprobó cotización", OrdenSecuencial = 4 },
                new EstadoOs { EstadoId = 5, Codigo = "EN_TRABAJO", Nombre = "En Trabajo", Descripcion = "Técnico trabajando", OrdenSecuencial = 5 },
                new EstadoOs { EstadoId = 6, Codigo = "PAUSADA", Nombre = "Pausada", Descripcion = "Trabajo detenido", OrdenSecuencial = 6 },
                new EstadoOs { EstadoId = 7, Codigo = "COMPLETADA", Nombre = "Completada", Descripcion = "Trabajo terminado", OrdenSecuencial = 7 },
                new EstadoOs { EstadoId = 8, Codigo = "FACTURADA", Nombre = "Facturada", Descripcion = "Factura generada", OrdenSecuencial = 8 },
                new EstadoOs { EstadoId = 9, Codigo = "CERRADA", Nombre = "Cerrada", Descripcion = "OS cerrada y pagada", OrdenSecuencial = 9 },
                new EstadoOs { EstadoId = 99, Codigo = "CANCELADA", Nombre = "Cancelada", Descripcion = "OS cancelada", OrdenSecuencial = 99 }
            );
        }
    }

    public class TecnicoConfiguration : IEntityTypeConfiguration<Tecnico>
    {
        public void Configure(EntityTypeBuilder<Tecnico> builder)
        {
            builder.ToTable("tecnicos");
            builder.HasKey(e => e.TecnicoId);
            builder.Property(e => e.TecnicoId).HasColumnName("tecnico_id");
            builder.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
            builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
            builder.Property(e => e.Apellidos).HasColumnName("apellidos").HasMaxLength(100).IsRequired();
            builder.Property(e => e.Especialidad).HasColumnName("especialidad").HasMaxLength(100);
            builder.Property(e => e.BahiaAsignada).HasColumnName("bahia_asignada");
            builder.Property(e => e.UsuarioId).HasColumnName("usuario_id").HasMaxLength(450);
            builder.Property(e => e.SucursalId).HasColumnName("sucursal_id");
            builder.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);
            builder.Property(e => e.UsuarioCreaId).HasColumnName("usuario_crea_id").HasMaxLength(450);
            builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            builder.Property(e => e.UsuarioModificaId).HasColumnName("usuario_modifica_id").HasMaxLength(450);
            builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");

            builder.HasOne(e => e.Sucursal).WithMany().HasForeignKey(e => e.SucursalId).OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(e => e.Codigo).IsUnique();
            builder.Ignore(e => e.NombreCompleto);
        }
    }

    #endregion

    #region Agenda

    public class CapacidadTallerConfiguration : IEntityTypeConfiguration<CapacidadTaller>
    {
        public void Configure(EntityTypeBuilder<CapacidadTaller> builder)
        {
            builder.ToTable("capacidad_taller");
            builder.HasKey(e => e.CapacidadId);
            builder.Property(e => e.CapacidadId).HasColumnName("capacidad_id");
            builder.Property(e => e.Fecha).HasColumnName("fecha").IsRequired();
            builder.Property(e => e.Turno).HasColumnName("turno").HasConversion<string>().HasMaxLength(20);
            builder.Property(e => e.TecnicosDisponibles).HasColumnName("tecnicos_disponibles").HasDefaultValue(0);
            builder.Property(e => e.BahiasDisponibles).HasColumnName("bahias_disponibles").HasDefaultValue(0);
            builder.Property(e => e.MinutosDisponibles).HasColumnName("minutos_disponibles").HasDefaultValue(0);
            builder.Property(e => e.MinutosReservados).HasColumnName("minutos_reservados").HasDefaultValue(0);
            builder.Property(e => e.MinutosUtilizados).HasColumnName("minutos_utilizados").HasDefaultValue(0);
            builder.Property(e => e.PermiteAgendamiento).HasColumnName("permite_agendamiento").HasDefaultValue(true);
            builder.Property(e => e.Observaciones).HasColumnName("observaciones").HasColumnType("text");
            builder.Property(e => e.SucursalId).HasColumnName("sucursal_id");
            builder.Property(e => e.UsuarioCreaId).HasColumnName("usuario_crea_id").HasMaxLength(450);
            builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            builder.Property(e => e.UsuarioModificaId).HasColumnName("usuario_modifica_id").HasMaxLength(450);
            builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");

            builder.HasOne(e => e.Sucursal).WithMany().HasForeignKey(e => e.SucursalId).OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(e => new { e.Fecha, e.Turno, e.SucursalId }).IsUnique();
            builder.Ignore(e => e.MinutosLibres);
            builder.Ignore(e => e.PorcentajeOcupacion);
        }
    }

    public class BloqueHorarioConfiguration : IEntityTypeConfiguration<BloqueHorario>
    {
        public void Configure(EntityTypeBuilder<BloqueHorario> builder)
        {
            builder.ToTable("bloques_horario");
            builder.HasKey(e => e.BloqueId);
            builder.Property(e => e.BloqueId).HasColumnName("bloque_id");
            builder.Property(e => e.CapacidadId).HasColumnName("capacidad_id").IsRequired();
            builder.Property(e => e.HoraInicio).HasColumnName("hora_inicio").IsRequired();
            builder.Property(e => e.HoraFin).HasColumnName("hora_fin").IsRequired();
            builder.Property(e => e.CapacidadMaximaVehiculos).HasColumnName("capacidad_maxima_vehiculos").HasDefaultValue(1);
            builder.Property(e => e.VehiculosAgendados).HasColumnName("vehiculos_agendados").HasDefaultValue(0);
            builder.Property(e => e.TipoBloque).HasColumnName("tipo_bloque").HasConversion<string>().HasMaxLength(30);
            builder.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);
            builder.Property(e => e.UsuarioCreaId).HasColumnName("usuario_crea_id").HasMaxLength(450);
            builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            builder.Property(e => e.UsuarioModificaId).HasColumnName("usuario_modifica_id").HasMaxLength(450);
            builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");

            builder.HasOne(e => e.Capacidad).WithMany(c => c.BloquesHorario).HasForeignKey(e => e.CapacidadId).OnDelete(DeleteBehavior.Cascade);
            builder.Ignore(e => e.EspaciosDisponibles);
            builder.Ignore(e => e.TieneEspacioDisponible);
            builder.Ignore(e => e.Duracion);
        }
    }

    public class CitaConfiguration : IEntityTypeConfiguration<Cita>
    {
        public void Configure(EntityTypeBuilder<Cita> builder)
        {
            builder.ToTable("citas");
            builder.HasKey(e => e.CitaId);
            builder.Property(e => e.CitaId).HasColumnName("cita_id");
            builder.Property(e => e.CodigoCita).HasColumnName("codigo_cita").HasMaxLength(50).IsRequired();
            builder.Property(e => e.ClienteId).HasColumnName("cliente_id").IsRequired();
            builder.Property(e => e.VehiculoId).HasColumnName("vehiculo_id").IsRequired();
            builder.Property(e => e.TipoServicioId).HasColumnName("tipo_servicio_id").IsRequired();
            builder.Property(e => e.FechaHoraInicio).HasColumnName("fecha_hora_inicio").IsRequired();
            builder.Property(e => e.FechaHoraFin).HasColumnName("fecha_hora_fin").IsRequired();
            builder.Property(e => e.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20);
            builder.Property(e => e.TipoIngreso).HasColumnName("tipo_ingreso").HasConversion<string>().HasMaxLength(20);
            builder.Property(e => e.MotivoVisita).HasColumnName("motivo_visita").HasColumnType("text").IsRequired();
            builder.Property(e => e.Observaciones).HasColumnName("observaciones").HasColumnType("text");
            builder.Property(e => e.MotivoCancelacion).HasColumnName("motivo_cancelacion").HasColumnType("text");
            builder.Property(e => e.PreOrdenId).HasColumnName("pre_orden_id").HasMaxLength(50);
            builder.Property(e => e.SucursalId).HasColumnName("sucursal_id");
            builder.Property(e => e.UsuarioCreaId).HasColumnName("usuario_crea_id").HasMaxLength(450);
            builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            builder.Property(e => e.UsuarioModificaId).HasColumnName("usuario_modifica_id").HasMaxLength(450);
            builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");

            builder.HasOne(e => e.Cliente).WithMany(c => c.Citas).HasForeignKey(e => e.ClienteId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Vehiculo).WithMany(v => v.Citas).HasForeignKey(e => e.VehiculoId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.TipoServicio).WithMany(t => t.Citas).HasForeignKey(e => e.TipoServicioId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Sucursal).WithMany().HasForeignKey(e => e.SucursalId).OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => e.CodigoCita).IsUnique();
            builder.HasIndex(e => e.FechaHoraInicio);
            builder.HasIndex(e => e.Estado);
            builder.Ignore(e => e.Duracion);
            builder.Ignore(e => e.EstaActiva);
            builder.Ignore(e => e.PuedeConvertirseEnOs);
        }
    }

    #endregion

}
