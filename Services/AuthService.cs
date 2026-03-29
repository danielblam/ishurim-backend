using Ishurim.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;

namespace Ishurim.Services
{
    public class AuthService
    {
        private static readonly string connectionString = new DbService().connectionString;
        private static readonly string salt = new DbService().salt;
        public enum Roles
        {
            USER = 1,
            ADMIN = 10
        }
        // Normal login
        public int LogIn(LoginDetails details)
        {
            using SqlConnection sqlCon = new(connectionString);
            SqlCommand command = new($"SELECT * FROM Users WHERE Username = @username", sqlCon);
            command.Parameters.AddWithValue("@username", details.Username);

            sqlCon.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int userId = reader.GetInt32(0);
                string password = reader.GetString(2);
                if (HashPassword(details.Password) != password) return -1; // Incorrect password
                return userId;
            }
            return -2; // Username doesn't exist, so the while block is never entered
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
            int userId = sessions.GetInt32(0);
            sessions.Dispose();

            SqlCommand command2 = new($"SELECT * FROM Users WHERE UserId = @userId", sqlCon);
            command2.Parameters.AddWithValue("@userId", userId);

            using SqlDataReader users = command2.ExecuteReader();
            if (!users.HasRows) return false;
            users.Read();
            byte role = users.GetByte(4);

            if (role >= (byte)requiredRole) return true;
            return false;
        }

        public string HashPassword(string password)
        {
            Utilities service = new();
            return service.Sha256(password + salt);
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

        public void SaveToken(int userId, string token)
        {
            using SqlConnection sqlCon = new(connectionString);
            SqlCommand check = new($"SELECT * FROM Sessions WHERE UserId = @userId", sqlCon);
            check.Parameters.AddWithValue("@userId", userId);

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
                $"WHERE UserId = @userId"
                , sqlCon);
                command.Parameters.AddWithValue("@token", token);
                command.Parameters.AddWithValue("@timestamp", timestamp);
                command.Parameters.AddWithValue("@userId", userId);

                command.ExecuteNonQuery();
            }
            else
            {
                SqlCommand command = new(
                $"INSERT INTO Sessions (UserId, Token, Timestamp) VALUES (@userId,@token,@timestamp)"
                , sqlCon);
                command.Parameters.AddWithValue("@token", token);
                command.Parameters.AddWithValue("@timestamp", timestamp);
                command.Parameters.AddWithValue("@userId", userId);

                command.ExecuteNonQuery();
            }
        }

        public int GetUserIdFromToken(string token)
        {
            SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new($"SELECT * FROM Sessions WHERE Token = @token", sqlCon);
            command.Parameters.AddWithValue("@token", token);

            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            int userId = reader.GetInt32(0);

            reader.Dispose();
            sqlCon.Close();

            return userId;
        }
    }
}
