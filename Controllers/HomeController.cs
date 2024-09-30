using Microsoft.AspNetCore.Mvc;

namespace friendly.Controllers
{
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
    }
}