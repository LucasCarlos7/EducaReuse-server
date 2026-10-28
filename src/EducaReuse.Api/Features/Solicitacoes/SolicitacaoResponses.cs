using EducaReuse.Api.Domain;

namespace EducaReuse.Api.Features.Solicitacoes;

public sealed record ContatoResponse(string Nome, string? Email, string? Telefone)
{
    // E-mail e telefone da outra parte só são exibidos depois que a solicitação é aceita,
    // para que doador e beneficiário combinem a entrega.
    public static ContatoResponse Para(StatusSolicitacao status, string nome, string email, string? telefone)
    {
        var liberado = status is StatusSolicitacao.Aceita or StatusSolicitacao.Entregue;
        return liberado ? new(nome, email, telefone) : new(nome, null, null);
    }
}

public sealed record SolicitacaoResponse(
    Guid Id,
    Guid MaterialId,
    string MaterialNome,
    CategoriaMaterial Categoria,
    StatusSolicitacao Status,
    string? Mensagem,
    DateTime CriadoEm,
    DateTime? AtualizadoEm,
    ContatoResponse Contato);
