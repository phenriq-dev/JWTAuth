using JWTAuth.Core.Interfaces;
using JWTAuth.Core.Services.Jwt;
using JWTAuth.Core.Services.Jwt.Models;
using JWTAuth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuth.Controllers
{
    [Route("/account")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly SigningConfigurations _signingConfigurations;
        private readonly TokenConfigurations _tokenConfigurations;
        private readonly ITokenService _tokenService;
        private readonly IGenericRepository<Entities.User, Models.User> _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<UserController> _logger;

        public UserController(
            SigningConfigurations signingConfigurations,
            TokenConfigurations tokenConfigurations,
            IGenericRepository<Entities.User, Models.User> userRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            ILogger<UserController> logger)
        {
            _signingConfigurations = signingConfigurations;
            _tokenConfigurations = tokenConfigurations;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] LoginCredentials credentials)
        {
            try
            {
                var userModel = _userRepository.FindBy(c => c.Username == credentials.Username).FirstOrDefault();

                if (userModel == null || !_passwordHasher.VerifyPassword(userModel.Password, credentials.Password))
                {
                    return NotFound(new { message = "Usuário ou senha inválidos" });
                }

                var userEntity = new Entities.User
                {
                    UserId = userModel.UserId,
                    Username = userModel.Username
                };

                var token = _tokenService.GenerateToken(userEntity, _tokenConfigurations, _signingConfigurations);

                userModel.Password = "";

                return new
                {
                    user = userModel,
                    token = token
                };
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Erro ao autenticar o usuário.");
                return StatusCode(500, new { message = "Ocorreu um erro ao processar sua solicitação." });
            }

        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> RegisterUser([FromBody] RegisterUser userModel)
        {
            try
            {
                var existingUser = _userRepository.FindBy(c => c.Username == userModel.Username).FirstOrDefault();
                if (existingUser != null)
                {
                    return BadRequest(new { message = "Nome de usuário já existe" });
                }

                string hashedPassword = _passwordHasher.HashPassword(userModel.Password);

                var newUser = new User
                {
                    Username = userModel.Username,
                    Password = hashedPassword
                };

                _userRepository.Add(newUser);

                newUser.Password = "";

                return Ok(new
                {
                    user = newUser
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar o usuário.");
                return StatusCode(500, new { message = "Ocorreu um erro ao processar sua solicitação." });
            }
        }


        [HttpGet]
        [Route("profile")]
        [Authorize]
        public IActionResult GetUserProfile()
        {
            try
            {
                var username = User.Identity.Name;

                var user = _userRepository.FindBy(c => c.Username == username).FirstOrDefault();

                if (user == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                user.Password = "";

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar o perfil do usuário.");
                return StatusCode(500, new { message = "Ocorreu um erro ao processar sua solicitação." });
            }
        }
    }
}
