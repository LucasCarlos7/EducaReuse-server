namespace EducaReuse.Api.Domain;

public sealed class Solicitacao
{
    private Solicitacao() { }

    public Guid Id { get; private set; }

    public Guid MaterialId { get; private set; }

    public Material Material { get; private set; } = null!;

    public Guid BeneficiarioId { get; private set; }

    public Usuario Beneficiario { get; private set; } = null!;

    public string? Mensagem { get; private set; }

    public StatusSolicitacao Status { get; private set; }

    public DateTime CriadoEm { get; private set; }

    public DateTime? AtualizadoEm { get; private set; }

    internal static Solicitacao Criar(Guid materialId, Guid beneficiarioId, string? mensagem) => new()
    {
        Id = Guid.CreateVersion7(),
        MaterialId = materialId,
        BeneficiarioId = beneficiarioId,
        Mensagem = string.IsNullOrWhiteSpace(mensagem) ? null : mensagem.Trim(),
        Status = StatusSolicitacao.Pendente,
        CriadoEm = DateTime.UtcNow
    };

    internal void MarcarComoAceita() => AlterarStatus(StatusSolicitacao.Aceita);

    internal void MarcarComoRecusada() => AlterarStatus(StatusSolicitacao.Recusada);

    internal void MarcarComoEntregue() => AlterarStatus(StatusSolicitacao.Entregue);

    private void AlterarStatus(StatusSolicitacao novoStatus)
    {
        Status = novoStatus;
        AtualizadoEm = DateTime.UtcNow;
    }
}
