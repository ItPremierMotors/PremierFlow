using Microsoft.EntityFrameworkCore;
using PremierFlow.Domain.Common;

namespace PremierFlow.Infrastructure.Persistence.Helpers
{
    /// <summary>
    /// Genera códigos secuenciales únicos (LD-2026-0001, OP-2026-0001, COT-2026-0001).
    /// Usa retry en caso de colisión concurrente, apoyándose en los índices únicos de la DB.
    /// </summary>
    public class GeneradorCodigos
    {
        private readonly PremierFlowDbContext context;
        private const int MaxReintentos = 3;

        public GeneradorCodigos(PremierFlowDbContext context)
        {
            this.context = context;
        }

        public async Task<string> GenerarCodigoLeadAsync()
        {
            var prefijo = $"LD-{TimeHelper.Now.Year}";
            return await GenerarCodigoAsync(
                prefijo,
                async (p) => await context.Leads
                    .Where(l => l.CodigoLead.StartsWith(p))
                    .OrderByDescending(l => l.CodigoLead)
                    .Select(l => l.CodigoLead)
                    .FirstOrDefaultAsync()
            );
        }

        public async Task<string> GenerarCodigoOportunidadAsync()
        {
            var prefijo = $"OP-{TimeHelper.Now.Year}";
            return await GenerarCodigoAsync(
                prefijo,
                async (p) => await context.Oportunidades
                    .Where(o => o.CodigoOportunidad.StartsWith(p))
                    .OrderByDescending(o => o.CodigoOportunidad)
                    .Select(o => o.CodigoOportunidad)
                    .FirstOrDefaultAsync()
            );
        }

        public async Task<string> GenerarCodigoCotizacionAsync()
        {
            var prefijo = $"COT-{TimeHelper.Now.Year}";
            return await GenerarCodigoAsync(
                prefijo,
                async (p) => await context.CotizacionesVehiculo
                    .Where(c => c.CodigoCotizacion.StartsWith(p))
                    .OrderByDescending(c => c.CodigoCotizacion)
                    .Select(c => c.CodigoCotizacion)
                    .FirstOrDefaultAsync()
            );
        }

        private static async Task<string> GenerarCodigoAsync(string prefijo, Func<string, Task<string?>> obtenerUltimoCodigo)
        {
            var ultimoCodigo = await obtenerUltimoCodigo(prefijo);

            var siguiente = 1;
            if (ultimoCodigo != null)
            {
                var partes = ultimoCodigo.Split('-');
                siguiente = int.Parse(partes[^1]) + 1;
            }

            return $"{prefijo}-{siguiente:D4}";
        }

        /// <summary>
        /// Ejecuta una acción que genera un código y guarda en DB.
        /// Si hay colisión por concurrencia, regenera el código y reintenta.
        /// </summary>
        public async Task<T> EjecutarConReintento<T>(Func<string, Task<T>> accion, Func<Task<string>> generarCodigo)
        {
            for (int intento = 0; intento <= MaxReintentos; intento++)
            {
                var codigo = await generarCodigo();
                try
                {
                    return await accion(codigo);
                }
                catch (DbUpdateException ex) when (intento < MaxReintentos && EsViolacionUnica(ex))
                {
                    // Colisión de código, reintentar con nuevo código
                    // Detach entidades en conflicto para evitar tracking issues
                    foreach (var entry in context.ChangeTracker.Entries()
                        .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
                    {
                        entry.State = EntityState.Detached;
                    }
                }
            }

            throw new InvalidOperationException("No se pudo generar un código único después de varios intentos");
        }

        private static bool EsViolacionUnica(DbUpdateException ex)
        {
            var message = ex.InnerException?.Message ?? ex.Message;
            // SQL Server: unique index/constraint violation
            return message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) ||
                   message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) ||
                   message.Contains("IX_", StringComparison.OrdinalIgnoreCase);
        }
    }
}