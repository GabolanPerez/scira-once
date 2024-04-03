using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "RepCertific", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ReporteCertificacionController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: ReporteGeneralBDEI
        public ActionResult Index()
        {
            var user = (IdentityPersonalizado)User.Identity;
            var model = Utilidades.Utilidades.RTCCertificacion(db.c_usuario.Find(user.Id_usuario), db);

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