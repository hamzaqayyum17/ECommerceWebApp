using System.Data;
using Microsoft.Data.SqlClient;
using ECommerceApp.DataAccess;
using ECommerceApp.Models;
using AppOrder = ECommerceApp.Models.Order;

namespace ECommerceApp.Services
{
    public class OrderService
    {
        private readonly DbAccess _db;
        public OrderService(DbAccess db) => _db = db;

        // Order place karo — COD
        public int PlaceOrder(int userId,
                              decimal totalAmount,
                              List<CartItem> cartItems)
        {
            string orderQuery = @"INSERT INTO Orders 
                                 (UserId, TotalAmount, Status, 
                                  OrderDate, StripePaymentId)
                                 OUTPUT INSERTED.OrderId
                                 VALUES 
                                 (@UserId, @Total, 'Pending', 
                                  GETDATE(), @PaymentMethod)";

            var orderParams = new SqlParameter[]
            {
                new("@UserId",        userId),
                new("@Total",         totalAmount),
                new("@PaymentMethod", "COD")
            };

            int orderId = Convert.ToInt32(
                              _db.ExecuteScalar(orderQuery, orderParams));

            foreach (var item in cartItems)
            {
                string itemQuery = @"INSERT INTO OrderItems 
                                    (OrderId, ProductId, Quantity, UnitPrice)
                                    VALUES 
                                    (@OrderId, @ProductId, @Qty, @Price)";

                var itemParams = new SqlParameter[]
                {
                    new("@OrderId",   orderId),
                    new("@ProductId", item.ProductId),
                    new("@Qty",       item.Quantity),
                    new("@Price",     item.Price)
                };
                _db.ExecuteNonQuery(itemQuery, itemParams);

                // Stock kam karo
                string stockQuery = @"UPDATE Products 
                                      SET Stock = Stock - @Qty
                                      WHERE ProductId = @ProductId";
                var stockParams = new SqlParameter[]
                {
                    new("@Qty",       item.Quantity),
                    new("@ProductId", item.ProductId)
                };
                _db.ExecuteNonQuery(stockQuery, stockParams);
            }

            return orderId;
        }

        // User ke saare orders
        public List<AppOrder> GetUserOrders(int userId)
        {
            string query = @"SELECT * FROM Orders 
                             WHERE UserId = @UserId 
                             ORDER BY OrderDate DESC";
            var parameters = new SqlParameter[]
            {
                new("@UserId", userId)
            };
            DataTable dt = _db.ExecuteQuery(query, parameters);
            var orders = new List<AppOrder>();

            foreach (DataRow row in dt.Rows)
            {
                orders.Add(new AppOrder
                {
                    OrderId = Convert.ToInt32(row["OrderId"]),
                    UserId = Convert.ToInt32(row["UserId"]),
                    TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                    Status = row["Status"].ToString() ?? "",
                    OrderDate = Convert.ToDateTime(row["OrderDate"]),
                    StripePaymentId = row["StripePaymentId"].ToString() ?? ""
                });
            }
            return orders;
        }

        // Single order detail
        public AppOrder? GetOrderDetail(int orderId)
        {
            string orderQuery = "SELECT * FROM Orders WHERE OrderId = @Id";
            var orderParams = new SqlParameter[]
            {
                new("@Id", orderId)
            };
            DataTable dt = _db.ExecuteQuery(orderQuery, orderParams);
            if (dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];
            var order = new AppOrder
            {
                OrderId = Convert.ToInt32(row["OrderId"]),
                UserId = Convert.ToInt32(row["UserId"]),
                TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                Status = row["Status"].ToString() ?? "",
                OrderDate = Convert.ToDateTime(row["OrderDate"]),
                StripePaymentId = row["StripePaymentId"].ToString() ?? "",
                Items = new List<OrderItemDetail>()
            };

            string itemsQuery = @"SELECT oi.*, p.Name, p.ImageUrl
                                  FROM OrderItems oi
                                  JOIN Products p 
                                  ON oi.ProductId = p.ProductId
                                  WHERE oi.OrderId = @OrderId";
            var itemsParams = new SqlParameter[]
            {
                new("@OrderId", orderId)
            };
            DataTable itemsDt = _db.ExecuteQuery(itemsQuery, itemsParams);

            foreach (DataRow itemRow in itemsDt.Rows)
            {
                order.Items.Add(new OrderItemDetail
                {
                    ProductId = Convert.ToInt32(itemRow["ProductId"]),
                    Name = itemRow["Name"].ToString() ?? "",
                    ImageUrl = itemRow["ImageUrl"].ToString() ?? "",
                    Quantity = Convert.ToInt32(itemRow["Quantity"]),
                    UnitPrice = Convert.ToDecimal(itemRow["UnitPrice"])
                });
            }
            return order;
        }

        // Admin — saare orders
        public List<AppOrder> GetAllOrders()
        {
            string query = @"SELECT o.*, u.FullName 
                             FROM Orders o
                             JOIN Users u ON o.UserId = u.UserId
                             ORDER BY o.OrderDate DESC";
            DataTable dt = _db.ExecuteQuery(query);
            var orders = new List<AppOrder>();

            foreach (DataRow row in dt.Rows)
            {
                orders.Add(new AppOrder
                {
                    OrderId = Convert.ToInt32(row["OrderId"]),
                    UserName = row["FullName"].ToString() ?? "",
                    TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                    Status = row["Status"].ToString() ?? "",
                    OrderDate = Convert.ToDateTime(row["OrderDate"])
                });
            }
            return orders;
        }

        // Admin — order status update
        public bool UpdateStatus(int orderId, string status)
        {
            string query = @"UPDATE Orders SET Status = @Status 
                             WHERE OrderId = @Id";
            var parameters = new SqlParameter[]
            {
                new("@Status", status),
                new("@Id",     orderId)
            };
            return _db.ExecuteNonQuery(query, parameters) > 0;
        }
    }
}