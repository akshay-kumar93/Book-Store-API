using System.ComponentModel.DataAnnotations;

namespace BookStore.Model
{
    public class Purchase
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        public Book Book { get; set; }

        [Required]
        public string BuyerId { get; set; }
    }
}
