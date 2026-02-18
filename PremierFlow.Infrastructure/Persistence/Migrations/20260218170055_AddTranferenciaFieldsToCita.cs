using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PremierFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTranferenciaFieldsToCita : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "cita_origen_id",
                table: "citas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "minutos_trabajados",
                table: "citas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_citas_cita_origen_id",
                table: "citas",
                column: "cita_origen_id");

            migrationBuilder.AddForeignKey(
                name: "FK_citas_citas_cita_origen_id",
                table: "citas",
                column: "cita_origen_id",
                principalTable: "citas",
                principalColumn: "cita_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_citas_citas_cita_origen_id",
                table: "citas");

            migrationBuilder.DropIndex(
                name: "IX_citas_cita_origen_id",
                table: "citas");

            migrationBuilder.DropColumn(
                name: "cita_origen_id",
                table: "citas");

            migrationBuilder.DropColumn(
                name: "minutos_trabajados",
                table: "citas");
        }
    }
}
