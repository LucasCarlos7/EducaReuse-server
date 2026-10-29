using System.Text;
using EducaReuse.Api.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace EducaReuse.Api.Infrastructure.Auth;

public sealed class TokenService(IOptions<JwtOptions> options)
{
    private readonly JsonWebTokenHandler _handler = new();

    public string Gerar(Usuario usuario)
    {
        var jwt = options.Value;
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));

        var descritor = new SecurityTokenDescriptor
        {
            Issuer = jwt.Issuer,
            Audience = jwt.Audience,
            Expires = DateTime.UtcNow.AddHours(jwt.ExpiracaoHoras),
            SigningCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256),
            Claims = new Dictionary<string, object>
            {
                [JwtRegisteredClaimNames.Sub] = usuario.Id.ToString(),
                [JwtRegisteredClaimNames.Email] = usuario.Email,
                [JwtRegisteredClaimNames.Name] = usuario.Nome
            }
        };

        return _handler.CreateToken(descritor);
    }
}
