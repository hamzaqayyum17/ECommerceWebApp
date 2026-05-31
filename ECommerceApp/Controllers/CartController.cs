using ECommerceApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        // Login check helper
        private int? GetUserId() =>
            HttpContext.Session.GetInt32("UserId");

        // Cart page
        public IActionResult Index()
        {
            int? userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var cartItems = _cartService.GetCart(userId.Value);
            ViewBag.Total = _cartService.GetCartTotal(userId.Value);
            return View(cartItems);
        }

        // AJAX — Add to cart
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            int? userId = GetUserId();
            if (userId == null)
                return Json(new
                {
                    success = false,
                    message = "Please login first"
                });

            _cartService.AddToCart(userId.Value, productId, quantity);

            int cartCount = _cartService.GetCartCount(userId.Value);

            return Json(new
            {
                success = true,
                cartCount = cartCount,
                message = "Added to cart!"
            });
        }

        // AJAX — Update quantity
        [HttpPost]
        public IActionResult UpdateQuantity(int cartId, int quantity)
        {
            int? userId = GetUserId();
            if (userId == null)
                return Json(new { success = false });

            if (quantity < 1) quantity = 1;
            _cartService.UpdateQuantity(cartId, quantity);

            decimal total = _cartService.GetCartTotal(userId.Value);
            int cartCount = _cartService.GetCartCount(userId.Value);

            return Json(new
            {
                success = true,
                total = total.ToString("N0"),
                cartCount = cartCount
            });
        }

        // AJAX — Remove item
        [HttpPost]
        public IActionResult RemoveFromCart(int cartId)
        {
            int? userId = GetUserId();
            if (userId == null)
                return Json(new { success = false });

            _cartService.RemoveFromCart(cartId);

            decimal total = _cartService.GetCartTotal(userId.Value);
            int cartCount = _cartService.GetCartCount(userId.Value);

            return Json(new
            {
                success = true,
                total = total.ToString("N0"),
                cartCount = cartCount
            });
        }
        [HttpGet]
        public IActionResult GetCartCount()
        {
            int? userId = GetUserId();
            if (userId == null)
                return Json(new { count = 0 });

            int count = _cartService.GetCartCount(userId.Value);
            return Json(new { count = count });
        }
    }
}