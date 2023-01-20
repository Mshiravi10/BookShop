using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BookShop.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Only in range 1 to 100!!")]
        public int DisplayOrder { get; set; }

        [DisplayName("Created Time")]
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}
