using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducaReuse.Api.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    SenhaHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Perfil = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "materiais",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DoadorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Categoria = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    EstadoConservacao = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_materiais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_materiais_usuarios_DoadorId",
                        column: x => x.DoadorId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "solicitacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: false),
                    BeneficiarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Mensagem = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_solicitacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_solicitacoes_materiais_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "materiais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_solicitacoes_usuarios_BeneficiarioId",
                        column: x => x.BeneficiarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_materiais_DoadorId",
                table: "materiais",
                column: "DoadorId");

            migrationBuilder.CreateIndex(
                name: "IX_materiais_Status_Categoria",
                table: "materiais",
                columns: new[] { "Status", "Categoria" });

            migrationBuilder.CreateIndex(
                name: "IX_solicitacoes_BeneficiarioId",
                table: "solicitacoes",
                column: "BeneficiarioId");

            migrationBuilder.CreateIndex(
                name: "IX_solicitacoes_MaterialId_Status",
                table: "solicitacoes",
                columns: new[] { "MaterialId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_Email",
                table: "usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "solicitacoes");

            migrationBuilder.DropTable(
                name: "materiais");

            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
