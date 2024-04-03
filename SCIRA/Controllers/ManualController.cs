using SCIRA.Models;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = null, ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ManualController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: TipoSolucion
        public ActionResult Index()
        {
            return View();
        }

        public FileResult DisplayPDF()
        {
            
            string path = "~/Content/"+ Strings.getMSG("rutamanual");
            var manual = File(path, "application/pdf");
            return manual;
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
