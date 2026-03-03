namespace PremierFlow.Application.Dtos.Taller
{
    /// <summary>
    /// DTO compuesto para iniciar atención desde cita.
    /// Crea OS + Recepción + cambia Cita a EnProceso en una transacción.
    /// </summary>
    public class IniciarRecepcionDTO
    {
        public int CitaId { get; set; }

        // Datos para OS
        public int Kilometraje { get; set; }
        public int NivelCombustiblePorcentaje { get; set; } // 0-100
        public string? ObservacionesApertura { get; set; }

        // Entrega
        public string EntregadoPor { get; set; } = null!;
        public bool EsPropietarioQuienEntrega { get; set; } = true;
        public string? RelacionEntregante { get; set; }
        public string? TelefonoEntregante { get; set; }

        // Daños exteriores (JSON string)
        public string? DanosExteriorJson { get; set; }

        // Checklist accesorios (originales)
        public bool LlantaRepuesto { get; set; }
        public bool Gato { get; set; }
        public bool Triangulos { get; set; }
        public bool Extintor { get; set; }
        public bool Herramientas { get; set; }
        public bool Radio { get; set; }
        public bool Tapetes { get; set; }

        // Checklist extendido - Exterior
        public bool Antena { get; set; }
        public bool EspejoIzquierdo { get; set; }
        public bool EspejoDerecho { get; set; }
        public bool Limpiaparabrisas { get; set; }
        public bool PlacaDelantera { get; set; }
        public bool PlacaTrasera { get; set; }
        public bool TapaCombustible { get; set; }

        // Documentos/Extras
        public bool ManualVehiculo { get; set; }
        public bool SegundaLlave { get; set; }

        // Ruedas (JSON string)
        public string? InspeccionRuedasJson { get; set; }

        // Motor
        public bool NivelAceiteOk { get; set; }
        public bool NivelRefrigeranteOk { get; set; }
        public bool NivelLiquidoFrenosOk { get; set; }
        public bool BateriaOk { get; set; }

        // Observaciones
        public string? ObservacionesGenerales { get; set; }

        // Firma
        public string? FirmaClienteBase64 { get; set; }
    }

    /// <summary>
    /// DTO compuesto para iniciar atención Walk-In (sin cita previa).
    /// Crea OS + Recepción + OsServicio en una transacción.
    /// </summary>
    public class IniciarRecepcionWalkInDTO
    {
        // Datos del vehiculo/cliente (en vez de CitaId)
        public int ClienteId { get; set; }
        public int VehiculoId { get; set; }
        public int TipoServicioId { get; set; }
        public string MotivoVisita { get; set; } = null!;
        public int SucursalId { get; set; }

        // Datos para OS
        public int Kilometraje { get; set; }
        public int NivelCombustiblePorcentaje { get; set; }
        public string? ObservacionesApertura { get; set; }

        // Entrega
        public string EntregadoPor { get; set; } = null!;
        public bool EsPropietarioQuienEntrega { get; set; } = true;
        public string? RelacionEntregante { get; set; }
        public string? TelefonoEntregante { get; set; }

        // Daños exteriores (JSON string)
        public string? DanosExteriorJson { get; set; }

        // Checklist accesorios
        public bool LlantaRepuesto { get; set; }
        public bool Gato { get; set; }
        public bool Triangulos { get; set; }
        public bool Extintor { get; set; }
        public bool Herramientas { get; set; }
        public bool Radio { get; set; }
        public bool Tapetes { get; set; }

        // Checklist extendido - Exterior
        public bool Antena { get; set; }
        public bool EspejoIzquierdo { get; set; }
        public bool EspejoDerecho { get; set; }
        public bool Limpiaparabrisas { get; set; }
        public bool PlacaDelantera { get; set; }
        public bool PlacaTrasera { get; set; }
        public bool TapaCombustible { get; set; }

        // Documentos/Extras
        public bool ManualVehiculo { get; set; }
        public bool SegundaLlave { get; set; }

        // Ruedas (JSON string)
        public string? InspeccionRuedasJson { get; set; }

        // Motor
        public bool NivelAceiteOk { get; set; }
        public bool NivelRefrigeranteOk { get; set; }
        public bool NivelLiquidoFrenosOk { get; set; }
        public bool BateriaOk { get; set; }

        // Observaciones
        public string? ObservacionesGenerales { get; set; }

        // Firma
        public string? FirmaClienteBase64 { get; set; }
    }

    /// <summary>
    /// DTO con datos de la cita para poblar el wizard de recepción.
    /// </summary>
    public class DatosCitaWizardDTO
    {
        public int CitaId { get; set; }
        public string CodigoCita { get; set; } = null!;
        public string ClienteNombre { get; set; } = null!;
        public string? ClienteTelefono { get; set; }
        public string VehiculoDescripcion { get; set; } = null!;
        public string? VehiculoPlaca { get; set; }
        public string? VehiculoVin { get; set; }
        public int KilometrajeActual { get; set; }
        public int SegmentoVehiculo { get; set; }
        public string TipoServicioNombre { get; set; } = null!;
        public string MotivoVisita { get; set; } = null!;
    }
}
