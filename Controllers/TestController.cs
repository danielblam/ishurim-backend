using Ishurim.Models;
using Ishurim.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ishurim.Controllers
{
    [Route("api/tests")]
    [ApiController]
    public class TestController : Controller
    {
        private readonly AuthService _auth;
        private readonly TestService _service;
        public TestController(AuthService auth, TestService service)
        {
            _auth = auth;
            _service = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.USER)) return Unauthorized("Insufficient permission.");

            List<Test> tests = _service.GetAllTests();
            return Ok(tests);
        }

        [HttpPost]
        public IActionResult Create(Test test)
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            _service.CreateNewTest(test);

            return Ok();
        }

        [HttpPut]
        public IActionResult Edit(Test test)
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            _service.EditTest(test);

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            _service.DeleteTest(id);

            return Ok();
        }
    }
}
