using Microsoft.AspNetCore.Identity;

namespace JWTAuth.Models
{
    public class User
    {
        public long UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
