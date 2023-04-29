using BlogLab.Models.Account;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens;

namespace BlogLab.Services
{
    public interface ITokenService
    {
        SecurityToken CreateToken(ApplicationUserIdentity user);
    }
}
