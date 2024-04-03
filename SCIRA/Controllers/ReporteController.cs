using SCIRA.Models;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    public class ReporteController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: Reporte/Estructura
        public ActionResult Estructura()
        {
            return View(db.v_estructura.ToList());
        }

        // GET: Reporte/MRyC
        public ActionResult MRyC()
        {
            return View(db.v_mryc.ToList());
        }

        // GET: Reporte/FlujoNarrativa
        public ActionResult FlujoNarrativa()
        {
            return View(db.v_flujo_narrativa.ToList());
        }

        // GET: Reporte/General BDEI
        public ActionResult GeneralBDEI()
        {
            return View(db.v_bdei.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}