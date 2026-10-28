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
        CategoriaMaterial? categoria,
        string? busca,
        CancellationToken ct)
    {
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
                m.Categoria,
                m.EstadoConservacao,
                m.Quantidade,
                m.Status,
                DoadorNome = m.Doador.Nome,
                m.CriadoEm
            })
            .ToListAsync(ct);

        var resposta = itens.Select(m => new MaterialResponse(
            m.Id,
            m.Nome,
            m.Descricao,
            m.Categoria,
            m.EstadoConservacao,
            m.Quantidade,
            m.Status,
            NomeExibicao.PrimeiroNome(m.DoadorNome),
            m.CriadoEm));

        return Results.Ok(resposta);
    }
}
