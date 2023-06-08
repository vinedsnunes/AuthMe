using AuthMe.Application.Interfaces.Services;
using AuthMe.Identity.Configurations;
using AuthMe.Identity.Data;
using AuthMe.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthMe.Api.IoC
{
    public static class NativeInjectorConfig
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IdentityDataContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("AuthMeConnection"))
            );

            services.AddDefaultIdentity<IdentityUser>()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<IdentityDataContext>()
                    .AddDefaultTokenProviders();

            services.AddSingleton<RedisConnector>();
            services.AddScoped<IIdentityService, IdentityService>();
        }
    }
}
