using Faculdade.InteligenciaArtifical.AlgoritmosGenerativos.Models;

namespace Faculdade.InteligenciaArtifical.AlgoritmosGenerativos.Factories;

public static class TradeDnaFactory
{
    public static TradeDna DnaTradeFinalPorCotacoes(List<Cotacao> cotacoes
        , int quantidadeGenes)
    {
        var codigoAcoes = cotacoes.DistinctBy(c => c.CodigoAcao)
                .Select(p => p.CodigoAcao)
                .ToList()
            ;
        
        var dnaAcoesOperacao = new TradeDna();
        var codigosGenes = new List<string>();

        for (var i = 0; i < quantidadeGenes; i++)
        {
            var indiceAleatorio = Random.Shared.Next(0, codigoAcoes.Count);
            codigosGenes.Add(codigoAcoes[indiceAleatorio]);
        }
        
        dnaAcoesOperacao.CodigosGenes = codigosGenes;
        
        return dnaAcoesOperacao;
    } 
}