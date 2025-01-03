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

            // Retourner le token dans le corps de la réponse (au lieu de rediriger)
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

            var result = await _authService.SignupAsync(
                model.Email,
                model.Password,
                model.Role,
                model.Phone,
                model.Name
            );

            if (result)
            {
                return Ok("Inscription réussie. Un email de confirmation a été envoyé.");
            }
            else
            {
                return BadRequest("Y a un problème lors de l'inscription.");
            }
        }
    }
}
