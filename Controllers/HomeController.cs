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
            ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;
            string userId = DataMethods.GetUserID(claimsIdentity);
            TempData["numBooksCheckedOut"] = DataMethods.GetNumberOfBooksCheckedOut(userId);
            TempData["userid"] = userId;
            TempData["books"] = DataMethods.ShowAllBooks(1);            

            return View();
        }

        [HttpPost]
        public IActionResult SearchBooks(string searchTerm, string searchColumn)
        {
            ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;
            string userId = DataMethods.GetUserID(claimsIdentity);
            TempData["numBooksCheckedOut"] = DataMethods.GetNumberOfBooksCheckedOut(userId);
            TempData["userid"] = userId;
            TempData["books"] = DataMethods.ShowBooksForSearch(searchTerm, searchColumn);            

            return View("Index");
        }

        [HttpPost]
        public IActionResult CheckOutBook(BookModel book)
        {
            ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;
            string userId = DataMethods.GetUserID(claimsIdentity);            
            DataMethods.CheckOutBookForUser(book, userId);
            TempData["numBooksCheckedOut"] = DataMethods.GetNumberOfBooksCheckedOut(userId);
            TempData["books"] = DataMethods.ShowAllBooks(1);

            return View("Index");
        }

        [HttpPost]
        public IActionResult CheckInBook(BookModel book)
        {
            ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;
            string userId = DataMethods.GetUserID(claimsIdentity);
            DataMethods.CheckInBookForUser(book, userId);
            TempData["numBooksCheckedOut"] = DataMethods.GetNumberOfBooksCheckedOut(userId);
            TempData["books"] = DataMethods.ShowAllBooks(1);

            return View("Index");
        }

        /// <summary>
        /// Home\Populate wipes the books table and then re-populates it from App_Data\books.csv
        /// Just here to make it easy to rebuild the database table without having to import data with
        /// another tool. Also good place to test other database functions.
        /// </summary>
        /// <returns>Just a message to say that the method is done</returns>       
        public ContentResult PopulateDB()
        {                         
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