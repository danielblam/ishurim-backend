using Ishurim.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Ishurim.Services.AuthService;

namespace Ishurim.Services
{
    public class AccountService
    {
        private static readonly string connectionString = new DbService().connectionString;
        public List<User> GetAllUsers()
        {
            List<User> users = [];
            using (SqlConnection sqlCon = new(connectionString))
            {
                sqlCon.Open();

                SqlCommand command = new($"SELECT * FROM Users", sqlCon);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    User user = new()
                    {
                        UserId = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        PasswordHash = string.Empty,
                        FullName = reader.GetString(3),
                        Role = reader.GetByte(4),
                        IsActive = reader.GetBoolean(5)
                    };
                    users.Add(user);
                }
            }
            return users;
        }

        public User GetUserById(int id)
        {
            using (SqlConnection sqlCon = new(connectionString))
            {
                sqlCon.Open();

                SqlCommand command = new($"SELECT * FROM Users WHERE UserId = @id", sqlCon);
                command.Parameters.AddWithValue("@id", id);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    User user = new()
                    {
                        UserId = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        PasswordHash = string.Empty,
                        FullName = reader.GetString(3),
                        Role = reader.GetByte(4),
                        IsActive = reader.GetBoolean(5)
                    };
                    return user;
                }
            }
            return null;
        }

        public int CreateAccount(User user)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();


            SqlCommand check = new($"SELECT * FROM Users WHERE Username = @username", sqlCon);
            check.Parameters.AddWithValue("@username", user.Username);

            using (SqlDataReader reader = check.ExecuteReader())
            {
                if (reader.HasRows) return -1;
            }

            AuthService auth = new();
            string hashedpassword = auth.HashPassword(user.PasswordHash);

            SqlCommand command = new(
                $"INSERT INTO Users (Username, PasswordHash, FullName, Role, IsActive) VALUES (@username, @passwordHash, @fullName, @role, 1);" +
                $"SELECT SCOPE_IDENTITY();", sqlCon);
            command.Parameters.AddWithValue("@username", user.Username);
            command.Parameters.AddWithValue("@passwordHash", hashedpassword);
            command.Parameters.AddWithValue("@fullName", user.FullName);
            command.Parameters.AddWithValue("@role", user.Role);

            int newUserID = int.Parse(command.ExecuteScalar().ToString());
            return newUserID;
        }

        public int DeleteAccount(int userId)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new(
                $"DELETE FROM Users WHERE UserId = @userId", sqlCon);
            command.Parameters.AddWithValue("@userId", userId);

            command.ExecuteNonQuery();
            return 0;
        }
    }

}
