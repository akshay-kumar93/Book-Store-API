using System.ComponentModel.DataAnnotations;

namespace BookStore.Model
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }
    }
}
