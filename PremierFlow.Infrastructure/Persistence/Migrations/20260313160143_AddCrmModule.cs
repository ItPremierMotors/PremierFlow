using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PremierFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCrmModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "leads",
                columns: table => new
                {
                    lead_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    codigo_lead = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    nombre_completo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    empresa = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ciudad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    origen = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    detalle_origen = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    sucursal_id = table.Column<int>(type: "int", nullable: false),
                    vendedor_asignado_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    fecha_ingreso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fecha_primera_respuesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    fecha_conversion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    fecha_descarte = table.Column<DateTime>(type: "datetime2", nullable: true),
                    motivo_descarte = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    vehiculo_interes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    presupuesto_estimado = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    cliente_id = table.Column<int>(type: "int", nullable: true),
                    usuario_crea_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_modifica_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leads", x => x.lead_id);
                    table.ForeignKey(
                        name: "FK_leads_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "cliente_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_leads_sucursales_sucursal_id",
                        column: x => x.sucursal_id,
                        principalTable: "sucursales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "oportunidades",
                columns: table => new
                {
                    oportunidad_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    codigo_oportunidad = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    lead_id = table.Column<int>(type: "int", nullable: false),
                    cliente_id = table.Column<int>(type: "int", nullable: true),
                    vehiculo_id = table.Column<int>(type: "int", nullable: true),
                    vendedor_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    sucursal_id = table.Column<int>(type: "int", nullable: false),
                    etapa = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    probabilidad_cierre = table.Column<int>(type: "int", nullable: false),
                    fecha_cierre_estimada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    resultado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    motivo_resultado = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    fecha_cierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    fecha_ultima_actividad = table.Column<DateTime>(type: "datetime2", nullable: true),
                    usuario_crea_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_modifica_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_oportunidades", x => x.oportunidad_id);
                    table.ForeignKey(
                        name: "FK_oportunidades_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "cliente_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_oportunidades_leads_lead_id",
                        column: x => x.lead_id,
                        principalTable: "leads",
                        principalColumn: "lead_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_oportunidades_sucursales_sucursal_id",
                        column: x => x.sucursal_id,
                        principalTable: "sucursales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_oportunidades_vehiculos_vehiculo_id",
                        column: x => x.vehiculo_id,
                        principalTable: "vehiculos",
                        principalColumn: "vehiculo_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "actividades_crm",
                columns: table => new
                {
                    actividad_crm_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    lead_id = table.Column<int>(type: "int", nullable: true),
                    oportunidad_id = table.Column<int>(type: "int", nullable: true),
                    realizada_por_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    tipo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    direccion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    asunto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(2000)", nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    fecha_programada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    fecha_realizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    duracion_minutos = table.Column<int>(type: "int", nullable: true),
                    resultado = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    proximo_contacto = table.Column<DateTime>(type: "datetime2", nullable: true),
                    usuario_crea_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_modifica_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_actividades_crm", x => x.actividad_crm_id);
                    table.ForeignKey(
                        name: "FK_actividades_crm_leads_lead_id",
                        column: x => x.lead_id,
                        principalTable: "leads",
                        principalColumn: "lead_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_actividades_crm_oportunidades_oportunidad_id",
                        column: x => x.oportunidad_id,
                        principalTable: "oportunidades",
                        principalColumn: "oportunidad_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cotizaciones_vehiculo",
                columns: table => new
                {
                    cotizacion_vehiculo_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    codigo_cotizacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    oportunidad_id = table.Column<int>(type: "int", nullable: false),
                    vehiculo_id = table.Column<int>(type: "int", nullable: false),
                    descuento = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    precio_ofertado = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    condiciones_pago = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    observaciones = table.Column<string>(type: "nvarchar(2000)", nullable: true),
                    fecha_emision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fecha_vencimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    usuario_crea_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_modifica_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cotizaciones_vehiculo", x => x.cotizacion_vehiculo_id);
                    table.ForeignKey(
                        name: "FK_cotizaciones_vehiculo_oportunidades_oportunidad_id",
                        column: x => x.oportunidad_id,
                        principalTable: "oportunidades",
                        principalColumn: "oportunidad_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_cotizaciones_vehiculo_vehiculos_vehiculo_id",
                        column: x => x.vehiculo_id,
                        principalTable: "vehiculos",
                        principalColumn: "vehiculo_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notas_crm",
                columns: table => new
                {
                    nota_crm_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    lead_id = table.Column<int>(type: "int", nullable: true),
                    oportunidad_id = table.Column<int>(type: "int", nullable: true),
                    autor_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    contenido = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                    es_privada = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    usuario_crea_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usuario_modifica_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notas_crm", x => x.nota_crm_id);
                    table.ForeignKey(
                        name: "FK_notas_crm_leads_lead_id",
                        column: x => x.lead_id,
                        principalTable: "leads",
                        principalColumn: "lead_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_notas_crm_oportunidades_oportunidad_id",
                        column: x => x.oportunidad_id,
                        principalTable: "oportunidades",
                        principalColumn: "oportunidad_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_actividades_crm_estado",
                table: "actividades_crm",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "ix_actividades_crm_lead",
                table: "actividades_crm",
                column: "lead_id");

            migrationBuilder.CreateIndex(
                name: "ix_actividades_crm_oportunidad",
                table: "actividades_crm",
                column: "oportunidad_id");

            migrationBuilder.CreateIndex(
                name: "ix_actividades_crm_vendedor",
                table: "actividades_crm",
                column: "realizada_por_id");

            migrationBuilder.CreateIndex(
                name: "ix_cotizaciones_codigo",
                table: "cotizaciones_vehiculo",
                column: "codigo_cotizacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cotizaciones_oportunidad",
                table: "cotizaciones_vehiculo",
                column: "oportunidad_id");

            migrationBuilder.CreateIndex(
                name: "IX_cotizaciones_vehiculo_vehiculo_id",
                table: "cotizaciones_vehiculo",
                column: "vehiculo_id");

            migrationBuilder.CreateIndex(
                name: "IX_leads_cliente_id",
                table: "leads",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "ix_leads_codigo",
                table: "leads",
                column: "codigo_lead",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_leads_estado",
                table: "leads",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "ix_leads_sucursal",
                table: "leads",
                column: "sucursal_id");

            migrationBuilder.CreateIndex(
                name: "ix_leads_vendedor",
                table: "leads",
                column: "vendedor_asignado_id");

            migrationBuilder.CreateIndex(
                name: "ix_notas_crm_lead",
                table: "notas_crm",
                column: "lead_id");

            migrationBuilder.CreateIndex(
                name: "ix_notas_crm_oportunidad",
                table: "notas_crm",
                column: "oportunidad_id");

            migrationBuilder.CreateIndex(
                name: "IX_oportunidades_cliente_id",
                table: "oportunidades",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "ix_oportunidades_codigo",
                table: "oportunidades",
                column: "codigo_oportunidad",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_oportunidades_etapa",
                table: "oportunidades",
                column: "etapa");

            migrationBuilder.CreateIndex(
                name: "IX_oportunidades_lead_id",
                table: "oportunidades",
                column: "lead_id");

            migrationBuilder.CreateIndex(
                name: "ix_oportunidades_sucursal",
                table: "oportunidades",
                column: "sucursal_id");

            migrationBuilder.CreateIndex(
                name: "IX_oportunidades_vehiculo_id",
                table: "oportunidades",
                column: "vehiculo_id");

            migrationBuilder.CreateIndex(
                name: "ix_oportunidades_vendedor",
                table: "oportunidades",
                column: "vendedor_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "actividades_crm");

            migrationBuilder.DropTable(
                name: "cotizaciones_vehiculo");

            migrationBuilder.DropTable(
                name: "notas_crm");

            migrationBuilder.DropTable(
                name: "oportunidades");

            migrationBuilder.DropTable(
                name: "leads");
        }
    }
}
