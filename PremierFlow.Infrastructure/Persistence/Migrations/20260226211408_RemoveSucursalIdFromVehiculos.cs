using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PremierFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSucursalIdFromVehiculos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_vehiculos_sucursales_sucursal_id",
                table: "vehiculos");

            migrationBuilder.DropIndex(
                name: "ix_vehiculos_sucursal_id",
                table: "vehiculos");

            migrationBuilder.DropColumn(
                name: "sucursal_id",
                table: "vehiculos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "sucursal_id",
                table: "vehiculos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_vehiculos_sucursal_id",
                table: "vehiculos",
                column: "sucursal_id");

            migrationBuilder.AddForeignKey(
                name: "FK_vehiculos_sucursales_sucursal_id",
                table: "vehiculos",
                column: "sucursal_id",
                principalTable: "sucursales",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
