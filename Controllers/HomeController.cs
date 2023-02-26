using BookLibrary.Models;
using BookLibrary.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.RegularExpressions;

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string GetUserID()
        {
            string userIdValue = "";

            ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var userIdClaim = claimsIdentity.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

                if (userIdClaim != null)
                {
                    userIdValue = (string)userIdClaim.Value;
                }
            }

            return userIdValue;
        }

        private void PopulateBookDatabase()
        {
            Regex regx = new Regex("," + "(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            StreamReader sr = new StreamReader(@"c:\work\book.csv");
            using (var db = new BookContext())
            {
                string? line = sr.ReadLine();
                do
                {
                    string[] cols = regx.Split(line);
                    if (cols[0] != "Name" || cols[1] != "Author")
                    {
                        BookModel book = new BookModel();
                        book.Name = cols[0];
                        book.Author = cols[1];
                        book.UserRating = Convert.ToDecimal(cols[2]);
                        book.Reviews = Convert.ToInt32(cols[3]);
                        book.Price = Convert.ToDecimal(cols[4]);
                        book.Year = Convert.ToInt32(cols[5]);
                        book.Genre = cols[6];

                        db.Add(book);
                        db.SaveChanges();
                    }

                    line = sr.ReadLine();
                }
                while (line != null);
            }
            sr.Close();
            sr.Dispose();
        }
    }
}