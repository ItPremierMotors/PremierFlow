using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PremierFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationFieldsToVehiculo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_limite_reserva",
                table: "vehiculos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_reserva",
                table: "vehiculos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reservado_por_id",
                table: "vehiculos",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fecha_limite_reserva",
                table: "vehiculos");

            migrationBuilder.DropColumn(
                name: "fecha_reserva",
                table: "vehiculos");

            migrationBuilder.DropColumn(
                name: "reservado_por_id",
                table: "vehiculos");
        }
    }
}
