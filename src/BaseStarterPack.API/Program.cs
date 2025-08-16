using BaseStarterPack.API;
using BaseStarterPack.Application;
using BaseStarterPack.Infrastructure;
using BaseStarterPack.Infrastructure.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// layers
builder.Services.AddPresentation();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// JWT (reads appsettings.json: Jwt:Secret/Issuer/Audience)
var secret = builder.Configuration["Jwt:Secret"] ?? "dev-insecure-key-change-me";
var issuer = builder.Configuration["Jwt:Issuer"] ?? "BaseStarterPack";
var audience = builder.Configuration["Jwt:Audience"] ?? "BaseStarterPack";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// optional: simple seed (sync so API project has no EF Core dependency)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.Users.Any())
    {
        var admin = new BaseStarterPack.Domain.Entities.User
        {
            Email = "admin@base.local",
            PasswordHash = BaseStarterPack.Application.Services.PasswordHasher.Hash("Admin123!"),
            Role = "Admin",
            FirstName = "Base",
            LastName = "Admin"
        };
        db.Users.Add(admin);
    }


    db.SaveChanges();
}

app.UseExceptionHandler();

// dev-friendly errors + Swagger at root
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BaseStarterPack API v1");
        c.RoutePrefix = string.Empty; // Swagger UI at "/"
    });
}

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
