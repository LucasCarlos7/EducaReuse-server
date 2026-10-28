using System.Security.Claims;
using EducaReuse.Api.Common;
using EducaReuse.Api.Infrastructure.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EducaReuse.Api.Features.Solicitacoes;

// RF04 — Beneficiário solicita um material disponível.
public static class CriarSolicitacao
{
    public sealed record Request(string? Mensagem);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Mensagem).MaximumLength(300).WithMessage("A mensagem deve ter no máximo 300 caracteres.");
        }
    }

    // Rota registrada no grupo /materiais: POST /api/materiais/{materialId}/solicitacoes
    public static void Map(RouteGroupBuilder grupoMateriais) =>
        grupoMateriais.MapPost("/{materialId:guid}/solicitacoes", Handle)
            .ComValidacao<Request>()
            .RequireAuthorization(Politicas.Beneficiario);

    private static async Task<IResult> Handle(
        Guid materialId,
        Request request,
        ClaimsPrincipal principal,
        AppDbContext db,
        CancellationToken ct)
    {
        var beneficiarioId = principal.ObterUsuarioId();

        var material = await db.Materiais
            .Include(m => m.Solicitacoes)
            .FirstOrDefaultAsync(m => m.Id == materialId, ct);

        if (material is null)
        {
            return Erros.Material.NaoEncontrado.ToProblem();
        }

        var resultado = material.Solicitar(beneficiarioId, request.Mensagem);

        if (resultado.Falhou)
        {
            return resultado.ToProblem();
        }

        var solicitacao = resultado.Valor;
        db.Solicitacoes.Add(solicitacao);
        await db.SaveChangesAsync(ct);

        return Results.Created(
            "/api/solicitacoes/minhas",
            new { solicitacao.Id, solicitacao.MaterialId, solicitacao.Status, solicitacao.CriadoEm });
    }
}
