using Microsoft.IdentityModel.Tokens;
using OroUostoSystem.Server.Interfaces;
using OroUostoSystem.Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OroUostoSystem.Server.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public string CreateToken(User user)
        {
            var secretKey = _config["AppSettings:TokenKey"];

            //check if keu isn ul or empt

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("Token key is not configured.");
            }
            // create the symetric security key

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id), // maps to User.Identity.Name
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken
                (
                    audience: null,
                    issuer: null,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(60),
                    signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
