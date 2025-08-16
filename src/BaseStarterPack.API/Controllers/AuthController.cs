using BaseStarterPack.Application.DTOs;
using BaseStarterPack.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseStarterPack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService auth) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest req, CancellationToken ct)
    {
        try
        {
            var result = await auth.RegisterAsync(req, ct);
            return StatusCode(201, result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            // include inner exception message to diagnose schema mismatches
            var detail = ex.InnerException?.Message ?? ex.Message;
            return Problem(title: "Registration failed", detail: detail, statusCode: 400);
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest req, CancellationToken ct)
    {
        try
        {
            var result = await auth.LoginAsync(req, ct);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return Problem(title: "Login failed", detail: ex.InnerException?.Message ?? ex.Message, statusCode: 400);
        }
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshTokenRequest req, CancellationToken ct)
    {
        try
        {
            var result = await auth.RefreshAsync(req, ct);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return Problem(title: "Refresh token failed", detail: ex.InnerException?.Message ?? ex.Message, statusCode: 400);
        }
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public IActionResult ForgotPassword([FromBody] object _)
        => Ok(new { message = "If the email exists, a reset link will be sent." });

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var sub = User.Claims.FirstOrDefault(c => c.Type is "sub" or "nameidentifier")?.Value
                  ?? User.Identity?.Name;
        if (!Guid.TryParse(sub, out var id)) return Unauthorized();
        var me = await auth.GetCurrentUserAsync(id, ct);
        return me is null ? NotFound() : Ok(me);
    }
}
