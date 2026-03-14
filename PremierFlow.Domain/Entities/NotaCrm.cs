using PremierFlow.Domain.Common;

namespace PremierFlow.Domain.Entities
{
    /// <summary>
    /// Nota libre sobre un lead o una oportunidad.
    /// </summary>
    public class NotaCrm : SoftDeletableEntity
    {
        public int NotaCrmId { get; set; }

        #region Vinculación

        /// <summary>
        /// Lead asociado (null si la nota es solo de una oportunidad).
        /// </summary>
        public int? LeadId { get; set; }

        /// <summary>
        /// Oportunidad asociada (null si la nota es solo de un lead).
        /// </summary>
        public int? OportunidadId { get; set; }

        /// <summary>
        /// Autor de la nota (ApplicationUser.Id).
        /// </summary>
        public string AutorId { get; set; } = null!;

        #endregion

        #region Contenido

        public string Contenido { get; set; } = null!;

        /// <summary>
        /// Si es true, solo el autor puede verla.
        /// </summary>
        public bool EsPrivada { get; set; }

        #endregion

        #region Navegación

        public virtual Lead? Lead { get; set; }
        public virtual Oportunidad? Oportunidad { get; set; }

        #endregion
    }
}
