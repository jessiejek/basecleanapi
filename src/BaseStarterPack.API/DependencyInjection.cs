using Microsoft.OpenApi.Models;
using System.Reflection;

namespace BaseStarterPack.API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(setup =>
        {
            setup.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "BaseStarterPack API",
                Version = "v1",
                Description = "JWT + Refresh Token + CRUD starter"
            });

            var securitySchema = new OpenApiSecurityScheme
            {
                Description = "JWT Bearer. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            setup.AddSecurityDefinition("Bearer", securitySchema);
            setup.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securitySchema, Array.Empty<string>() }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath)) setup.IncludeXmlComments(xmlPath);
        });

        services.AddCors(policy =>
        {
            policy.AddPolicy("CorsPolicy", opt =>
            {
                opt.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
        });

        services.AddControllers();
        services.AddProblemDetails();
        return services;
    }
}
