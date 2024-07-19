using Microsoft.Data.SqlClient;
using Progetto.Models;
using Progetto.Service;

namespace Progetto.DAO
{
    public class TrasgressoreDao : SqlServerServiceBase, ITrasgressoreDao
    {
        private const string SELECT_ALL_TRASGRESSORI = @"SELECT * FROM Anagrafica;
        ";

        // Anagrafare nuovi trasgressori
        private const string INSERT_TRASGRESSORE = @"
        INSERT INTO Anagrafica (Nome, Cognome, Indirizzo, Citta, CAP, Cod_Fiscale) 
        VALUES (@Nome, @Cognome, @Indirizzo, @Citta, @CAP, @CodFiscale);
        SELECT SCOPE_IDENTITY();";

        public TrasgressoreDao(IConfiguration configuration) : base(configuration)
        {
        }


        public IEnumerable<Anagrafica> GetAll()
        {
            var result = new List<Anagrafica>();
            try
            {
                using var conn = CreateConnection();
                conn.Open();
                using var cmd = GetCommand(SELECT_ALL_TRASGRESSORI, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Anagrafica
                    {
                        Id = reader.GetInt32(0),
                        Cognome = reader.GetString(1),
                        Nome = reader.GetString(2),
                        Indirizzo = reader.GetString(3),
                        Citta = reader.GetString(4),
                        CAP = reader.GetString(5),
                        CodFiscale = reader.GetString(6),
                    });
                }
            }
            catch (Exception ex) 
            {
                throw new Exception("Errore", ex);
            }
            return result;
        }
        public void CreateTrasgressore(Anagrafica anagrafica)
        {
            try
            {
                using var conn = CreateConnection();
                conn.Open();
                using var cmd = GetCommand(INSERT_TRASGRESSORE, conn);
                cmd.Parameters.Add(new SqlParameter("@Nome", anagrafica.Nome));
                cmd.Parameters.Add(new SqlParameter("@Cognome", anagrafica.Cognome));
                cmd.Parameters.Add(new SqlParameter("@Indirizzo", anagrafica.Indirizzo));
                cmd.Parameters.Add(new SqlParameter("@Citta", anagrafica.Citta));
                cmd.Parameters.Add(new SqlParameter("@CAP", anagrafica.CAP));
                cmd.Parameters.Add(new SqlParameter("@CodFiscale", anagrafica.CodFiscale));

                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    anagrafica.Id = Convert.ToInt32(result);
                }
                else
                {
                    throw new Exception("Trasgressore non inserito");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'inserimento del trasgressore.", ex);
            }
        }


    }
}
