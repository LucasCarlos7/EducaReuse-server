using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducaReuse.Api.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarImagemMaterial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Imagem",
                table: "materiais",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imagem",
                table: "materiais");
        }
    }
}
