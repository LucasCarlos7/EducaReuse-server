using System.Security.Claims;
using EducaReuse.Api.Domain;
using EducaReuse.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EducaReuse.Api.Features.Materiais;

// RF03 — Consulta de materiais disponíveis, com filtro por categoria e busca por texto.
public static class ListarMateriais
{
    private const int LimiteResultados = 100;

    public static void Map(RouteGroupBuilder grupo) =>
        grupo.MapGet("/", Handle).AllowAnonymous();

    private static async Task<IResult> Handle(
        AppDbContext db,
        ClaimsPrincipal principal,
        CategoriaMaterial? categoria,
        string? busca,
        CancellationToken ct)
    {
        // Endpoint público: se houver token válido, marca os materiais do próprio usuário.
        Guid? usuarioId = Guid.TryParse(principal.FindFirst("sub")?.Value, out var id) ? id : null;

        var consulta = db.Materiais
            .AsNoTracking()
            .Where(m => m.Status == StatusMaterial.Disponivel);

        if (categoria is not null)
        {
            consulta = consulta.Where(m => m.Categoria == categoria);
        }

        if (!string.IsNullOrWhiteSpace(busca))
        {
            var termo = $"%{busca.Trim()}%";
            consulta = consulta.Where(m =>
                EF.Functions.ILike(m.Nome, termo) ||
                (m.Descricao != null && EF.Functions.ILike(m.Descricao, termo)));
        }

        var itens = await consulta
            .OrderByDescending(m => m.CriadoEm)
            .Take(LimiteResultados)
            .Select(m => new
            {
                m.Id,
                m.Nome,
                m.Descricao,
                m.Imagem,
                m.Categoria,
                m.EstadoConservacao,
                m.Quantidade,
                m.Status,
                DoadorNome = m.Doador.Nome,
                m.DoadorId,
                m.CriadoEm
            })
            .ToListAsync(ct);

        var resposta = itens.Select(m => new MaterialResponse(
            m.Id,
            m.Nome,
            m.Descricao,
            m.Imagem,
            m.Categoria,
            m.EstadoConservacao,
            m.Quantidade,
            m.Status,
            NomeExibicao.PrimeiroNome(m.DoadorNome),
            m.CriadoEm,
            m.DoadorId == usuarioId));

        return Results.Ok(resposta);
    }
}
