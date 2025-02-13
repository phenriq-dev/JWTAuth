namespace JWTAuth.Core.Services.Jwt.Models
{
    public class CredentialToken
    {
        public bool authenticated { get; set; }
        public string created { get; set; }
        public string expiration { get; set; }
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
        public string message { get; set; }
    }
}
