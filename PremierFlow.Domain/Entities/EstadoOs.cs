using PremierFlow.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Domain.Entities
{
    /// <summary>
    /// Estados del ciclo de vida de una Orden de Servicio.
    /// </summary>
    public class EstadoOs : AuditableEntity
    {
        public int EstadoId { get; set; }

        public string Codigo { get; set; } = null!;

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        /// <summary>
        /// Define el orden en el flujo del proceso.
        /// </summary>
        public int OrdenSecuencial { get; set; }

        public bool Activo { get; set; } = true;

        // Navegación
        public virtual ICollection<OrdenServicio> OrdenesServicio { get; set; } = new List<OrdenServicio>();

        // Constantes de estados conocidos
        public static class Estados
        {
            public const string Abierta = "ABIERTA";
            public const string Diagnostico = "DIAGNOSTICO";
            public const string Cotizada = "COTIZADA";
            public const string Aprobada = "APROBADA";
            public const string EnTrabajo = "EN_TRABAJO";
            public const string Pausada = "PAUSADA";
            public const string Completada = "COMPLETADA";
            public const string Facturada = "FACTURADA";
            public const string Cerrada = "CERRADA";
            public const string Cancelada = "CANCELADA";
        }

        // Métodos de dominio
        public bool EsEstadoFinal => Codigo == Estados.Cerrada || Codigo == Estados.Cancelada;
        public bool PermiteModificacion => !EsEstadoFinal;
    }
}
