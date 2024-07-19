using Progetto.Models;

namespace Progetto.DAO
{
    public interface ITrasgressoreDao
    {
        IEnumerable<Anagrafica> GetAll();
        void CreateTrasgressore(Anagrafica anagrafica);
    }
}
