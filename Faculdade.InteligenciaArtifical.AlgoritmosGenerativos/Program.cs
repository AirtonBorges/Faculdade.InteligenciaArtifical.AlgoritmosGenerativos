using System.Globalization;

var linhas = File.ReadAllLines("cotacoes_b3_202_05.csv");
var cotacoes = new List<Cotacao>();

for (int i = 1; i < linhas.Length; i++) {
    var partes = linhas[i].Split(';');
    var data = DateTime.Parse(partes[0]);
    var codigo = partes[1];
    if (codigo.Length != 5) continue;
    var preco = double.Parse(partes[2].Replace(",", "."), CultureInfo.InvariantCulture);
    cotacoes.Add(new Cotacao { Data = data, Codigo = codigo, Preco = preco });
}

var diasUnicos = cotacoes.Select(c => c.Data).Distinct().OrderBy(d => d).ToList();
double capital = 1000;
var dna = GerarDnaAleatorio(12, 10, cotacoes);

for (int i = 0; i < dna.Count; i++) {
    var compraDia = diasUnicos[i * 2];
    var vendaDia = diasUnicos[i * 2 + 1];
    var cotacoesCompra = cotacoes.Where(c => c.Data == compraDia).ToDictionary(c => c.Codigo, c => c.Preco);
    var cotacoesVenda = cotacoes.Where(c => c.Data == vendaDia).ToDictionary(c => c.Codigo, c => c.Preco);

    double pote = capital / 10;
    double novoCapital = 0;

    foreach (var codigo in dna[i]) {
        if (cotacoesCompra.ContainsKey(codigo) && cotacoesVenda.ContainsKey(codigo)) {
            var precoCompra = cotacoesCompra[codigo];
            var precoVenda = cotacoesVenda[codigo];
            var qtd = pote / precoCompra;
            novoCapital += qtd * precoVenda;
        } else {
            novoCapital += pote;
        }
    }

    capital = novoCapital;
}

Console.WriteLine($"Capital final: R$ {capital:F2}");


List<List<string>> GerarDnaAleatorio(int grupos, int potes, List<Cotacao> cotacoes) {
    var random = new Random();
    var acoes = cotacoes.Select(c => c.Codigo).Distinct().ToList();
    var dna = new List<List<string>>();
    for (int i = 0; i < grupos; i++) {
        var grupo = new List<string>();
        for (int j = 0; j < potes; j++) {
            var codigo = acoes[random.Next(acoes.Count)];
            grupo.Add(codigo);
        }
        dna.Add(grupo);
    }
    return dna;
}

class Cotacao {
    public DateTime Data { get; set; }
    public string Codigo { get; set; }
    public double Preco { get; set; }
}