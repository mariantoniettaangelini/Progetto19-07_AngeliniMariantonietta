namespace Progetto.Models
{
    public class Anagrafica
    {
        public int Id { get; set; }
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public string Indirizzo { get; set; }
        public string Citta { get; set; }
        public string CAP { get; set; }
        public string CodFiscale { get; set; }
        public IEnumerable<Verbale> Verbali { get; set; }
    }
}
