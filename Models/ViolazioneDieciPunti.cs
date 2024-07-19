namespace Progetto.Models
{
    public class ViolazioneDieciPunti
    {
        public int IdVerbale { get; set; }
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public DateTime DataViolazione { get; set; }
        public decimal Importo { get; set; }
        public int PuntiDecurtati { get; set; }
    }
}
