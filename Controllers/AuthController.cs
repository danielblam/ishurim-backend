using Ishurim.Models;
using Ishurim.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ishurim.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly AuthService _service;
        public AuthController(AuthService service)
        {
            _service = service;
        }


        [HttpGet("ping")]
        public IActionResult Ping()
        {
            var token = _service.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect");

            if (_service.Authorize(token, AuthService.Roles.USER)) return Ok();
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDetails details)
        {
            var result = _service.LogIn(details);
            switch (result)
            {
                case -1000: return Unauthorized("Incorrect password.");
                case -2000: return NotFound("No account with this username.");
            }
            var token = (new Utilities()).GenerateToken();

            _service.SaveToken(details.Username, token);

            LoginResponse response = new()
            {
                Token = token,
                Role = result
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
