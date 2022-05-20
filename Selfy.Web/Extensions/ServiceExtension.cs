using Microsoft.AspNetCore.Identity;
using Selfy.Business.MappingProfiles;
using Selfy.Business.Repositories;
using Selfy.Business.Repositories.Abstracts;
using Selfy.Business.Services.Email;
using Selfy.Core.Entities;
using Selfy.Data.EntityFramework;
using Selfy.Data.Identity;

namespace Selfy.Web.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            #region Identity

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = false;

                // User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<MyContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);

                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            #endregion

            services.AddTransient<IEmailService, SmtpEmailService>();

            services.AddScoped<IRepository<Product, Guid>, ProductRepo>();
            services.AddScoped<IRepository<Category, int>, CategoryRepo>();

            //builder.Services.AddAutoMapper(options => options.AddMaps("AdminTemplate.MappingProfiles"));
            services.AddAutoMapper(options =>
            {
                options.AddProfile<EntityMappingProfile>();
            });

            return services;
        }
    }
}