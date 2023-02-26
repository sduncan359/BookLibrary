using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Models
{
    public class BookModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public decimal UserRating { get; set; }
        [Required]
        public int Reviews { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public string Genre { get; set; }
        public string? CheckOutUserId { get; set; }
    }
}
