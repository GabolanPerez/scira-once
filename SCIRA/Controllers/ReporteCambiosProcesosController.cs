using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "RepCambios", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ReporteCambiosProcesosController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: ReporteCambiosMRyC
        public ActionResult Index()
        {
            var user = ((IdentityPersonalizado)User.Identity);
            var prs = Utilidades.Utilidades.RTCObject(db.c_usuario.Find(user.Id_usuario), db, "c_proceso").Cast<c_proceso>();
            ViewBag.prs = prs.Where(c => c.r_proceso.Count > 0).ToList();

            return View();
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