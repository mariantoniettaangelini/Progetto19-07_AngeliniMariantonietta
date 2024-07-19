namespace Progetto.Models
{
    public class TipoVerbale
    {
        public int IdViolazione { get; set; }
        public string Descrizione { get; set; }
        public IEnumerable<Verbale> Verbali { get; set; }
    }
}
