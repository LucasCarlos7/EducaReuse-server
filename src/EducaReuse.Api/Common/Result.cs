namespace EducaReuse.Api.Common;

public class Result
{
    protected Result(bool sucesso, Erro? erro)
    {
        Sucesso = sucesso;
        Erro = erro;
    }

    public bool Sucesso { get; }

    public bool Falhou => !Sucesso;

    public Erro? Erro { get; }

    public static Result Ok() => new(true, null);

    public static Result Falha(Erro erro) => new(false, erro);

    public static implicit operator Result(Erro erro) => Falha(erro);
}

public sealed class Result<T> : Result
{
    private readonly T? _valor;

    private Result(T valor) : base(true, null) => _valor = valor;

    private Result(Erro erro) : base(false, erro) { }

    public T Valor => Sucesso
        ? _valor!
        : throw new InvalidOperationException("Não é possível acessar o valor de um resultado com falha.");

    public static implicit operator Result<T>(T valor) => new(valor);

    public static implicit operator Result<T>(Erro erro) => new(erro);
}
