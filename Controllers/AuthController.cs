using Ishurim.Models;
using Ishurim.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ishurim.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(AuthService service, AccountService accountservice) : Controller
    {
        private readonly AuthService _service = service;
        private readonly AccountService _accountservice = accountservice;

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

        [Authorize]
        [HttpGet("windowslogin")]
        public IActionResult WindowsLogin()
        {
            string name = User.Identity?.Name;
            if (name == null) return BadRequest("User is null.");

            int? createRole = null;
            string domain = "CARMEL";
            if (!User.IsInRole($"{domain}\\ISHURIM")) return Unauthorized($"אין לך גישה למערכת . ({name})");
            if (User.IsInRole($"{domain}\\IshurimUser")) createRole = 0;
            if (User.IsInRole($"{domain}\\IshurimAdmin")) createRole = -1;

            if (createRole == null) return Unauthorized($"אין לך גישה למערכת . ({name})");

            var result = _service.WindowsAuthLogIn(name);
            if(result == -1000)
            {
                result = _accountservice.CreateAccount(new User()
                {
                    Username = name,
                    Password = "",
                    Role = (int)createRole
                    //Role = 0
                });
            }

            var token = (new Utilities()).GenerateToken();

            _service.SaveToken(name, token);

            LoginResponse response = new()
            {
                Token = token,
                Role = result,
                Name = name
            };

            return Ok(response);
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
