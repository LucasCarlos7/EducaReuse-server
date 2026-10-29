using System.Security.Claims;
using EducaReuse.Api.Common;
using EducaReuse.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EducaReuse.Api.Features.Solicitacoes;

// RF06 — Beneficiário acompanha o status das próprias solicitações.
public static class MinhasSolicitacoes
{
    public static void Map(RouteGroupBuilder grupo) =>
        grupo.MapGet("/minhas", Handle).RequireAuthorization();

    private static async Task<IResult> Handle(ClaimsPrincipal principal, AppDbContext db, CancellationToken ct)
    {
        var beneficiarioId = principal.ObterUsuarioId();

        var itens = await db.Solicitacoes
            .AsNoTracking()
            .Where(s => s.BeneficiarioId == beneficiarioId)
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
                ContatoNome = s.Material.Doador.Nome,
                ContatoEmail = s.Material.Doador.Email,
                ContatoTelefone = s.Material.Doador.Telefone
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
