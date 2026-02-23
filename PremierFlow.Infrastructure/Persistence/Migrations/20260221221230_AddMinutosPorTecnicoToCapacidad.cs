using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PremierFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMinutosPorTecnicoToCapacidad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "minutos_por_tecnico",
                table: "capacidad_taller",
                type: "int",
                nullable: false,
                defaultValue: 480);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "minutos_por_tecnico",
                table: "capacidad_taller");
        }
    }
}
