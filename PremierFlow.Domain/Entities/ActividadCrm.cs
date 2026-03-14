using PremierFlow.Domain.Common;
using PremierFlow.Domain.Enums;

namespace PremierFlow.Domain.Entities
{
    /// <summary>
    /// Interacción con un prospecto/cliente: llamada, WhatsApp, visita, test drive, etc.
    /// </summary>
    public class ActividadCrm : SoftDeletableEntity
    {
        public int ActividadCrmId { get; set; }

        #region Vinculación

        /// <summary>
        /// Lead asociado (puede ser null si la actividad es solo de una oportunidad).
        /// </summary>
        public int? LeadId { get; set; }

        /// <summary>
        /// Oportunidad asociada (puede ser null si el lead aún no se convirtió).
        /// </summary>
        public int? OportunidadId { get; set; }

        /// <summary>
        /// Vendedor que realizó la actividad (ApplicationUser.Id).
        /// </summary>
        public string RealizadaPorId { get; set; } = null!;

        #endregion

        #region Detalle

        public TipoActividadCrm Tipo { get; set; }

        /// <summary>
        /// Si el contacto fue entrante (el cliente nos buscó) o saliente (nosotros lo contactamos).
        /// </summary>
        public DireccionActividad Direccion { get; set; }

        public string Asunto { get; set; } = null!;
        public string? Descripcion { get; set; }
        public EstadoActividad Estado { get; set; } = EstadoActividad.Pendiente;

        #endregion

        #region Programación

        /// <summary>
        /// Cuándo se debe realizar la actividad.
        /// </summary>
        public DateTime? FechaProgramada { get; set; }

        /// <summary>
        /// Cuándo se realizó efectivamente.
        /// </summary>
        public DateTime? FechaRealizacion { get; set; }

        public int? DuracionMinutos { get; set; }

        #endregion

        #region Resultado

        /// <summary>
        /// Resultado de la actividad: "Interesado", "No contestó", "Agendó visita", etc.
        /// </summary>
        public string? Resultado { get; set; }

        /// <summary>
        /// Fecha sugerida para el próximo contacto (genera recordatorio).
        /// </summary>
        public DateTime? ProximoContacto { get; set; }

        #endregion

        #region Navegación

        public virtual Lead? Lead { get; set; }
        public virtual Oportunidad? Oportunidad { get; set; }

        #endregion

        #region Métodos de Dominio

        /// <summary>
        /// Indica si la actividad está vencida (pasó la fecha programada sin completarse).
        /// </summary>
        public bool EstaVencida => Estado == EstadoActividad.Pendiente &&
                                    FechaProgramada.HasValue &&
                                    FechaProgramada.Value < TimeHelper.Now;

        /// <summary>
        /// Completa la actividad con un resultado.
        /// </summary>
        public void Completar(string resultado, DateTime? proximoContacto = null)
        {
            if (Estado != EstadoActividad.Pendiente)
                throw new InvalidOperationException("Solo se pueden completar actividades pendientes");

            Estado = EstadoActividad.Completada;
            Resultado = resultado;
            FechaRealizacion = TimeHelper.Now;
            ProximoContacto = proximoContacto;
        }

        /// <summary>
        /// Cancela la actividad.
        /// </summary>
        public void CancelarActividad()
        {
            if (Estado != EstadoActividad.Pendiente)
                throw new InvalidOperationException("Solo se pueden cancelar actividades pendientes");

            Estado = EstadoActividad.Cancelada;
        }

        #endregion
    }
}
