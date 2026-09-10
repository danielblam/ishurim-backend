namespace Ishurim.Models
{
    public class Institute
    {
        public int InstituteId { get; set; }
        public string Name { get; set; }
        public int? HospitalId { get; set; }
        public bool Active { get; set; }
    }
}
