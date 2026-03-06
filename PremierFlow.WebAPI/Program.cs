using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using PremierFlow.Infrastructure;
using PremierFlow.Infrastructure.Security;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);

//configurar autenticacion jwt
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSection.GetValue<string>("Key")!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(Options=>
    {
       Options.RequireHttpsMetadata= false; //solo para desarrollo
       Options.SaveToken= true; //guardar el token en la solicitud
         Options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
         {
             ValidateIssuer = true, //validar emisor
             ValidateAudience = true, //validar audiencia
             ValidateLifetime = true, //validar tiempo de expiracion
             ValidateIssuerSigningKey = true, //validar la firma del token
             ValidIssuer = jwtSection.GetValue<string>("Issuer"), //emisor valido
             ValidAudience = jwtSection.GetValue<string>("Audience"), //audiencia valida
             IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key), //clave de firma
             ClockSkew = TimeSpan.Zero //eliminar el tiempo de tolerancia para la expiracion del token
         };
});

builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>(); //registrar el proveedor de politicas de autorizacion personalizado

// CORS - Permitir que SmartAdmin acceda a la API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSmartAdmin",
        policy => policy
        .WithOrigins(
            "https://localhost:7003",  // Puerto HTTPS de SmartAdmin
            "http://localhost:5003"    // Puerto HTTP de SmartAdmin,
            
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
    );
});

var app = builder.Build();



////ejecutar seed de infrastructura (roles, usuario admin, etc)
using(var scope = app.Services.CreateScope())
{
   var services = scope.ServiceProvider;
   await DependencyInjection.InitializeDatabaseAsync(services);
}

gir 


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();//documentacion automatica de la api

}

app.UseHttpsRedirection();

// IMPORTANTE: CORS debe ir antes de Authorization
app.UseCors("AllowSmartAdmin");

// Servir archivos estáticos (evidencias, uploads, etc.)
app.UseStaticFiles();

app.UseAuthentication(); //habilitar autenticacion ya que ahora si usa jwt
app.UseAuthorization();
 
app.MapControllers();
app.MapGet("/", () => "PremierFlow API funcionando");

app.Run();
