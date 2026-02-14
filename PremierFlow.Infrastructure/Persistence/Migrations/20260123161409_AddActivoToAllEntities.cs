using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PremierFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddActivoToAllEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "activo",
                table: "recepciones",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "activo",
                table: "os_servicios",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "activo",
                table: "ordenes_servicio",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "activo",
                table: "historial_ats",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "activo",
                table: "evidencias",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "activo",
                table: "citas",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "activo",
                table: "capacidad_taller",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "activo",
                table: "asignaciones_tecnico",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "activo",
                table: "recepciones");

            migrationBuilder.DropColumn(
                name: "activo",
                table: "os_servicios");

            migrationBuilder.DropColumn(
                name: "activo",
                table: "ordenes_servicio");

            migrationBuilder.DropColumn(
                name: "activo",
                table: "historial_ats");

            migrationBuilder.DropColumn(
                name: "activo",
                table: "evidencias");

            migrationBuilder.DropColumn(
                name: "activo",
                table: "citas");

            migrationBuilder.DropColumn(
                name: "activo",
                table: "capacidad_taller");

            migrationBuilder.DropColumn(
                name: "activo",
                table: "asignaciones_tecnico");
        }
    }
}
