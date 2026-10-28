namespace EducaReuse.Api.Domain;

public enum PerfilUsuario
{
    Doador = 1,
    Beneficiario = 2
}

public enum CategoriaMaterial
{
    Caderno = 1,
    Livro = 2,
    Mochila = 3,
    Estojo = 4,
    LapisECanetas = 5,
    MaterialDeArte = 6,
    Uniforme = 7,
    Outros = 8
}

public enum EstadoConservacao
{
    Novo = 1,
    Seminovo = 2,
    Usado = 3
}

public enum StatusMaterial
{
    Disponivel = 1,
    Reservado = 2,
    Doado = 3
}

public enum StatusSolicitacao
{
    Pendente = 1,
    Aceita = 2,
    Recusada = 3,
    Entregue = 4
}
