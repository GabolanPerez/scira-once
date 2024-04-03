using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "RepCES", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ReporteCertificacionSubProcesoController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            var user = (IdentityPersonalizado)User.Identity;
            var model = Utilidades.Utilidades.RTCCertificacionEstructura(db.c_usuario.Find(user.Id_usuario), db, "S");

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