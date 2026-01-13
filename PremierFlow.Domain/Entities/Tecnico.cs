using PremierFlow.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Domain.Entities
{

    /// <summary>
    /// Representa un técnico del taller.
    /// </summary>
    public class Tecnico : SoftDeletableEntity
    {
        public int TecnicoId { get; set; }

        public string Codigo { get; set; } = null!;

        public string Nombre { get; set; } = null!;

        public string Apellidos { get; set; } = null!;

        public string? Especialidad { get; set; }

        /// <summary>
        /// Bahía asignada por defecto al técnico.
        /// </summary>
        public int? BahiaAsignada { get; set; }
        /// <summary>
        /// FK opcional a ApplicationUser si el técnico tiene acceso al sistema.
        /// </summary>
        public string? UsuarioId { get; set; }
        /// <summary>
        /// Sucursal donde trabaja el técnico.
        /// </summary>
        public int? SucursalId { get; set; }

        // Navegación
        public virtual Sucursal? Sucursal { get; set; }
        public virtual ICollection<OsServicio> ServiciosAsignados { get; set; } = new List<OsServicio>();
        public virtual ICollection<AsignacionTecnico> Asignaciones { get; set; } = new List<AsignacionTecnico>();

        // Métodos de dominio
        public string NombreCompleto => $"{Nombre} {Apellidos}";
    }

}
