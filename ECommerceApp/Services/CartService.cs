using System.Data;
using System.Data.SqlClient;
using ECommerceApp.DataAccess;
using ECommerceApp.Models;

namespace ECommerceApp.Services
{
    public class CartService
    {
        private readonly DbAccess _db;
        public CartService(DbAccess db) => _db = db;

        // User ka poora cart lao
        public List<CartItem> GetCart(int userId)
        {
            string query = @"SELECT c.CartId, c.Quantity,
                                    p.ProductId, p.Name, 
                                    p.Price, p.ImageUrl,
                                    (p.Price * c.Quantity) AS SubTotal
                             FROM Cart c
                             JOIN Products p ON c.ProductId = p.ProductId
                             WHERE c.UserId = @UserId";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId)
            };

            DataTable dt = _db.ExecuteQuery(query, parameters);
            var cartItems = new List<CartItem>();

            foreach (DataRow row in dt.Rows)
            {
                cartItems.Add(new CartItem
                {
                    CartId = Convert.ToInt32(row["CartId"]),
                    ProductId = Convert.ToInt32(row["ProductId"]),
                    Name = row["Name"].ToString(),
                    Price = Convert.ToDecimal(row["Price"]),
                    ImageUrl = row["ImageUrl"].ToString(),
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    SubTotal = Convert.ToDecimal(row["SubTotal"])
                });
            }
            return cartItems;
        }

        // Cart mein product add karo
        public void AddToCart(int userId, int productId, int quantity = 1)
        {
            // Already exist karta hai toh quantity update karo
            string checkQuery = @"SELECT CartId FROM Cart 
                                  WHERE UserId = @UserId 
                                  AND ProductId = @ProductId";

            var checkParams = new SqlParameter[]
            {
                new SqlParameter("@UserId",    userId),
                new SqlParameter("@ProductId", productId)
            };

            var existing = _db.ExecuteScalar(checkQuery, checkParams);

            if (existing != null)
            {
                string updateQuery = @"UPDATE Cart 
                                       SET Quantity = Quantity + @Qty
                                       WHERE UserId    = @UserId 
                                       AND ProductId = @ProductId";
                var updateParams = new SqlParameter[]
                {
                    new SqlParameter("@Qty",       quantity),
                    new SqlParameter("@UserId",    userId),
                    new SqlParameter("@ProductId", productId)
                };
                _db.ExecuteNonQuery(updateQuery, updateParams);
            }
            else
            {
                string insertQuery = @"INSERT INTO Cart 
                                       (UserId, ProductId, Quantity)
                                       VALUES 
                                       (@UserId, @ProductId, @Qty)";
                var insertParams = new SqlParameter[]
                {
                    new SqlParameter("@UserId",    userId),
                    new SqlParameter("@ProductId", productId),
                    new SqlParameter("@Qty",       quantity)
                };
                _db.ExecuteNonQuery(insertQuery, insertParams);
            }
        }

        // Quantity update karo
        public void UpdateQuantity(int cartId, int quantity)
        {
            string query = @"UPDATE Cart SET Quantity = @Qty 
                             WHERE CartId = @CartId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Qty",    quantity),
                new SqlParameter("@CartId", cartId)
            };
            _db.ExecuteNonQuery(query, parameters);
        }

        // Cart se item remove karo
        public void RemoveFromCart(int cartId)
        {
            string query = "DELETE FROM Cart WHERE CartId = @CartId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@CartId", cartId)
            };
            _db.ExecuteNonQuery(query, parameters);
        }

        // Poora cart clear karo (order place hone ke baad)
        public void ClearCart(int userId)
        {
            string query = "DELETE FROM Cart WHERE UserId = @UserId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId)
            };
            _db.ExecuteNonQuery(query, parameters);
        }

        // Cart total calculate karo
        public decimal GetCartTotal(int userId)
        {
            string query = @"SELECT ISNULL(SUM(p.Price * c.Quantity), 0)
                             FROM Cart c
                             JOIN Products p ON c.ProductId = p.ProductId
                             WHERE c.UserId = @UserId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId)
            };
            return Convert.ToDecimal(_db.ExecuteScalar(query, parameters));
        }

        // Cart items ki count (navbar badge ke liye)
        public int GetCartCount(int userId)
        {
            string query = @"SELECT ISNULL(SUM(Quantity), 0) 
                             FROM Cart WHERE UserId = @UserId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId)
            };
            return Convert.ToInt32(_db.ExecuteScalar(query, parameters));
        }
    }
}