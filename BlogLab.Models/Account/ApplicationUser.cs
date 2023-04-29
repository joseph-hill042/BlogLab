using Microsoft.IdentityModel.Tokens;

namespace BlogLab.Models.Account
{
    public class ApplicationUser
    {
        public int ApplicationUserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public SecurityToken Token { get; set; }
    }
}
