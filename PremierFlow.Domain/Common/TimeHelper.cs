using System;

namespace PremierFlow.Domain.Common
{
    /// <summary>
    /// Helper centralizado de zona horaria.
    /// Siempre devuelve hora local de Honduras (UTC-6), sin importar
    /// la zona horaria del servidor donde se despliegue la aplicación.
    /// </summary>
    public static class TimeHelper
    {
        private static readonly TimeZoneInfo HondurasZone =
            TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");

        /// <summary>
        /// Fecha y hora actual en Honduras.
        /// </summary>
        public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, HondurasZone);

        /// <summary>
        /// Fecha actual en Honduras (sin hora).
        /// </summary>
        public static DateTime Today => Now.Date;
    }
}
