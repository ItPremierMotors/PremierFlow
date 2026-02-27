using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PremierFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHistorialAtsAddProximaRevisionToOs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "historial_ats");

            migrationBuilder.AddColumn<string>(
                name: "proxima_revision",
                table: "ordenes_servicio",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "proxima_revision",
                table: "ordenes_servicio");

            migrationBuilder.CreateTable(
                name: "historial_ats",
                columns: table => new
                {
                    ats_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    os_id = table.Column<int>(type: "int", nullable: false),
                    vehiculo_id = table.Column<int>(type: "int", nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    fecha_registro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fecha_servicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    kilometraje = table.Column<int>(type: "int", nullable: false),
                    monto_total = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    observaciones_tecnicas = table.Column<string>(type: "nvarchar(2000)", nullable: true),
                    proxima_revision = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    tipo_servicio = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    trabajos_realizados = table.Column<string>(type: "nvarchar(2000)", nullable: false),
                    usuario_crea_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    usuario_modifica_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historial_ats", x => x.ats_id);
                    table.ForeignKey(
                        name: "FK_historial_ats_ordenes_servicio_os_id",
                        column: x => x.os_id,
                        principalTable: "ordenes_servicio",
                        principalColumn: "os_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_historial_ats_vehiculos_vehiculo_id",
                        column: x => x.vehiculo_id,
                        principalTable: "vehiculos",
                        principalColumn: "vehiculo_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_historial_ats_os_id",
                table: "historial_ats",
                column: "os_id");

            migrationBuilder.CreateIndex(
                name: "IX_historial_ats_vehiculo_id",
                table: "historial_ats",
                column: "vehiculo_id");

            migrationBuilder.CreateIndex(
                name: "IX_historial_ats_vehiculo_id_fecha_servicio",
                table: "historial_ats",
                columns: new[] { "vehiculo_id", "fecha_servicio" });
        }
    }
}
