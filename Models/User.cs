namespace Ishurim.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string PasswordHash { get; set; }
        public int Role { get; set; }
        public bool IsActive { get; set; }
    }
}
