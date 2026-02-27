using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PremierFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUsuarioSucursal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsuarioSucursales");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsuarioSucursales",
                columns: table => new
                {
                    UsuarioID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SucursalID = table.Column<int>(type: "int", nullable: false),
                    EsSucursualPrincipal = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioSucursales", x => new { x.UsuarioID, x.SucursalID });
                    table.ForeignKey(
                        name: "FK_UsuarioSucursales_AspNetUsers_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioSucursales_sucursales_SucursalID",
                        column: x => x.SucursalID,
                        principalTable: "sucursales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSucursales_SucursalID",
                table: "UsuarioSucursales",
                column: "SucursalID");
        }
    }
}
