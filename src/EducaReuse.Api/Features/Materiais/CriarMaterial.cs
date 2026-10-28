using System.Security.Claims;
using EducaReuse.Api.Common;
using EducaReuse.Api.Domain;
using EducaReuse.Api.Infrastructure.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EducaReuse.Api.Features.Materiais;

// RF02 — Doador cadastra material escolar.
public static class CriarMaterial
{
    public sealed record Request(
        string Nome,
        string? Descricao,
        CategoriaMaterial Categoria,
        EstadoConservacao EstadoConservacao,
        int Quantidade);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Nome).NotEmpty().WithMessage("Informe o nome do material.").MaximumLength(120);
            RuleFor(r => r.Descricao).MaximumLength(500);
            RuleFor(r => r.Categoria).IsInEnum().WithMessage("Escolha uma categoria válida.");
            RuleFor(r => r.EstadoConservacao).IsInEnum().WithMessage("Escolha um estado de conservação válido.");
            RuleFor(r => r.Quantidade).InclusiveBetween(1, 100).WithMessage("A quantidade deve estar entre 1 e 100.");
        }
    }

    public static void Map(RouteGroupBuilder grupo) =>
        grupo.MapPost("/", Handle)
            .ComValidacao<Request>()
            .RequireAuthorization(Politicas.Doador);

    private static async Task<IResult> Handle(Request request, ClaimsPrincipal principal, AppDbContext db, CancellationToken ct)
    {
        var doadorId = principal.ObterUsuarioId();
        var doador = await db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == doadorId, ct);

        if (doador is null)
        {
            return Erros.Usuario.NaoEncontrado.ToProblem();
        }

        var material = Material.Criar(
            doadorId,
            request.Nome,
            request.Descricao,
            request.Categoria,
            request.EstadoConservacao,
            request.Quantidade);

        db.Materiais.Add(material);
        await db.SaveChangesAsync(ct);

        var resposta = new MaterialResponse(
            material.Id,
            material.Nome,
            material.Descricao,
            material.Categoria,
            material.EstadoConservacao,
            material.Quantidade,
            material.Status,
            NomeExibicao.PrimeiroNome(doador.Nome),
            material.CriadoEm);

        return Results.Created($"/api/materiais/{material.Id}", resposta);
    }
}
