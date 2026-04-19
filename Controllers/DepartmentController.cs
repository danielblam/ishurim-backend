using Ishurim.Models;
using Ishurim.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Ishurim.Controllers
{
    [Route("api/departments")]
    [ApiController]
    public class DepartmentController : Controller
    {
        private readonly AuthService _auth;
        private readonly DepartmentService _service;
        public DepartmentController(AuthService auth, DepartmentService service)
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

            List<Department> departments = _service.GetAllDepartments();
            return Ok(departments);
        }

        [HttpPost]
        public IActionResult Create(Department department)
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            _service.CreateNewDepartment(department);

            return Ok();
        }

        [HttpPut]
        public IActionResult Edit(Department department)
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            _service.EditDepartment(department);

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            _service.DeleteDepartment(id);

            return Ok();
        }
    }
}
