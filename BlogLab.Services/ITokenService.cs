using BlogLab.Models.Account;
using Microsoft.IdentityModel.Tokens;

namespace BlogLab.Services
{
    public interface ITokenService
    {
        SecurityToken CreateToken(ApplicationUserIdentity user);
    }
}
