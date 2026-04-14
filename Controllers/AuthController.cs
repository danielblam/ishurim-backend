using Ishurim.Models;
using Ishurim.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ishurim.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            AuthService service = new();
            var token = service.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect");

            if (service.Authorize(token, AuthService.Roles.USER)) return Ok();
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDetails details)
        {
            AuthService service = new();
            var userId = service.LogIn(details);
            switch (userId)
            {
                case -1: return Unauthorized("Incorrect password.");
                case -2: return NotFound("No account with this username.");
            }
            var token = (new Utilities()).GenerateToken();

            service.SaveToken(details.Username, token);

            LoginResponse response = new()
            {
                Token = token
            };

            //Response.Cookies.Append("Session", token, new CookieOptions
            //{
            //    HttpOnly = true,
            //    Secure = true,
            //    SameSite = SameSiteMode.Strict,
            //    Expires = DateTime.UtcNow.AddHours(2)
            //}); // would be nice to use eventually, but i dont have time for this right now

            return Ok(response);
        }
    }
}
