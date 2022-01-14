using Microsoft.AspNetCore.Mvc;

namespace EasyNChatWebServer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
