using EducaReuse.Api.Common;
using EducaReuse.Api.Domain;
using EducaReuse.Api.Infrastructure.Auth;
using EducaReuse.Api.Infrastructure.Data;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EducaReuse.Api.Features.Auth;

// RF01 — Cadastro de usuário. A mesma conta pode doar e solicitar materiais.
public static class RegistrarUsuario
{
    public sealed record Request(string Nome, string Email, string Senha, string? Telefone);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Nome).NotEmpty().WithMessage("Informe seu nome.").MaximumLength(120);
            RuleFor(r => r.Email).NotEmpty().WithMessage("Informe seu e-mail.")
                .EmailAddress().WithMessage("Informe um e-mail válido.")
                .MaximumLength(160);
            RuleFor(r => r.Senha).NotEmpty().WithMessage("Informe uma senha.")
                .MinimumLength(8).WithMessage("A senha deve ter pelo menos 8 caracteres.")
                .MaximumLength(100);
            RuleFor(r => r.Telefone).MaximumLength(20);
        }
    }

    public static void Map(RouteGroupBuilder grupo) =>
        grupo.MapPost("/registrar", Handle)
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

        if (await db.Usuarios.AnyAsync(u => u.Email == email, ct))
        {
            return Erros.Usuario.EmailEmUso.ToProblem();
        }

        var usuario = Usuario.Criar(request.Nome, request.Email, request.Telefone);
        usuario.DefinirSenhaHash(hasher.HashPassword(usuario, request.Senha));

        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync(ct);

        return Results.Created("/api/auth/me", new AuthResponse(tokens.Gerar(usuario), UsuarioResponse.De(usuario)));
    }
}
