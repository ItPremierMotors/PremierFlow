using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PremierFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Infrastructure.Configurations
{
    #region Orden de Servicio

    public class OrdenServicioConfiguration : IEntityTypeConfiguration<OrdenServicio>
    {
        public void Configure(EntityTypeBuilder<OrdenServicio> builder)
        {
            builder.ToTable("ordenes_servicio");
            builder.HasKey(e => e.OsId);
            builder.Property(e => e.OsId).HasColumnName("os_id");
            builder.Property(e => e.NumeroOs).HasColumnName("numero_os").HasMaxLength(50).IsRequired();
            builder.Property(e => e.CitaId).HasColumnName("cita_id");
            builder.Property(e => e.VehiculoId).HasColumnName("vehiculo_id").IsRequired();
            builder.Property(e => e.ClienteId).HasColumnName("cliente_id").IsRequired();
            builder.Property(e => e.FechaApertura).HasColumnName("fecha_apertura");
            builder.Property(e => e.FechaCierre).HasColumnName("fecha_cierre");
            builder.Property(e => e.EstadoId).HasColumnName("estado_id").IsRequired();
            builder.Property(e => e.KilometrajeIngreso).HasColumnName("kilometraje_ingreso").IsRequired();
            builder.Property(e => e.NivelCombustible).HasColumnName("nivel_combustible").HasPrecision(3, 2);
            builder.Property(e => e.TipoIngreso).HasColumnName("tipo_ingreso").HasConversion<string>().HasMaxLength(20);
            builder.Property(e => e.EsGarantia).HasColumnName("es_garantia").HasDefaultValue(false);
            builder.Property(e => e.ObservacionesApertura).HasColumnName("observaciones_apertura").HasColumnType("nvarchar(2000)");
            builder.Property(e => e.ObservacionesCierre).HasColumnName("observaciones_cierre").HasColumnType("nvarchar(2000)");
            builder.Property(e => e.TotalManoObra).HasColumnName("total_mano_obra").HasPrecision(10, 2).HasDefaultValue(0);
            builder.Property(e => e.TotalRepuestos).HasColumnName("total_repuestos").HasPrecision(10, 2).HasDefaultValue(0);
            builder.Property(e => e.TotalGeneral).HasColumnName("total_general").HasPrecision(10, 2).HasDefaultValue(0);
            builder.Property(e => e.SucursalId).HasColumnName("sucursal_id");
            builder.Property(e => e.ProximaRevision).HasColumnName("proxima_revision").HasMaxLength(200);
            builder.Property(e => e.UsuarioCreaId).HasColumnName("usuario_crea_id").HasMaxLength(450);
            builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            builder.Property(e => e.UsuarioModificaId).HasColumnName("usuario_modifica_id").HasMaxLength(450);
            builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");
            builder.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);
            builder.HasOne(e => e.Cita).WithOne(c => c.OrdenServicio).HasForeignKey<OrdenServicio>(e => e.CitaId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Vehiculo).WithMany(v => v.OrdenesServicio).HasForeignKey(e => e.VehiculoId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Cliente).WithMany(c => c.OrdenesServicio).HasForeignKey(e => e.ClienteId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Estado).WithMany(s => s.OrdenesServicio).HasForeignKey(e => e.EstadoId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Sucursal).WithMany().HasForeignKey(e => e.SucursalId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Recepcion).WithOne(r => r.OrdenServicio).HasForeignKey<Recepcion>(r => r.OsId).OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.NumeroOs).IsUnique();
            builder.HasIndex(e => e.VehiculoId);
            builder.HasIndex(e => e.ClienteId);
            builder.HasIndex(e => e.FechaApertura);
            builder.HasIndex(e => e.EstadoId);
            builder.Ignore(e => e.EsWalkIn);
            builder.Ignore(e => e.EstaAbierta);
            builder.Ignore(e => e.PuedeModificarse);
            builder.Ignore(e => e.NivelCombustiblePorcentaje);
        }
    }

    public class RecepcionConfiguration : IEntityTypeConfiguration<Recepcion>
    {
        public void Configure(EntityTypeBuilder<Recepcion> builder)
        {
            builder.ToTable("recepciones");
            builder.HasKey(e => e.RecepcionId);
            builder.Property(e => e.RecepcionId).HasColumnName("recepcion_id");
            builder.Property(e => e.OsId).HasColumnName("os_id").IsRequired();
            builder.Property(e => e.FechaHoraRecepcion).HasColumnName("fecha_hora_recepcion");
            builder.Property(e => e.RecibidoPorId).HasColumnName("recibido_por_id").HasMaxLength(450).IsRequired();
            builder.Property(e => e.EntregadoPor).HasColumnName("entregado_por").HasMaxLength(100).IsRequired();

            // Confirmación entrega
            builder.Property(e => e.EsPropietarioQuienEntrega).HasColumnName("es_propietario_quien_entrega").HasDefaultValue(true);
            builder.Property(e => e.RelacionEntregante).HasColumnName("relacion_entregante").HasMaxLength(100);
            builder.Property(e => e.TelefonoEntregante).HasColumnName("telefono_entregante").HasMaxLength(20);

            builder.Property(e => e.EstadoCarroceria).HasColumnName("estado_carroceria").HasColumnType("nvarchar(2000)");
            builder.Property(e => e.AccesoriosRecibidos).HasColumnName("accesorios_recibidos").HasColumnType("nvarchar(2000)");

            // Daños exteriores JSON
            builder.Property(e => e.DanosExteriorJson).HasColumnName("danos_exterior_json").HasColumnType("nvarchar(max)");

            builder.Property(e => e.LlantaRepuesto).HasColumnName("llanta_repuesto").HasDefaultValue(false);
            builder.Property(e => e.Gato).HasColumnName("gato").HasDefaultValue(false);
            builder.Property(e => e.Triangulos).HasColumnName("triangulos").HasDefaultValue(false);
            builder.Property(e => e.Extintor).HasColumnName("extintor").HasDefaultValue(false);
            builder.Property(e => e.Herramientas).HasColumnName("herramientas").HasDefaultValue(false);
            builder.Property(e => e.Radio).HasColumnName("radio").HasDefaultValue(false);
            builder.Property(e => e.Tapetes).HasColumnName("tapetes").HasDefaultValue(false);

            // Checklist extendido - Exterior
            builder.Property(e => e.Antena).HasColumnName("antena").HasDefaultValue(false);
            builder.Property(e => e.EspejoIzquierdo).HasColumnName("espejo_izquierdo").HasDefaultValue(false);
            builder.Property(e => e.EspejoDerecho).HasColumnName("espejo_derecho").HasDefaultValue(false);
            builder.Property(e => e.Limpiaparabrisas).HasColumnName("limpiaparabrisas").HasDefaultValue(false);
            builder.Property(e => e.PlacaDelantera).HasColumnName("placa_delantera").HasDefaultValue(false);
            builder.Property(e => e.PlacaTrasera).HasColumnName("placa_trasera").HasDefaultValue(false);
            builder.Property(e => e.TapaCombustible).HasColumnName("tapa_combustible").HasDefaultValue(false);

            // Documentos/Extras
            builder.Property(e => e.ManualVehiculo).HasColumnName("manual_vehiculo").HasDefaultValue(false);
            builder.Property(e => e.SegundaLlave).HasColumnName("segunda_llave").HasDefaultValue(false);

            // Inspección ruedas JSON
            builder.Property(e => e.InspeccionRuedasJson).HasColumnName("inspeccion_ruedas_json").HasColumnType("nvarchar(max)");

            // Motor
            builder.Property(e => e.NivelAceiteOk).HasColumnName("nivel_aceite_ok").HasDefaultValue(false);
            builder.Property(e => e.NivelRefrigeranteOk).HasColumnName("nivel_refrigerante_ok").HasDefaultValue(false);
            builder.Property(e => e.NivelLiquidoFrenosOk).HasColumnName("nivel_liquido_frenos_ok").HasDefaultValue(false);
            builder.Property(e => e.BateriaOk).HasColumnName("bateria_ok").HasDefaultValue(false);

            builder.Property(e => e.ObservacionesGenerales).HasColumnName("observaciones_generales").HasColumnType("nvarchar(2000)");
            builder.Property(e => e.FirmaClienteBase64).HasColumnName("firma_cliente_base64").HasColumnType("nvarchar(max)");
            builder.Property(e => e.ChecklistCompletado).HasColumnName("checklist_completado").HasDefaultValue(false);
            builder.Property(e => e.UsuarioCreaId).HasColumnName("usuario_crea_id").HasMaxLength(450);
            builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            builder.Property(e => e.UsuarioModificaId).HasColumnName("usuario_modifica_id").HasMaxLength(450);
            builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");
            builder.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);
            builder.HasIndex(e => e.OsId).IsUnique();
            builder.Ignore(e => e.TieneFirmaCliente);
            builder.Ignore(e => e.CantidadAccesoriosVerificados);
            builder.Ignore(e => e.EstaCompleta);
        }
    }

    public class EvidenciaConfiguration : IEntityTypeConfiguration<Evidencia>
    {
        public void Configure(EntityTypeBuilder<Evidencia> builder)
        {
            builder.ToTable("evidencias");
            builder.HasKey(e => e.EvidenciaId);
            builder.Property(e => e.EvidenciaId).HasColumnName("evidencia_id");
            builder.Property(e => e.OsId).HasColumnName("os_id").IsRequired();
            builder.Property(e => e.RecepcionId).HasColumnName("recepcion_id");
            builder.Property(e => e.TipoEvidencia).HasColumnName("tipo_evidencia").HasConversion<string>().HasMaxLength(30);
            builder.Property(e => e.UrlArchivo).HasColumnName("url_archivo").HasMaxLength(500).IsRequired();
            builder.Property(e => e.Descripcion).HasColumnName("descripcion").HasColumnType("nvarchar(1000)");
            builder.Property(e => e.FechaCaptura).HasColumnName("fecha_captura");
            builder.Property(e => e.UsuarioRegistroId).HasColumnName("usuario_registro_id").HasMaxLength(450);
            builder.Property(e => e.UsuarioCreaId).HasColumnName("usuario_crea_id").HasMaxLength(450);
            builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            builder.Property(e => e.UsuarioModificaId).HasColumnName("usuario_modifica_id").HasMaxLength(450);
            builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");
            builder.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);
            builder.HasOne(e => e.OrdenServicio).WithMany(o => o.Evidencias).HasForeignKey(e => e.OsId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(e => e.Recepcion).WithMany(r => r.Evidencias).HasForeignKey(e => e.RecepcionId).OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(e => e.OsId);
            builder.Ignore(e => e.EsFotoRecepcion);
            builder.Ignore(e => e.EsFotoDano);
            builder.Ignore(e => e.NombreArchivo);
            builder.Ignore(e => e.Extension);
            builder.Ignore(e => e.EsImagen);
        }
    }

    #endregion

    #region Trabajo y Asignaciones

    public class OsServicioConfiguration : IEntityTypeConfiguration<OsServicio>
    {
        public void Configure(EntityTypeBuilder<OsServicio> builder)
        {
            builder.ToTable("os_servicios");
            builder.HasKey(e => e.OsServicioId);
            builder.Property(e => e.OsServicioId).HasColumnName("os_servicio_id");
            builder.Property(e => e.OsId).HasColumnName("os_id").IsRequired();
            builder.Property(e => e.TipoServicioId).HasColumnName("tipo_servicio_id").IsRequired();
            builder.Property(e => e.DescripcionTrabajo).HasColumnName("descripcion_trabajo").HasColumnType("nvarchar(3000)").IsRequired();
            builder.Property(e => e.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20);
            builder.Property(e => e.PrecioUnitario).HasColumnName("precio_unitario").HasPrecision(10, 2);
            builder.Property(e => e.Cantidad).HasColumnName("cantidad").HasDefaultValue(1);
            builder.Property(e => e.Subtotal).HasColumnName("subtotal").HasPrecision(10, 2);
            builder.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
            builder.Property(e => e.FechaFin).HasColumnName("fecha_fin");
            builder.Property(e => e.TecnicoAsignadoId).HasColumnName("tecnico_asignado_id");
            builder.Property(e => e.Observaciones).HasColumnName("observaciones").HasColumnType("nvarchar(1000)");
            builder.Property(e => e.UsuarioCreaId).HasColumnName("usuario_crea_id").HasMaxLength(450);
            builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            builder.Property(e => e.UsuarioModificaId).HasColumnName("usuario_modifica_id").HasMaxLength(450);
            builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");
            builder.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);
            builder.HasOne(e => e.OrdenServicio).WithMany(o => o.Servicios).HasForeignKey(e => e.OsId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(e => e.TipoServicio).WithMany(t => t.OsServicios).HasForeignKey(e => e.TipoServicioId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Tecnico).WithMany(t => t.ServiciosAsignados).HasForeignKey(e => e.TecnicoAsignadoId).OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(e => e.OsId);
            builder.Ignore(e => e.EstaCompletado);
            builder.Ignore(e => e.EstaCancelado);
            builder.Ignore(e => e.EstaEnProceso);
            builder.Ignore(e => e.TiempoTrabajo);
        }
    }

    public class AsignacionTecnicoConfiguration : IEntityTypeConfiguration<AsignacionTecnico>
    {
        public void Configure(EntityTypeBuilder<AsignacionTecnico> builder)
        {
            builder.ToTable("asignaciones_tecnico");
            builder.HasKey(e => e.AsignacionId);
            builder.Property(e => e.AsignacionId).HasColumnName("asignacion_id");
            builder.Property(e => e.OsId).HasColumnName("os_id").IsRequired();
            builder.Property(e => e.TecnicoId).HasColumnName("tecnico_id").IsRequired();
            builder.Property(e => e.OsServicioId).HasColumnName("os_servicio_id");
            builder.Property(e => e.FechaAsignacion).HasColumnName("fecha_asignacion");
            builder.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
            builder.Property(e => e.FechaFin).HasColumnName("fecha_fin");
            builder.Property(e => e.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20);
            builder.Property(e => e.Observaciones).HasColumnName("observaciones").HasColumnType("nvarchar(1000)");
            builder.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);
            builder.Property(e => e.UsuarioCreaId).HasColumnName("usuario_crea_id").HasMaxLength(450);
            builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            builder.Property(e => e.UsuarioModificaId).HasColumnName("usuario_modifica_id").HasMaxLength(450);
            builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");

            builder.HasOne(e => e.OrdenServicio).WithMany(o => o.AsignacionesTecnico).HasForeignKey(e => e.OsId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(e => e.Tecnico).WithMany(t => t.Asignaciones).HasForeignKey(e => e.TecnicoId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.OsServicio).WithMany(s => s.Asignaciones).HasForeignKey(e => e.OsServicioId).OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(e => e.OsId);
            builder.HasIndex(e => e.TecnicoId);
            builder.Ignore(e => e.EstaActiva);
            builder.Ignore(e => e.TiempoTrabajado);
        }
    }

    #endregion

}
