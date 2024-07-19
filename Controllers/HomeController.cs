using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Progetto.DAO;
using Progetto.Models;
using System.Diagnostics;

namespace Progetto.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IVerbaleDao _verbaleDao;  
        private readonly ITrasgressoreDao _trasgressoreDao;


        public HomeController(ILogger<HomeController> logger, IVerbaleDao verbaleDao, ITrasgressoreDao trasgressoreDao)
        {
            _logger = logger;
            _verbaleDao = verbaleDao;
            _trasgressoreDao = trasgressoreDao;
        }

        // Sezione per la compilazione del verbale
        public IActionResult Create()
        {
            var verbale = new Verbale();
            ViewBag.Trasgressori = _trasgressoreDao.GetAll().Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = $"{a.Cognome} {a.Nome} - {a.CodFiscale}"
            }).ToList();
            return View(verbale);
        }

        [HttpPost]
        public IActionResult Create(Verbale verbale)
        {
            if (verbale == null)
                return BadRequest("Verbale non valido");

            _verbaleDao.Create(verbale);
            return RedirectToAction("Index");
        }

        // Sezione per anagrafare nuovi trasgressori
        public IActionResult CreateTrasgressore()
        {
            var anagrafica = new Anagrafica();
            return View(anagrafica);
        }

        [HttpPost]
        public IActionResult CreateTrasgressore(Anagrafica anagrafica)
        {
            if (anagrafica == null)
                return BadRequest("Trasgressore non valido");

            _trasgressoreDao.CreateTrasgressore(anagrafica);
            return RedirectToAction("Index");
        }
        
        public IActionResult Index()
        {
            var multe = _verbaleDao.GetAllMulte();
            var trasgressori = _trasgressoreDao.GetAll(); 
            ViewBag.Trasgressori = trasgressori;
            return View(multe);
        }



        // Totale dei verbali trascritti raggruppati per trasgressore
        public IActionResult MultePerTrasgressore()
        {
            try
            {
                var totali = _verbaleDao.GetTotaleMultePerTrasgressore();
                return View(totali);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei totali per trasgressore");
                return View("Error");
            }
        }

        // Totale dei punti decurtati raggruppati per trasgressore
        public IActionResult PuntiPerTrasgressore()
        {
            try
            {
                var totaliPunti = _verbaleDao.GetTotPuntiPerTrasgressore(); 
                return View(totaliPunti);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei totali dei punti decurtati per trasgressore");
                return View("Error");
            }
        }

        // Totale delle violazioni che superano i 10 punti
        public IActionResult ViolazioneDieciPunti()
        {
            try
            {
                var violazioneDieciPunti = _verbaleDao.GetViolazioniConDieciPunti();
                return View(violazioneDieciPunti);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle violazioni che superano i 10 punti decurtati");
                return View("Error");
            }
        }

        // Totale delle violazioni il cui importo è maggiore di 400€
        public IActionResult ViolazioniCostose()
        {
            try
            {
                var violazioniCostose = _verbaleDao.GetViolazioni400();
                return View(violazioniCostose);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle violazioni che superano i 400€");
                return View("Error");
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
