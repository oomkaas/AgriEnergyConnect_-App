using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AgriEnergy.Models
{
    public class Product
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = "";

        [MaxLength(100)]
        public string Category { get; set; } = "";

        [Precision(16, 2)]
        public decimal price { get; set; }

        [MaxLength(100)]
        public string ImageFileName { get; set; } = "";
        public DateTime ProdDate { get; set; }
    }
}
