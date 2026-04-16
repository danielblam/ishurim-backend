namespace Ishurim.Models
{
    public class LoginResponse
    {
        public required string Token { get; set; }
        public int Role { get; set; }
    }
}
