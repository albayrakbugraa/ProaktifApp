using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProaktifArizaTahmini.UI.Models;
using System.Diagnostics;

namespace ProaktifArizaTahmini.UI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
