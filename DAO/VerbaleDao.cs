using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Progetto.Models;
using Progetto.Service;

namespace Progetto.DAO
{
    public class VerbaleDao : SqlServerServiceBase, IVerbaleDao
    {
        private const string SELECT_ALL_VERBALI = @"
            SELECT v.IdVerbale, v.DataViolazione, v.IndirizzoViolazione, v.NominativoAgente, v.DataTrascrizioneVerbale, v.Importo, 
                   a.Cognome, a.Nome, a.Indirizzo, a.Citta, a.CAP, a.Cod_Fiscale, 
                   t.Descrizione
            FROM Verbale as v 
            JOIN Anagrafica as a ON v.IdAnagrafica = a.Id 
            JOIN [Tipo Violazione] as t ON v.IdViolazione = t.IdViolazione";

        private const string INSERT_VERBALE = @"
            INSERT INTO Verbale (DataViolazione, IndirizzoViolazione, NominativoAgente, DataTrascrizioneVerbale, Importo, IdAnagrafica, IdViolazione) 
            OUTPUT INSERTED.IdVerbale 
            VALUES (@DataViolazione, @IndirizzoViolazione, @NominativoAgente, @DataTrascrizioneVerbale, @Importo, @IdAnagrafica, @IdViolazione)";

        // Totale dei verbali trascritti raggruppati per trasgressore
        private const string SELECT_TOTALE_VERBALI_PER_TRASGRESSORE = @"
        SELECT a.Id, a.Cognome, a.Nome, SUM(v.Importo) AS TotalImporto
        FROM Verbale AS v
        JOIN Anagrafica AS a ON v.IdAnagrafica = a.Id
        GROUP BY a.Id, a.Cognome, a.Nome";

        // Totale dei punti decurtati raggruppati per trasgressore
        private const string SELECT_TOT_PUNTI_PER_TRASGRESSORE = @"
        SELECT a.Id, a.Cognome, a.Nome, SUM(v.DecurtamentoPunti) AS TotalPuntiDecurtati
        FROM Verbale AS v
        JOIN Anagrafica AS a ON v.IdAnagrafica = a.Id
        GROUP BY a.Id, a.Cognome, a.Nome";

        // Totale delle violazioni che superano i 10 punti
        private const string SELECT_VIOLAZIONI_DIECI_PUNTI = @" 
        SELECT v.IdVerbale, a.Cognome, a.Nome, v.DataViolazione, v.Importo, v.DecurtamentoPunti
        FROM Verbale AS v
        JOIN Anagrafica AS a ON v.IdAnagrafica = a.Id
        WHERE v.DecurtamentoPunti > 10";

        // Totale delle violazioni il cui importo è maggiore di 400€
        private const string SELECT_VIOLAZIONI_400 = @"
        SELECT v.IdVerbale, a.Cognome, a.Nome, v.DataViolazione, v.Importo, v.DecurtamentoPunti
        FROM Verbale AS v
        JOIN Anagrafica AS a ON v.IdAnagrafica = a.Id
        WHERE v.Importo > 400";
        public VerbaleDao(IConfiguration config) : base(config)
        {
        }

        public IEnumerable<Verbale> GetAllMulte()
        {
            var result = new List<Verbale>();
            try
            {
                using var conn = CreateConnection();
                conn.Open();
                using var cmd = GetCommand(SELECT_ALL_VERBALI, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Verbale
                    {
                        IdVerbale = reader.GetInt32(0),
                        DataViolazione = reader.GetDateTime(1),
                        IndirizzoViolazione = reader.GetString(2),
                        NominativoAgente = reader.GetString(3),
                        DataTrascrizioneVerbale = reader.GetDateTime(4),
                        Importo = reader.GetDecimal(5),
                        Anagrafica = new Anagrafica
                        {
                            Cognome = reader.GetString(6),
                            Nome = reader.GetString(7),
                            Indirizzo = reader.GetString(8),
                            Citta = reader.GetString(9),
                            CAP = reader.GetString(10),
                            CodFiscale = reader.GetString(11),
                        },
                        TipoViolazione = new TipoVerbale
                        {
                            Descrizione = reader.GetString(12)
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore", ex);
            }
            return result;
        }

        public void Create(Verbale verbale)
        {
            try
            {
                using var conn = CreateConnection();
                conn.Open();
                using var cmd = GetCommand(INSERT_VERBALE, conn);
                cmd.Parameters.Add(new SqlParameter("@DataViolazione", verbale.DataViolazione));
                cmd.Parameters.Add(new SqlParameter("@IndirizzoViolazione", verbale.IndirizzoViolazione));
                cmd.Parameters.Add(new SqlParameter("@NominativoAgente", verbale.NominativoAgente));
                cmd.Parameters.Add(new SqlParameter("@DataTrascrizioneVerbale", verbale.DataTrascrizioneVerbale));
                cmd.Parameters.Add(new SqlParameter("@Importo", verbale.Importo));
                cmd.Parameters.Add(new SqlParameter("@IdAnagrafica", verbale.IdAnagrafica));
                cmd.Parameters.Add(new SqlParameter("@IdViolazione", verbale.IdViolazione));
                var result = cmd.ExecuteScalar();
                if (result == null)
                    throw new Exception("Verbale non inserito");
                verbale.IdVerbale = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore.", ex);
            }
        }

        // Totale dei verbali trascritti raggruppati per trasgressore
        public IEnumerable<MultePerTrasgressore> GetTotaleMultePerTrasgressore()
        {
            var result = new List<MultePerTrasgressore>();
            try
            {
                using var conn = CreateConnection();
                conn.Open();
                using var cmd = GetCommand(SELECT_TOTALE_VERBALI_PER_TRASGRESSORE, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new MultePerTrasgressore
                    {
                        IdAnagrafica = reader.GetInt32(0),
                        Cognome = reader.GetString(1),
                        Nome = reader.GetString(2),
                        TotalImporto = reader.GetDecimal(3)
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante il recupero dei totali per trasgressore.", ex);
            }
            return result;
        }

        //Totale dei punti decurtati raggruppati per trasgressore
        public IEnumerable<PuntiDecurtatiPerTrasgressore>GetTotPuntiPerTrasgressore()
        {
            var result = new List<PuntiDecurtatiPerTrasgressore>();
            try
            {
                using var conn = CreateConnection();
                conn.Open();
                using var cmd = GetCommand(SELECT_TOT_PUNTI_PER_TRASGRESSORE, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new PuntiDecurtatiPerTrasgressore
                    {
                        IdAnagrafica = reader.GetInt32(0),
                        Cognome = reader.GetString(1),
                        Nome = reader.GetString(2),
                        TotalPuntiDecurtati = reader.GetInt32(3)
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante il recupero dei totali dei punti decurtati per trasgressore.", ex);
            }
            return result;
        }

        // Totale delle violazioni che superano i 10 punti
        public IEnumerable<ViolazioneDieciPunti> GetViolazioniConDieciPunti()
        {
            var result = new List<ViolazioneDieciPunti>();
            try
            {
                using var conn = CreateConnection();
                conn.Open();
                using var cmd = GetCommand(SELECT_VIOLAZIONI_DIECI_PUNTI, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new ViolazioneDieciPunti
                    {
                        IdVerbale = reader.GetInt32(0),
                        Cognome = reader.GetString(1),
                        Nome = reader.GetString(2),
                        DataViolazione = reader.GetDateTime(3),
                        Importo = reader.GetDecimal(4),
                        PuntiDecurtati = reader.GetInt32(5)
                    });
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Errore durante il recupero delle violazioni che superano i 10 punti decurtati", ex);
            }
            return result;
        }

        // Totale delle violazioni il cui importo è maggiore di 400€
        public IEnumerable<ViolazioniCostose> GetViolazioni400()
        {
            var result = new List<ViolazioniCostose>();
            try
            {
                using var conn = CreateConnection();
                conn.Open();
                using var cmd = GetCommand(SELECT_VIOLAZIONI_400, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new ViolazioniCostose
                    {
                        IdVerbale = reader.GetInt32(0),
                        Cognome = reader.GetString(1),
                        Nome = reader.GetString(2),
                        DataViolazione = reader.GetDateTime(3),
                        Importo = reader.GetDecimal(4),
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante il recupero delle violazioni che superano i 400€", ex);
            }
            return result;
        }
    }

}

