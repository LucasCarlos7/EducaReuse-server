using System.Security.Claims;
using EducaReuse.Api.Common;
using EducaReuse.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EducaReuse.Api.Features.Solicitacoes;

// RF06 — Doador acompanha as solicitações recebidas para os seus materiais.
public static class SolicitacoesRecebidas
{
    public static void Map(RouteGroupBuilder grupo) =>
        grupo.MapGet("/recebidas", Handle).RequireAuthorization();

    private static async Task<IResult> Handle(ClaimsPrincipal principal, AppDbContext db, CancellationToken ct)
    {
        var doadorId = principal.ObterUsuarioId();

        var itens = await db.Solicitacoes
            .AsNoTracking()
            .Where(s => s.Material.DoadorId == doadorId)
            .OrderByDescending(s => s.CriadoEm)
            .Select(s => new
            {
                s.Id,
                s.MaterialId,
                MaterialNome = s.Material.Nome,
                s.Material.Categoria,
                s.Status,
                s.Mensagem,
                s.CriadoEm,
                s.AtualizadoEm,
                ContatoNome = s.Beneficiario.Nome,
                ContatoEmail = s.Beneficiario.Email,
                ContatoTelefone = s.Beneficiario.Telefone
            })
            .ToListAsync(ct);

        var resposta = itens.Select(s => new SolicitacaoResponse(
            s.Id,
            s.MaterialId,
            s.MaterialNome,
            s.Categoria,
            s.Status,
            s.Mensagem,
            s.CriadoEm,
            s.AtualizadoEm,
            ContatoResponse.Para(s.Status, s.ContatoNome, s.ContatoEmail, s.ContatoTelefone)));

        return Results.Ok(resposta);
    }
}
