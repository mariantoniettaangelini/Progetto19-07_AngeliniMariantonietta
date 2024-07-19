using Progetto.Models;

namespace Progetto.DAO
{
    public interface IVerbaleDao
    {
        IEnumerable<Verbale> GetAllMulte();
        void Create(Verbale verbale);
        public IEnumerable<MultePerTrasgressore> GetTotaleMultePerTrasgressore(); // Per il totale dei verbali trascritti raggruppati per trasgressore
        IEnumerable<PuntiDecurtatiPerTrasgressore> GetTotPuntiPerTrasgressore(); // Totale dei punti decurtati raggruppati per trasgressore
        IEnumerable<ViolazioneDieciPunti> GetViolazioniConDieciPunti(); // Totale delle violazioni che superano i 10 punti
        IEnumerable<ViolazioniCostose> GetViolazioni400(); // Totale delle violazioni il cui importo è maggiore di 400€

    }
}
