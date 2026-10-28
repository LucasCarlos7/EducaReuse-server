namespace EducaReuse.Api.Infrastructure.Auth;

public sealed class JwtOptions
{
    public const string Secao = "Jwt";

    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    // Chave HMAC-SHA256: no mínimo 32 caracteres.
    public string Key { get; init; } = string.Empty;

    public int ExpiracaoHoras { get; init; } = 8;
}
