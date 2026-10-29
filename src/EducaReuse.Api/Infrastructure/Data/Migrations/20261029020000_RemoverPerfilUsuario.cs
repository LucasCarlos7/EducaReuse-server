using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducaReuse.Api.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoverPerfilUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Perfil",
                table: "usuarios");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Perfil",
                table: "usuarios",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Doador");
        }
    }
}
