namespace EducaReuse.Api.Common;

public static class Erros
{
    public static class Usuario
    {
        public static readonly Erro EmailEmUso =
            Erro.Conflito("usuario.email_em_uso", "Já existe uma conta cadastrada com este e-mail.");

        public static readonly Erro NaoEncontrado =
            Erro.NaoEncontrado("usuario.nao_encontrado", "Usuário não encontrado.");
    }

    public static class Auth
    {
        public static readonly Erro CredenciaisInvalidas =
            Erro.NaoAutorizado("auth.credenciais_invalidas", "E-mail ou senha incorretos.");
    }

    public static class Material
    {
        public static readonly Erro NaoEncontrado =
            Erro.NaoEncontrado("material.nao_encontrado", "Material não encontrado.");

        public static readonly Erro Indisponivel =
            Erro.Conflito("material.indisponivel", "Este material não está mais disponível.");
    }

    public static class Solicitacao
    {
        public static readonly Erro NaoEncontrada =
            Erro.NaoEncontrado("solicitacao.nao_encontrada", "Solicitação não encontrada.");

        public static readonly Erro ProprioMaterial =
            Erro.Validacao("solicitacao.proprio_material", "Você não pode solicitar um material que você mesmo cadastrou.");

        public static readonly Erro Duplicada =
            Erro.Conflito("solicitacao.duplicada", "Você já tem uma solicitação pendente para este material.");

        public static readonly Erro StatusInvalido =
            Erro.Conflito("solicitacao.status_invalido", "A solicitação não está em um status que permita esta ação.");

        public static readonly Erro AlteradaPorOutraOperacao =
            Erro.Conflito("solicitacao.concorrencia", "Este material foi alterado por outra operação. Atualize a página e tente novamente.");
    }
}
