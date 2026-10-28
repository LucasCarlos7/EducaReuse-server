using EducaReuse.Api.Common;
using EducaReuse.Api.Domain;
using EducaReuse.Api.Infrastructure.Auth;
using EducaReuse.Api.Infrastructure.Data;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EducaReuse.Api.Features.Auth;

// RF01 — Autenticação.
public static class Login
{
    public sealed record Request(string Email, string Senha);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Email).NotEmpty().WithMessage("Informe seu e-mail.");
            RuleFor(r => r.Senha).NotEmpty().WithMessage("Informe sua senha.");
        }
    }

    public static void Map(RouteGroupBuilder grupo) =>
        grupo.MapPost("/login", Handle)
            .ComValidacao<Request>()
            .AllowAnonymous();

    private static async Task<IResult> Handle(
        Request request,
        AppDbContext db,
        IPasswordHasher<Usuario> hasher,
        TokenService tokens,
        CancellationToken ct)
    {
        var email = Usuario.NormalizarEmail(request.Email);
        var usuario = await db.Usuarios.FirstOrDefaultAsync(u => u.Email == email, ct);

        if (usuario is null)
        {
            return Erros.Auth.CredenciaisInvalidas.ToProblem();
        }

        var verificacao = hasher.VerifyHashedPassword(usuario, usuario.SenhaHash, request.Senha);

        if (verificacao == PasswordVerificationResult.Failed)
        {
            return Erros.Auth.CredenciaisInvalidas.ToProblem();
        }

        if (verificacao == PasswordVerificationResult.SuccessRehashNeeded)
        {
            usuario.DefinirSenhaHash(hasher.HashPassword(usuario, request.Senha));
            await db.SaveChangesAsync(ct);
        }

        return Results.Ok(new AuthResponse(tokens.Gerar(usuario), UsuarioResponse.De(usuario)));
    }
}
