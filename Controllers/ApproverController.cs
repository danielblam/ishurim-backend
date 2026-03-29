using Ishurim.Models;
using Ishurim.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ishurim.Controllers
{
    [Route("api/approvers")]
    [ApiController]
    public class ApproverController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            AuthService auth = new();
            var token = auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!auth.Authorize(token, AuthService.Roles.USER)) return Unauthorized("Insufficient permission.");

            ApproverService service = new();
            List<Approver> approvers = service.GetAllApprovers();
            return Ok(approvers);
        }

        [HttpPost]
        public IActionResult Create(Approver approver)
        {
            AuthService auth = new();
            var token = auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!auth.Authorize(token, AuthService.Roles.USER)) return Unauthorized("Insufficient permission.");

            ApproverService service = new();
            service.CreateNewApprover(approver);

            return Ok();
        }

        [HttpPut]
        public IActionResult Edit(Approver approver)
        {
            AuthService auth = new();
            var token = auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!auth.Authorize(token, AuthService.Roles.USER)) return Unauthorized("Insufficient permission.");

            ApproverService service = new();
            service.EditApprover(approver);

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            AuthService auth = new();
            var token = auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!auth.Authorize(token, AuthService.Roles.USER)) return Unauthorized("Insufficient permission.");

            ApproverService service = new();
            service.DeleteApprover(id);

            return Ok();
        }
    }
}
