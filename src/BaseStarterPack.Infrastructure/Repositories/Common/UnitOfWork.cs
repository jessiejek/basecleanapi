using BaseStarterPack.Application.Interfaces.Common;
using BaseStarterPack.Application.Interfaces.Repositories;
using BaseStarterPack.Infrastructure.Context;

namespace BaseStarterPack.Infrastructure.Repositories.Common;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private bool _disposed;

    public UnitOfWork(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        Users = new UsersRepository(_context);
        RefreshTokens = new RefreshTokensRepository(_context);
        Clinics = new ClinicsRepository(_context);
    }

    public IUsersRepository Users { get; }
    public IRefreshTokensRepository RefreshTokens { get; }
    public IClinicsRepository Clinics { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing) _context.Dispose();
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
