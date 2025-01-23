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
        public async Task<ActionResult<ApiResponse<dynamic>>> Authenticate([FromBody] LoginCredentials credentials)
        {
            try
            {
                var userModel = _userRepository.FindBy(c => c.Username.ToLower() == credentials.Username.ToLower()).FirstOrDefault();

                if (userModel == null || !_passwordHasher.VerifyPassword(userModel.Password, credentials.Password))
                {
                    return NotFound(new ApiResponse<dynamic>
                    {
                        Success = false,
                        Message = "Usuário ou senha inválidos",
                        Data = null
                    });
                }

                var userEntity = new Entities.User
                {
                    UserId = userModel.UserId,
                    Username = userModel.Username
                };

                var token = _tokenService.GenerateToken(userEntity, _tokenConfigurations, _signingConfigurations);

                userModel.Password = "";

                return Ok(new ApiResponse<dynamic>
                {
                    Success = true,
                    Message = "Autenticação realizada com sucesso",
                    Data = new
                    {
                        user = userModel,
                        token = token
                    }
                });
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Erro ao autenticar o usuário");
                return StatusCode(500, new ApiResponse<dynamic>
                {
                    Success = false,
                    Message = "Erro interno no servidor",
                    Error = ex.Message
                });
            }

        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> RegisterUser([FromBody] RegisterUser userModel)
        {
            try
            {
                var existingUser = _userRepository.FindBy(c => c.Username.ToLower() == userModel.Username.ToLower()).FirstOrDefault();
                if (existingUser != null)
                {
                    return BadRequest(new ApiResponse<dynamic>
                    {
                        Success = false,
                        Message = "Nome de usuário já existe",
                        Data = null
                    });
                }

                string hashedPassword = _passwordHasher.HashPassword(userModel.Password);

                var newUser = new User
                {
                    Username = userModel.Username,
                    Password = hashedPassword
                };

                _userRepository.Add(newUser);

                newUser.Password = "";

                return Ok(new ApiResponse<dynamic>
                {
                    Success = true,
                    Message = "Usuário registrado com sucesso",
                    Data = newUser
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar usuário");
                return StatusCode(500, new ApiResponse<dynamic>
                {
                    Success = false,
                    Message = "Erro interno no servidor",
                    Error = ex.Message
                });
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

                var user = _userRepository.FindBy(c => c.Username.ToLower() == username.ToLower()).FirstOrDefault();

                if (user == null)
                {
                    return NotFound(new ApiResponse<dynamic>
                    {
                        Success = false,
                        Message = "Usuário não encontrado",
                        Data = null
                    });
                }

                user.Password = "";

                return Ok(new ApiResponse<dynamic>
                {
                    Success = true,
                    Message = "Perfil do usuário recuperado com sucesso",
                    Data = user
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar perfil do usuário");
                return StatusCode(500, new ApiResponse<dynamic>
                {
                    Success = false,
                    Message = "Erro interno no servidor",
                    Error = ex.Message
                });
            }
        }
    }
}
