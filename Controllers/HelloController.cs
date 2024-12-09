using Microsoft.AspNetCore.Mvc;

namespace PicnicFinder.Controllers
{
    public class HelloController : Controller
    {
        // GET: HelloController
        public ActionResult Index()
        {
            return View();
        }

        public String salama()
        {
            return "Defaulf Salama";
        }

        
        public String salut(String nom, int age)
        {
            return "Salama " + nom + ", taona " + age;
        }

    }
}
