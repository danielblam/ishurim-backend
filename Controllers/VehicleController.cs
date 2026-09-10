using Ishurim.Models;
using Ishurim.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ishurim.Controllers
{
    [Route("api/vehicles")]
    [ApiController]
    public class VehicleController : Controller
    {
        private readonly AuthService _auth;
        private readonly VehicleService _service;
        public VehicleController(AuthService auth, VehicleService service)
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

            List<Vehicle> vehicles = _service.GetAllVehicles();
            return Ok(vehicles);
        }

        [HttpPost]
        public IActionResult Create(Vehicle vehicle)
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            _service.CreateNewVehicle(vehicle);

            return Ok();
        }

        [HttpPut]
        public IActionResult Edit(Vehicle vehicle)
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            _service.EditVehicle(vehicle);

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return StatusCode(StatusCodes.Status410Gone);

            //var token = _auth.GetToken(Request);
            //if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            //if (!_auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            //_service.DeleteVehicle(id);

            //return Ok();
        }

        [HttpGet("active/{id}")]
        public IActionResult Toggle(int id, bool? active) // will be used to toggle an objects activity
        {
            if (active == null) return BadRequest("Incomplete request.");

            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            var result = _service.SetActive(id, (bool)active);
            if (result == -1) return BadRequest("Something went wrong.");

            return Ok();
        }
    }
}
