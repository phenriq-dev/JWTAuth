using JWTAuth.Core.Interfaces;
using JWTAuth.Core.Services.Jwt;
using JWTAuth.Core.Services.Jwt.Models;
using JWTAuth.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace JWTAuth.Controllers
{
    public class AuthController : Controller
    {
        private readonly SigningConfigurations _signingConfigurations;
        private readonly TokenConfigurations _tokenConfigurations;
        private readonly ITokenService _tokenService;
        private readonly IGenericRepository<Entities.User, Models.User> _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<AuthController> _logger;
        private readonly IDistributedCache _cache;

        public AuthController(
            SigningConfigurations signingConfigurations,
            TokenConfigurations tokenConfigurations,
            IGenericRepository<Entities.User, Models.User> userRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            ILogger<AuthController> logger,
            IDistributedCache cache)
        {
            _signingConfigurations = signingConfigurations;
            _tokenConfigurations = tokenConfigurations;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _cache  = cache;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Login credentials)
        {
            try
            {
                var userModel = _userRepository.FindBy(c => c.Username.ToLower() == credentials.Username.ToLower()).FirstOrDefault();

                if (userModel == null || !_passwordHasher.VerifyPassword(userModel.Password, credentials.Password))
                {
                    ViewBag.ErrorMessage = "Usuario ou senha inválidos";
                    return View();
                }

                var userEntity = new Entities.User
                {
                    UserId = userModel.UserId,
                    Username = userModel.Username
                };

                var token = _tokenService.GenerateToken(userEntity, _tokenConfigurations, _signingConfigurations, _cache);

                Response.Cookies.Append("AccessToken", token.accessToken, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddSeconds(_tokenConfigurations.Seconds)
                });

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Erro ao autenticar o usuário");
                ViewBag.ErrorMessage = "Erro interno no servidor";
                return View();
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(); 
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterUser userModel)
        {
            try
            {
                var existingUser = _userRepository.FindBy(c => c.Username.ToLower() == userModel.Username.ToLower()).FirstOrDefault();
                if (existingUser != null)
                {
                    if (existingUser != null)
                    {
                        ViewBag.ErrorMessage = "Nome de usuário já existe";
                        return View();
                    }
                }

                string hashedPassword = _passwordHasher.HashPassword(userModel.Password);

                var newUser = new User
                {
                    Username = userModel.Username,
                    Password = hashedPassword
                };

                _userRepository.Add(newUser);

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar usuário");
                ViewBag.ErrorMessage = "Erro interno no servidor";
                return View();
            }
        }

        //[HttpPost]
        //[Authorize]
        //public async Task<IActionResult> RefreshToken(RefreshToken model)
        //{
        //    var userIdClaim = User.FindFirst("UserId")?.Value;

        //    if (string.IsNullOrEmpty(userIdClaim))
        //    {
        //        return Unauthorized(new ApiResponse<dynamic>
        //        {
        //            Success = false,
        //            Message = "Token inválido",
        //            Data = null
        //        });
        //    }

        //    var userId = long.Parse(userIdClaim);
        //    var user = _userRepository.FindBy(c => c.UserId == userId).FirstOrDefault();

        //    if (user == null)
        //    {
        //        return NotFound(new ApiResponse<dynamic>
        //        {
        //            Success = false,
        //            Message = "Usuário não encontrado",
        //            Data = null
        //        });
        //    }

        //    var userEntity = new Entities.User
        //    {
        //        UserId = user.UserId,
        //        Username = user.Username
        //    };

        //    var refreshedToken = await _tokenService.RefreshTokenAsync(userEntity, _tokenConfigurations, _signingConfigurations, _cache, model.refreshToken);

        //    if (refreshedToken == null)
        //    {
        //        await RemoveRefreshToken(model.refreshToken);
        //        return Unauthorized(new ApiResponse<dynamic>
        //        {
        //            Success = false,
        //            Message = "Refresh token inválido ou expirado",
        //            Data = null
        //        });
        //    }

        //    return Ok(refreshedToken);
        //}


        [HttpGet]
        [Authorize]
        public IActionResult Profile()
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return RedirectToAction("Login");
                }

                var userId = long.Parse(userIdClaim);
                var user = _userRepository.FindBy(c => c.UserId == userId).FirstOrDefault();

                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                user.Password = "";

                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar perfil do usuário");
                ViewBag.ErrorMessage = "Erro interno no servidor";
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout([FromServices] IDistributedCache cache)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    _logger.LogWarning("Erro ao encontrar UserId");
                }

                var userId = long.Parse(userIdClaim);

                Response.Cookies.Delete("AccessToken");

                await cache.RemoveAsync($"refreshToken:{userId}");

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return RedirectToAction("Login");
            }   
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer logout");
                return RedirectToAction("Login");
            }
        }

        public IActionResult AccessDenied()
        {
            return RedirectToAction("Login");
        }
    }
}
