using BaseStarterPack.Application.Interfaces.Common;
using BaseStarterPack.Application.Interfaces.Repositories;
using BaseStarterPack.Application.Interfaces.Services;
using BaseStarterPack.Infrastructure.Context;
using BaseStarterPack.Infrastructure.Repositories;
using BaseStarterPack.Infrastructure.Repositories.Common;
using BaseStarterPack.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BaseStarterPack.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var defaultConn = config.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(defaultConn))
        {
            services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("AppDb"));
        }
        else
        {
            services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(defaultConn));
        }

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IRefreshTokensRepository, RefreshTokensRepository>();
        services.AddScoped<IClinicsRepository, ClinicsRepository>();

        services.AddScoped<IJwtTokenService, JwtTokenService>();

        return services;
    }
}
