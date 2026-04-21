namespace BaseStarterPack.API.Controllers;

public sealed class CreateClinicRequest
{
    public string? ClinicNo { get; set; }
    public string? Location { get; set; }
    public string? Landmark { get; set; }
}
