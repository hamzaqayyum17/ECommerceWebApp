using System.Data;
using System.Data.SqlClient;

namespace ECommerceApp.DataAccess
{
    public class DbAccess
    {
        private readonly string _connectionString;

        public DbAccess(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        // SELECT queries ke liye — DataTable return karta hai
        public DataTable ExecuteQuery(string query,
                                      SqlParameter[]? parameters = null)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(query, conn);

            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            conn.Open();
            var dt = new DataTable();
            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }

        // INSERT / UPDATE / DELETE ke liye
        public int ExecuteNonQuery(string query,
                                   SqlParameter[]? parameters = null)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(query, conn);

            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        // Single value return ke liye (COUNT, MAX, etc.)
        public object? ExecuteScalar(string query,
                                     SqlParameter[]? parameters = null)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(query, conn);

            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            conn.Open();
            return cmd.ExecuteScalar();
        }
    }
}