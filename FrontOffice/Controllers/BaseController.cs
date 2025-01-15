using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FrontOffice.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ILogger<BaseController> _logger;
        protected readonly IConfiguration _configuration;
        protected readonly string _apiBaseUrl;

        public BaseController(ILogger<BaseController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            _apiBaseUrl =
                _configuration["ServerSettings:ApiBaseUrl"]
                ?? throw new ArgumentNullException(
                    "ServerSettings:ApiBaseUrl",
                    "API base URL not found in configuration."
                );
        }

        // Méthode utilitaire pour obtenir une configuration
        protected string GetApiBaseUrl()
        {
            return _configuration.GetValue<string>("ServerSettings:ApiBaseUrl");
        }
        
        // Méthode utilitaire pour obtenir une configuration
        protected string GetImageBaseUrl()
        {
            return _configuration.GetValue<string>("ServerSettings:ImageBaseUrl");
        }
    }
}
