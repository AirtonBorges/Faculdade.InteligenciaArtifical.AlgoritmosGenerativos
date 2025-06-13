namespace Faculdade.InteligenciaArtifical.AlgoritmosGenerativos.Models;

public class TradeDna
{
    public TradeDna(List<string> dnaPai, List<string> dnaMae)
    {
        CodigosGenes = new List<string>();
        CodigosGenes.AddRange(dnaPai);
        CodigosGenes.AddRange(dnaMae);
    }

    public TradeDna()
    {
        
    }

    public List<string> CodigosGenes { get; set; } = new List<string>();
    public int? ValorFinal { get; set; }
}
