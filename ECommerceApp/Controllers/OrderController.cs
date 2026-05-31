using ECommerceApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly CartService _cartService;
        private readonly OrderService _orderService;

        public OrderController(CartService cartService,
                               OrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }

        private int? GetUserId() =>
            HttpContext.Session.GetInt32("UserId");

        // Checkout page
        public IActionResult Checkout()
        {
            int? userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var cartItems = _cartService.GetCart(userId.Value);
            if (!cartItems.Any())
                return RedirectToAction("Index", "Cart");

            // Stock check karo
            var outOfStock = _cartService.CheckStock(userId.Value);
            if (outOfStock.Any())
            {
                TempData["StockError"] = string.Join(", ", outOfStock);
                return RedirectToAction("Index", "Cart");
            }

            ViewBag.Total = _cartService.GetCartTotal(userId.Value);
            return View(cartItems);
        }

        [HttpPost]
        public IActionResult PlaceOrder()
        {
            int? userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var cartItems = _cartService.GetCart(userId.Value);
            if (!cartItems.Any())
                return RedirectToAction("Index", "Cart");

            decimal total = _cartService.GetCartTotal(userId.Value);

            // 3 parameters — Stripe parameter hata diya
            int orderId = _orderService.PlaceOrder(
                              userId.Value,
                              total,
                              cartItems);

            _cartService.ClearCart(userId.Value);

            return RedirectToAction("Confirmation", new { orderId = orderId });
        }

        // Confirmation page
        public IActionResult Confirmation(int orderId)
        {
            var order = _orderService.GetOrderDetail(orderId);
            if (order == null) return NotFound();
            return View(order);
        }

        // My Orders
        public IActionResult MyOrders()
        {
            int? userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var orders = _orderService.GetUserOrders(userId.Value);
            return View(orders);
        }
    }
}