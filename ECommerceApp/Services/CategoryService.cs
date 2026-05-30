using System.Data;
using System.Data.SqlClient;
using ECommerceApp.DataAccess;
using ECommerceApp.Models;

namespace ECommerceApp.Services
{
    public class CategoryService
    {
        private readonly DbAccess _db;
        public CategoryService(DbAccess db) => _db = db;

        // Saari categories lao
        public List<Category> GetAll()
        {
            string query = "SELECT * FROM Categories";
            DataTable dt = _db.ExecuteQuery(query);

            var list = new List<Category>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Category
                {
                    CategoryId = Convert.ToInt32(row["CategoryId"]),
                    Name = row["Name"].ToString(),
                    Description = row["Description"].ToString()
                });
            }
            return list;
        }

        // Single category by ID
        public Category? GetById(int id)
        {
            string query = "SELECT * FROM Categories WHERE CategoryId = @Id";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", id)
            };
            DataTable dt = _db.ExecuteQuery(query, parameters);
            if (dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];
            return new Category
            {
                CategoryId = Convert.ToInt32(row["CategoryId"]),
                Name = row["Name"].ToString(),
                Description = row["Description"].ToString()
            };
        }

        // Add
        public bool Add(Category category)
        {
            string query = @"INSERT INTO Categories (Name, Description) 
                             VALUES (@Name, @Description)";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Name",        category.Name),
                new SqlParameter("@Description", category.Description ?? "")
            };
            return _db.ExecuteNonQuery(query, parameters) > 0;
        }

        // Update
        public bool Update(Category category)
        {
            string query = @"UPDATE Categories 
                             SET Name = @Name, Description = @Description 
                             WHERE CategoryId = @Id";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Name",        category.Name),
                new SqlParameter("@Description", category.Description ?? ""),
                new SqlParameter("@Id",          category.CategoryId)
            };
            return _db.ExecuteNonQuery(query, parameters) > 0;
        }

        // Delete
        public bool Delete(int id)
        {
            string query = "DELETE FROM Categories WHERE CategoryId = @Id";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", id)
            };
            return _db.ExecuteNonQuery(query, parameters) > 0;
        }
    }
}