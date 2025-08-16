namespace BaseStarterPack.Domain.Entities;

public class Clinic : BaseEntity
{
    public int ClinicId { get; set; }
    public string? ClinicNo { get; set; }
    public string? Location { get; set; }
    public string? Landmark { get; set; }
}
