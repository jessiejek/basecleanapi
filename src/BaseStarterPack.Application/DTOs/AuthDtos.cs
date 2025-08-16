namespace BaseStarterPack.Application.DTOs;

public record RegisterRequest(string Email, string Password, string? Role,
    string FirstName, string? MiddleName, string LastName,
    DateTime? DateOfBirth, string? Gender, string? PhoneNumber, string? Address);

public record LoginRequest(string Email, string Password);

public record RefreshTokenRequest(string RefreshToken);

public record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc);

public record CurrentUserDto(Guid Id, string Email, string Role, string FirstName, string? MiddleName, string LastName);
