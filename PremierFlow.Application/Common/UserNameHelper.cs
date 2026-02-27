using System.Globalization;
using System.Text;

namespace PremierFlow.Application.Common
{
    public static class UserNameHelper
    {
        public static string GenerarUserName(string nombreCompleto)
        {
            if (string.IsNullOrWhiteSpace(nombreCompleto))
                return string.Empty;

            var partes = nombreCompleto
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (partes.Length < 2)
                return partes[0].Substring(0, 1).ToUpper();

            var inicialNombre = partes[0].Substring(0, 1);
            var apellido = partes.Length >= 3 ? partes[2] : partes[1];

            var username = inicialNombre + apellido;

            return QuitarAcentos(username).ToUpper();
        }

        private static string QuitarAcentos(string texto)
        {
            var normalized = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
