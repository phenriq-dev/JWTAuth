using System.ComponentModel.DataAnnotations;

namespace JWTAuth.Models
{
    public class Login
    {

        [Required(ErrorMessage = "O nome de usuário é obrigatório")]
        public string Username { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        public string Password { get; set; }
    }

}
