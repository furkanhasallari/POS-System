using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace POS_System.Klasat
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["POSConnection"].ConnectionString;
        }

        // Metoda për autentikimin e përdoruesit
        public User AuthenticateUser(string username, string password)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand(
                    "SELECT UserID, Username, Role FROM Users WHERE Username = @Username AND Password = @Password", con);

                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);

                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User(
                            Convert.ToInt32(reader["UserID"]),
                            reader["Username"].ToString(),
                            "", // Nuk e kthejmë passwordin
                            reader["Role"].ToString()
                        );
                    }
                }
            }
            return null;
        }
    }
}