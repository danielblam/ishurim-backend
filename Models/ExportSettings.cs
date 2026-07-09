namespace Ishurim.Models
{
    public class ExportSettings
    {
        public string DateRange { get; set; }
        public int StartYear { get; set; }
        public int StartMonth { get; set; }
        public int EndYear { get; set; }
        public int EndMonth { get; set; }
        public int QuarterYear { get; set; }
        public int QuarterNumber { get; set; }


        public string ByInstitutesOrHospitals { get; set; }
        public List<int> InstituteIds { get; set; }
        public List<int> HospitalIds { get; set; }


        public string ByDepartments { get; set; }
        public List<int> DepartmentIds { get; set; }


        public string ByTests { get; set; }
        public List<int> TestIds { get; set; }
    }
}
