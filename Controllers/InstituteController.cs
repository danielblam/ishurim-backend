using Ishurim.Models;
using Ishurim.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ishurim.Controllers
{
    [Route("api/institutes")]
    [ApiController]
    public class InstituteController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            AuthService auth = new();
            var token = auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!auth.Authorize(token, AuthService.Roles.USER)) return Unauthorized("Insufficient permission.");

            InstituteService service = new();
            List<Institute> institutes = service.GetAllInstitutes();
            return Ok(institutes);
        }

        [HttpPost]
        public IActionResult Create(Institute institute)
        {
            AuthService auth = new();
            var token = auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            InstituteService service = new();
            service.CreateNewInstitute(institute);

            return Ok();
        }

        [HttpPut]
        public IActionResult Edit(Institute institute)
        {
            AuthService auth = new();
            var token = auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            InstituteService service = new();
            service.EditInstitute(institute);

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            AuthService auth = new();
            var token = auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            InstituteService service = new();
            service.DeleteInstitute(id);

            return Ok();
        }
    }
}
