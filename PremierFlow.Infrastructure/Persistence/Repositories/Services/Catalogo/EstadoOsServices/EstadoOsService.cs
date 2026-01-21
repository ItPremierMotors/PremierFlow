using Microsoft.EntityFrameworkCore;
using PremierFlow.Application.Common;
using PremierFlow.Application.Dtos.Catalogo;
using PremierFlow.Application.Interfaces;
using PremierFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Infrastructure.Persistence.Repositories.Services.Catalogo.IEstadoOsServices
{
    public class EstadoOsService : IEstadoOsService
    {
        private readonly PremierFlowDbContext context;

        private static readonly Dictionary<string, string[]> TransicionesValidas = new()
        {
            { EstadoOs.Estados.Abierta, new[] { EstadoOs.Estados.Diagnostico, EstadoOs.Estados.Cancelada } },
            { EstadoOs.Estados.Diagnostico, new[] { EstadoOs.Estados.Cotizada, EstadoOs.Estados.Cancelada } },
            { EstadoOs.Estados.Cotizada, new[] { EstadoOs.Estados.Aprobada, EstadoOs.Estados.Cancelada } },
            { EstadoOs.Estados.Aprobada, new[] { EstadoOs.Estados.EnTrabajo, EstadoOs.Estados.Cancelada } },
            { EstadoOs.Estados.EnTrabajo, new[] { EstadoOs.Estados.Pausada, EstadoOs.Estados.Completada } },
            { EstadoOs.Estados.Pausada, new[] { EstadoOs.Estados.EnTrabajo, EstadoOs.Estados.Cancelada } },
            { EstadoOs.Estados.Completada, new[] { EstadoOs.Estados.Facturada } },
            { EstadoOs.Estados.Facturada, new[] { EstadoOs.Estados.Cerrada } },
            { EstadoOs.Estados.Cerrada, Array.Empty<string>() },
            { EstadoOs.Estados.Cancelada, Array.Empty<string>() }
        };

        public EstadoOsService(PremierFlowDbContext context)
        {
           this.context = context;
        }

        public async Task<ApiResponse<List<EstadoOsDTO>>> GetAllAsync()
        {
            var estados = await context.EstadosOs
             .AsNoTracking()
             .Where(e => e.Activo)
             .OrderBy(e => e.OrdenSecuencial)
             .ToListAsync();

            var dtos = estados.Select(MapToDto).ToList();

            return ApiResponse<List<EstadoOsDTO>>.ok(dtos, "Estados obtenidos.");
        }

        public async Task<ApiResponse<EstadoOsDTO>> GetByCodigoAsync(string codigo)
        {
            var estado = await context.EstadosOs
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Codigo == codigo && e.Activo);

            if (estado == null)
                return ApiResponse<EstadoOsDTO>.fail(404, null, "Estado no encontrado.");

            return ApiResponse<EstadoOsDTO>.ok(MapToDto(estado), "Estado obtenido.");
        }

        public async Task<ApiResponse<EstadoOsDTO>> GetByIdAsync(int estadoId)
        {
            var estado = await context.EstadosOs
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EstadoId == estadoId && e.Activo);
             
            if (estado == null)
                return ApiResponse<EstadoOsDTO>.fail(404, null, "Estado no encontrado.");

            return ApiResponse<EstadoOsDTO>.ok(MapToDto(estado), "Estado obtenido.");
        }

        public async Task<ApiResponse<List<EstadoOsDTO>>> GetTransicionesValidasAsync(int estadoActualId)
        {
            var estadoActual = await context.EstadosOs
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EstadoId == estadoActualId && e.Activo);

            if (estadoActual == null)
                return ApiResponse<List<EstadoOsDTO>>.fail(404, null, "Estado actual no encontrado.");

            if (!TransicionesValidas.TryGetValue(estadoActual.Codigo, out var codigosValidos))
                return ApiResponse<List<EstadoOsDTO>>.ok(new List<EstadoOsDTO>(), "No hay transiciones válidas.");

            var estadosValidos = await context.EstadosOs
                .AsNoTracking()
                .Where(e => codigosValidos.Contains(e.Codigo) && e.Activo)
                .OrderBy(e => e.OrdenSecuencial)
                .ToListAsync();

            var dtos = estadosValidos.Select(MapToDto).ToList();

            return ApiResponse<List<EstadoOsDTO>>.ok(dtos, "Transiciones válidas obtenidas.");
        }

        #region Helper

        private static EstadoOsDTO MapToDto(EstadoOs e)
        {
            return new EstadoOsDTO
            {
                EstadoId = e.EstadoId,
                Codigo = e.Codigo,
                Nombre = e.Nombre,
                Descripcion = e.Descripcion,
                OrdenSecuencial = e.OrdenSecuencial,
                EsEstadoFinal = e.EsEstadoFinal,
                PermiteModificacion = e.PermiteModificacion
            };
        }

        #endregion
    }
}

//{ EstadoOs.Estados.Abierta, new[] { EstadoOs.Estados.Diagnostico, EstadoOs.Estados.Cancelada } }
////       ↑ Estado actual              ↑ Estados a los que puede ir
//```

//**"Si la OS está en ABIERTA, solo puede cambiar a DIAGNOSTICO o CANCELADA" * *

//---

//## Tabla de transiciones:

//| Estado actual | Puede ir a |
//|---------------|------------|
//| ABIERTA | DIAGNOSTICO, CANCELADA |
//| DIAGNOSTICO | COTIZADA, CANCELADA |
//| COTIZADA | APROBADA, CANCELADA |
//| APROBADA | EN_TRABAJO, CANCELADA |
//| EN_TRABAJO | PAUSADA, COMPLETADA |
//| PAUSADA | EN_TRABAJO, CANCELADA |
//| COMPLETADA | FACTURADA |
//| FACTURADA | CERRADA |
//| CERRADA | (ninguno - estado final) |
//| CANCELADA | (ninguno - estado final) |

//---

//## Ejemplo de uso:
//```
//OS está en "COTIZADA"
//   ↓
//Usuario quiere cambiar a "EN_TRABAJO"
//   ↓
//Sistema revisa: TransicionesValidas["COTIZADA"] = [APROBADA, CANCELADA]
//   ↓
//"EN_TRABAJO" NO está en la lista
//   ↓
//❌ ERROR: "No puede saltar de COTIZADA a EN_TRABAJO"
//```
//```
//OS está en "COTIZADA"
//   ↓
//Usuario quiere cambiar a "APROBADA"
//   ↓
//Sistema revisa: TransicionesValidas["COTIZADA"] = [APROBADA, CANCELADA]
//   ↓
//"APROBADA" SÍ está en la lista
//   ↓
//✅ OK: Cambio permitido
//```

//---

//## Diagrama visual:
//```
//ABIERTA 
//    ↓
//DIAGNOSTICO 
//    ↓
//COTIZADA 
//    ↓
//APROBADA 
//    ↓
//EN_TRABAJO ←→ PAUSADA
//    ↓
//COMPLETADA 
//    ↓
//FACTURADA 
//    ↓
//CERRADA ✓ (fin)

//Cualquier estado (excepto COMPLETADA, FACTURADA, CERRADA) → CANCELADA ✓ (fin)
//```

//---

//## ¿Por qué es útil?

//Evita errores como:
//```
//❌ Saltar de ABIERTA → FACTURADA (sin hacer el trabajo)
//❌ Cambiar de CERRADA → ABIERTA (reabrir OS cerrada)
//❌ Ir de COMPLETADA → DIAGNOSTICO (retroceder)