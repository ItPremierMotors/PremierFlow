using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Dtos.User
{
    public class UsersDTO
    {
        public string Id { get; set; }=null!; // aseguramos que no sea nulo
        public string NombreCompleto { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool Activo { get; set; }
        public string Departamento { get; set; } = null!;
        public string Cargo { get; set; } = null!;
        public List<string> Roles { get; set; } = new();
    }

    public class CreateUserRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? NombreCompleto { get; set; }
        public string? UserName { get; set; }
        public string? Cargo { get; set; }
        public string Departamento { get; set; } = null!;
        public bool Activo { get; set; } = true;

        public List<string>? Roles { get; set; }

    }

    public class UpdateUserRequest
    {
        public string? NombreCompleto { get; set; }
        public string? Cargo { get; set; }
        public bool? Activo { get; set; } // si lo mandas false => deshabilita
        public List<string>? Roles { get; set; }

    }
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
        public string? ConfirmPassword { get; set; } // opcional, pero útil
    }

    public class ResetPasswordRequest
    {
        public string NewPassword { get; set; } = null!;
    }

}
