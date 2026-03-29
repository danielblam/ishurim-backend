using Ishurim.Models;
using Ishurim.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ishurim.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult GetUsers()
        {
            AuthService auth = new();
            var token = auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!auth.Authorize(token, AuthService.Roles.USER)) return Unauthorized("Insufficient permission.");

            AccountService service = new();
            List<User> users = service.GetAllUsers();

            return Ok(users);
        }

        [HttpPost]
        public IActionResult CreateUser(User user)
        {
            AuthService auth = new();
            var token = auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            AccountService service = new();
            var result = service.CreateAccount(user);
            if (result == -1) return BadRequest("This user already exists.");

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            AuthService auth = new();
            var token = auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");
            if (id == auth.GetUserIdFromToken(token)) return Forbid("Can't delete your own account.");

            AccountService service = new();
            var result = service.DeleteAccount(id);

            return NoContent();
        }
    }
}
