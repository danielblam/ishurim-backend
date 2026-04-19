using Ishurim.Models;
using Ishurim.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ishurim.Controllers
{
    [Route("api/institutes")]
    [ApiController]
    public class InstituteController : Controller
    {
        private readonly AuthService _auth;
        private readonly InstituteService _service;
        public InstituteController(AuthService auth, InstituteService service)
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

            List<Institute> institutes = _service.GetAllInstitutes();
            return Ok(institutes);
        }

        [HttpPost]
        public IActionResult Create(Institute institute)
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            _service.CreateNewInstitute(institute);

            return Ok();
        }

        [HttpPut]
        public IActionResult Edit(Institute institute)
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            _service.EditInstitute(institute);

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            _service.DeleteInstitute(id);

            return Ok();
        }
    }
}
