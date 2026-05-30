using System.Data;
using System.Data.SqlClient;
using ECommerceApp.DataAccess;
using ECommerceApp.Models;

namespace ECommerceApp.Services
{
    public class UserService
    {
        private readonly DbAccess _db;

        public UserService(DbAccess db)
        {
            _db = db;
        }

        // Register — naya user save karo
        public bool Register(string fullName, string email, string password)
        {
            // Pehle check karo email already exist toh nahi karta
            if (EmailExists(email)) return false;

            string hash = BCrypt.Net.BCrypt.HashPassword(password);

            string query = @"INSERT INTO Users 
                            (FullName, Email, PasswordHash, Role, CreatedAt)
                            VALUES 
                            (@FullName, @Email, @Hash, 'Customer', GETDATE())";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FullName", fullName),
                new SqlParameter("@Email",    email),
                new SqlParameter("@Hash",     hash)
            };

            int rows = _db.ExecuteNonQuery(query, parameters);
            return rows > 0;
        }

        // Login — email + password verify karo
        public User? Login(string email, string password)
        {
            string query = "SELECT * FROM Users WHERE Email = @Email";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Email", email)
            };

            DataTable dt = _db.ExecuteQuery(query, parameters);

            if (dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];
            string hash = row["PasswordHash"].ToString();

            // BCrypt se verify karo
            bool isValid = BCrypt.Net.BCrypt.Verify(password, hash);
            if (!isValid) return null;

            return new User
            {
                UserId = Convert.ToInt32(row["UserId"]),
                FullName = row["FullName"].ToString(),
                Email = row["Email"].ToString(),
                Role = row["Role"].ToString()
            };
        }

        // Email already registered hai ya nahi
        public bool EmailExists(string email)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Email", email)
            };
            int count = Convert.ToInt32(_db.ExecuteScalar(query, parameters));
            return count > 0;
        }
    }
}