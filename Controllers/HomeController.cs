using BookLibrary.Models;
using BookLibrary.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BookLibrary.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize()]
        public ContentResult PopulateDB()
        {             
            //ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;
            //DataMethods.GetUserID(claimsIdentity);
            DataMethods.PopulateBookDatabase();
            return base.Content("<div>Database populate complete</div><br /><a href=\"/Home\">Home</a>", "text/html");        
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }              
    }
}