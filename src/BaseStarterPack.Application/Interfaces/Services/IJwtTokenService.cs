using BaseStarterPack.Domain.Entities;

namespace BaseStarterPack.Application.Interfaces.Services;

public interface IJwtTokenService
{
    (string token, DateTime expiresAtUtc) CreateAccessToken(User user);
    RefreshToken CreateRefreshToken(User user);
}
