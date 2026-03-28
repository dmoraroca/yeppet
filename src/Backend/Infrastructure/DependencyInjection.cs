using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using YepPet.Application.Auth;
using YepPet.Domain.Abstractions;
using YepPet.Infrastructure.Auth;
using YepPet.Infrastructure.Persistence;
using YepPet.Infrastructure.Persistence.Repositories;

namespace YepPet.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("YepPet")
            ?? "Host=localhost;Port=5433;Database=yeppet;Username=app;Password=app";
        var expiresInMinutes = int.TryParse(configuration["Auth:Jwt:ExpiresInMinutes"], out var parsedExpiresInMinutes)
            ? parsedExpiresInMinutes
            : 480;
        var authOptions = new AuthOptions
        {
            Jwt = new AuthOptions.JwtOptions
            {
                Issuer = configuration["Auth:Jwt:Issuer"] ?? "YepPet",
                Audience = configuration["Auth:Jwt:Audience"] ?? "YepPet.Web",
                SigningKey = configuration["Auth:Jwt:SigningKey"]
                    ?? "dev-only-yep-pet-signing-key-change-in-production-123456789",
                ExpiresInMinutes = expiresInMinutes
            }
        };

        services.AddSingleton(Options.Create(authOptions));
        services.AddDbContext<YepPetDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddScoped<IAccessTokenIssuer, JwtAccessTokenIssuer>();
        services.AddScoped<DevelopmentIdentitySeeder>();
        services.AddScoped<IPlaceRepository, PlaceRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFavoriteListRepository, FavoriteListRepository>();
        services.AddScoped<IPlaceReviewRepository, PlaceReviewRepository>();

        return services;
    }
}
