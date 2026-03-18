using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PremierFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddModeloVehiculoCRM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "modelo_id",
                table: "oportunidades",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_oportunidades_modelo_id",
                table: "oportunidades",
                column: "modelo_id");

            migrationBuilder.AddForeignKey(
                name: "FK_oportunidades_modelos_modelo_id",
                table: "oportunidades",
                column: "modelo_id",
                principalTable: "modelos",
                principalColumn: "modelo_id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_oportunidades_modelos_modelo_id",
                table: "oportunidades");

            migrationBuilder.DropIndex(
                name: "IX_oportunidades_modelo_id",
                table: "oportunidades");

            migrationBuilder.DropColumn(
                name: "modelo_id",
                table: "oportunidades");
        }
    }
}
