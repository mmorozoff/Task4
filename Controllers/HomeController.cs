using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Task4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Task4.Controllers
{
    public class HomeController : Controller
    {
        private UserContext db;
        public HomeController(UserContext context)
        {
            db = context;
        }

        [HttpPost]
        public ActionResult Index(User u)
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await db.Users.ToListAsync());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
