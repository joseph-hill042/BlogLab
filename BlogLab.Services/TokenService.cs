using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlogLab.Models.Account;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BlogLab.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly string _issuer;

        public TokenService(IConfiguration _config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            _issuer = _config["Jwt:Issuer"];
        }

        public SecurityToken CreateToken(ApplicationUserIdentity user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.ApplicationUserId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _issuer,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = creds
            };

            return new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);
        }
    }
}
