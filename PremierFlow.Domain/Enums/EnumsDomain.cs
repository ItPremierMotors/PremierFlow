using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Domain.Enums
{

    /// <summary>
    /// Tipo de cliente (persona natural o empresa).
    /// </summary>
    public enum TipoCliente
    {
        Persona = 1,
        Empresa = 2
    }

    /// <summary>
    /// Segmento de vehículo según su carrocería.
    /// </summary>
    public enum SegmentoVehiculo
    {
        Sedan = 1,
        Suv = 2,
        Pickup = 3,
        Van = 4,
        Coupe = 5,
        Hatchback = 6,
        Deportivo = 7,
        Electrico = 8,
        Hibrido = 9,
        Otro = 99
    }

    /// <summary>
    /// Tipo de tracción del vehículo.
    /// </summary>
    public enum TipoTraccion
    {
        Fwd = 1,  // Front-Wheel Drive
        Rwd = 2,  // Rear-Wheel Drive
        Awd = 3,  // All-Wheel Drive
        FourWd = 4  // 4-Wheel Drive
    }

    /// <summary>
    /// Tipo de combustible del vehículo.
    /// </summary>
    public enum TipoCombustible
    {
        Gasolina = 1,
        Diesel = 2,
        Hibrido = 3,
        Electrico = 4,
        GasNatural = 5,
        Glp = 6
    }

    /// <summary>
    /// Procedencia del vehículo.
    /// </summary>
    public enum ProcedenciaVehiculo
    {
        Local = 1,
        Importado = 2
    }

    /// <summary>
    /// Clasificación de servicio según duración.
    /// </summary>
    public enum ClasificacionServicio
    {
        Rapido = 1,
        Estandar = 2,
        Largo = 3
    }

    /// <summary>
    /// Estado de la cita.
    /// </summary>
    public enum EstadoCita
    {
        Agendada = 1,
        Confirmada = 2,
        EnProceso = 3,
        Completada = 4,
        Cancelada = 5,
        NoShow = 6
    }

    /// <summary>
    /// Tipo de ingreso del vehículo.
    /// </summary>
    public enum TipoIngreso
    {
        Cita = 1,
        WalkIn = 2,
        Garantia = 3
    }

    /// <summary>
    /// Turno del taller.
    /// </summary>
    public enum TurnoTaller
    {
        Manana = 1,
        Tarde = 2,
        Completo = 3
    }

    /// <summary>
    /// Tipo de bloque horario.
    /// </summary>
    public enum TipoBloqueHorario
    {
        Estandar = 1,
        Rapido = 2,
        ColisionPintura = 3
    }

    /// <summary>
    /// Estado de un servicio dentro de la OS.
    /// </summary>
    public enum EstadoServicioOS
    {
        Pendiente = 1,
        EnProceso = 2,
        Completado = 3,
        Cancelado = 4
    }

    /// <summary>
    /// Estado de asignación de técnico.
    /// </summary>
    public enum EstadoAsignacion
    {
        Asignado = 1,
        EnProceso = 2,
        Completado = 3,
        Pausado = 4
    }

    /// <summary>
    /// Tipo de evidencia fotográfica.
    /// </summary>
    public enum TipoEvidencia
    {
        FotoFrontal = 1,
        FotoTrasera = 2,
        FotoLateralIzq = 3,
        FotoLateralDer = 4,
        FotoInterior = 5,
        FotoTablero = 6,
        FotoDano = 7,
        Otro = 99
    }
    public enum EstadoVehiculo
    {
        EnTransito = 1,        // Viene en camino (importación)
        EnAduana = 2,          // En proceso de nacionalización
        EnBodega = 3,          // Recibido, en inventario
        EnExhibicion = 4,      // En showroom para venta
        Reservado = 5,         // Cliente lo apartó
        Vendido = 6,           // Ya tiene dueño
        Entregado = 7          // Entregado al cliente
    }
    public enum TipoTransmision
    {
        Manual,
        Automatica,
        CVT ,
        DualClutch,
        Semiautomatica 
    }
}
