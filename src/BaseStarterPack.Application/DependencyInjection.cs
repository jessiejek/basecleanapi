using BaseStarterPack.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BaseStarterPack.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<Services.AuthService>();
        services.AddScoped<Services.ClinicService>();
        return services;
    }
}
