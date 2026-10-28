using System.Security.Claims;
using EducaReuse.Api.Common;
using EducaReuse.Api.Domain;
using EducaReuse.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EducaReuse.Api.Features.Solicitacoes;

// RF05 — Doador aceita ou recusa uma solicitação; RF06 — doador confirma a entrega.
public static class ResponderSolicitacao
{
    public static void Map(RouteGroupBuilder grupo)
    {
        grupo.MapPost("/{id:guid}/aceitar",
                (Guid id, ClaimsPrincipal principal, AppDbContext db, CancellationToken ct) =>
                    Executar(id, principal, db, (material, solicitacaoId) => material.Aceitar(solicitacaoId), ct))
            .RequireAuthorization(Politicas.Doador);

        grupo.MapPost("/{id:guid}/recusar",
                (Guid id, ClaimsPrincipal principal, AppDbContext db, CancellationToken ct) =>
                    Executar(id, principal, db, (material, solicitacaoId) => material.Recusar(solicitacaoId), ct))
            .RequireAuthorization(Politicas.Doador);

        grupo.MapPost("/{id:guid}/entregar",
                (Guid id, ClaimsPrincipal principal, AppDbContext db, CancellationToken ct) =>
                    Executar(id, principal, db, (material, solicitacaoId) => material.ConfirmarEntrega(solicitacaoId), ct))
            .RequireAuthorization(Politicas.Doador);
    }

    private static async Task<IResult> Executar(
        Guid solicitacaoId,
        ClaimsPrincipal principal,
        AppDbContext db,
        Func<Material, Guid, Result> acao,
        CancellationToken ct)
    {
        var doadorId = principal.ObterUsuarioId();

        // Só encontra a solicitação se o material pertencer ao doador autenticado.
        var material = await db.Materiais
            .Include(m => m.Solicitacoes)
            .FirstOrDefaultAsync(m => m.DoadorId == doadorId && m.Solicitacoes.Any(s => s.Id == solicitacaoId), ct);

        if (material is null)
        {
            return Erros.Solicitacao.NaoEncontrada.ToProblem();
        }

        var resultado = acao(material, solicitacaoId);

        if (resultado.Falhou)
        {
            return resultado.ToProblem();
        }

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Erros.Solicitacao.AlteradaPorOutraOperacao.ToProblem();
        }

        return Results.NoContent();
    }
}
