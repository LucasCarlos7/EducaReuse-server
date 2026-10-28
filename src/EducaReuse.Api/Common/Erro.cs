namespace EducaReuse.Api.Common;

public enum TipoErro
{
    Validacao,
    NaoAutorizado,
    Proibido,
    NaoEncontrado,
    Conflito
}

public sealed record Erro(string Codigo, string Mensagem, TipoErro Tipo)
{
    public static Erro Validacao(string codigo, string mensagem) => new(codigo, mensagem, TipoErro.Validacao);
    public static Erro NaoAutorizado(string codigo, string mensagem) => new(codigo, mensagem, TipoErro.NaoAutorizado);
    public static Erro Proibido(string codigo, string mensagem) => new(codigo, mensagem, TipoErro.Proibido);
    public static Erro NaoEncontrado(string codigo, string mensagem) => new(codigo, mensagem, TipoErro.NaoEncontrado);
    public static Erro Conflito(string codigo, string mensagem) => new(codigo, mensagem, TipoErro.Conflito);
}
