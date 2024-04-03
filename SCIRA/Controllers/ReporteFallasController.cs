using SCIRA.Models;
using SCIRA.Validaciones;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "RepFallas", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ReporteFallasController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: ReporteGeneralBDEI
        public ActionResult Index()
        {
            var vista = db.h_excepcion.OrderByDescending(e => e.fe_excepcion).ToList();
            return View(vista);
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