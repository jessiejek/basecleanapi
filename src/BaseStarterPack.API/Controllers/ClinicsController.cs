using BaseStarterPack.Application.Services;
using BaseStarterPack.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseStarterPack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClinicsController(ClinicService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Clinic>>> GetAll(CancellationToken ct)
        => Ok(await service.GetAllAsync(ct));

    [HttpGet("{clinicId:int}")]
    public async Task<ActionResult<Clinic>> GetById(int clinicId, CancellationToken ct)
    {
        var clinic = await service.GetByIdAsync(clinicId, ct);
        return clinic is null ? NotFound() : Ok(clinic);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<int>> Create([FromBody] Clinic clinic, CancellationToken ct)
    {
        var id = await service.CreateAsync(clinic, ct);
        return CreatedAtAction(nameof(GetById), new { clinicId = clinic.ClinicId }, id);
    }

    [HttpPut("{clinicId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int clinicId, [FromBody] Clinic clinic, CancellationToken ct)
    {
        if (clinic.ClinicId != clinicId) return BadRequest("Mismatched clinicId.");
        var ok = await service.UpdateAsync(clinic, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{clinicId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int clinicId, CancellationToken ct)
    {
        var ok = await service.DeleteAsync(clinicId, ct);
        return ok ? NoContent() : NotFound();
    }
}
