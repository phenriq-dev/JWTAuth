using JWTAuth.Models;
using JWTAuth.Services.Jwt;
using JWTAuth.Services.Jwt.Interfaces;
using JWTAuth.Services.Jwt.Manager;
using JWTAuth.Services.Jwt.Models;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuth.Controllers
{
    [Route("/account")]
    public class UserController : Controller
    {
        private readonly SigningConfigurations _signingConfigurations;
        private readonly TokenConfigurations _tokenConfigurations;
        private readonly ITokenService _tokenService;

        public UserController(
            SigningConfigurations signingConfigurations,
            TokenConfigurations tokenConfigurations,
            ITokenService tokenService)
        {
            _signingConfigurations = signingConfigurations;
            _tokenConfigurations = tokenConfigurations;
            _tokenService = tokenService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] User model)
        {
            var user = UserRepository.Get(model.Username, model.Password);

            if (user == null)
                return NotFound(new { message = "Usuário ou senha inválidos" });

            var token = _tokenService.GenerateToken(user, _tokenConfigurations, _signingConfigurations);

            user.Password = "";

            return new
            {
                user = user,
                token = token
            };
        }
    }
}
