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

    [HttpGet("{clinicId:guid}")]
    public async Task<ActionResult<Clinic>> GetById(Guid clinicId, CancellationToken ct)
    {
        var clinic = await service.GetByIdAsync(clinicId, ct);
        return clinic is null ? NotFound() : Ok(clinic);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<object>> Create([FromBody] CreateClinicRequest request, CancellationToken ct)
    {
        var clinic = new Clinic
        {
            ClinicNo = request.ClinicNo,
            Location = request.Location,
            Landmark = request.Landmark
        };

        var clinicId = await service.CreateAsync(clinic, ct);
        var createdClinic = await service.GetByIdAsync(clinicId, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { clinicId },
            new
            {
                clinicId,
                id = createdClinic?.Id
            });
    }
    [HttpPut("{clinicId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        Guid clinicId,
        [FromBody] UpdateClinicRequest request,
        CancellationToken ct)
    {
        // 1. Fetch directly by ID (no full table scan)
        var clinic = await service.GetByIdAsync(clinicId, ct);
        if (clinic is null)
            return NotFound();

        // 2. Map updates
        clinic.Status = request.Status;
        clinic.ClinicNo = request.ClinicNo;
        clinic.Location = request.Location;
        clinic.Landmark = request.Landmark;

        // 3. Save
        var affected = await service.UpdateAsync(clinic, ct);

        // 4. Response strategy
        if (affected == 0)
        {
            // No actual changes were made
            return Ok(new { message = "No changes detected" });
        }

        return NoContent(); // standard for successful update
    }

    [HttpDelete("{clinicId:Guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid clinicId, CancellationToken ct)
    {
        var ok = await service.DeleteAsync(clinicId, ct);
        return ok ? NoContent() : NotFound();
    }
}
