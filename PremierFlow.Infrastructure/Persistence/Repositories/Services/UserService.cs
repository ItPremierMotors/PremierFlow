using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Dtos.User;
using PremierFlow.Application.Interfaces;
using PremierFlow.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services
{
    public class UserService : IUserService
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        public UserService(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        public async Task<bool> ChangePasswordAsync(String id, ChangePasswordRequest request)
        {
            //si no se confirmo la nueva contraseña o no coincide lanzamos error
           
            if (string.IsNullOrWhiteSpace(request.ConfirmPassword) ||
             !string.Equals(request.NewPassword, request.ConfirmPassword, StringComparison.Ordinal))
            {
                throw new Exception("La nueva contraseña no coincide con la confirmación");
            }
            //buscamos el usuario 
            var user=await userManager.FindByIdAsync(id) ?? throw new InvalidOperationException("Usuario no encontrado");
            var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            EnsureSuccess(result);
            return true;
        }

        public async Task<String> CreateAsync(CreateUserRequest request)
        {
            //VALIDAR QUE NO EXISTA OTRO USUARIO CON EL MISMO EMAIL O Nombre DE USUARIO
            var userExist = await userManager.FindByEmailAsync(request.Email);

            if (userExist != null)
            {
                throw new Exception("El usuario con el mismo email ya existe");
            }

            var newUser = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                NombreCompleto = request.NombreCompleto ?? "N/A",
                Activo = request.Activo,
                Cargo = request.Cargo ?? "N/A",
                Departamento = request.Departamento ?? "N/A",
                FechaCreacion = DateTime.UtcNow
            };

            var createResult = await userManager.CreateAsync(newUser, request.Password);
            EnsureSuccess(createResult);

            if (!request.Activo)
            {
                await disableUserAsync(newUser); //si el usuario no esta activo lo deshabilitamos
            }
            //SI VINENE ROLES ASIGNAMOSLOS
            if(request.Roles is {Count:> 0 })
            {
                await EnsureRolesExistAsync(request.Roles); //verificamos que los roles existan
                var addToRolesResult = await userManager.AddToRolesAsync(newUser, request.Roles);
                EnsureSuccess(addToRolesResult);
            }

            return newUser.Id;

        }


        public async Task<List<UsersDTO>> GetAllAsync()
        {
            
            var user =await userManager.Users
                .AsNoTracking()
                .Where(u => u.Activo)
                .ToListAsync();
            
            var result = new List<UsersDTO>();
            //llenamos lista pero por cada lista asignamos los roles
            foreach (var u in user)
            {
                var rols = await userManager.GetRolesAsync(u);
                result.Add(new UsersDTO
                {
                    Id = u.Id,
                    NombreCompleto = u.NombreCompleto,
                    Email = u.Email ?? "N/A",
                    Activo = u.Activo,
                    Cargo = u.Cargo,
                    Roles = rols.ToList()
                });
            }
            //retornamos la lista
            return result;
        }

        public async Task<UsersDTO?>? GetByIdAsync(String id)
        {
            var user =await userManager.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u=>u.Id==id);
            
            if (user == null) return null;
            var roles = await userManager.GetRolesAsync(user);

            var result = new UsersDTO
            {
                Id = user.Id,
                NombreCompleto = user.NombreCompleto,
                Email = user.Email ?? "N/A",
                Activo = user.Activo,
                Cargo = user.Cargo,
                Roles = roles.ToList()
            };

            return  result;
        }

        public async Task<bool> ResetPasswordAsync(String id, ResetPasswordRequest request)
        {
            //1. debemos buscar el usuario
            var user = await userManager.FindByIdAsync(id) ?? throw new InvalidOperationException("Usuario no encontrado");

            //2. generamos el token de reseteo 
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);
            EnsureSuccess(resetResult);
            return true;
        }

        public async Task<bool> UpdateAsync(String id, UpdateUserRequest request)
        {
           var user =await userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new Exception("Usuario no encontrado");
            }

            //actualizamos los campos si vienen en el request
            if (!string.IsNullOrWhiteSpace(request.NombreCompleto))
            {
                user.NombreCompleto = request.NombreCompleto;
            }
            if (request.Activo.HasValue) //hasValue indica si el valor es nulo o no, si es nulo es false
            {
                user.Activo = request.Activo.Value;
                //si el usuario viene como inactivo lo deshabilitamos
                if (!request.Activo.Value)
                {
                    await disableUserAsync(user);
                }
                else
                {
                   await EnableUserAsync(user);
                }
            }
            var updateResult = await userManager.UpdateAsync(user);
            EnsureSuccess(updateResult);

            // Roles:si vienen roles los actualizamos
            // null => no tocar roles
            // [] => quitar todos
            // [..] => reemplazar
            
            if(request.Roles != null)
            {
                var newRoles = request.Roles
                               .Where(r => !string.IsNullOrWhiteSpace(r))
                               .Select(r => r.Trim())
                               .Distinct(StringComparer.OrdinalIgnoreCase)
                               .ToList();

                await EnsureRolesExistAsync(newRoles); //verificamos que los roles existan
                var currentRoles = await userManager.GetRolesAsync(user); //obtenemos los roles actuales del usuario

                var removeResult= await userManager.RemoveFromRolesAsync(user, currentRoles);//eliminamos todos los roles actuales
                EnsureSuccess(removeResult);
                if(newRoles.Count > 0)
                {
                    var addResult = await userManager.AddToRolesAsync(user, newRoles); //asignamos los nuevos roles
                    EnsureSuccess(addResult);
                }
            }
            return true;

        }

       
        private static void EnsureSuccess(IdentityResult result)
        {
            if (result.Succeeded) return;

            var errors = string.Join(" | ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
            throw new InvalidOperationException(errors);
        }

        private async Task EnsureRolesExistAsync(List<string> roles)
        {
            foreach (var role in roles.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(role))
                    throw new InvalidOperationException("Nombre de rol inválido.");

                var exists = await roleManager.RoleExistsAsync(role);
                if (!exists)
                    throw new InvalidOperationException($"El rol '{role}' no existe.");
            }
        }
        private static bool IsActive(ApplicationUser user)
        {
            // Si usas lockout como “activo/inactivo”:
            // inactivo => LockoutEnabled true y LockoutEnd en futuro
            if (!user.LockoutEnabled) return true;
            if (user.LockoutEnd == null) return true;
            return user.LockoutEnd <= DateTimeOffset.UtcNow;
        }
        private async Task disableUserAsync(ApplicationUser user)
        {
            var enableLockoutResult = await userManager.SetLockoutEnabledAsync(user, true); //con esto bloqueamos el usuario
            EnsureSuccess(enableLockoutResult); //verificamos que se haya bloqueado correctamente

            //lo bloqueamos por mucho tiempo
            var lockoutResult = await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
            EnsureSuccess(lockoutResult);

        }

        private async Task EnableUserAsync(ApplicationUser user)
        {
            var enableLockout = await userManager.SetLockoutEnabledAsync(user, true);
            EnsureSuccess(enableLockout);

            var unlockResult = await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
            EnsureSuccess(unlockResult);
        }
    }
}
