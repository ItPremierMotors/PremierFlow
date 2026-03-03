using PremierFlow.Domain.Common;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;

namespace PremierFlow.Domain.Entities
{
    /// <summary>
    /// Documenta el check-in del vehículo (recepción física).
    /// </summary>
    public class Recepcion : SoftDeletableEntity
    {
        public int RecepcionId { get; set; }

        public int OsId { get; set; }

        public DateTime FechaHoraRecepcion { get; set; } = TimeHelper.Now;

        /// <summary>
        /// FK a ApplicationUser que recibe el vehículo.
        /// </summary>
        public string RecibidoPorId { get; set; } = null!;

        /// <summary>
        /// Nombre de la persona que entrega el vehículo.
        /// </summary>
        public string EntregadoPor { get; set; } = null!;

        // Confirmación de entrega
        public bool EsPropietarioQuienEntrega { get; set; } = true;
        public string? RelacionEntregante { get; set; }
        public string? TelefonoEntregante { get; set; }

        /// <summary>
        /// Descripción del estado externo de la carrocería.
        /// </summary>
        public string? EstadoCarroceria { get; set; }

        /// <summary>
        /// Lista de accesorios recibidos.
        /// </summary>
        public string? AccesoriosRecibidos { get; set; }

        /// <summary>
        /// Daños exteriores en JSON: [{tipo:int, x:float, y:float, descripcion:string}]
        /// </summary>
        public string? DanosExteriorJson { get; set; }

        // Checklist de accesorios estándar
        public bool LlantaRepuesto { get; set; } = false;
        public bool Gato { get; set; } = false;
        public bool Triangulos { get; set; } = false;
        public bool Extintor { get; set; } = false;
        public bool Herramientas { get; set; } = false;
        public bool Radio { get; set; } = false;
        public bool Tapetes { get; set; } = false;

        // Checklist extendido - Exterior
        public bool Antena { get; set; } = false;
        public bool EspejoIzquierdo { get; set; } = false;
        public bool EspejoDerecho { get; set; } = false;
        public bool Limpiaparabrisas { get; set; } = false;
        public bool PlacaDelantera { get; set; } = false;
        public bool PlacaTrasera { get; set; } = false;
        public bool TapaCombustible { get; set; } = false;

        // Checklist extendido - Documentos/Extras
        public bool ManualVehiculo { get; set; } = false;
        public bool SegundaLlave { get; set; } = false;

        /// <summary>
        /// Inspección de ruedas en JSON: [{posicion:string, estadoLlanta:int, estadoRin:int, tuercasCompletas:bool, tieneCopa:bool}]
        /// </summary>
        public string? InspeccionRuedasJson { get; set; }

        // Motor
        public bool NivelAceiteOk { get; set; } = false;
        public bool NivelRefrigeranteOk { get; set; } = false;
        public bool NivelLiquidoFrenosOk { get; set; } = false;
        public bool BateriaOk { get; set; } = false;

        public string? ObservacionesGenerales { get; set; }

        /// <summary>
        /// Firma digital del cliente en base64.
        /// </summary>
        public string? FirmaClienteBase64 { get; set; }

        public bool ChecklistCompletado { get; set; } = false;

      
      
        // Navegación
        public virtual OrdenServicio OrdenServicio { get; set; } = null!;
        public virtual ICollection<Evidencia> Evidencias { get; set; } = new List<Evidencia>();

        // Métodos de dominio
        public bool TieneFirmaCliente => !string.IsNullOrEmpty(FirmaClienteBase64);

        public int CantidadAccesoriosVerificados
        {
            get
            {
                int count = 0;
                if (LlantaRepuesto) count++;
                if (Gato) count++;
                if (Triangulos) count++;
                if (Extintor) count++;
                if (Herramientas) count++;
                if (Radio) count++;
                if (Tapetes) count++;
                if (Antena) count++;
                if (EspejoIzquierdo) count++;
                if (EspejoDerecho) count++;
                if (Limpiaparabrisas) count++;
                if (PlacaDelantera) count++;
                if (PlacaTrasera) count++;
                if (TapaCombustible) count++;
                if (ManualVehiculo) count++;
                if (SegundaLlave) count++;
                return count;
            }
        }

        public bool EstaCompleta => ChecklistCompletado && TieneFirmaCliente;

        public void CompletarChecklist()
        {
            ChecklistCompletado = true;
        }

        public void RegistrarFirma(string firmaBase64)
        {
            if (string.IsNullOrWhiteSpace(firmaBase64))
                throw new ArgumentException("La firma no puede estar vacía");

            FirmaClienteBase64 = firmaBase64;
        }
    }
}
