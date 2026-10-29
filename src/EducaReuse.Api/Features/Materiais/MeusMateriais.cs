using System.Security.Claims;
using EducaReuse.Api.Common;
using EducaReuse.Api.Domain;
using EducaReuse.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EducaReuse.Api.Features.Materiais;

// Lista os materiais cadastrados pelo doador autenticado, com o número de solicitações pendentes.
public static class MeusMateriais
{
    public static void Map(RouteGroupBuilder grupo) =>
        grupo.MapGet("/meus", Handle).RequireAuthorization();

    private static async Task<IResult> Handle(ClaimsPrincipal principal, AppDbContext db, CancellationToken ct)
    {
        var doadorId = principal.ObterUsuarioId();

        var itens = await db.Materiais
            .AsNoTracking()
            .Where(m => m.DoadorId == doadorId)
            .OrderByDescending(m => m.CriadoEm)
            .Select(m => new MeuMaterialResponse(
                m.Id,
                m.Nome,
                m.Descricao,
                m.Imagem,
                m.Categoria,
                m.EstadoConservacao,
                m.Quantidade,
                m.Status,
                m.Solicitacoes.Count(s => s.Status == StatusSolicitacao.Pendente),
                m.CriadoEm))
            .ToListAsync(ct);

        return Results.Ok(itens);
    }
}
