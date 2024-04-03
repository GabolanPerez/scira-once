using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "RepBenchmark", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ReporteBenchmarkController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: ReporteBenchmark
        public ActionResult Index()
        {
            var user = (IdentityPersonalizado)User.Identity;
            var benchmarks = Utilidades.Utilidades.RTCBenchmark(db.c_usuario.Find(user.Id_usuario), db);

            return View(benchmarks);
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