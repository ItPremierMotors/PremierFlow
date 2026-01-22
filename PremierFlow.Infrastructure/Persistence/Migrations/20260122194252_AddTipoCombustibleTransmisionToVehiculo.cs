using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PremierFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoCombustibleTransmisionToVehiculo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_vehiculos_vin",
                table: "vehiculos",
                newName: "ix_vehiculos_vin");

            migrationBuilder.RenameIndex(
                name: "IX_vehiculos_sucursal_id",
                table: "vehiculos",
                newName: "ix_vehiculos_sucursal_id");

            migrationBuilder.RenameIndex(
                name: "IX_vehiculos_placa",
                table: "vehiculos",
                newName: "ix_vehiculos_placa");

            migrationBuilder.RenameIndex(
                name: "IX_vehiculos_marca_id",
                table: "vehiculos",
                newName: "ix_vehiculos_marca_id");

            migrationBuilder.RenameIndex(
                name: "IX_vehiculos_estado",
                table: "vehiculos",
                newName: "ix_vehiculos_estado");

            migrationBuilder.RenameIndex(
                name: "IX_vehiculos_cliente_id",
                table: "vehiculos",
                newName: "ix_vehiculos_cliente_id");

            migrationBuilder.AlterColumn<decimal>(
                name: "precio_venta",
                table: "vehiculos",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,2)",
                oldPrecision: 12,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "precio_lista",
                table: "vehiculos",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,2)",
                oldPrecision: 12,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "placa",
                table: "vehiculos",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "observaciones",
                table: "vehiculos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "costo_importacion",
                table: "vehiculos",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,2)",
                oldPrecision: 12,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tipo_combustible",
                table: "vehiculos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "transmision",
                table: "vehiculos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tipo_combustible",
                table: "vehiculos");

            migrationBuilder.DropColumn(
                name: "transmision",
                table: "vehiculos");

            migrationBuilder.RenameIndex(
                name: "ix_vehiculos_vin",
                table: "vehiculos",
                newName: "IX_vehiculos_vin");

            migrationBuilder.RenameIndex(
                name: "ix_vehiculos_sucursal_id",
                table: "vehiculos",
                newName: "IX_vehiculos_sucursal_id");

            migrationBuilder.RenameIndex(
                name: "ix_vehiculos_placa",
                table: "vehiculos",
                newName: "IX_vehiculos_placa");

            migrationBuilder.RenameIndex(
                name: "ix_vehiculos_marca_id",
                table: "vehiculos",
                newName: "IX_vehiculos_marca_id");

            migrationBuilder.RenameIndex(
                name: "ix_vehiculos_estado",
                table: "vehiculos",
                newName: "IX_vehiculos_estado");

            migrationBuilder.RenameIndex(
                name: "ix_vehiculos_cliente_id",
                table: "vehiculos",
                newName: "IX_vehiculos_cliente_id");

            migrationBuilder.AlterColumn<decimal>(
                name: "precio_venta",
                table: "vehiculos",
                type: "decimal(12,2)",
                precision: 12,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "precio_lista",
                table: "vehiculos",
                type: "decimal(12,2)",
                precision: 12,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "placa",
                table: "vehiculos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "observaciones",
                table: "vehiculos",
                type: "nvarchar(1000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "costo_importacion",
                table: "vehiculos",
                type: "decimal(12,2)",
                precision: 12,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);
        }
    }
}
