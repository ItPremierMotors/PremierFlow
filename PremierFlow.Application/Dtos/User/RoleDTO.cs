using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Dtos.User
{
    public class RoleDTO
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int UsersCount { get; set; } // cantidad de usuarios con este rol
    }

    public class CreateRoleRequest
    {
        public string Name { get; set; } = null!;
    }

    public class UpdateRoleRequest
    {
        public string Name { get; set; } = null!;
    }

    // Para asignar/remover roles a usuarios
    public class AssignRoleRequest
    {
        public string UserId { get; set; } = null!;
        public List<string> Roles { get; set; } = new();
    }
}
