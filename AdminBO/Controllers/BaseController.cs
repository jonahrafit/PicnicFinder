using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AdminBO.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ILogger<BaseController> _logger;
        protected readonly IConfiguration _configuration;

        public BaseController(ILogger<BaseController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public long GetCurrentUserId()
        {
            // Cette m�thode doit �tre impl�ment�e pour r�cup�rer l'ID de l'utilisateur connect�.
            // Par exemple, vous pouvez l'extraire des claims du jeton JWT.
            var userIdClaim = User?.FindFirst("UserId")?.Value;
            return userIdClaim != null ? long.Parse(userIdClaim) : 0;
        }
    }
}
