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
    public class ReporteCambiosSubProcesosController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: ReporteCambiosMRyC
        public ActionResult Index()
        {
            var user = ((IdentityPersonalizado)User.Identity);
            c_usuario Us = db.c_usuario.Find(user.Id_usuario);

            var sps = Utilidades.Utilidades.RTCObject(Us, db, "c_sub_proceso").Cast<c_sub_proceso>();

            
            ViewBag.sps = sps.Where(c => c.r_sub_proceso.Count > 0).ToList();
            ViewBag.CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);

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