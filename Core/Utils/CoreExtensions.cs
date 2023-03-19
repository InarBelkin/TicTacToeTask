using Core.Entities;
using Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core.Utils;

public static class CoreExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationContext>(builder =>
            builder.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));


        services.AddIdentity<User, IdentityRole<int>>()
            .AddEntityFrameworkStores<ApplicationContext>();

        services.AddHttpContextAccessor();

        services.AddScoped<GameService>();
        services.AddScoped<GameHelperService>();

        services.AddScoped<IUsersService, UsersService>();
        return services;
    }
}