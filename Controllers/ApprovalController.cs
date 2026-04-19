using Ishurim.Models;
using Ishurim.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Ishurim.Controllers
{
    [Route("api/approvals")]
    [ApiController]
    public class ApprovalController : Controller
    {
        private readonly AuthService _auth;
        private readonly ApprovalService _service;
        private readonly PdfService _pdfService;
        public ApprovalController(AuthService auth, ApprovalService service, PdfService pdfService)
        {
            _auth = auth;
            _service = service;
            _pdfService = pdfService;
        }


        [HttpGet]
        public IActionResult Get()
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.USER)) return Unauthorized("Insufficient permission.");

            List<Approval> approvals = _service.GetAllApprovals();
            return Ok(approvals);
        }

        [HttpPost]
        public IActionResult Create(Approval approval)
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.USER)) return Unauthorized("Insufficient permission.");

            _service.CreateNewApproval(approval);

            return Ok();
        }

        [HttpPut]
        public IActionResult Edit(Approval approval)
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.USER)) return Unauthorized("Insufficient permission.");

            _service.EditApproval(approval);

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.ADMIN)) return Unauthorized("Insufficient permission.");

            _service.DeleteApproval(id);

            return Ok();
        }

        [HttpGet("pdf/{id}")]
        public IActionResult GetPdf(int id)
        {
            var token = _auth.GetToken(Request);
            if (token == null) return BadRequest("Authorization header is missing or incorrect.");
            if (!_auth.Authorize(token, AuthService.Roles.USER)) return Unauthorized("Insufficient permission.");

            Approval approval = _service.GetAllApprovals().Find(x => x.ApprovalId == id);

            var pdfBytes = _pdfService.GenerateApproval(approval);

            return File(pdfBytes, "application/pdf", "approval.pdf");
        }
    }
}
