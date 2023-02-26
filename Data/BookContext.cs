using Microsoft.EntityFrameworkCore;
using BookLibrary.Models;

namespace BookLibrary.Data
{
    public class BookContext : DbContext
    {
        public DbSet<BookModel> Books { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string appDataPath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data");
            string connectionString = "Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename=" +
                Directory.GetCurrentDirectory() + "\\App_Data\\aspnet-LibraryBookData.mdf;Initial Catalog=LibraryBookData;Integrated Security=True";
            options.UseSqlServer(connectionString);
        }
    }
}
