using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PremierFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceDocumentoIdentidadWithDNIAndRTN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_clientes_documento_identidad",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "documento_identidad",
                table: "clientes");

            migrationBuilder.RenameIndex(
                name: "IX_clientes_telefono",
                table: "clientes",
                newName: "ix_clientes_telefono");

            migrationBuilder.RenameIndex(
                name: "IX_clientes_email",
                table: "clientes",
                newName: "ix_clientes_email");

            migrationBuilder.AlterColumn<string>(
                name: "direccion",
                table: "clientes",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ciudad",
                table: "clientes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "dni",
                table: "clientes",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rtn",
                table: "clientes",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "telefono_secundario",
                table: "clientes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_clientes_dni",
                table: "clientes",
                column: "dni");

            migrationBuilder.CreateIndex(
                name: "ix_clientes_rtn",
                table: "clientes",
                column: "rtn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_clientes_dni",
                table: "clientes");

            migrationBuilder.DropIndex(
                name: "ix_clientes_rtn",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "ciudad",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "dni",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "rtn",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "telefono_secundario",
                table: "clientes");

            migrationBuilder.RenameIndex(
                name: "ix_clientes_telefono",
                table: "clientes",
                newName: "IX_clientes_telefono");

            migrationBuilder.RenameIndex(
                name: "ix_clientes_email",
                table: "clientes",
                newName: "IX_clientes_email");

            migrationBuilder.AlterColumn<string>(
                name: "direccion",
                table: "clientes",
                type: "nvarchar(500)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "documento_identidad",
                table: "clientes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_clientes_documento_identidad",
                table: "clientes",
                column: "documento_identidad",
                unique: true,
                filter: "[documento_identidad] IS NOT NULL");
        }
    }
}
