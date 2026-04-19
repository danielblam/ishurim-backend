using Ishurim.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;

namespace Ishurim.Services
{
    public class AuthService(IConfiguration config)
    {
        private readonly string connectionString = config.GetConnectionString("DefaultConnection");
        private static readonly string salt = String.Empty; //new DbService().salt;
        public enum Roles
        {
            USER = 0,
            ADMIN = -1
        }
        // Normal login
        public int LogIn(LoginDetails details)
        {
            using SqlConnection sqlCon = new(connectionString);
            SqlCommand command = new($"SELECT * FROM Users WHERE [User] = @username", sqlCon);
            command.Parameters.AddWithValue("@username", details.Username);

            sqlCon.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string user = reader.GetString(0);
                string password = reader.GetString(1);
                if (HashPassword(details.Password) != password) return -1000; // Incorrect password
                int role = reader.GetInt32(2);
                return role;
            }
            return -2000; // Username doesn't exist, so the while block is never entered
        }

        public bool Authorize(string token, Roles requiredRole)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command1 = new($"SELECT * FROM Sessions WHERE Token = @token", sqlCon);
            command1.Parameters.AddWithValue("@token", token);

            using SqlDataReader sessions = command1.ExecuteReader();
            if (!sessions.HasRows) return false;
            sessions.Read();
            long timestamp = sessions.GetInt64(2);
            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - timestamp > 5400) return false; // Tokens expire after 90 minutes  ...  Yes, it's an arbitrary number, okay
            string user = sessions.GetString(0);
            sessions.Dispose();

            SqlCommand command2 = new($"SELECT * FROM Users WHERE [User] = @user", sqlCon);
            command2.Parameters.AddWithValue("@user", user);

            using SqlDataReader users = command2.ExecuteReader();
            if (!users.HasRows) return false;
            users.Read();
            int role = users.GetInt32(2);

            if (role <= (int)requiredRole) return true;
            return false;
        }

        public string HashPassword(string password)
        {
            //Utilities service = new();
            //return service.Sha256(password + salt);
            return password; // temporary, hopefully
        }

        public string? GetToken(HttpRequest Request)
        {
            if (Request.Headers.TryGetValue("Authorization", out StringValues authHeader))
            {
                try
                {
                    string token = authHeader.ToString();
                    Debug.WriteLine(token);
                    token = token.Split(" ")[1];
                    return token;
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public void SaveToken(string username, string token)
        {
            using SqlConnection sqlCon = new(connectionString);
            SqlCommand check = new($"SELECT * FROM Sessions WHERE [User] = @user", sqlCon);
            check.Parameters.AddWithValue("@user", username);

            sqlCon.Open();
            bool hasrows;
            using (SqlDataReader reader = check.ExecuteReader())
            {
                hasrows = reader.HasRows; // I dont like having to do this weird maneuver but whatever
            }
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (hasrows)
            {
                SqlCommand command = new(
                $"UPDATE Sessions " +
                $"SET Token = @token, Timestamp = @timestamp " +
                $"WHERE [User] = @user"
                , sqlCon);
                command.Parameters.AddWithValue("@token", token);
                command.Parameters.AddWithValue("@timestamp", timestamp);
                command.Parameters.AddWithValue("@user", username);

                command.ExecuteNonQuery();
            }
            else
            {
                SqlCommand command = new(
                $"INSERT INTO Sessions ([User], Token, Timestamp) VALUES (@user,@token,@timestamp)"
                , sqlCon);
                command.Parameters.AddWithValue("@token", token);
                command.Parameters.AddWithValue("@timestamp", timestamp);
                command.Parameters.AddWithValue("@user", username);

                command.ExecuteNonQuery();
            }
        }

        public string GetUserFromToken(string token)
        {
            SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"SELECT * FROM Sessions WHERE Token = @token", sqlCon);
            command.Parameters.AddWithValue("@token", token);

            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            string user = reader.GetString(0);

            reader.Dispose();
            sqlCon.Close();

            return user;
        }
    }
}
