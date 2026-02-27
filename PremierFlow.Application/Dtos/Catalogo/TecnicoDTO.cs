using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Dtos.Catalogo
{
    public class TecnicoDTO
    {
        public int TecnicoId { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string? Especialidad { get; set; }
        public int? BahiaAsignada { get; set; }
        public string? UsuarioId { get; set; }
        public int? SucursalId { get; set; }

        // Extras para UI
        public string NombreCompleto { get; set; } = null!;   // "Juan Pérez"
        public string? SucursalNombre { get; set; }           // "SPS Norte"
    }
    public class CreateTecnicoDTO
    {
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string? Especialidad { get; set; }
        public int? BahiaAsignada { get; set; }
        public string? UsuarioId { get; set; }
        public int? SucursalId { get; set; }

        // Cuenta de acceso (opcional)
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
    public class UpdateTecnicoDTO
    {
        public int TecnicoId { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string? Especialidad { get; set; }
        public int? BahiaAsignada { get; set; }
        public string? UsuarioId { get; set; }
        public int? SucursalId { get; set; }
    }

}
