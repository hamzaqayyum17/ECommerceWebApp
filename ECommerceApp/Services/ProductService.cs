using System.Data;
using Microsoft.Data.SqlClient;
using ECommerceApp.DataAccess;
using ECommerceApp.Models;

namespace ECommerceApp.Services
{
    public class ProductService
    {
        private readonly DbAccess _db;
        public ProductService(DbAccess db) => _db = db;

        // Helper — DataRow to Product
        private Product MapRow(DataRow row) => new Product
        {
            ProductId = Convert.ToInt32(row["ProductId"]),
            CategoryId = Convert.ToInt32(row["CategoryId"]),
            CategoryName = row.Table.Columns.Contains("CategoryName")
                               ? row["CategoryName"].ToString() : "",
            Name = row["Name"].ToString(),
            Description = row["Description"].ToString(),
            Price = Convert.ToDecimal(row["Price"]),
            Stock = Convert.ToInt32(row["Stock"]),
            ImageUrl = row["ImageUrl"].ToString()
        };

        // Saare products (category name ke saath)
        public List<Product> GetAll()
        {
            string query = @"SELECT p.*, c.Name AS CategoryName 
                             FROM Products p 
                             JOIN Categories c ON p.CategoryId = c.CategoryId";
            DataTable dt = _db.ExecuteQuery(query);
            var list = new List<Product>();
            foreach (DataRow row in dt.Rows)
                list.Add(MapRow(row));
            return list;
        }

        // Single product
        public Product? GetById(int id)
        {
            string query = @"SELECT p.*, c.Name AS CategoryName 
                             FROM Products p 
                             JOIN Categories c ON p.CategoryId = c.CategoryId
                             WHERE p.ProductId = @Id";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", id)
            };
            DataTable dt = _db.ExecuteQuery(query, parameters);
            if (dt.Rows.Count == 0) return null;
            return MapRow(dt.Rows[0]);
        }

        // Category wise products
        public List<Product> GetByCategory(int categoryId)
        {
            string query = @"SELECT p.*, c.Name AS CategoryName 
                             FROM Products p 
                             JOIN Categories c ON p.CategoryId = c.CategoryId
                             WHERE p.CategoryId = @CatId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@CatId", categoryId)
            };
            DataTable dt = _db.ExecuteQuery(query, parameters);
            var list = new List<Product>();
            foreach (DataRow row in dt.Rows)
                list.Add(MapRow(row));
            return list;
        }

        // Search by name
        public List<Product> Search(string keyword)
        {
            string query = @"SELECT p.*, c.Name AS CategoryName 
                             FROM Products p 
                             JOIN Categories c ON p.CategoryId = c.CategoryId
                             WHERE p.Name LIKE @Keyword 
                             OR p.Description LIKE @Keyword";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Keyword", $"%{keyword}%")
            };
            DataTable dt = _db.ExecuteQuery(query, parameters);
            var list = new List<Product>();
            foreach (DataRow row in dt.Rows)
                list.Add(MapRow(row));
            return list;
        }

        // Add
        public bool Add(Product product)
        {
            string query = @"INSERT INTO Products 
                            (CategoryId, Name, Description, Price, Stock, ImageUrl)
                            VALUES 
                            (@CatId, @Name, @Desc, @Price, @Stock, @Image)";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@CatId",  product.CategoryId),
                new SqlParameter("@Name",   product.Name),
                new SqlParameter("@Desc",   product.Description ?? ""),
                new SqlParameter("@Price",  product.Price),
                new SqlParameter("@Stock",  product.Stock),
                new SqlParameter("@Image",  product.ImageUrl ?? "")
            };
            return _db.ExecuteNonQuery(query, parameters) > 0;
        }

        // Update
        public bool Update(Product product)
        {
            string query = @"UPDATE Products 
                             SET CategoryId  = @CatId,
                                 Name        = @Name,
                                 Description = @Desc,
                                 Price       = @Price,
                                 Stock       = @Stock,
                                 ImageUrl    = @Image
                             WHERE ProductId = @Id";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@CatId",  product.CategoryId),
                new SqlParameter("@Name",   product.Name),
                new SqlParameter("@Desc",   product.Description ?? ""),
                new SqlParameter("@Price",  product.Price),
                new SqlParameter("@Stock",  product.Stock),
                new SqlParameter("@Image",  product.ImageUrl ?? ""),
                new SqlParameter("@Id",     product.ProductId)
            };
            return _db.ExecuteNonQuery(query, parameters) > 0;
        }

        // Delete
        public bool Delete(int id)
        {
            string query = "DELETE FROM Products WHERE ProductId = @Id";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", id)
            };
            return _db.ExecuteNonQuery(query, parameters) > 0;
        }
    }
}