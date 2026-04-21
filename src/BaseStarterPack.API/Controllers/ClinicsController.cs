using BaseStarterPack.Application.Common;
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
    public async Task<ActionResult<ApiResponse<IEnumerable<Clinic>>>> GetAll(CancellationToken ct)
    {
        var clinics = await service.GetAllAsync(ct);
        if (clinics is null || !clinics.Any())
            return BadRequest(ApiResponse<object>.Fail("No record found"));

        return Ok(ApiResponse<IEnumerable<Clinic>>.Success(clinics, "Successfully retrieved data"));
    }

    [HttpGet("{clinicId:guid}")]
    public async Task<ActionResult<ApiResponse<Clinic>>> GetById(Guid clinicId, CancellationToken ct)
    {
        var clinic = await service.GetByIdAsync(clinicId, ct);
        if (clinic is null)
            return NotFound(ApiResponse<object>.Fail("No record found"));

        return Ok(ApiResponse<Clinic>.Success(clinic, "Successfully retrieved data"));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] CreateClinicRequest request, CancellationToken ct)
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
            ApiResponse<object>.Success(
                new
                {
    
                    id = createdClinic?.Id
                },
                "Clinic created successfully."));
    }
    [HttpPut("{clinicId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        Guid clinicId,
        [FromBody] UpdateClinicRequest request,
        CancellationToken ct)
    {
        var clinic = await service.GetByIdAsync(clinicId, ct);
        if (clinic is null)
            return NotFound(ApiResponse<object>.Fail("No record found"));

        clinic.Status = request.Status;
        clinic.ClinicNo = request.ClinicNo;
        clinic.Location = request.Location;
        clinic.Landmark = request.Landmark;

        var affected = await service.UpdateAsync(clinic, ct);

        if (affected == 0)
            return Ok(ApiResponse<object>.EmptySuccess("No changes detected"));

        return Ok(ApiResponse<object>.EmptySuccess("Clinic updated successfully"));
    }

    [HttpDelete("{clinicId:Guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid clinicId, CancellationToken ct)
    {
        var ok = await service.DeleteAsync(clinicId, ct);
        if (!ok)
            return NotFound(ApiResponse<object>.Fail("No record found"));

        return Ok(ApiResponse<object>.EmptySuccess("Clinic deleted successfully"));
    }
}
