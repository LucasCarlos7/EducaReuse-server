using EducaReuse.Api.Features.Auth;
using EducaReuse.Api.Features.Materiais;
using EducaReuse.Api.Features.Solicitacoes;

namespace EducaReuse.Api.Features;

public static class EndpointMappings
{
    public static IEndpointRouteBuilder MapEducaReuseEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api");

        var auth = api.MapGroup("/auth");
        RegistrarUsuario.Map(auth);
        Login.Map(auth);
        ObterUsuarioAtual.Map(auth);

        var materiais = api.MapGroup("/materiais");
        ListarMateriais.Map(materiais);
        MeusMateriais.Map(materiais);
        ObterMaterial.Map(materiais);
        CriarMaterial.Map(materiais);
        CriarSolicitacao.Map(materiais);

        var solicitacoes = api.MapGroup("/solicitacoes");
        MinhasSolicitacoes.Map(solicitacoes);
        SolicitacoesRecebidas.Map(solicitacoes);
        ResponderSolicitacao.Map(solicitacoes);

        return app;
    }
}
