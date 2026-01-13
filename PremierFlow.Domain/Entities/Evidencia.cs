using PremierFlow.Domain.Common;
using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Domain.Entities
{
    /// <summary>
    /// Almacena fotos y evidencias de la recepción del vehículo.
    /// </summary>
    public class Evidencia : AuditableEntity
    {
        public int EvidenciaId { get; set; }

        public int OsId { get; set; }

        public int? RecepcionId { get; set; }

        public TipoEvidencia TipoEvidencia { get; set; }

        /// <summary>
        /// Ruta del archivo o URL de almacenamiento.
        /// </summary>
        public string UrlArchivo { get; set; } = null!;

        public string? Descripcion { get; set; }

        public DateTime FechaCaptura { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// FK a ApplicationUser que tomó la foto.
        /// </summary>
        public string? UsuarioRegistroId { get; set; }

        // Navegación
        public virtual OrdenServicio OrdenServicio { get; set; } = null!;
        public virtual Recepcion? Recepcion { get; set; }

        // Métodos de dominio
        public bool EsFotoRecepcion => RecepcionId.HasValue;

        public bool EsFotoDano => TipoEvidencia == TipoEvidencia.FotoDano;

        public string NombreArchivo => Path.GetFileName(UrlArchivo);

        public string? Extension => Path.GetExtension(UrlArchivo)?.ToLowerInvariant();

        public bool EsImagen => new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }
            .Contains(Extension);
    }

}
