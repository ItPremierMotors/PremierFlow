using PremierFlow.Domain.Common;
using PremierFlow.Domain.Enums;


namespace PremierFlow.Domain.Entities
{
    /// <summary>
    /// Negociación concreta de venta vinculada a un lead y opcionalmente a un vehículo.
    /// </summary>
    public class Oportunidad : SoftDeletableEntity
    {
        public int OportunidadId { get; set; }

        /// <summary>
        /// Código único de la oportunidad (OP-2026-0001).
        /// </summary>
        public string CodigoOportunidad { get; set; } = null!;

        #region Vinculación

        public int LeadId { get; set; }

        /// <summary>
        /// Cliente formal (se llena al calificar el lead).
        /// </summary>
        public int? ClienteId { get; set; }

        /// <summary>
        /// Vehículo específico del inventario que se está negociando.
        /// </summary>
        public int? VehiculoId { get; set; }

        /// <summary>
        /// Vendedor responsable de la oportunidad (ApplicationUser.Id).
        /// </summary>
        public string VendedorId { get; set; } = null!;

        public int SucursalId { get; set; }

        #endregion

        #region Pipeline

        public EtapaOportunidad Etapa { get; set; } = EtapaOportunidad.Prospeccion;

        /// <summary>
        /// Probabilidad de cierre (0-100).
        /// </summary>
        public int ProbabilidadCierre { get; set; }

        public DateTime? FechaCierreEstimada { get; set; }

        #endregion

        #region Resultado

        public ResultadoOportunidad? Resultado { get; set; }

        /// <summary>
        /// Motivo del resultado: "Compró en competencia", "Precio muy alto", etc.
        /// </summary>
        public string? MotivoResultado { get; set; }

        public DateTime? FechaCierre { get; set; }

        #endregion

        /// <summary>
        /// Se actualiza cada vez que se registra una actividad.
        /// </summary>
        public DateTime? FechaUltimaActividad { get; set; }

        #region Navegación

        public virtual Lead Lead { get; set; } = null!;
        public virtual Cliente? Cliente { get; set; }
        public virtual Vehiculo? Vehiculo { get; set; }
        public virtual Sucursal Sucursal { get; set; } = null!;
        public virtual ICollection<ActividadCrm> Actividades { get; set; } = new List<ActividadCrm>();
        public virtual ICollection<NotaCrm> Notas { get; set; } = new List<NotaCrm>();
        public virtual ICollection<CotizacionVehiculo> Cotizaciones { get; set; } = new List<CotizacionVehiculo>();

        #endregion

        #region Métodos de Dominio

        /// <summary>
        /// Indica si la oportunidad está abierta (sin resultado).
        /// </summary>
        public bool EstaAbierta => Resultado == null;

        /// <summary>
        /// Indica si la oportunidad fue ganada.
        /// </summary>
        public bool FueGanada => Resultado == ResultadoOportunidad.Ganada;

        /// <summary>
        /// Avanza a la siguiente etapa del pipeline.
        /// </summary>
        public void AvanzarEtapa()
        {
            if (!EstaAbierta)
                throw new InvalidOperationException("No se puede avanzar una oportunidad cerrada");

            if (Etapa == EtapaOportunidad.Cierre)
                throw new InvalidOperationException("La oportunidad ya está en la etapa final");

            Etapa = (EtapaOportunidad)((int)Etapa + 1);
        }

        /// <summary>
        /// Retrocede a la etapa anterior del pipeline.
        /// </summary>
        public void RetrocederEtapa()
        {
            if (!EstaAbierta)
                throw new InvalidOperationException("No se puede retroceder una oportunidad cerrada");

            if (Etapa == EtapaOportunidad.Prospeccion)
                throw new InvalidOperationException("La oportunidad ya está en la primera etapa");

            Etapa = (EtapaOportunidad)((int)Etapa - 1);
        }

        /// <summary>
        /// Cierra la oportunidad como ganada.
        /// </summary>
        public void CerrarGanada()
        {
            if (!EstaAbierta)
                throw new InvalidOperationException("La oportunidad ya está cerrada");

            if (VehiculoId == null)
                throw new InvalidOperationException("Debe vincular un vehículo antes de cerrar como ganada");

            Resultado = ResultadoOportunidad.Ganada;
            FechaCierre = TimeHelper.Now;
        }

        /// <summary>
        /// Cierra la oportunidad como perdida.
        /// </summary>
        public void CerrarPerdida(string motivo)
        {
            if (!EstaAbierta)
                throw new InvalidOperationException("La oportunidad ya está cerrada");

            Resultado = ResultadoOportunidad.Perdida;
            MotivoResultado = motivo;
            FechaCierre = TimeHelper.Now;
        }

        /// <summary>
        /// Cancela la oportunidad.
        /// </summary>
        public void Cancelar(string motivo)
        {
            if (!EstaAbierta)
                throw new InvalidOperationException("La oportunidad ya está cerrada");

            Resultado = ResultadoOportunidad.Cancelada;
            MotivoResultado = motivo;
            FechaCierre = TimeHelper.Now;
        }

        /// <summary>
        /// Registra que hubo una actividad reciente.
        /// </summary>
        public void RegistrarActividad()
        {
            FechaUltimaActividad = TimeHelper.Now;
        }

        #endregion
    }
}
