using Microsoft.AspNetCore.Mvc;

namespace BlogCore.Areas.Admin.Controllers
{
    public class SliderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
