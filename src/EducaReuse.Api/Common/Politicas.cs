using System.Security.Claims;
using EducaReuse.Api.Domain;

namespace EducaReuse.Api.Common;

public static class Politicas
{
    public const string Doador = nameof(PerfilUsuario.Doador);
    public const string Beneficiario = nameof(PerfilUsuario.Beneficiario);
}

public static class ClaimsPrincipalExtensions
{
    public static Guid ObterUsuarioId(this ClaimsPrincipal usuario)
    {
        var valor = usuario.FindFirst("sub")?.Value;

        return Guid.TryParse(valor, out var id)
            ? id
            : throw new InvalidOperationException("Token sem identificador de usuário válido.");
    }
}
