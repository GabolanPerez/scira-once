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
    public class ReporteCambiosMacroProcesosController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: ReporteCambiosMRyC
        public ActionResult Index()
        {
            var user = ((IdentityPersonalizado)User.Identity);
            var mps = Utilidades.Utilidades.RTCObject(db.c_usuario.Find(user.Id_usuario), db, "c_macro_proceso").Cast<c_macro_proceso>();
            ViewBag.mps = mps.Where(c => c.r_macro_proceso.Count > 0).ToList();

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