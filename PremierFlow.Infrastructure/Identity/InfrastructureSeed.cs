using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PremierFlow.Application;
using PremierFlow.Domain.Entities;
using PremierFlow.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using System.Text;

namespace PremierFlow.Infrastructure.Identity
{
    public static class InfrastructureSeed
    {
        public static async Task SeedAsync(PremierFlowDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            //aplicar migraciones si no existe
            await context.Database.MigrateAsync();

            //crear roles
            var roles = new[] {
                "AdminTI",
                "Gerencia",
                "Operaciones",
                "Aduanas",
                "Contabilidad",
                "JefeVentas",
                "AsesorVentas"
            };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new IdentityRole(roleName);
                    await roleManager.CreateAsync(role);
                }
            }

            //crear usuario admin
            string adminEmail = "tecnologia@premiermotors.com.hn";
            string adminPassword = "P@ssw0rd";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "Jcastro",
                    Email = adminEmail,
                    NombreCompleto = "Jefferson Castro",
                    Departamento = "Tecnología",
                    Cargo = "Administrador",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "AdminTI");
                }

            }

            //crear Sucursales
            if (!await context.Sucursales.AnyAsync())
            {
                var sucursales = new List<Sucursal>
                {
                    new Sucursal { Nombre = "Sucursal Tegucigalpa", Direccion = "Col. Altamira, Tegucigalpa",Activa=true, Ciudad="Tegucigalpa", Codigo="TGU-CENTRO" },
                    new Sucursal { Nombre = "Sucursal San Pedro Sula", Direccion = "Col. Prado Alto, San Pedro Sula",Activa=true, Ciudad="San Pedro Sula", Codigo="SPS-Cemesa" }
                };
                context.Sucursales.AddRange(sucursales);
                await context.SaveChangesAsync();

            }
            //crear ubicaciones
            if (!await context.Ubicaciones.AnyAsync())
            {

               
                var ubicaciones = new List<Ubicacion>
                {
                    new Ubicacion{Nombre="ShowRoom SPS",Tipo=Domain.Enums.TipoUbicacion.Showroom, SucursalID=1},
                    new Ubicacion { Nombre = "Bodega SPS", Tipo = Domain.Enums.TipoUbicacion.Bodega, SucursalID = 1 },

                    new Ubicacion { Nombre = "Showroom TGU", Tipo = Domain.Enums.TipoUbicacion.Showroom, SucursalID = 2 },
                    new Ubicacion { Nombre = "Bodega TGU", Tipo = Domain.Enums.TipoUbicacion.Bodega,  SucursalID = 2},

                    new Ubicacion { Nombre = "Depósito Fiscal Puerto Cortés", Tipo = Domain.Enums.TipoUbicacion.DepositoFiscal },
                    new Ubicacion { Nombre = "Depósito Fiscal", Tipo = Domain.Enums.TipoUbicacion.DepositoFiscal }
                };
                context.Ubicaciones.AddRange(ubicaciones);
                await context.SaveChangesAsync();

            }

        }
    }
}
