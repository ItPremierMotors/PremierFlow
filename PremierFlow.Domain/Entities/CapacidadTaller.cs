using PremierFlow.Domain.Common;
using PremierFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Domain.Entities
{
    public class CapacidadTaller : SoftDeletableEntity
    {
        public int CapacidadId { get; set; }
        public DateTime Fecha { get; set; }
        public TurnoTaller Turno { get; set; } = TurnoTaller.Completo;
        public int TecnicosDisponibles { get; set; } = 0;
        public int BahiasDisponibles { get; set; } = 0;

        // Planificación
        public int MinutosDisponibles { get; set; } = 0;
        public int MinutosReservados { get; set; } = 0;

        // Ejecución real
        public int MinutosUtilizados { get; set; } = 0;
        public int MinutosSobretiempo { get; set; } = 0;        // ← NUEVO
        public bool PermiteSobretiempo { get; set; } = true;    // ← NUEVO

        public bool PermiteAgendamiento { get; set; } = true;
        public string? Observaciones { get; set; }
        public int? SucursalId { get; set; }

        // Navegación
        public virtual Sucursal? Sucursal { get; set; }
        public virtual ICollection<BloqueHorario> BloquesHorario { get; set; } = new List<BloqueHorario>();

        // Métodos de dominio - PLANIFICACIÓN
        public int MinutosLibres => MinutosDisponibles - MinutosReservados;

        public decimal PorcentajeOcupacion => MinutosDisponibles > 0
            ? (decimal)MinutosReservados / MinutosDisponibles * 100
            : 0;

        public bool TieneCapacidadPara(int minutosRequeridos)
            => PermiteAgendamiento && MinutosLibres >= minutosRequeridos;

        public void ReservarMinutos(int minutos)
        {
            if (!TieneCapacidadPara(minutos))
                throw new InvalidOperationException("No hay capacidad disponible para agendar.");
            MinutosReservados += minutos;
        }

        public void LiberarMinutos(int minutos)
        {
            MinutosReservados = Math.Max(0, MinutosReservados - minutos);
        }

        // Métodos de dominio - EJECUCIÓN REAL
        public void RegistrarTiempoTrabajado(int minutos)
        {
            MinutosUtilizados += minutos;

            if (MinutosUtilizados > MinutosDisponibles)
            {
                MinutosSobretiempo = MinutosUtilizados - MinutosDisponibles;
            }
        }

        public decimal PorcentajeEficiencia => MinutosReservados > 0
            ? (decimal)MinutosUtilizados / MinutosReservados * 100
            : 0;

        public bool TuvoSobretiempo => MinutosSobretiempo > 0;
    }
//```

//---

//## Resumen:

//| Campo | Propósito |
//|-------|-----------|
//| `MinutosDisponibles` | Lo que planificas(8 horas = 480) |
//| `MinutosReservados` | Lo que agendas(citas) |
//| `MinutosUtilizados` | Lo que realmente se trabajó |
//| `MinutosSobretiempo` | Tiempo extra trabajado |
//| `PermiteSobretiempo` | ¿Permitir que el día se extienda? |

//---

//## Reportes que podrás generar:
//```
//Día: 15/01/2026
//- Capacidad: 480 min(8h)
//- Agendado: 450 min
//- Trabajado: 520 min
//- Sobretiempo: 40 min
//- Eficiencia: 115% (trabajó más de lo agendado)
}