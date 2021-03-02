using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Controllers
{
    public class HomeController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}