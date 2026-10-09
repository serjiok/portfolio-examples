using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Configurations;

namespace WebApi.Services;

internal static class TokenFactory
{
    private readonly static JwtSecurityTokenHandler jwtSecurityTokenHandler = new();

    public static string CreateToken(IConfiguration config, string login, IEnumerable<string> roles)
    {
        var timeLife = TimeSpan.FromDays(1);
        var now = DateTime.UtcNow;
        var expire = now.Add(timeLife);

        var claims = new List<Claim>(2 + roles.Count())
        {
            new (ClaimTypes.Name, login),
            new (ClaimTypes.NameIdentifier, login)
        };
        claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)));

        var jwt = new JwtSecurityToken(
                            issuer: config.ISSUER,
                            audience: config.AUDIENCE,
                            notBefore: now,
                            claims: claims,
                            expires: expire,
                            signingCredentials:
                            new SigningCredentials(
                                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.SECRETKEY)),
                                    SecurityAlgorithms.HmacSha256));

        var token = jwtSecurityTokenHandler.WriteToken(jwt);

        return token;
    }

    public static class SHA256
    {
        public static string GetSha256(string str)
            => Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(str)));
    }
}
