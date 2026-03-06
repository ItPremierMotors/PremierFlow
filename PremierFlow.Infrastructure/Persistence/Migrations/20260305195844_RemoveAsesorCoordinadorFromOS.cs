using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PremierFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAsesorCoordinadorFromOS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "asesor_id",
                table: "ordenes_servicio");

            migrationBuilder.DropColumn(
                name: "coordinador_id",
                table: "ordenes_servicio");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "asesor_id",
                table: "ordenes_servicio",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "coordinador_id",
                table: "ordenes_servicio",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);
        }
    }
}
