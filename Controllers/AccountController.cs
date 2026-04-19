using Ishurim.Models;
using Ishurim.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ishurim.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly AuthService _auth;
        private readonly AccountService _service;
        public AccountController(AuthService auth, AccountService service)
        {
            _auth = auth;
            _service = service;
        }


        [HttpGet]
        public IActionResult GetUsers()
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.USER)) return Unauthorized("Insufficient permission.");

            List<User> users = _service.GetAllUsers();

            return Ok(users);
        }

        [HttpPost]
        public IActionResult CreateUser(User user)
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            var result = _service.CreateAccount(user);
            if (result == -1) return BadRequest("This user already exists.");

            return Ok();
        }

        [HttpDelete("{username}")]
        public IActionResult DeleteUser(string username)
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");
            if (username == _auth.GetUserFromToken(token)) return Forbid("Can't delete your own account.");

            var result = _service.DeleteAccount(username);

            return NoContent();
        }
    }
}
