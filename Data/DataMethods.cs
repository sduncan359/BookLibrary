using BookLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace BookLibrary.Data
{
    public class DataMethods
    {
        private const int c_PageSize = 25;

        // pageNumber is 1-based
        public static List<BookModel> ShowAllBooks(int pageNumber)
        {
            List<BookModel> books = new List<BookModel>();

            using (var db = new BookContext())
            {
                books = db.Books.OrderBy(b => b.Name).Skip(pageNumber - 1).Take(c_PageSize).ToList();
            }

            return books;
        }

        public static void CheckOutBookForUser(BookModel book, string userId)
        {
            using (var db = new BookContext())
            {
                var UpdateBook = db.Books.Where(b => b.Id == book.Id && b.CheckOutUserId == null).FirstOrDefault();

                if (UpdateBook != null)
                {
                    UpdateBook.CheckOutUserId = userId;
                    db.SaveChanges();
                }                
            }
        }

        public static void CheckInBookForUser(BookModel book, string userId)
        {
            using (var db = new BookContext())
            {
                var UpdateBook = db.Books.Where(b => b.Id == book.Id).FirstOrDefault();

                if (UpdateBook != null)
                {
                    UpdateBook.CheckOutUserId = null;
                    db.SaveChanges();
                }
            }
        }

        public static int GetNumberOfBooksCheckedOut(string userId)
        {
            int numBooks = 0;

            using (var db = new BookContext())
            {               
                numBooks = db.Books.Count(b => b.CheckOutUserId == userId);
            }

            return numBooks;
        }

        public static List<BookModel> ShowBooksForSearch(string searchTerm, string searchColumn, int pageNumber = 1)
        {
            List<BookModel> books = new List<BookModel>();

            if (searchTerm.Trim().Length > 0)
            {
                using (var db = new BookContext())
                {
                    switch (searchColumn)
                    {
                        case "Author":
                            books = db.Books.Where(b => b.Author.ToLower().Contains(searchTerm.Trim().ToLower())).OrderBy(b => b.Name).Skip(pageNumber - 1).Take(c_PageSize).ToList();
                            break;
                        case "Year":
                            int result = -1;
                            if (Int32.TryParse(searchTerm, out result))
                            {
                                books = db.Books.Where(b => b.Year == result).OrderBy(b => b.Name).Skip(pageNumber - 1).Take(c_PageSize).ToList();
                            }
                            break;
                        default: // Title
                            books = db.Books.Where(b => b.Name.ToLower().Contains(searchTerm.Trim().ToLower())).OrderBy(b => b.Name).Skip(pageNumber - 1).Take(c_PageSize).ToList();
                            break;
                    }
                }
            }

            return books;
        }

        // pass in User.Identity as ClaimsIdentity (or cast it)
        public static string GetUserID(ClaimsIdentity claimsIdentity)
        {
            string userIdValue = "";
            
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

        // populate book database
        public static void PopulateBookDatabase()
        {
            Regex regx = new Regex("," + "(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            string appDataPath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data");
            StreamReader sr = new StreamReader(Path.Combine(appDataPath, "book.csv"));
            using (var db = new BookContext())
            {
                db.RemoveRange(db.Books);
                string? line = sr.ReadLine();
                do
                {
                    string[] cols = regx.Split(line);
                    if (cols[0] != "Name" || cols[1] != "Author")
                    {
                        BookModel book = new BookModel();
                        string bookName = cols[0];
                        book.Name = Regex.Replace(bookName.Trim(), "^\"|\"$", "");                        
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
