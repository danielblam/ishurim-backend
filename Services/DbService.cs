namespace Ishurim.Services
{
    public class DbService
    {
        public string connectionString;
        public string salt = "rFsAaP7Dz5yvetv7oqleJ0ynkqyYdica";

        public DbService(IConfiguration config)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
        }
    }
}
