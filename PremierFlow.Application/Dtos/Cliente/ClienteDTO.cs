using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Dtos.Cliente
{
    public class ClienteDTO
    {
        public int ClienteId { get; set; }
        public TipoCliente TipoCliente { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Apellidos { get; set; }
        public string? DNI { get; set; }
        public string? RTN { get; set; }
        public string Telefono { get; set; } = null!;
        public string? TelefonoSecundario { get; set; }
        public string? Email { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int NoShowCount { get; set; }

        // Extras para UI
        public string NombreCompleto { get; set; } = null!;
        public bool TieneAlertaNoShow { get; set; }
        public int CantidadVehiculos { get; set; }
    }
    public class CreateClienteDTO
    {
        public TipoCliente TipoCliente { get; set; } = TipoCliente.Persona;
        public string Nombre { get; set; } = null!;
        public string? Apellidos { get; set; }
        public string? DNI { get; set; }
        public string? RTN { get; set; }
        public string Telefono { get; set; } = null!;
        public string? TelefonoSecundario { get; set; }
        public string? Email { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
    }

    public class UpdateClienteDTO
    {
        public int ClienteId { get; set; }
        public TipoCliente TipoCliente { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Apellidos { get; set; }
        public string? DNI { get; set; }
        public string? RTN { get; set; }
        public string Telefono { get; set; } = null!;
        public string? TelefonoSecundario { get; set; }
        public string? Email { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
    }
}
