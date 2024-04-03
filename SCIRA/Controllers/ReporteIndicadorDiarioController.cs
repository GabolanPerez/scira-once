using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "RepINDD", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ReporteIndicadorDiarioController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: ReporteGeneralBDEI
        public ActionResult Index()
        {
            var user = ((IdentityPersonalizado)User.Identity);
            var us = db.c_usuario.Find(user.Id_usuario);

            var model = Utilidades.Utilidades.RTCObject(us, db, "k_evaluacion_diaria").Cast<k_evaluacion_diaria>().ToList();

            return View(model);
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