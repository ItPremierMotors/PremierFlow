using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PremierFlow.Application.Interfaces.Auth;
using PremierFlow.Application.Interfaces.Catalogo;
using PremierFlow.Application.Interfaces.Cliente;
using PremierFlow.Application.Interfaces.Taller;
using PremierFlow.Application.Interfaces.Vehiculo;
using PremierFlow.Infrastructure.Identity;
using PremierFlow.Infrastructure.Persistence;
using PremierFlow.Infrastructure.Persistence.Repositories.Services.AuthServices;
using PremierFlow.Infrastructure.Persistence.Repositories.Services.Catalogo.IEstadoOsServices;
using PremierFlow.Infrastructure.Persistence.Repositories.Services.Catalogo.MarcaServices;
using PremierFlow.Infrastructure.Persistence.Repositories.Services.Catalogo.ModeloServices;
using PremierFlow.Infrastructure.Persistence.Repositories.Services.Catalogo.TecnicoServices;
using PremierFlow.Infrastructure.Persistence.Repositories.Services.Catalogo.TipoServicioServices;
using PremierFlow.Infrastructure.Persistence.Repositories.Services.Catalogo.SucursalServices;
using PremierFlow.Infrastructure.Persistence.Repositories.Services.Catalogo.UbicacionServices;
using PremierFlow.Infrastructure.Persistence.Repositories.Services.Catalogo.VersionVehiculoServices;
using PremierFlow.Infrastructure.Persistence.Repositories.Services.Cliente;
using PremierFlow.Infrastructure.Persistence.Repositories.Services.Taller;
using PremierFlow.Infrastructure.Persistence.Repositories.Services.VehiculoService;
using PremierFlow.Application.Interfaces.Dashboard;
using PremierFlow.Infrastructure.Persistence.Repositories.Services.Dashboard;
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

            // ============================================
            // 5. SERVICIOS DE NEGOCIO - PremierFlow DMS
            // ============================================
            // Fase 1: Catálogos
            services.AddScoped<IMarcaService, MarcaService>();
            services.AddScoped<IModeloService,ModeloService>();
            services.AddScoped<IVersionVehiculoService, VersionVehiculoService>();
            services.AddScoped<ITipoServicioService, TipoServicioService>();
            services.AddScoped<ITecnicoService, TecnicoService>();
            services.AddScoped<IEstadoOsService, EstadoOsService>();
            services.AddScoped<ISucursalService, SucursalService>();
            services.AddScoped<IUbicacionService, UbicacionService>();

            // Fase 2: Cliente y Vehículo
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IVehiculoService, VehiculoService>();
            // Fase 3: Flujo de Taller
            services.AddScoped<ICapacidadTallerService, CapacidadTallerService>();
            services.AddScoped<IBloqueHorarioService, BloqueHorarioService>();
            services.AddScoped<ICitaService, CitaService>();
            services.AddScoped<IOrdenServicioService, OrdenServicioService>(); //OrdenServicioService OrdenServicioService
            services.AddScoped<IRecepcionService, RecepcionService>();
            services.AddScoped<IOsServicioService, OsServicioService>();
            services.AddScoped<IAsignacionTecnicoService, AsignacionTecnicoService>();
            services.AddScoped<IEvidenciaService, EvidenciaService>();

            // Fase 4: Dashboard
            services.AddScoped<IDashboardService, DashboardService>();

            //servicio de seguridad
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IAuth, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IRoleClaimService, RoleClaimService>();
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
