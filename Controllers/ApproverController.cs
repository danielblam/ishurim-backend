using Ishurim.Models;
using Ishurim.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ishurim.Controllers
{
    [Route("api/approvers")]
    [ApiController]
    public class ApproverController : Controller
    {
        private readonly AuthService _auth;
        private readonly ApproverService _service;
        public ApproverController(AuthService auth, ApproverService service)
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

            List<Approver> approvers = _service.GetAllApprovers();
            return Ok(approvers);
        }

        [HttpPost]
        public IActionResult Create(Approver approver)
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            _service.CreateNewApprover(approver);

            return Ok();
        }

        [HttpPut]
        public IActionResult Edit(Approver approver)
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            _service.EditApprover(approver);

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            _service.DeleteApprover(id);

            return Ok();
        }
    }
}
