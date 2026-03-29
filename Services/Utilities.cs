using System.Security.Cryptography;
using System.Text;

namespace Ishurim.Services
{
    public class Utilities
    {
        private static readonly string connectionString = new DbService().connectionString;
        public string GenerateToken()
        {
            Random rng = new();
            string token = "";
            string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            for (int i = 0; i < 64; i++)
            {
                token += characters[rng.Next(characters.Length)];
            }
            return token;
        }
        public string Sha256(string str)
        {
            using SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(str));
            string hash = "";
            foreach (byte b in bytes) hash += b.ToString("x2");
            return hash;
        }
    }
}
