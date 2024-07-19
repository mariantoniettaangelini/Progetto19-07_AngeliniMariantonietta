namespace Progetto.Models
{
    public class Verbale
    {
        public int IdVerbale { get; set; }
        public DateTime? DataViolazione { get; set; }
        public string IndirizzoViolazione { get; set; }
        public string NominativoAgente { get; set; }
        public DateTime? DataTrascrizioneVerbale { get; set; }
        public decimal Importo { get; set; }
        public int IdAnagrafica { get; set; }
        public Anagrafica Anagrafica { get; set; }  
        public int IdViolazione { get; set; }
        public TipoVerbale TipoViolazione { get; set; }
    }
}
