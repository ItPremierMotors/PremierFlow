using PremierFlow.Domain.Common;
using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Domain.Entities
{
    public class Cliente : SoftDeletableEntity
    {
        public int ClienteId { get; set; }

        public TipoCliente TipoCliente { get; set; } = TipoCliente.Persona;

        public string Nombre { get; set; } = null!;

        public string? Apellidos { get; set; }

        public string? DocumentoIdentidad { get; set; }

        public string Telefono { get; set; } = null!;

        public string? Email { get; set; }

        public string? Direccion { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Contador de inasistencias (no-show).
        /// </summary>
        public int NoShowCount { get; set; } = 0;
        /// <summary>
        /// FK opcional para vincular cliente con usuario del sistema.
        /// </summary>
        public string? UsuarioId { get; set; }

        // Navegación
        public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
        public virtual ICollection<Cita> Citas { get; set; } = new List<Cita>();
        public virtual ICollection<OrdenServicio> OrdenesServicio { get; set; } = new List<OrdenServicio>();

        // Métodos de dominio
        public string NombreCompleto => string.IsNullOrEmpty(Apellidos)
            ? Nombre
            : $"{Nombre} {Apellidos}";

        public void IncrementarNoShow() =>              ++;

        public bool TieneAlertaNoShow => NoShowCount >= 3;

    }
}
