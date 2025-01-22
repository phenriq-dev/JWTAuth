using Microsoft.AspNetCore.Identity;

namespace JWTAuth.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
