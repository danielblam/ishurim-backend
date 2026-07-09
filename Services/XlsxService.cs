using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Ishurim.Models;

namespace Ishurim.Services
{
    public class XlsxService
    {
        private readonly ApproverService _approverService;
        private readonly HospitalService _hospitalService;
        private readonly TestService _testService;
        private readonly InstituteService _instituteService;
        private readonly DepartmentService _departmentService;
        private readonly VehicleService _vehicleService;

        public XlsxService(ApproverService approverService, HospitalService hospitalService, TestService testService,
            InstituteService instituteService, DepartmentService departmentService, VehicleService vehicleService)
        {
            _approverService = approverService;
            _hospitalService = hospitalService;
            _testService = testService;
            _instituteService = instituteService;
            _departmentService = departmentService;
            _vehicleService = vehicleService;
        }

        public byte[] ExportToXlsx(List<Approval> approvals, ExportSettings exportSettings)
        {
            switch(exportSettings.DateRange)
            {
                case "all":
                    break;
                case "monthRange":
                    approvals = (List<Approval>)approvals.Where(approval =>
                    {
                        if (approval.Date == null) return false;

                        var date = (DateOnly)approval.Date;
                        var startDate = new DateOnly(exportSettings.StartYear, exportSettings.StartMonth, 1);
                        int lastDay = DateTime.DaysInMonth(exportSettings.EndYear, exportSettings.EndMonth);
                        var endDate = new DateOnly(exportSettings.EndYear, exportSettings.EndMonth, lastDay);

                        return date >= startDate && date <= endDate;
                    });
                    break;
                case "quarter":
                    approvals = (List<Approval>)approvals.Where(approval =>
                    {
                        if (approval.Date == null) return false;

                        var date = (DateOnly)approval.Date;
                        var startDate = new DateOnly(exportSettings.QuarterYear, exportSettings.QuarterNumber * 3, 1);
                        int lastDay = DateTime.DaysInMonth(exportSettings.QuarterYear, exportSettings.QuarterNumber * 3 + 2);
                        var endDate = new DateOnly(exportSettings.QuarterYear, exportSettings.QuarterNumber * 3 + 2, lastDay);

                        return date >= startDate && date <= endDate;
                    });
                    break;
            }

            switch(exportSettings.ByInstitutesOrHospitals)
            {
                case "all":
                    break;
                case "institutes":
                    approvals = approvals.Where(approval =>
                    {
                        return exportSettings.InstituteIds.Contains(approval.InstituteId);
                    }).ToList();
                    break;
                case "hospitals":
                    approvals = approvals.Where(approval =>
                    {
                        Institute institute = _instituteService.GetInstituteById(approval.InstituteId);
                        int? hospitalId = institute.HospitalId;
                        return hospitalId == null ? false : exportSettings.HospitalIds.Contains((int)hospitalId);
                    }).ToList();
                    break;
            }

            if(exportSettings.ByDepartments == "departments")
            {
                approvals = approvals.Where(approval =>
                {
                    return approval.DepartmentId == null ? false : exportSettings.DepartmentIds.Contains((int)approval.DepartmentId);
                }).ToList();
            }

            if(exportSettings.ByTests == "tests")
            {
                approvals = approvals.Where(approval =>
                {
                    return exportSettings.TestIds.Contains(approval.TestId);
                }).ToList();
            }

            using var workbook = new XLWorkbook();

            var ws = workbook.Worksheets.Add("Approvals");

            ws.Cell(1, 1).Value = "מספר שובר";
            ws.Cell(1, 2).Value = "מספר אשפוז";
            ws.Cell(1, 3).Value = "תאריך";
            ws.Cell(1, 4).Value = "בית חולים";
            ws.Cell(1, 5).Value = "מכון";
            ws.Cell(1, 6).Value = "מחלקה שולחת";
            ws.Cell(1, 7).Value = "סוג בדיקה";
            ws.Cell(1, 8).Value = "המאשר";
            ws.Cell(1, 9).Value = "פקיד";
            ws.Cell(1, 10).Value = "שם החולה";
            ws.Cell(1, 11).Value = "תעודת זהות";
            ws.Cell(1, 12).Value = "כלי תחבורה";
            ws.Cell(1, 13).Value = "הערה";

            ws.RightToLeft = true;

            ws.Columns().Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Right;

            ws.Row(1).Style.Font.Bold = true;

            int row = 2;

            foreach(Approval approval in approvals)
            {
                Institute institute = _instituteService.GetInstituteById(approval.InstituteId);
                int? hospitalId = institute.HospitalId;
                string hospitalName = hospitalId == null ? "" : _hospitalService.GetHospitalById((int)hospitalId).Name;

                ws.Cell(row, 1).Value = approval.ApprovalId;
                ws.Cell(row, 2).Value = approval.HospitalizationId;
                ws.Cell(row, 3).Value = approval.Date == null ? "" : ((DateOnly)approval.Date).ToString("dd/MM/yyyy");
                ws.Cell(row, 4).Value = hospitalName;
                ws.Cell(row, 5).Value = institute.Name;
                ws.Cell(row, 6).Value = approval.DepartmentId == null ? "" : _departmentService.GetDepartmentById((int)approval.DepartmentId).Name;
                ws.Cell(row, 7).Value = _testService.GetTestById(approval.TestId).Name; // testid is not null in SQL so...
                ws.Cell(row, 8).Value = _approverService.GetApproverById(approval.ApproverId).FullName;
                ws.Cell(row, 9).Value = approval.Clerk;
                ws.Cell(row, 10).Value = approval.FirstName + " " + approval.LastName;
                ws.Cell(row, 11).Value = approval.IdNumber;
                ws.Cell(row, 12).Value = approval.VehicleId == null ? "" : _vehicleService.GetVehicleById((int)approval.VehicleId).Name;
                ws.Cell(row, 13).Value = approval.Note;

                row++;
            }

            ws.Columns().AdjustToContents();

            var range = ws.RangeUsed();

            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            ws.SheetView.FreezeRows(1);

            using MemoryStream stream = new();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }
    }
}
