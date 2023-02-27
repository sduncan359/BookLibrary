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

        private void SetTempData(string userId, ref List<BookModel> books, int pageNumber = 1,
            string searchColumn = "", string searchTerm = "")
        {
            TempData["numBooksCheckedOut"] = DataMethods.GetNumberOfBooksCheckedOut(userId);
            TempData["userid"] = userId;
            TempData["books"] = books;
            TempData["currentPage"] = pageNumber;
            TempData["totalPages"] = DataMethods.GetTotalPages(searchColumn, searchTerm);
            TempData["searchColumn"] = searchColumn;
            TempData["searchTerm"] = searchTerm;
        }

        public IActionResult Index()
        {            
            ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;
            string userId = DataMethods.GetUserID(claimsIdentity);
            List<BookModel> books = DataMethods.ShowAllBooks(1);
            SetTempData(userId, ref books);

            return View();
        }

        [HttpPost]
        public IActionResult ChangePage(int selectPage, string searchColumn, string searchTerm)
        {
            ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;
            string userId = DataMethods.GetUserID(claimsIdentity);
            List<BookModel> books = null;
            if (searchColumn.Length > 0 && searchTerm.Trim().Length > 0)
            {
                books = DataMethods.ShowBooksForSearch(searchColumn, searchTerm, selectPage);
            }
            else
            {
                books = DataMethods.ShowAllBooks(selectPage);
            }
            SetTempData(userId, ref books, selectPage, searchColumn, searchTerm);

            return View("Index");
        }

        [HttpPost]
        public IActionResult SearchBooks(string searchTerm, string searchColumn)
        {
            ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;
            string userId = DataMethods.GetUserID(claimsIdentity);           
            List<BookModel> books = DataMethods.ShowBooksForSearch(searchColumn, searchTerm);
            SetTempData(userId, ref books, 1, searchColumn, searchTerm);

            return View("Index");
        }

        [HttpPost]
        public IActionResult CheckOutBook(BookModel book)
        {
            ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;
            string userId = DataMethods.GetUserID(claimsIdentity);                       
            DataMethods.CheckOutBookForUser(book, userId);
            List<BookModel> books = DataMethods.ShowAllBooks(1);
            SetTempData(userId, ref books);

            return View("Index");
        }

        [HttpPost]
        public IActionResult CheckInBook(BookModel book)
        {
            ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;
            string userId = DataMethods.GetUserID(claimsIdentity);
            DataMethods.CheckInBookForUser(book, userId);
            List<BookModel> books = DataMethods.ShowAllBooks(1);
            SetTempData(userId, ref books);

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