using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Selfy.Business.Services;
using Selfy.Data.EntityFramework;
using Selfy.Data.Identity;
using System.Security.Claims;

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

            //services.AddScoped<IRepository<Product, Guid>, ProductRepo>();
            //services.AddScoped<IRepository<Category, int>, CategoryRepo>();

            //builder.Services.AddAutoMapper(options => options.AddMaps("AdminTemplate.MappingProfiles"));
            //services.AddAutoMapper(options =>
            //{
            //    options.AddProfile<EntityMappingProfile>();
            //});

            return services;
        }

        public static string GetUserId(this HttpContext context)
        {
            return context.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        }
        public static string ToFullErrorString(this ModelStateDictionary modelState)
        {
            var messages = new List<string>();

            foreach (var entry in modelState.Values)
            {
                foreach (var error in entry.Errors)
                    messages.Add(error.ErrorMessage);
            }

            return String.Join(" ", messages);
        }
    }
}