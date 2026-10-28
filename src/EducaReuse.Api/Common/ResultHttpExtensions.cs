namespace EducaReuse.Api.Common;

public static class ResultHttpExtensions
{
    public static IResult ToProblem(this Erro erro)
    {
        var status = erro.Tipo switch
        {
            TipoErro.Validacao => StatusCodes.Status400BadRequest,
            TipoErro.NaoAutorizado => StatusCodes.Status401Unauthorized,
            TipoErro.Proibido => StatusCodes.Status403Forbidden,
            TipoErro.NaoEncontrado => StatusCodes.Status404NotFound,
            TipoErro.Conflito => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return Results.Problem(
            statusCode: status,
            title: erro.Mensagem,
            extensions: new Dictionary<string, object?> { ["codigo"] = erro.Codigo });
    }

    public static IResult ToProblem(this Result resultado) =>
        resultado.Erro?.ToProblem()
        ?? throw new InvalidOperationException("Não é possível converter um resultado de sucesso em erro.");
}
