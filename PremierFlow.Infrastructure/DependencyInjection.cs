using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PremierFlow.Application.Interfaces.Auth;
using PremierFlow.Infrastructure.Identity;
using PremierFlow.Infrastructure.Persistence;
using PremierFlow.Infrastructure.Persistence.Repositories.Services.AuthServices;
using PremierFlow.Infrastructure.Security;

namespace PremierFlow.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration){
            //1. cadena de conexion
            var connectionString = configuration.GetConnectionString("PremierFlowConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión 'PremierFlowConnection' no está configurada.");
            }

            //2. dbcontext con sql server
            services.AddDbContext<PremierFlowDbContext>(option =>
                option.UseSqlServer(connectionString)
            );

            //3. identity (usuarios, roles) usando nuestro dbcontext
            //services.AddIdentityCore
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                //configuraciones de password, lockout, etc. si es necesario
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                // Lockout por intentos fallidos
                options.Lockout.AllowedForNewUsers = true; // usuarios nuevos también aplican
                options.Lockout.MaxFailedAccessAttempts = 5; // ej: 5 intentos
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // 15 min bloqueado
            }).AddRoles<IdentityRole>()
            .AddSignInManager<SignInManager<ApplicationUser>>()
            .AddEntityFrameworkStores<PremierFlowDbContext>()
            .AddDefaultTokenProviders();

            //4. otros servicios de infraestructura (ejemplo: servicios de seguridad, email, etc.)

            //servicio de seguridad
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IAuth, AuthService>();
            services.AddScoped<IUserService, UserService>();
            return services;
        }
       
           
        public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider) {
            using var scope= serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<PremierFlowDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await InfrastructureSeed.SeedAsync(context, userManager, roleManager);

        }
    }
}
