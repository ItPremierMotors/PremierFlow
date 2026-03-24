using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PremierFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModifyTableCRM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LineaNegocio",
                table: "sucursales",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_ultima_actividad",
                table: "leads",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "lead_id",
                table: "actividades_crm",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "vendedores_sucursal",
                columns: table => new
                {
                    vendedor_sucursal_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vendedor_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    sucursal_id = table.Column<int>(type: "int", nullable: false),
                    ultima_asignacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    esta_activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    UsuarioCreaId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioModificaId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vendedores_sucursal", x => x.vendedor_sucursal_id);
                    table.ForeignKey(
                        name: "FK_vendedores_sucursal_sucursales_sucursal_id",
                        column: x => x.sucursal_id,
                        principalTable: "sucursales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_vendedor_sucursal_unico",
                table: "vendedores_sucursal",
                columns: new[] { "vendedor_id", "sucursal_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vendedores_sucursal_sucursal_id",
                table: "vendedores_sucursal",
                column: "sucursal_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vendedores_sucursal");

            migrationBuilder.DropColumn(
                name: "LineaNegocio",
                table: "sucursales");

            migrationBuilder.DropColumn(
                name: "fecha_ultima_actividad",
                table: "leads");

            migrationBuilder.AlterColumn<int>(
                name: "lead_id",
                table: "actividades_crm",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
