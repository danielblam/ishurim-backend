namespace Ishurim.Models
{
    public class Approval
    {
        public int ApprovalId { get; set; }
        public string HospitalizationId { get; set; }
        public DateOnly Date { get; set; }
        public int TestId { get; set; }
        public string TestCode { get; set; } // Super Fun Mystery Field
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int IdNumber { get; set; }
        public int DepartmentId { get; set; }
        public int VehicleId { get; set; }
        public int ApproverId { get; set; }
        public string Clerk { get; set; }
        public int InstituteId { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
