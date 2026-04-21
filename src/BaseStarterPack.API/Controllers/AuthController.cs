using BaseStarterPack.Application.Common;
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
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(RegisterRequest req, CancellationToken ct)
    {
        try
        {
            var result = await auth.RegisterAsync(req, ct);
            return StatusCode(201, ApiResponse<AuthResponse>.Success(result, "Registration successful."));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<object>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            var detail = ex.InnerException?.Message ?? ex.Message;
            return BadRequest(ApiResponse<object>.Fail($"Registration failed: {detail}"));
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(LoginRequest req, CancellationToken ct)
    {
        try
        {
            var result = await auth.LoginAsync(req, ct);
            return Ok(ApiResponse<AuthResponse>.Success(result, "Login successful."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<object>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            var detail = ex.InnerException?.Message ?? ex.Message;
            return BadRequest(ApiResponse<object>.Fail($"Login failed: {detail}"));
        }
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Refresh(RefreshTokenRequest req, CancellationToken ct)
    {
        try
        {
            var result = await auth.RefreshAsync(req, ct);
            return Ok(ApiResponse<AuthResponse>.Success(result, "Token refreshed successfully."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<object>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            var detail = ex.InnerException?.Message ?? ex.Message;
            return BadRequest(ApiResponse<object>.Fail($"Refresh token failed: {detail}"));
        }
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public IActionResult ForgotPassword([FromBody] object _)
        => Ok(ApiResponse<object>.EmptySuccess("If the email exists, a reset link will be sent."));

   /* [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var sub = User.Claims.FirstOrDefault(c => c.Type is "sub" or "nameidentifier")?.Value
                  ?? User.Identity?.Name;
        if (!Guid.TryParse(sub, out var id)) return Unauthorized();
        var me = await auth.GetCurrentUserAsync(id, ct);
        return me is null ? NotFound() : Ok(me);
    }*/
}
