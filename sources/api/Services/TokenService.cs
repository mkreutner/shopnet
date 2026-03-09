using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ShopNetApi.Models.Entities;

namespace ShopNetApi.Services;

public interface ITokenService
{
    string CreateToken(ApplicationUser user, IList<string> roles);
}

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _key;

    public TokenService(IConfiguration config)
    {
        _config = config;
        // On récupère la clé secrète définie dans appsettings.json
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
    }

    public string CreateToken(ApplicationUser user, IList<string> roles)
    {
        // 1. On définit les "Claims" (les infos transportées par le token)
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName!),
            new(JwtRegisteredClaimNames.Email, user.Email!)
        };

        // 2. On ajoute les rôles de l'utilisateur
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // 3. On définit les identifiants de signature (l'algorithme)
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        // 4. On prépare la description du token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:DurationInMinutes"])),
            SigningCredentials = creds,
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"]
        };

        // 5. On génère et on écrit le token
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}