using Ishurim.Models;
using Ishurim.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Ishurim.Controllers
{
    [Route("api/hospitals")]
    [ApiController]
    public class HospitalController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            AuthService auth = new();
            var token = auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!auth.Authorize(token, AuthService.Roles.USER)) return Unauthorized("Insufficient permission.");

            HospitalService service = new();
            List<Hospital> hospitals = service.GetAllHospitals();
            return Ok(hospitals);
        }

        [HttpPost]
        public IActionResult Create(Hospital hospital)
        {
            AuthService auth = new();
            var token = auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            HospitalService service = new();
            service.CreateNewHospital(hospital);

            return Ok();
        }

        [HttpPut]
        public IActionResult Edit(Hospital hospital)
        {
            AuthService auth = new();
            var token = auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            HospitalService service = new();
            service.EditHospital(hospital);

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            AuthService auth = new();
            var token = auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            HospitalService service = new();
            service.DeleteHospital(id);

            return Ok();
        }
    }
}
