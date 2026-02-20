using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PremierFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCapacidadBloqueFK_EstadoTransferida : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "bloque_horario_id",
                table: "citas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "capacidad_id",
                table: "citas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_citas_bloque_horario_id",
                table: "citas",
                column: "bloque_horario_id");

            migrationBuilder.CreateIndex(
                name: "IX_citas_capacidad_id",
                table: "citas",
                column: "capacidad_id");

            migrationBuilder.AddForeignKey(
                name: "FK_citas_bloques_horario_bloque_horario_id",
                table: "citas",
                column: "bloque_horario_id",
                principalTable: "bloques_horario",
                principalColumn: "bloque_id");

            migrationBuilder.AddForeignKey(
                name: "FK_citas_capacidad_taller_capacidad_id",
                table: "citas",
                column: "capacidad_id",
                principalTable: "capacidad_taller",
                principalColumn: "capacidad_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_citas_bloques_horario_bloque_horario_id",
                table: "citas");

            migrationBuilder.DropForeignKey(
                name: "FK_citas_capacidad_taller_capacidad_id",
                table: "citas");

            migrationBuilder.DropIndex(
                name: "IX_citas_bloque_horario_id",
                table: "citas");

            migrationBuilder.DropIndex(
                name: "IX_citas_capacidad_id",
                table: "citas");

            migrationBuilder.DropColumn(
                name: "bloque_horario_id",
                table: "citas");

            migrationBuilder.DropColumn(
                name: "capacidad_id",
                table: "citas");
        }
    }
}
