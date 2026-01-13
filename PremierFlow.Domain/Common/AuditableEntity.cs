using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Domain.Common
{
    /// <summary>
    /// Clase base para entidades que requieren auditoría de creación y modificación.
    /// </summary>
    public abstract class AuditableEntity
    {
        /// <summary>
        /// ID del usuario que creó el registro (ApplicationUser.Id)
        /// </summary>
        public string? UsuarioCreaId { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// ID del usuario que modificó el registro (ApplicationUser.Id)
        /// </summary>
        public string? UsuarioModificaId { get; set; }

        public DateTime? FechaModificacion
        {
            get; set;
        }

}
