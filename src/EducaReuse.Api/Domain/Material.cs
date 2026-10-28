using EducaReuse.Api.Common;

namespace EducaReuse.Api.Domain;

public sealed class Material
{
    private readonly List<Solicitacao> _solicitacoes = [];

    private Material() { }

    public Guid Id { get; private set; }

    public Guid DoadorId { get; private set; }

    public Usuario Doador { get; private set; } = null!;

    public string Nome { get; private set; } = null!;

    public string? Descricao { get; private set; }

    public CategoriaMaterial Categoria { get; private set; }

    public EstadoConservacao EstadoConservacao { get; private set; }

    public int Quantidade { get; private set; }

    public StatusMaterial Status { get; private set; }

    public DateTime CriadoEm { get; private set; }

    // Mapeado para a coluna de sistema xmin do PostgreSQL (controle de concorrência otimista).
    public uint Versao { get; private set; }

    public IReadOnlyCollection<Solicitacao> Solicitacoes => _solicitacoes.AsReadOnly();

    public static Material Criar(
        Guid doadorId,
        string nome,
        string? descricao,
        CategoriaMaterial categoria,
        EstadoConservacao estadoConservacao,
        int quantidade) => new()
    {
        Id = Guid.CreateVersion7(),
        DoadorId = doadorId,
        Nome = nome.Trim(),
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim(),
        Categoria = categoria,
        EstadoConservacao = estadoConservacao,
        Quantidade = quantidade,
        Status = StatusMaterial.Disponivel,
        CriadoEm = DateTime.UtcNow
    };

    public Result<Solicitacao> Solicitar(Guid beneficiarioId, string? mensagem)
    {
        if (Status != StatusMaterial.Disponivel)
        {
            return Erros.Material.Indisponivel;
        }

        if (beneficiarioId == DoadorId)
        {
            return Erros.Solicitacao.ProprioMaterial;
        }

        if (_solicitacoes.Any(s => s.BeneficiarioId == beneficiarioId && s.Status == StatusSolicitacao.Pendente))
        {
            return Erros.Solicitacao.Duplicada;
        }

        var solicitacao = Solicitacao.Criar(Id, beneficiarioId, mensagem);
        _solicitacoes.Add(solicitacao);

        return solicitacao;
    }

    public Result Aceitar(Guid solicitacaoId)
    {
        if (Status != StatusMaterial.Disponivel)
        {
            return Erros.Material.Indisponivel;
        }

        var solicitacao = _solicitacoes.FirstOrDefault(s => s.Id == solicitacaoId);

        if (solicitacao is null)
        {
            return Erros.Solicitacao.NaoEncontrada;
        }

        if (solicitacao.Status != StatusSolicitacao.Pendente)
        {
            return Erros.Solicitacao.StatusInvalido;
        }

        solicitacao.MarcarComoAceita();

        // Regra: ao aceitar uma solicitação, as demais pendentes do mesmo material são recusadas.
        foreach (var outra in _solicitacoes.Where(s => s.Id != solicitacaoId && s.Status == StatusSolicitacao.Pendente))
        {
            outra.MarcarComoRecusada();
        }

        Status = StatusMaterial.Reservado;

        return Result.Ok();
    }

    public Result Recusar(Guid solicitacaoId)
    {
        var solicitacao = _solicitacoes.FirstOrDefault(s => s.Id == solicitacaoId);

        if (solicitacao is null)
        {
            return Erros.Solicitacao.NaoEncontrada;
        }

        if (solicitacao.Status != StatusSolicitacao.Pendente)
        {
            return Erros.Solicitacao.StatusInvalido;
        }

        solicitacao.MarcarComoRecusada();

        return Result.Ok();
    }

    public Result ConfirmarEntrega(Guid solicitacaoId)
    {
        var solicitacao = _solicitacoes.FirstOrDefault(s => s.Id == solicitacaoId);

        if (solicitacao is null)
        {
            return Erros.Solicitacao.NaoEncontrada;
        }

        if (solicitacao.Status != StatusSolicitacao.Aceita)
        {
            return Erros.Solicitacao.StatusInvalido;
        }

        solicitacao.MarcarComoEntregue();
        Status = StatusMaterial.Doado;

        return Result.Ok();
    }
}
