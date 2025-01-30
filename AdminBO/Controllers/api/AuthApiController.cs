using System.IdentityModel.Tokens.Jwt; // Pour 'JwtSecurityToken', 'JwtSecurityTokenHandler'
using System.Security.Claims; // Pour 'Claim', 'ClaimTypes'
using System.Text; // Pour 'Encoding'
using System.Threading.Tasks;
using AdminBO.Models;
using AdminBO.Services;
using Microsoft.AspNetCore.Authorization; // Pour 'AuthorizeAttribute'
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AdminBO.Controllers.Api
{
    [Route("api/auth")]
    [ApiController]
    public class AuthApiController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AuthService _authService;
        private readonly AdminBOContext _dbContext;

        public AuthApiController(IConfiguration configuration, AdminBOContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _authService = new AuthService(_configuration, _dbContext);
        }

        // POST: Auth/Authenticate
        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] SigninModel signinModel)
        {
            var token = await _authService.AuthenticateAsync(
                signinModel.Username,
                signinModel.Password
            );
            if (token == null)
            {
                return Unauthorized(
                    new { message = "Nom d'utilisateur ou mot de passe incorrect." }
                );
            }
            return Ok(new { token });
        }

        // POST: Auth/Signup
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupModel model)
        {
            if (
                model == null
                || string.IsNullOrEmpty(model.Email)
                || string.IsNullOrEmpty(model.Password)
                || string.IsNullOrEmpty(model.Role)
                || string.IsNullOrEmpty(model.Name)
            )
            {
                return BadRequest("Veuillez fournir toutes les informations nécessaires.");
            }

            var token = await _authService.SignupAsync(
                model.Email,
                model.Password,
                model.Role,
                model.Phone,
                model.Name
            );

            if (token != null)
            {
                return Ok(new { token });
            }
            else
            {
                return BadRequest("Y a un problème lors de l'inscription.");
            }
        }

        [Authorize]
        [HttpGet("user-info")]
        public async Task<IActionResult> GetUserInfo()
        {
            var authorizationHeader = HttpContext.Request.Headers["Authorization"].ToString();
            if (
                string.IsNullOrEmpty(authorizationHeader)
                || !authorizationHeader.StartsWith("Bearer ")
            )
            {
                return BadRequest("Token JWT manquant.");
            }

            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            try
            {
                var userInfo = await _authService.GetUserInfoFromTokenAsync(token);
                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            if (Request.Cookies.ContainsKey("jwt"))
            {
                Response.Cookies.Delete("jwt");
            }
            return Ok();
        }
    }
}
