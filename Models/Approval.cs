namespace Ishurim.Models
{
    public class Approval
    {
        public int ApprovalId { get; set; }
        public string HospitalizationId { get; set; }
        public DateOnly ApprovalDate { get; set; }
        public int TestId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdNumber { get; set; }
        public string Department { get; set; }
        public int VehicleId { get; set; }
        public int ApproverId { get; set; }
        public int ClerkId { get; set; }
        public int InstituteId { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
