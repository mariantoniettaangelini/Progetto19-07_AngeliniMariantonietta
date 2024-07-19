namespace Progetto.Models
{
    public class ViolazioniCostose
    {
        public int IdVerbale { get; set; }
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public DateTime DataViolazione { get; set; }
        public decimal Importo { get; set; }
    }
}
