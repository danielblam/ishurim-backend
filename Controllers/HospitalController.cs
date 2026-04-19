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
        private readonly AuthService _auth;
        private readonly HospitalService _service;
        public HospitalController(AuthService auth, HospitalService service)
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

            List<Hospital> hospitals = _service.GetAllHospitals();
            return Ok(hospitals);
        }

        [HttpPost]
        public IActionResult Create(Hospital hospital)
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            _service.CreateNewHospital(hospital);

            return Ok();
        }

        [HttpPut]
        public IActionResult Edit(Hospital hospital)
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            _service.EditHospital(hospital);

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            _service.DeleteHospital(id);

            return Ok();
        }
    }
}
