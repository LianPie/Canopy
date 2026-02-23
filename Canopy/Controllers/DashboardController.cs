using Microsoft.AspNetCore.Mvc;

namespace Canopy.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
