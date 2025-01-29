using Microsoft.AspNetCore.Mvc;

namespace TaskPlannerWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Task");
        }
    }
}