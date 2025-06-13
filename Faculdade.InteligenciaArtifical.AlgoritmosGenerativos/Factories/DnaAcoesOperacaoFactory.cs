using Faculdade.InteligenciaArtifical.AlgoritmosGenerativos.Models;

namespace Faculdade.InteligenciaArtifical.AlgoritmosGenerativos;

public static class DnaTradeFinalFactory
{
    public static DnaTradeFinal DnaTradeFinalPorCotacoes(List<Cotacao> cotacoes
        , int quantidadeGenes)
    {
        var codigoAcoes = cotacoes.DistinctBy(c => c.CodigoAcao)
                .Select(p => p.CodigoAcao)
                .ToList()
            ;
        
        var dnaAcoesOperacao = new DnaTradeFinal();
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