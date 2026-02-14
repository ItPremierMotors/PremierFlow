using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PremierFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixUniqueIndexSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tipos_servicio_codigo",
                table: "tipos_servicio");

            migrationBuilder.DropIndex(
                name: "IX_tecnicos_codigo",
                table: "tecnicos");

            migrationBuilder.CreateIndex(
                name: "IX_tipos_servicio_codigo",
                table: "tipos_servicio",
                column: "codigo",
                unique: true,
                filter: "[activo] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_tecnicos_codigo",
                table: "tecnicos",
                column: "codigo",
                unique: true,
                filter: "[activo] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tipos_servicio_codigo",
                table: "tipos_servicio");

            migrationBuilder.DropIndex(
                name: "IX_tecnicos_codigo",
                table: "tecnicos");

            migrationBuilder.CreateIndex(
                name: "IX_tipos_servicio_codigo",
                table: "tipos_servicio",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tecnicos_codigo",
                table: "tecnicos",
                column: "codigo",
                unique: true);
        }
    }
}
