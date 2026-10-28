using EducaReuse.Api.Common;
using EducaReuse.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EducaReuse.Api.Features.Materiais;

public static class ObterMaterial
{
    public static void Map(RouteGroupBuilder grupo) =>
        grupo.MapGet("/{id:guid}", Handle).AllowAnonymous();

    private static async Task<IResult> Handle(Guid id, AppDbContext db, CancellationToken ct)
    {
        var material = await db.Materiais
            .AsNoTracking()
            .Include(m => m.Doador)
            .FirstOrDefaultAsync(m => m.Id == id, ct);

        if (material is null)
        {
            return Erros.Material.NaoEncontrado.ToProblem();
        }

        return Results.Ok(new MaterialResponse(
            material.Id,
            material.Nome,
            material.Descricao,
            material.Imagem,
            material.Categoria,
            material.EstadoConservacao,
            material.Quantidade,
            material.Status,
            NomeExibicao.PrimeiroNome(material.Doador.Nome),
            material.CriadoEm));
    }
}
