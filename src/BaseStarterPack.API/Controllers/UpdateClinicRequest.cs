namespace BaseStarterPack.API.Controllers;

public sealed class UpdateClinicRequest
{
    public char Status { get; set; }
    public string? ClinicNo { get; set; }
    public string? Location { get; set; }
    public string? Landmark { get; set; }
}
