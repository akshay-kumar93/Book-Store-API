using System.ComponentModel.DataAnnotations;

namespace BookStore.Model
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; }

        [Required]
        public string SequenceNumber { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public Category Category { get; set; }
    }
}
