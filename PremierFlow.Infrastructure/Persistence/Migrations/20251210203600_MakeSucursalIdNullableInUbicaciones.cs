using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PremierFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeSucursalIdNullableInUbicaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ubicaciones_Sucursales_SucursalID",
                table: "Ubicaciones");

            migrationBuilder.AlterColumn<int>(
                name: "SucursalID",
                table: "Ubicaciones",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Ubicaciones_Sucursales_SucursalID",
                table: "Ubicaciones",
                column: "SucursalID",
                principalTable: "Sucursales",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ubicaciones_Sucursales_SucursalID",
                table: "Ubicaciones");

            migrationBuilder.AlterColumn<int>(
                name: "SucursalID",
                table: "Ubicaciones",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Ubicaciones_Sucursales_SucursalID",
                table: "Ubicaciones",
                column: "SucursalID",
                principalTable: "Sucursales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
