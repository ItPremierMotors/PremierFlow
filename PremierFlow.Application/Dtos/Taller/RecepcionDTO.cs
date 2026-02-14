using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Dtos.Taller
{
    public class RecepcionDTO
    {
        public int RecepcionId { get; set; }
        public int OsId { get; set; }
        public DateTime FechaHoraRecepcion { get; set; }
        public string RecibidoPorId { get; set; } = null!;
        public string EntregadoPor { get; set; } = null!;
        public string? EstadoCarroceria { get; set; }
        public string? AccesoriosRecibidos { get; set; }

        // Checklist
        public bool LlantaRepuesto { get; set; }
        public bool Gato { get; set; }
        public bool Triangulos { get; set; }
        public bool Extintor { get; set; }
        public bool Herramientas { get; set; }
        public bool Radio { get; set; }
        public bool Tapetes { get; set; }

        public string? ObservacionesGenerales { get; set; }
        public string? FirmaClienteBase64 { get; set; }
        public bool ChecklistCompletado { get; set; }

        // Extras para UI
        public string NumeroOs { get; set; } = null!;
        public string? VehiculoDescripcion { get; set; }
        public string? VehiculoPlaca { get; set; }
        public string? ClienteNombre { get; set; }
        public bool TieneFirmaCliente { get; set; }
        public int CantidadAccesoriosVerificados { get; set; }
        public bool EstaCompleta { get; set; }
        public int CantidadEvidencias { get; set; }
    }
    public class CreateRecepcionDTO
    {
        public int OsId { get; set; }
        public string EntregadoPor { get; set; } = null!;
        public string? EstadoCarroceria { get; set; }
        public string? AccesoriosRecibidos { get; set; }

        // Checklist
        public bool LlantaRepuesto { get; set; } = false;
        public bool Gato { get; set; } = false;
        public bool Triangulos { get; set; } = false;
        public bool Extintor { get; set; } = false;
        public bool Herramientas { get; set; } = false;
        public bool Radio { get; set; } = false;
        public bool Tapetes { get; set; } = false;

        public string? ObservacionesGenerales { get; set; }
    }
    public class UpdateRecepcionDTO
    {
        public int RecepcionId { get; set; }
        public string EntregadoPor { get; set; } = null!;
        public string? EstadoCarroceria { get; set; }
        public string? AccesoriosRecibidos { get; set; }

        // Checklist
        public bool LlantaRepuesto { get; set; }
        public bool Gato { get; set; }
        public bool Triangulos { get; set; }
        public bool Extintor { get; set; }
        public bool Herramientas { get; set; }
        public bool Radio { get; set; }
        public bool Tapetes { get; set; }

        public string? ObservacionesGenerales { get; set; }
    }
    public class RegistrarFirmaDTO
    {
        public int RecepcionId { get; set; }
        public string FirmaClienteBase64 { get; set; } = null!;
    }
}
