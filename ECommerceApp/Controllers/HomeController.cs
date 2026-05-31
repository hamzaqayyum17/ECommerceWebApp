using ECommerceApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;

        public HomeController(ProductService productService,
                              CategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        // Home page — saare products
        public IActionResult Index(string? search, int? categoryId)
        {
            var products = string.IsNullOrEmpty(search)
                ? _productService.GetAll()
                : _productService.Search(search);

            if (categoryId.HasValue && categoryId > 0)
                products = products
                    .Where(p => p.CategoryId == categoryId)
                    .ToList();

            ViewBag.Categories = _categoryService.GetAll();
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;
            return View(products);
        }

        // Product detail page
        public IActionResult Detail(int id)
        {
            var product = _productService.GetById(id);
            if (product == null) return NotFound();
            return View(product);
        }
    }
}