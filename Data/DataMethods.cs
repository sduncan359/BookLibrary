using BookLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace BookLibrary.Data
{
    public class DataMethods
    {      
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
