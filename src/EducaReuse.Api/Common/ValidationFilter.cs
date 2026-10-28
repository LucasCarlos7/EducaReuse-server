using FluentValidation;

namespace EducaReuse.Api.Common;

public sealed class ValidationFilter<TRequest>(IValidator<TRequest> validator) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().FirstOrDefault();

        if (request is null)
        {
            return Results.Problem(statusCode: StatusCodes.Status400BadRequest, title: "Requisição inválida.");
        }

        var resultado = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);

        if (!resultado.IsValid)
        {
            return Results.ValidationProblem(resultado.ToDictionary(), title: "Verifique os dados informados.");
        }

        return await next(context);
    }
}

public static class ValidationFilterExtensions
{
    public static RouteHandlerBuilder ComValidacao<TRequest>(this RouteHandlerBuilder builder) =>
        builder.AddEndpointFilter<ValidationFilter<TRequest>>();
}
