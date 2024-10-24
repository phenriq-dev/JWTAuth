using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace JWTAuth.Core.Services.Jwt
{
    public class SigningConfigurations
    {
        public SecurityKey Key { get; }
        public SigningCredentials SigningCredentials { get; }

        public SigningConfigurations(string secret)
        {
            Key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
            SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
        }
    }

}
