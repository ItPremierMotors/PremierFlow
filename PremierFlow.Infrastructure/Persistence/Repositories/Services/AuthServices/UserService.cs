// PremierFlow.Infrastructure/Persistence/Repositories/Services/AuthServices/UserService.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.User;
using PremierFlow.Application.Interfaces.Auth;
using PremierFlow.Domain.Common;
using PremierFlow.Infrastructure.Identity;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.AuthServices
{
    public class UserService : IUserService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        #region READS

        public async Task<ApiResponse<List<UsersDTO>>> GetAllAsync()
        {
            try
            {
                var users = await _userManager.Users
                    .AsNoTracking()
                    .Where(u => u.Activo)
                    .ToListAsync();

                var result = new List<UsersDTO>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    result.Add(new UsersDTO
                    {
                        Id = user.Id,
                        NombreCompleto = user.NombreCompleto,
                        Email = user.Email ?? "N/A",
                        Activo = user.Activo,
                        Departamento=user.Departamento,
                        Cargo = user.Cargo,
                        Roles = roles.ToList()
                    });
                }

                return ApiResponse<List<UsersDTO>>.ok(result, "Usuarios obtenidos correctamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<UsersDTO>>.fail(500,
                    new[] { ex.Message },
                    "Error al obtener los usuarios");
            }
        }

        public async Task<ApiResponse<UsersDTO?>> GetByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return ApiResponse<UsersDTO?>.fail(400,
                        new[] { "El ID es requerido" },
                        "Datos inválidos");
                }

                var user = await _userManager.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return ApiResponse<UsersDTO?>.fail(404,
                        new[] { "Usuario no encontrado" },
                        $"No se encontró el usuario con ID: {id}");
                }

                var roles = await _userManager.GetRolesAsync(user);

                var result = new UsersDTO
                {
                    Id = user.Id,
                    NombreCompleto = user.NombreCompleto,
                    Email = user.Email ?? "N/A",
                    Activo = user.Activo,
                    Cargo = user.Cargo,
                    Roles = roles.ToList()
                };

                return ApiResponse<UsersDTO?>.ok(result, "Usuario obtenido correctamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<UsersDTO?>.fail(500,
                    new[] { ex.Message },
                    "Error al obtener el usuario");
            }
        }

        public async Task<ApiResponse<List<UsersDTO>>> GetByDepartmentAsync(string departamento)
        {
            try
            {
                var users = await _userManager.Users
                    .AsNoTracking()
                    .Where(u => u.Activo && u.Departamento.ToLower() == departamento.ToLower())
                    .ToListAsync();

                var result = new List<UsersDTO>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    result.Add(new UsersDTO
                    {
                        Id = user.Id,
                        NombreCompleto = user.NombreCompleto,
                        Email = user.Email ?? "N/A",
                        Activo = user.Activo,
                        Departamento = user.Departamento,
                        Cargo = user.Cargo,
                        Roles = roles.ToList()
                    });
                }

                return ApiResponse<List<UsersDTO>>.ok(result, "Usuarios obtenidos correctamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<UsersDTO>>.fail(500,
                    new[] { ex.Message },
                    "Error al obtener usuarios por departamento");
            }
        }

        #endregion

        #region WRITES

        public async Task<ApiResponse<string>> CreateAsync(CreateUserRequest request)
        {
            try
            {
                // Validar request
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return ApiResponse<string>.fail(400,
                        new[] { "El email es requerido" },
                        "Datos inválidos");
                }

                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    return ApiResponse<string>.fail(400,
                        new[] { "La contraseña es requerida" },
                        "Datos inválidos");
                }

                // Verificar si ya existe
                var userExist = await _userManager.FindByEmailAsync(request.Email);
                if (userExist != null)
                {
                    return ApiResponse<string>.fail(409,
                        new[] { "El email ya está registrado" },
                        "Ya existe un usuario con este email");
                }

                var newUser = new ApplicationUser
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    NombreCompleto = request.NombreCompleto ?? "N/A",
                    Activo = request.Activo,
                    Cargo = request.Cargo ?? "N/A",
                    Departamento = request.Departamento ?? "N/A",
                    FechaCreacion = TimeHelper.Now
                };

                var createResult = await _userManager.CreateAsync(newUser, request.Password);

                if (!createResult.Succeeded)
                {
                    var errors = createResult.Errors.Select(e => e.Description);
                    return ApiResponse<string>.fail(400, errors, "Error al crear el usuario");
                }

                // Si el usuario no está activo, deshabilitarlo
                if (!request.Activo)
                {
                    await DisableUserAsync(newUser);
                }

                // Asignar roles si vienen
                if (request.Roles is { Count: > 0 })
                {
                    var rolesValidation = await ValidateRolesExistAsync(request.Roles);
                    if (!rolesValidation.Success)
                    {
                        return ApiResponse<string>.fail(rolesValidation.StatusCode,
                            rolesValidation.Errors,
                            rolesValidation.message);
                    }

                    var addToRolesResult = await _userManager.AddToRolesAsync(newUser, request.Roles);
                    if (!addToRolesResult.Succeeded)
                    {
                        var errors = addToRolesResult.Errors.Select(e => e.Description);
                        return ApiResponse<string>.fail(400, errors, "Error al asignar roles");
                    }
                }

                return ApiResponse<string>.ok(newUser.Id, "Usuario creado correctamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.fail(500,
                    new[] { ex.Message },
                    "Error interno al crear el usuario");
            }
        }

        public async Task<ApiResponse<bool>> UpdateAsync(string id, UpdateUserRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return ApiResponse<bool>.fail(400,
                        new[] { "El ID es requerido" },
                        "Datos inválidos");
                }

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return ApiResponse<bool>.fail(404,
                        new[] { "Usuario no encontrado" },
                        $"No se encontró el usuario con ID: {id}");
                }

                // Actualizar campos si vienen en el request
                if (!string.IsNullOrWhiteSpace(request.NombreCompleto))
                {
                    user.NombreCompleto = request.NombreCompleto;
                }
                if (!string.IsNullOrWhiteSpace(request.Cargo))
                {
                    user.Cargo = request.Cargo;
                }

                if (request.Activo.HasValue)
                {
                    user.Activo = request.Activo.Value;

                    if (!request.Activo.Value)
                    {
                        await DisableUserAsync(user);
                    }
                    else
                    {
                        await EnableUserAsync(user);
                    }
                }

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var errors = updateResult.Errors.Select(e => e.Description);
                    return ApiResponse<bool>.fail(400, errors, "Error al actualizar el usuario");
                }

                // Actualizar roles si vienen
                if (request.Roles != null)
                {
                    var newRoles = request.Roles
                        .Where(r => !string.IsNullOrWhiteSpace(r))
                        .Select(r => r.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    if (newRoles.Count > 0)
                    {
                        var rolesValidation = await ValidateRolesExistAsync(newRoles);
                        if (!rolesValidation.Success)
                        {
                            return ApiResponse<bool>.fail(rolesValidation.StatusCode,
                                rolesValidation.Errors,
                                rolesValidation.message);
                        }
                    }

                    var currentRoles = await _userManager.GetRolesAsync(user);
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

                    if (!removeResult.Succeeded)
                    {
                        var errors = removeResult.Errors.Select(e => e.Description);
                        return ApiResponse<bool>.fail(400, errors, "Error al remover roles anteriores");
                    }

                    if (newRoles.Count > 0)
                    {
                        var addResult = await _userManager.AddToRolesAsync(user, newRoles);
                        if (!addResult.Succeeded)
                        {
                            var errors = addResult.Errors.Select(e => e.Description);
                            return ApiResponse<bool>.fail(400, errors, "Error al asignar nuevos roles");
                        }
                    }
                }

                return ApiResponse<bool>.ok(true, "Usuario actualizado correctamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.fail(500,
                    new[] { ex.Message },
                    "Error interno al actualizar el usuario");
            }
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(string id, ChangePasswordRequest request)
        {
            try
            {
                // Validar confirmación de contraseña
                if (string.IsNullOrWhiteSpace(request.ConfirmPassword) ||
                    !string.Equals(request.NewPassword, request.ConfirmPassword, StringComparison.Ordinal))
                {
                    return ApiResponse<bool>.fail(400,
                        new[] { "Las contraseñas no coinciden" },
                        "La nueva contraseña no coincide con la confirmación");
                }

                if (string.IsNullOrWhiteSpace(request.CurrentPassword))
                {
                    return ApiResponse<bool>.fail(400,
                        new[] { "La contraseña actual es requerida" },
                        "Datos inválidos");
                }

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return ApiResponse<bool>.fail(404,
                        new[] { "Usuario no encontrado" },
                        $"No se encontró el usuario con ID: {id}");
                }

                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return ApiResponse<bool>.fail(400, errors, "Error al cambiar la contraseña");
                }

                return ApiResponse<bool>.ok(true, "Contraseña cambiada correctamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.fail(500,
                    new[] { ex.Message },
                    "Error interno al cambiar la contraseña");
            }
        }

        #endregion

        #region ADMIN

        public async Task<ApiResponse<bool>> ResetPasswordAsync(string id, ResetPasswordRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.NewPassword))
                {
                    return ApiResponse<bool>.fail(400,
                        new[] { "La nueva contraseña es requerida" },
                        "Datos inválidos");
                }

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return ApiResponse<bool>.fail(404,
                        new[] { "Usuario no encontrado" },
                        $"No se encontró el usuario con ID: {id}");
                }

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);

                if (!resetResult.Succeeded)
                {
                    var errors = resetResult.Errors.Select(e => e.Description);
                    return ApiResponse<bool>.fail(400, errors, "Error al resetear la contraseña");
                }

                return ApiResponse<bool>.ok(true, "Contraseña reseteada correctamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.fail(500,
                    new[] { ex.Message },
                    "Error interno al resetear la contraseña");
            }
        }

        #endregion

        #region PRIVATE METHODS

        private async Task<ApiResponse<bool>> ValidateRolesExistAsync(List<string> roles)
        {
            var invalidRoles = new List<string>();

            foreach (var role in roles.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(role))
                {
                    return ApiResponse<bool>.fail(400,
                        new[] { "Nombre de rol inválido" },
                        "Los nombres de rol no pueden estar vacíos");
                }

                var exists = await _roleManager.RoleExistsAsync(role);
                if (!exists)
                {
                    invalidRoles.Add(role);
                }
            }

            if (invalidRoles.Count > 0)
            {
                return ApiResponse<bool>.fail(400,
                    invalidRoles.Select(r => $"El rol '{r}' no existe"),
                    "Uno o más roles no existen");
            }

            return ApiResponse<bool>.ok(true);
        }

        private async Task DisableUserAsync(ApplicationUser user)
        {
            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
        }

        private async Task EnableUserAsync(ApplicationUser user)
        {
            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
        }

        #endregion
    }
}