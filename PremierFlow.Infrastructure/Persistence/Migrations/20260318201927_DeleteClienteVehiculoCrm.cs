using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PremierFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DeleteClienteVehiculoCrm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_leads_clientes_cliente_id",
                table: "leads");

            migrationBuilder.DropForeignKey(
                name: "FK_oportunidades_clientes_cliente_id",
                table: "oportunidades");

            migrationBuilder.DropForeignKey(
                name: "FK_oportunidades_vehiculos_vehiculo_id",
                table: "oportunidades");

            migrationBuilder.DropIndex(
                name: "IX_oportunidades_cliente_id",
                table: "oportunidades");

            migrationBuilder.DropIndex(
                name: "IX_oportunidades_vehiculo_id",
                table: "oportunidades");

            migrationBuilder.DropIndex(
                name: "IX_leads_cliente_id",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "cliente_id",
                table: "oportunidades");

            migrationBuilder.DropColumn(
                name: "vehiculo_id",
                table: "oportunidades");

            migrationBuilder.DropColumn(
                name: "cliente_id",
                table: "leads");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "cliente_id",
                table: "oportunidades",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "vehiculo_id",
                table: "oportunidades",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "cliente_id",
                table: "leads",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_oportunidades_cliente_id",
                table: "oportunidades",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_oportunidades_vehiculo_id",
                table: "oportunidades",
                column: "vehiculo_id");

            migrationBuilder.CreateIndex(
                name: "IX_leads_cliente_id",
                table: "leads",
                column: "cliente_id");

            migrationBuilder.AddForeignKey(
                name: "FK_leads_clientes_cliente_id",
                table: "leads",
                column: "cliente_id",
                principalTable: "clientes",
                principalColumn: "cliente_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_oportunidades_clientes_cliente_id",
                table: "oportunidades",
                column: "cliente_id",
                principalTable: "clientes",
                principalColumn: "cliente_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_oportunidades_vehiculos_vehiculo_id",
                table: "oportunidades",
                column: "vehiculo_id",
                principalTable: "vehiculos",
                principalColumn: "vehiculo_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
