using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.User;
using PremierFlow.Application.Interfaces.Auth;
using PremierFlow.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.AuthServices
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public RoleService(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        #region READS
        public async Task<ApiResponse<List<RoleDTO>>> GetAllAsync()
        {
            var roles=await _roleManager.Roles
                .AsNoTracking()
                .ToListAsync(); 
            //obtiene todos los roles crear lista para devolverlos
            var result=new List<RoleDTO>();
            foreach (var role in roles)
            {
              var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
                result.Add(new RoleDTO
                {
                    Id=role.Id,
                    Name=role.Name!,
                    UsersCount=usersInRole.Count
                });
            }
            return ApiResponse<List<RoleDTO>>.ok(result,"Roles Obtenidos correctamente");
        }
        public async Task<ApiResponse<RoleDTO>> GetByIdAsync(string id)
        {
            var role=await _roleManager.FindByIdAsync(id);
            if(role== null)
            {
                return ApiResponse<RoleDTO>.fail(404, null, "Rol no encontrado.");
            }
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);

            var dto= new RoleDTO
            {
                Id = role.Id,
                Name = role.Name!,
                UsersCount = usersInRole.Count
            };
            return ApiResponse<RoleDTO>.ok(dto, "Role Obtenido Correctamente");
        }
        public async Task<ApiResponse<RoleDTO>> GetByNameAsync(string name)
        {
            var role = await _roleManager.FindByNameAsync(name);
            if (role == null)
            {
                return ApiResponse<RoleDTO>.fail(404, null, "Rol no encontrado.");
            }
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);

            var dto = new RoleDTO
            {
                Id = role.Id,
                Name = role.Name!,
                UsersCount = usersInRole.Count
            };
            return ApiResponse<RoleDTO>.ok(dto, "Role Obtenido Correctamente");
        }
        public async Task<ApiResponse<List<string>>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return ApiResponse<List<string>>.fail(404, null, "Usuario no encontrado.");

            var roles = await _userManager.GetRolesAsync(user);

            return ApiResponse<List<string>>.ok(roles.ToList(), "Roles obtenidos correctamente");
        }
        public async Task<ApiResponse<bool>> RoleExistsAsync(string roleName)
        {
            var banderin =await _roleManager.RoleExistsAsync(roleName);
            return ApiResponse<bool>.ok(banderin, "Existencia del rol");
        }
        #endregion

        #region WRITES
        public async Task<ApiResponse<string>> CreateAsync(CreateRoleRequest request)
        {
            // Validar nombre
            if (string.IsNullOrWhiteSpace(request.Name))
                return ApiResponse<string>.fail(400, null, "El nombre del rol es requerido.");

            // Normalizar nombre
            var normalizedName = NormalizeRoleName(request.Name);

            // Verificar si ya existe
            if (await _roleManager.RoleExistsAsync(normalizedName))
                return ApiResponse<string>.fail(409, null, $"El rol '{normalizedName}' ya existe.");

            var newRole = new IdentityRole(normalizedName);
            var result = await _roleManager.CreateAsync(newRole);

            if (!result.Succeeded)
                return ApiResponse<string>.fail(400, null, GetIdentityErrors(result));

            return ApiResponse<string>.ok(newRole.Id, $"Rol '{normalizedName}' creado correctamente");
        }
        public async Task<ApiResponse<bool>> UpdateAsync(string id, UpdateRoleRequest request)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return ApiResponse<bool>.fail(404, null, "Rol no encontrado.");

            // Validar nombre
            if (string.IsNullOrWhiteSpace(request.Name))
                return ApiResponse<bool>.fail(400, null, "El nombre del rol es requerido.");

            var normalizedName = NormalizeRoleName(request.Name);

            // Verificar si el nuevo nombre ya existe (y no es el mismo rol)
            var existingRole = await _roleManager.FindByNameAsync(normalizedName);
            if (existingRole != null && existingRole.Id != id)
                return ApiResponse<bool>.fail(409, null, $"Ya existe otro rol con el nombre '{normalizedName}'.");

            role.Name = normalizedName;
            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
                return ApiResponse<bool>.fail(400, null, GetIdentityErrors(result));

            return ApiResponse<bool>.ok(true, $"Rol actualizado a '{normalizedName}' correctamente");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return ApiResponse<bool>.fail(404, null, "Rol no encontrado.");

            // Proteger roles del sistema
            var protectedRoles = new[] { "Admin", "AdminTI", "User", "Manager" };
            if (protectedRoles.Contains(role.Name, StringComparer.OrdinalIgnoreCase))
                return ApiResponse<bool>.fail(403, null, $"El rol '{role.Name}' es un rol del sistema y no puede ser eliminado.");

            // Verificar si hay usuarios con este rol
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
            if (usersInRole.Any())
                return ApiResponse<bool>.fail(409, null,
                    $"No se puede eliminar el rol '{role.Name}' porque tiene {usersInRole.Count} usuario(s) asignado(s).");

            var result = await _roleManager.DeleteAsync(role);

            if (!result.Succeeded)
                return ApiResponse<bool>.fail(400, null, GetIdentityErrors(result));

            return ApiResponse<bool>.ok(true, $"Rol '{role.Name}' eliminado correctamente");
        }

        #endregion

        #region ASIGNACIÓN DE ROLES A USUARIOS
        public async Task<ApiResponse<bool>> AssignRolesToUserAsync(string userId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponse<bool>.fail(404, null, "Usuario no encontrado.");

            if (roles == null || !roles.Any())
                return ApiResponse<bool>.fail(400, null, "Debe proporcionar al menos un rol.");

            // Validar que todos los roles existen
            var validationResult = await ValidateRolesExistAsync(roles);
            if (!validationResult.Success)
                return validationResult;

            // Filtrar roles que el usuario ya tiene
            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = roles
                .Where(r => !currentRoles.Contains(r, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (!rolesToAdd.Any())
                return ApiResponse<bool>.ok(true, "El usuario ya tiene todos los roles especificados.");

            var result = await _userManager.AddToRolesAsync(user, rolesToAdd);

            if (!result.Succeeded)
                return ApiResponse<bool>.fail(400, null, GetIdentityErrors(result)); ;

            return ApiResponse<bool>.ok(true, $"Se asignaron {rolesToAdd.Count} rol(es) al usuario correctamente");
        }

        public async Task<ApiResponse<bool>> RemoveRolesFromUserAsync(string userId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponse<bool>.fail(404, null, "Usuario no encontrado.");;

            if (roles == null || !roles.Any())
                return ApiResponse<bool>.fail(400, null, "Debe proporcionar al menos un rol.");

            // Solo remover roles que el usuario tiene
            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToRemove = roles
                .Where(r => currentRoles.Contains(r, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (!rolesToRemove.Any())
                return ApiResponse<bool>.ok(true, "El usuario no tiene ninguno de los roles especificados.");

            var result = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            if (!result.Succeeded)
                return ApiResponse<bool>.fail(400, null, GetIdentityErrors(result));

            return ApiResponse<bool>.ok(true, $"Se removieron {rolesToRemove.Count} rol(es) del usuario correctamente");
        }

        public async Task<ApiResponse<bool>> SetUserRolesAsync(string userId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponse<bool>.fail(404, null, "Usuario no encontrado.");

            // Normalizar y limpiar roles
            var newRoles = (roles ?? new List<string>())
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => r.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Validar que todos los roles existen (si hay alguno)
            if (newRoles.Any())
            {
                var validationResult = await ValidateRolesExistAsync(newRoles);
                if (!validationResult.Success)
                    return validationResult;
            }

            // Obtener roles actuales
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Remover todos los roles actuales
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                    return ApiResponse<bool>.fail(400, null, GetIdentityErrors(removeResult));
            }

            // Asignar nuevos roles
            if (newRoles.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, newRoles);
                if (!addResult.Succeeded)
                    return ApiResponse<bool>.fail(400, null, GetIdentityErrors(addResult));
            }

            var message = newRoles.Any()
                ? $"Se establecieron {newRoles.Count} rol(es) para el usuario"
                : "Se removieron todos los roles del usuario";

            return ApiResponse<bool>.ok(true, message);
        }

        #endregion
        #region HELPERS

        private static string GetIdentityErrors(IdentityResult result)
        {
            return string.Join(" | ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
        }

        private async Task<ApiResponse<bool>> ValidateRolesExistAsync(List<string> roles)
        {
            foreach (var role in roles.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(role))
                    return ApiResponse<bool>.fail(400, null, "Nombre de rol inválido.");

                var exists = await _roleManager.RoleExistsAsync(role);
                if (!exists)
                    return ApiResponse<bool>.fail(404, null, $"El rol '{role}' no existe.");
            }
            return ApiResponse<bool>.ok(true, "Roles válidos");
        }

        private static string NormalizeRoleName(string name)
        {
            name = name.Trim();
            if (string.IsNullOrEmpty(name)) return name;
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower());
        }

        #endregion
    }
}
/*
public async Task<ApiResponse<List<RoleDTO>>> GetAllAsync()
1.Consulta todos los roles en la base de datos
2. Por cada rol, cuenta cuántos usuarios lo tienen
3. Crea una lista de DTOs con la información
4. Retorna la lista

public async Task<ApiResponse<RoleDTO>> GetByIdAsync(string id)
1.Busca el rol por ID
2. Si no existe → retorna error 404
3. Si existe → cuenta usuarios y retorna el DTO

public async Task<ApiResponse<RoleDTO>> GetByNameAsync(string name)

1.Busca el rol por nombre
2. Si no existe → retorna error 404
3. Si existe → cuenta usuarios y retorna el DTO

public async Task<ApiResponse<List<string>>> GetUserRolesAsync(string userId)
**¿Qué hace?**
Obtiene todos los roles asignados a un usuario específico.


1. Busca el usuario por ID
2. Si no existe → retorna error 404
3. Si existe → obtiene sus roles y retorna la lista

public async Task<ApiResponse<bool>> RoleExistsAsync(string roleName)
¿Qué hace?
Verifica si un rol existe en el sistema

public class CreateRoleRequest
{
    public string Name { get; set; } = null!;
}
```

**Flujo:**
```
1. Valida que el nombre no esté vacío
2. Normaliza el nombre (ej: "ADMIN" → "Admin")
3. Verifica que no exista otro rol con ese nombre
4. Crea el rol
5. Retorna el ID del nuevo rol

public class UpdateRoleRequest
{
    public string Name { get; set; } = null!;
}
```

**Flujo:**
```
1. Busca el rol por ID
2. Si no existe → error 404
3. Valida que el nuevo nombre no esté vacío
4. Verifica que no exista OTRO rol con ese nombre
5. Actualiza el nombre
6. Retorna true


public async Task<ApiResponse<bool>> DeleteAsync(string id)
```

**¿Qué hace?**
Elimina un rol del sistema.

**Parámetro:**
- `id` - ID del rol a eliminar

**Flujo:**
```
1. Busca el rol por ID
2. Si no existe → error 404
3. Verifica si es rol protegido (Admin, User, Manager)
   → Si es protegido → error 403
4. Verifica si tiene usuarios asignados
   → Si tiene usuarios → error 409
5. Elimina el rol
6. Retorna true

public async Task<ApiResponse<bool>> AssignRolesToUserAsync(string userId, List<string> roles)
```

**¿Qué hace?**
Agrega roles adicionales a un usuario (sin quitar los que ya tiene).

**Parámetros:**
- `userId` - ID del usuario
- `roles` - Lista de roles a agregar

**Flujo:**
```
1. Busca el usuario
2. Valida que la lista no esté vacía
3. Verifica que todos los roles existan
4. Filtra roles que el usuario ya tiene (no duplicar)
5. Agrega solo los roles nuevos
6. Retorna true
```

public async Task<ApiResponse<bool>> RemoveRolesFromUserAsync(string userId, List<string> roles)
```

**¿Qué hace?**
Quita roles específicos de un usuario.

**Parámetros:**
- `userId` - ID del usuario
- `roles` - Lista de roles a quitar

**Flujo:**
```
1. Busca el usuario
2. Valida que la lista no esté vacía
3. Filtra solo los roles que el usuario realmente tiene
4. Remueve esos roles
5. Retorna true

public async Task<ApiResponse<bool>> SetUserRolesAsync(string userId, List<string> roles)
```

**¿Qué hace?**
Reemplaza TODOS los roles de un usuario. Es como hacer "reset" de roles.

**Parámetros:**
- `userId` - ID del usuario
- `roles` - Nueva lista de roles (reemplaza completamente)

**Flujo:**
```
1. Busca el usuario
2. Limpia y normaliza la lista de roles
3. Valida que todos los roles existan
4. ELIMINA todos los roles actuales del usuario
5. AGREGA los nuevos roles
6. Retorna true
*/