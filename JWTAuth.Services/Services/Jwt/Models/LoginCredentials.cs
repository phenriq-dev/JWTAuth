﻿namespace JWTAuth.Core.Services.Jwt.Models
{
    public class LoginCredentials
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
