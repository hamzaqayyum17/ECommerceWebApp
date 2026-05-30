using ECommerceApp.Models;
using ECommerceApp.Models.ViewModels;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly ImageService _imageService;

        public AdminController(ProductService productService,
                               CategoryService categoryService,
                               ImageService imageService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _imageService = imageService;
        }

        // Session check — Admin hi access kare
        private bool IsAdmin() =>
            HttpContext.Session.GetString("Role") == "Admin";

        // Dashboard
        public IActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            ViewBag.TotalProducts = _productService.GetAll().Count;
            ViewBag.TotalCategories = _categoryService.GetAll().Count;
            return View();
        }

        // ─── PRODUCTS ────────────────────────────────────

        public IActionResult Products()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var products = _productService.GetAll();
            return View(products);
        }

        public IActionResult AddProduct()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var vm = new ProductVM
            {
                Categories = _categoryService.GetAll()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductVM model)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                model.Categories = _categoryService.GetAll();
                return View(model);
            }

            string imageUrl = "/images/no-image.png";
            if (model.ImageFile != null)
                imageUrl = await _imageService.SaveImageAsync(model.ImageFile);

            var product = new Product
            {
                CategoryId = model.CategoryId,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                ImageUrl = imageUrl
            };

            _productService.Add(product);
            TempData["Success"] = "Product added successfully!";
            return RedirectToAction("Products");
        }

        public IActionResult EditProduct(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var product = _productService.GetById(id);
            if (product == null) return NotFound();

            var vm = new ProductVM
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                ExistingImage = product.ImageUrl,
                Categories = _categoryService.GetAll()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductVM model)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                model.Categories = _categoryService.GetAll();
                return View(model);
            }

            string imageUrl = model.ExistingImage ?? "/images/no-image.png";

            // Nai image upload hui hai toh replace karo
            if (model.ImageFile != null)
            {
                _imageService.DeleteImage(imageUrl);
                imageUrl = await _imageService.SaveImageAsync(model.ImageFile);
            }

            var product = new Product
            {
                ProductId = model.ProductId,
                CategoryId = model.CategoryId,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                ImageUrl = imageUrl
            };

            _productService.Update(product);
            TempData["Success"] = "Product updated successfully!";
            return RedirectToAction("Products");
        }

        public IActionResult DeleteProduct(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var product = _productService.GetById(id);
            if (product != null)
                _imageService.DeleteImage(product.ImageUrl);
            _productService.Delete(id);
            TempData["Success"] = "Product deleted!";
            return RedirectToAction("Products");
        }

        // ─── CATEGORIES ──────────────────────────────────

        public IActionResult Categories()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var categories = _categoryService.GetAll();
            return View(categories);
        }

        [HttpPost]
        public IActionResult AddCategory(Category model)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            _categoryService.Add(model);
            TempData["Success"] = "Category added!";
            return RedirectToAction("Categories");
        }

        [HttpPost]
        public IActionResult EditCategory(Category model)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            _categoryService.Update(model);
            TempData["Success"] = "Category updated!";
            return RedirectToAction("Categories");
        }

        public IActionResult DeleteCategory(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            _categoryService.Delete(id);
            TempData["Success"] = "Category deleted!";
            return RedirectToAction("Categories");
        }
    }
}