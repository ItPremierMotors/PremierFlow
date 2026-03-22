using PremierFlow.Domain.Common;
using PremierFlow.Domain.Enums;


namespace PremierFlow.Domain.Entities
{
    /// <summary>
    /// Prospecto comercial capturado desde cualquier canal.
    /// </summary>
    public class Lead : SoftDeletableEntity
    {
        public int LeadId { get; set; }

        /// <summary>
        /// Código único del lead (LD-2026-0001).
        /// </summary>
        public string CodigoLead { get; set; } = null!;

        #region Datos del prospecto

        public string NombreCompleto { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Empresa { get; set; }
        public string? Ciudad { get; set; }

        #endregion

        #region Origen y asignación

        /// <summary>
        /// Canal por donde llegó el prospecto.
        /// </summary>
        public OrigenLead Origen { get; set; }

        /// <summary>
        /// Detalle del origen (URL de campaña, nombre del referido, etc.).
        /// </summary>
        public string? DetalleOrigen { get; set; }

        public int? SucursalId { get; set; }

        /// <summary>
        /// Vendedor asignado para dar seguimiento (ApplicationUser.Id).
        /// </summary>
        public string? VendedorAsignadoId { get; set; }

        /// <summary>
        ///   TIPO DE VEHÍCULO DE INTERÉS (Nuevo, Usado, etc.) - para segmentación comercial y análisis de datos.
        /// </summary>
        public TipoVehiculo? TipoVehiculoInteres { get; set; }

        #endregion

        #region Estado

        public EstadoLead Estado { get; set; } = EstadoLead.Nuevo;
        /// <summary>
        /// Se actualiza cada vez que se registra una actividad.
        /// </summary>
        public DateTime? FechaUltimaActividad { get; set; }
        public DateTime FechaIngreso { get; set; } = TimeHelper.Now;

        /// <summary>
        /// Cuándo el vendedor contactó al lead por primera vez.
        /// </summary>
        public DateTime? FechaPrimeraRespuesta { get; set; }

        /// <summary>
        /// Cuándo se convirtió en oportunidad.
        /// </summary>
        public DateTime? FechaConversion { get; set; }

        public DateTime? FechaDescarte { get; set; }
        public string? MotivoDescarte { get; set; }

        #endregion

        #region Interés

        /// <summary>
        /// Texto libre: "RAV4 2025", "SUV familiar", etc.
        /// </summary>
        public string? VehiculoInteres { get; set; }
        public decimal? PresupuestoEstimado { get; set; }

        #endregion

        #region Navegación

        public virtual Sucursal? Sucursal { get; set; } 
        public virtual ICollection<Oportunidad> Oportunidades { get; set; } = new List<Oportunidad>();
        public virtual ICollection<ActividadCrm> Actividades { get; set; } = new List<ActividadCrm>();
        public virtual ICollection<NotaCrm> Notas { get; set; } = new List<NotaCrm>();

        #endregion

        #region Métodos de Dominio

        /// <summary>
        /// Indica si el lead aún no ha sido contactado.
        /// </summary>
        public bool SinContactar => (Estado == EstadoLead.Nuevo || Estado == EstadoLead.Incompleto) && FechaPrimeraRespuesta == null;

        /// <summary>
        /// Indica si ya fue convertido a oportunidad.
        /// </summary>
        public bool FueConvertido => Estado == EstadoLead.ConvertidoAOportunidad;

        /// <summary>
        /// Marca como contactado y registra la fecha de primera respuesta.
        /// </summary>
        public void MarcarContactado()
        {
            //solo puede macar como contactado si es nuevo o incompleto
            if (Estado != EstadoLead.Nuevo && Estado != EstadoLead.Incompleto)
                return;

            Estado = EstadoLead.Contactado;
            FechaPrimeraRespuesta ??= TimeHelper.Now;
        }

        /// <summary>
        /// Marca como calificado (tiene interés real y presupuesto).
        /// </summary>
        public void Calificar()
        {
            if (Estado != EstadoLead.Contactado)
                throw new InvalidOperationException("El lead debe estar contactado para calificarse");

            Estado = EstadoLead.Calificado;
        }

        /// <summary>
        /// Marca como convertido a oportunidad.
        /// </summary>
        public void Convertir()
        {
            if (Estado != EstadoLead.Calificado)
                throw new InvalidOperationException("El lead debe estar calificado para convertirse");

            Estado = EstadoLead.ConvertidoAOportunidad;
            FechaConversion = TimeHelper.Now;
        }

        /// <summary>
        /// Descarta el lead con un motivo.
        /// </summary>
        public void Descartar(string motivo)
        {
            if (Estado == EstadoLead.ConvertidoAOportunidad)
                throw new InvalidOperationException("No se puede descartar un lead ya convertido");

            Estado = EstadoLead.Descartado;
            MotivoDescarte = motivo;
            FechaDescarte = TimeHelper.Now;
        }
        public void RegistrarActividad()
        {
            FechaUltimaActividad = TimeHelper.Now;
        }

        #endregion
    }
}
