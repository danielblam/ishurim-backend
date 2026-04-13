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
        [HttpGet]
        public IActionResult Get()
        {
            AuthService auth = new();
            var token = auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!auth.Authorize(token, AuthService.Roles.USER)) return Unauthorized("Insufficient permission.");

            DepartmentService service = new();
            List<Department> departments = service.GetAllDepartments();
            return Ok(departments);
        }

        [HttpPost]
        public IActionResult Create(Department department)
        {
            AuthService auth = new();
            var token = auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!auth.Authorize(token, AuthService.Roles.USER)) return Unauthorized("Insufficient permission.");

            DepartmentService service = new();
            service.CreateNewDepartment(department);

            return Ok();
        }

        [HttpPut]
        public IActionResult Edit(Department department)
        {
            AuthService auth = new();
            var token = auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!auth.Authorize(token, AuthService.Roles.USER)) return Unauthorized("Insufficient permission.");

            DepartmentService service = new();
            service.EditDepartment(department);

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            AuthService auth = new();
            var token = auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!auth.Authorize(token, AuthService.Roles.USER)) return Unauthorized("Insufficient permission.");

            DepartmentService service = new();
            service.DeleteDepartment(id);

            return Ok();
        }
    }
}
