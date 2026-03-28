using Microsoft.Extensions.DependencyInjection;
using YepPet.Application.Auth;
using YepPet.Application.Favorites;
using YepPet.Application.Places;
using YepPet.Application.Reviews;
using YepPet.Application.Users;

namespace YepPet.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthApplicationService, AuthApplicationService>();
        services.AddScoped<IPlaceApplicationService, PlaceApplicationService>();
        services.AddScoped<IFavoriteListApplicationService, FavoriteListApplicationService>();
        services.AddScoped<IUserApplicationService, UserApplicationService>();
        services.AddScoped<IPlaceReviewApplicationService, PlaceReviewApplicationService>();

        return services;
    }
}
