namespace EducaReuse.Api.Domain;

public sealed class Usuario
{
    private Usuario() { }

    public Guid Id { get; private set; }

    public string Nome { get; private set; } = null!;

    public string Email { get; private set; } = null!;

    public string SenhaHash { get; private set; } = null!;

    public string? Telefone { get; private set; }

    public PerfilUsuario Perfil { get; private set; }

    public DateTime CriadoEm { get; private set; }

    public static string NormalizarEmail(string email) => email.Trim().ToLowerInvariant();

    public static Usuario Criar(string nome, string email, string? telefone, PerfilUsuario perfil) => new()
    {
        Id = Guid.CreateVersion7(),
        Nome = nome.Trim(),
        Email = NormalizarEmail(email),
        Telefone = string.IsNullOrWhiteSpace(telefone) ? null : telefone.Trim(),
        Perfil = perfil,
        CriadoEm = DateTime.UtcNow
    };

    public void DefinirSenhaHash(string senhaHash) => SenhaHash = senhaHash;
}
