using BaseStarterPack.Application.DTOs;

namespace BaseStarterPack.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest req, CancellationToken ct = default);
    Task<AuthResponse> LoginAsync(LoginRequest req, CancellationToken ct = default);
    Task<AuthResponse> RefreshAsync(RefreshTokenRequest req, CancellationToken ct = default);
    Task<CurrentUserDto?> GetCurrentUserAsync(Guid id, CancellationToken ct = default);
}
