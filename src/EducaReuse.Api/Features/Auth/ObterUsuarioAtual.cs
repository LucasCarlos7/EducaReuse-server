using System.Security.Claims;
using EducaReuse.Api.Common;
using EducaReuse.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EducaReuse.Api.Features.Auth;

public static class ObterUsuarioAtual
{
    public static void Map(RouteGroupBuilder grupo) =>
        grupo.MapGet("/me", Handle).RequireAuthorization();

    private static async Task<IResult> Handle(ClaimsPrincipal principal, AppDbContext db, CancellationToken ct)
    {
        var id = principal.ObterUsuarioId();
        var usuario = await db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

        return usuario is null
            ? Erros.Usuario.NaoEncontrado.ToProblem()
            : Results.Ok(UsuarioResponse.De(usuario));
    }
}
