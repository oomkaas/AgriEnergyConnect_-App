using System.ComponentModel.DataAnnotations;

namespace AgriEnergy.Models
{
    public class ProductDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        [Required, MaxLength(100)]

        public string Category { get; set; } = "";

        [Required]
        public decimal price { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
