using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ECommerceApp.Models.ViewModels
{
    public class ProductVM
    {
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required, Range(1, 999999)]
        public decimal Price { get; set; }

        [Required, Range(0, 10000)]
        public int Stock { get; set; }

        [Required]
        public int CategoryId { get; set; }

        // Image upload ke liye
        public IFormFile? ImageFile { get; set; }

        // Edit mein purani image dikhane ke liye
        public string? ExistingImage { get; set; }

        // Dropdown ke liye categories list
        public List<Category>? Categories { get; set; }
    }
}