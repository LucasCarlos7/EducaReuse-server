using System.Security.Claims;
using EducaReuse.Api.Common;
using EducaReuse.Api.Domain;
using EducaReuse.Api.Infrastructure.Data;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EducaReuse.Api.Features.Materiais;

// RF02 — Doador cadastra material escolar.
public static class CriarMaterial
{
    private const long TamanhoMaximoImagem = 5 * 1024 * 1024; // 5 MB

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
            .RequireAuthorization(Politicas.Doador)
            .DisableAntiforgery();

    private static async Task<IResult> Handle(
        [FromForm] string nome,
        [FromForm] string? descricao,
        [FromForm] CategoriaMaterial categoria,
        [FromForm] EstadoConservacao estadoConservacao,
        [FromForm] int quantidade,
        IFormFile? imagem,
        ClaimsPrincipal principal,
        AppDbContext db,
        IValidator<Request> validator,
        CancellationToken ct)
    {
        var request = new Request(nome, descricao, categoria, estadoConservacao, quantidade);
        var validacao = await validator.ValidateAsync(request, ct);
        if (!validacao.IsValid)
            return Results.ValidationProblem(validacao.ToDictionary(), title: "Verifique os dados informados.");

        if (imagem is not null)
        {
            if (imagem.Length > TamanhoMaximoImagem)
                return Results.ValidationProblem(
                    new Dictionary<string, string[]> { ["imagem"] = ["A imagem deve ter no máximo 5 MB."] },
                    title: "Verifique os dados informados.");

            if (!imagem.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                return Results.ValidationProblem(
                    new Dictionary<string, string[]> { ["imagem"] = ["Envie um arquivo de imagem (JPEG, PNG, WebP)."] },
                    title: "Verifique os dados informados.");
        }

        var doadorId = principal.ObterUsuarioId();
        var doador = await db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == doadorId, ct);

        if (doador is null)
            return Erros.Usuario.NaoEncontrado.ToProblem();

        string? imagemDataUrl = null;
        if (imagem is not null)
        {
            using var ms = new MemoryStream();
            await imagem.CopyToAsync(ms, ct);
            imagemDataUrl = $"data:{imagem.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}";
        }

        var material = Material.Criar(
            doadorId,
            nome,
            descricao,
            categoria,
            estadoConservacao,
            quantidade,
            imagemDataUrl);

        db.Materiais.Add(material);
        await db.SaveChangesAsync(ct);

        var resposta = new MaterialResponse(
            material.Id,
            material.Nome,
            material.Descricao,
            material.Imagem,
            material.Categoria,
            material.EstadoConservacao,
            material.Quantidade,
            material.Status,
            NomeExibicao.PrimeiroNome(doador.Nome),
            material.CriadoEm);

        return Results.Created($"/api/materiais/{material.Id}", resposta);
    }
}
