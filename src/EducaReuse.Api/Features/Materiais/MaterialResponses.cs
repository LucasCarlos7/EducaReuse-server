using EducaReuse.Api.Domain;

namespace EducaReuse.Api.Features.Materiais;

public sealed record MaterialResponse(
    Guid Id,
    string Nome,
    string? Descricao,
    CategoriaMaterial Categoria,
    EstadoConservacao EstadoConservacao,
    int Quantidade,
    StatusMaterial Status,
    string DoadorNome,
    DateTime CriadoEm);

public sealed record MeuMaterialResponse(
    Guid Id,
    string Nome,
    string? Descricao,
    CategoriaMaterial Categoria,
    EstadoConservacao EstadoConservacao,
    int Quantidade,
    StatusMaterial Status,
    int SolicitacoesPendentes,
    DateTime CriadoEm);

internal static class NomeExibicao
{
    // Na listagem pública, exibe apenas o primeiro nome do doador.
    public static string PrimeiroNome(string nomeCompleto)
    {
        var partes = nomeCompleto.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return partes.Length > 0 ? partes[0] : nomeCompleto;
    }
}
