using EducaReuse.Api.Domain;

namespace EducaReuse.Api.Features.Auth;

public sealed record UsuarioResponse(Guid Id, string Nome, string Email, string? Telefone, PerfilUsuario Perfil)
{
    public static UsuarioResponse De(Usuario usuario) =>
        new(usuario.Id, usuario.Nome, usuario.Email, usuario.Telefone, usuario.Perfil);
}

public sealed record AuthResponse(string Token, UsuarioResponse Usuario);
