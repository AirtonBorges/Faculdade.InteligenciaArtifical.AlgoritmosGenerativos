using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Faculdade.InteligenciaArtifical.AlgoritmosGenerativos.Factories;
using Faculdade.InteligenciaArtifical.AlgoritmosGenerativos.Models;

const int quantidadeDias = 24;
const int quantidadeAcoesPorDia = 10;
const int quantidadeGenes = (quantidadeDias / 2) * quantidadeAcoesPorDia;
const int montanteInicialCentavos = 100000;

var csvReader = new CsvReader(
    new StreamReader("cotacoes_b3_202_05.csv"),
    new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" }
);

csvReader.Context.RegisterClassMap<CotacaoMap>();
var cotacoes = csvReader.GetRecords<Cotacao>()
    .Where(p => p.DataHora.HasValue)
    .OrderBy(p => p.DataHora)
    .ToList()
;

var poolGenes = Enumerable.Range(0, 20)
    .Select(_ => TradeDnaFactory.DnaTradeFinalPorCotacoes(cotacoes, quantidadeGenes))
    .ToList();

for (int i = 0; i < 100; i++)
{
    Console.WriteLine($"\nGerando nova geração {i + 1}...");
    poolGenes = AlgoritmoGenetico(poolGenes, cotacoes, montanteInicialCentavos);
}

return;

List<TradeDna> AlgoritmoGenetico(List<TradeDna> tradeDnas
    , List<Cotacao> pCotacoes
    , int pMontanteInicialCentavos
)
{
    var individuos = tradeDnas
        .Select(p => AvaliarTradeDna(p, pCotacoes, pMontanteInicialCentavos))
        .ToList();

    var melhores = individuos
        .OrderByDescending(p => p.ValorFinal)
        .Take(quantidadeAcoesPorDia)
        .ToList()
    ;

    Console.WriteLine($"Melhor indivíduo: {melhores.Max(p => p.ValorFinal / 100):F2}");

    Console.WriteLine($"Top {melhores.Count} indivíduos selecionados");
    var pares = SelecionarPares(melhores);
    Console.WriteLine($"Gerando {pares.Count} filhos...");

    var filhos = pares
        .Select(ObterFilho)
        .Select(Mutar)
        .ToList()
    ;

    return filhos;
}

static TradeDna AvaliarTradeDna(TradeDna tradeDna, List<Cotacao> cotacoes, int montanteInicialCentavos)
{
    var valorFinal = montanteInicialCentavos;
    var diaZero = cotacoes.Min(p => p.DataHora!.Value);
    var quantidadeAcoesProcessadas = 0;

    for (var dia = 0; dia < 24; dia++)
    {
        var acoesDoGene = tradeDna.CodigosGenes
            .Skip(quantidadeAcoesProcessadas)
            .Take(quantidadeAcoesPorDia)
            .ToList()
        ;

        var dataInicio = diaZero.AddDays(dia);
        var dataFim = dataInicio.AddDays(1);

        var acoesDoDia = cotacoes
            .Where(p => p.DataHora >= dataInicio && p.DataHora <= dataFim)
            .Where(p => acoesDoGene.Contains(p.CodigoAcao))
            .ToList()
        ;

        foreach (var acao in acoesDoDia)
        {
            if (dia % 2 == 0)
                valorFinal += acao.FechamentoPrecoCentavos;
            else
                valorFinal -= acao.FechamentoPrecoCentavos;
        }

        quantidadeAcoesProcessadas += quantidadeAcoesPorDia;
    }

    tradeDna.ValorFinal = valorFinal;
    return tradeDna;
}

static TradeDna Mutar(TradeDna tradeDna)
{
    var genes = tradeDna.CodigosGenes;
    var i1 = Random.Shared.Next(genes.Count);
    var i2 = Random.Shared.Next(genes.Count);

    (genes[i1], genes[i2]) = (genes[i2], genes[i1]);

    tradeDna.CodigosGenes = genes;
    return tradeDna;
}

static TradeDna ObterFilho((TradeDna pai, TradeDna mae) par)
{
    var corte = Random.Shared.Next(quantidadeGenes / 2, (quantidadeGenes / 2) + 40);
    var dnaMae = par.mae.CodigosGenes.Take(corte).ToList();
    var dnaPai = par.pai.CodigosGenes.Skip(corte).ToList();
    return new TradeDna(dnaPai, dnaMae);
}

List<(TradeDna, TradeDna)> SelecionarPares(List<TradeDna> individuos)
{
    var pares = new List<(TradeDna, TradeDna)>();
    var total = individuos.Count;

    for (int i = 0; i < total; i++)
    {
        for (int j = i + 1; j < total; j++)
        {
            pares.Add((individuos[i], individuos[j]));
            pares.Add((individuos[j], individuos[i]));
        }
    }

    var aleatorios = new Random();
    for (int i = 0; i < total; i++)
    {
        var pai = individuos[aleatorios.Next(total)];
        var mae = individuos[aleatorios.Next(total)];
        if (pai != mae)
        {
            pares.Add((pai, mae));
        }
    }

    return pares;
}
