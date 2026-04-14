namespace Ishurim.Models
{
    public class Institute
    {
        public int InstituteId { get; set; }
        public string Name { get; set; }
        public int? HospitalId { get; set; } // 0 = self, maybe? or 1? define כרמל as a hospital then?
    }
}
