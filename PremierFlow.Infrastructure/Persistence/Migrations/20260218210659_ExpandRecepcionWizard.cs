using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PremierFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ExpandRecepcionWizard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "antena",
                table: "recepciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "bateria_ok",
                table: "recepciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "danos_exterior_json",
                table: "recepciones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "es_propietario_quien_entrega",
                table: "recepciones",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "espejo_derecho",
                table: "recepciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "espejo_izquierdo",
                table: "recepciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "inspeccion_ruedas_json",
                table: "recepciones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "limpiaparabrisas",
                table: "recepciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "manual_vehiculo",
                table: "recepciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "nivel_aceite_ok",
                table: "recepciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "nivel_liquido_frenos_ok",
                table: "recepciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "nivel_refrigerante_ok",
                table: "recepciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "placa_delantera",
                table: "recepciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "placa_trasera",
                table: "recepciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "relacion_entregante",
                table: "recepciones",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "segunda_llave",
                table: "recepciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "tapa_combustible",
                table: "recepciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "telefono_entregante",
                table: "recepciones",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "antena",
                table: "recepciones");

            migrationBuilder.DropColumn(
                name: "bateria_ok",
                table: "recepciones");

            migrationBuilder.DropColumn(
                name: "danos_exterior_json",
                table: "recepciones");

            migrationBuilder.DropColumn(
                name: "es_propietario_quien_entrega",
                table: "recepciones");

            migrationBuilder.DropColumn(
                name: "espejo_derecho",
                table: "recepciones");

            migrationBuilder.DropColumn(
                name: "espejo_izquierdo",
                table: "recepciones");

            migrationBuilder.DropColumn(
                name: "inspeccion_ruedas_json",
                table: "recepciones");

            migrationBuilder.DropColumn(
                name: "limpiaparabrisas",
                table: "recepciones");

            migrationBuilder.DropColumn(
                name: "manual_vehiculo",
                table: "recepciones");

            migrationBuilder.DropColumn(
                name: "nivel_aceite_ok",
                table: "recepciones");

            migrationBuilder.DropColumn(
                name: "nivel_liquido_frenos_ok",
                table: "recepciones");

            migrationBuilder.DropColumn(
                name: "nivel_refrigerante_ok",
                table: "recepciones");

            migrationBuilder.DropColumn(
                name: "placa_delantera",
                table: "recepciones");

            migrationBuilder.DropColumn(
                name: "placa_trasera",
                table: "recepciones");

            migrationBuilder.DropColumn(
                name: "relacion_entregante",
                table: "recepciones");

            migrationBuilder.DropColumn(
                name: "segunda_llave",
                table: "recepciones");

            migrationBuilder.DropColumn(
                name: "tapa_combustible",
                table: "recepciones");

            migrationBuilder.DropColumn(
                name: "telefono_entregante",
                table: "recepciones");
        }
    }
}
