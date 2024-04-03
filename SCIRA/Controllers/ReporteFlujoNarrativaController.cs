using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "RepFN", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ReporteFlujoNarrativaController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: ReporteFlujoNarrativa
        public ActionResult Index()
        {
            var user = ((IdentityPersonalizado)User.Identity);
            var sps = Utilidades.Utilidades.RTCObject(db.c_usuario.Find(user.Id_usuario), db, "c_sub_proceso").Cast<c_sub_proceso>();

            return View(sps);
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