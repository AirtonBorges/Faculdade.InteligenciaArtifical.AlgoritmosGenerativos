using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace Faculdade.InteligenciaArtifical.AlgoritmosGenerativos.Models;

public class Cotacao {
    [Name("Data")]
    public DateTime? DataHora { get; set; }
    [Name("Codigo")]
    public string CodigoAcao { get; set; }
    [Name("Fechamento")]
    public int FechamentoPrecoCentavos { get; set; }
}

public sealed class CotacaoMap: ClassMap<Cotacao>
{
    public CotacaoMap()
    {
        Map(p => p.CodigoAcao).Name("Codigo");
        Map(p => p.FechamentoPrecoCentavos).Convert(p =>
        {
            var field = p.Row.GetField("Fechamento");
            
            if (string.IsNullOrWhiteSpace(field))
            {
                return 0;
            }

            var fechamento = double.Parse(field);
            var retorno = (int)(fechamento * 100);
            return retorno;
        });
        Map(p => p.DataHora).Convert(p =>
        {
            var field = p.Row.GetField("Data");

            if (string.IsNullOrWhiteSpace(field))
                return null;

            var parsedDate = DateTime.ParseExact(field, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            var dataHora = TimeZoneInfo.ConvertTime(parsedDate, timeZone);

            return dataHora;
        });
    }
}