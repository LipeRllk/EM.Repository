using EM.Domain.Models;

namespace EM.Domain.Helpers
{
    public static class SexoHelper
    {
        public static EnumeradorSexo ParseOrDefault(string? valor, EnumeradorSexo padrao = EnumeradorSexo.Masculino) =>
            Enum.TryParse<EnumeradorSexo>(valor, ignoreCase: true, out var sexo) ? sexo : padrao;

        public static string ToStorageString(EnumeradorSexo sexo) => ((int)sexo).ToString();
    }
}