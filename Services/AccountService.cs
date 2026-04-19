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
        private readonly string connectionString;
        private readonly AuthService _auth;

        public AccountService(IConfiguration config, AuthService auth)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
            _auth = auth;
        }
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
                        Username = reader.GetString(0),
                        Password = string.Empty,
                        Role = reader.GetInt32(2)
                    };
                    users.Add(user);
                }
            }
            return users;
        }

        public User GetUserByName(string username)
        {
            using (SqlConnection sqlCon = new(connectionString))
            {
                sqlCon.Open();

                SqlCommand command = new($"SELECT * FROM Users WHERE [User] = @user", sqlCon);
                command.Parameters.AddWithValue("@user", username);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    User user = new()
                    {
                        Username = reader.GetString(0),
                        Password = string.Empty,
                        Role = reader.GetInt32(2)
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

            SqlCommand check = new($"SELECT * FROM Users WHERE [User] = @user", sqlCon);
            check.Parameters.AddWithValue("@user", user.Username);

            using (SqlDataReader reader = check.ExecuteReader())
            {
                if (reader.HasRows) return -1;
            }

            string hashedpassword = _auth.HashPassword(user.Password);
            //string hashedpassword = user.Password; // this is (hopefully) temporary

            SqlCommand command = new(
                $"INSERT INTO Users ([User], Sisma, Harshaot) VALUES (@username, @passwordHash, 0);" +
                $"SELECT SCOPE_IDENTITY();", sqlCon);
            command.Parameters.AddWithValue("@username", user.Username);
            command.Parameters.AddWithValue("@passwordHash", hashedpassword);

            int newUserID = int.Parse(command.ExecuteScalar().ToString());
            return newUserID;
        }

        public int DeleteAccount(string username)
        {
            using SqlConnection sqlCon = new(connectionString);
            sqlCon.Open();

            SqlCommand command = new(
                $"DELETE FROM Users WHERE [User] = @user", sqlCon);
            command.Parameters.AddWithValue("@user", username);

            command.ExecuteNonQuery();
            return 0;
        }
    }

}
