using BaseStarterPack.Application.DTOs;
using BaseStarterPack.Application.Interfaces.Common;
using BaseStarterPack.Application.Interfaces.Services;
using BaseStarterPack.Domain.Entities;

namespace BaseStarterPack.Application.Services;

public class AuthService(IUnitOfWork uow, IJwtTokenService jwt) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest req, CancellationToken ct = default)
    {
        var exists = await uow.Users.GetAsync(u => u.Email == req.Email, cancellationToken: ct);
        if (exists is not null)
            throw new InvalidOperationException("Email already registered.");

        var user = new User
        {
            Email = req.Email,
            PasswordHash = PasswordHasher.Hash(req.Password),
            Role = string.IsNullOrWhiteSpace(req.Role) ? "User" : req.Role!,
            FirstName = req.FirstName,
            MiddleName = req.MiddleName,
            LastName = req.LastName,
            DateOfBirth = req.DateOfBirth,
            Gender = req.Gender,
            PhoneNumber = req.PhoneNumber,
            Address = req.Address
        };

        var (access, exp) = jwt.CreateAccessToken(user);
        var refreshEntity = jwt.CreateRefreshToken(user); // sets UserId, CreatedAtUtc, ExpiresAtUtc

        await uow.Users.AddAsync(user, ct);
        await uow.RefreshTokens.AddAsync(refreshEntity, ct);
        await uow.SaveChangesAsync(ct);

        return new AuthResponse(access, refreshEntity.Token, exp);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest req, CancellationToken ct = default)
    {
        var user = await uow.Users.GetAsync(u => u.Email == req.Email, cancellationToken: ct);
        if (user is null || !PasswordHasher.Verify(req.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var (access, exp) = jwt.CreateAccessToken(user);
        var refresh = jwt.CreateRefreshToken(user);
        await uow.RefreshTokens.AddAsync(refresh, ct);
        await uow.SaveChangesAsync(ct);

        return new AuthResponse(access, refresh.Token, exp);
    }

    public async Task<AuthResponse> RefreshAsync(RefreshTokenRequest req, CancellationToken ct = default)
    {
        var token = await uow.RefreshTokens.GetAsync(rt => rt.Token == req.RefreshToken, includeProperties: "User", cancellationToken: ct);
        if (token is null || !token.IsActive)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        // rotate: mark old as revoked, issue new
        token.Revoked = true;
        uow.RefreshTokens.Update(token);

        var user = token.User;
        var (access, exp) = jwt.CreateAccessToken(user);
        var newRefresh = jwt.CreateRefreshToken(user);
        await uow.RefreshTokens.AddAsync(newRefresh, ct);
        await uow.SaveChangesAsync(ct);

        return new AuthResponse(access, newRefresh.Token, exp);
    }

    public async Task<CurrentUserDto?> GetCurrentUserAsync(Guid id, CancellationToken ct = default)
    {
        var user = await uow.Users.GetAsync(u => u.Id == id, cancellationToken: ct);
        return user is null ? null :
            new CurrentUserDto(user.Id, user.Email, user.Role, user.FirstName, user.MiddleName, user.LastName);
    }
}
