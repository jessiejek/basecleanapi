using BaseStarterPack.Application.Interfaces.Repositories;

namespace BaseStarterPack.Application.Interfaces.Common;

public interface IUnitOfWork : IDisposable
{
    IUsersRepository Users { get; }
    IRefreshTokensRepository RefreshTokens { get; }
    IClinicsRepository Clinics { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
