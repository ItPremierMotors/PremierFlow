using PremierFlow.Domain.Common;
using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Domain.Entities
{

    /// <summary>
    /// Catálogo de tipos de servicio que ofrece el taller.
    /// </summary>
    public class TipoServicio : SoftDeletableEntity
    {
        public int TipoServicioId { get; set; }

        public string Codigo { get; set; } = null!;

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        /// <summary>
        /// Duración estimada en minutos.
        /// </summary>
        public int DuracionEstimadaMin { get; set; }

        public ClasificacionServicio Clasificacion { get; set; } = ClasificacionServicio.Estandar;

        /// <summary>
        /// Indica si permite atención sin cita (walk-in).
        /// </summary>
        public bool PermiteWalkIn { get; set; } = false;

        /// <summary>
        /// Indica si requiere cita previa obligatoria.
        /// </summary>
        public bool RequiereCita { get; set; } = true;

        public decimal PrecioBase { get; set; } = 0;

        /// <summary>
        /// Indica si requiere repuestos específicos en stock.
        /// </summary>
        public int StockRequerido { get; set; } = 0;

        // Navegación
        public virtual ICollection<Cita> Citas { get; set; } = new List<Cita>();
        public virtual ICollection<OsServicio> OsServicios { get; set; } = new List<OsServicio>();

        // Métodos de dominio
        public bool EsServicioRapido => Clasificacion == ClasificacionServicio.Rapido;

        public TimeSpan DuracionEstimada => TimeSpan.FromMinutes(DuracionEstimadaMin);
    }

}
