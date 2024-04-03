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
    public class ReporteCambiosMRyCController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: ReporteCambiosMRyC
        public ActionResult Index()
        {
            var user = ((IdentityPersonalizado)User.Identity);

            //var riesgos = Utilidades.Utilidades.RTCRiesgo(db.c_usuario.Find(user.Id_usuario), db,sps);
            //var controles = Utilidades.Utilidades.RTCControl(db.c_usuario.Find(user.Id_usuario), db,sps);

            var riesgos = Utilidades.Utilidades.RTCRiesgo(db.c_usuario.Find(user.Id_usuario), db);
            var controles = Utilidades.Utilidades.RTCObject(db.c_usuario.Find(user.Id_usuario), db, "k_control", "1").Cast<k_control>();

            List<k_control> c2 = Utilidades.Utilidades.GetLinkedControls(controles.ToList());

            ViewBag.Riesgos = riesgos.Where(r => r.r_riesgo.Count > 0).ToList();
            ViewBag.Controles = c2.Where(c => c.r_control.Count > 0).ToList();

            ViewBag.CamposExtraControl = Utilidades.Utilidades.infoCamposExtra("k_control", 20);
            ViewBag.CamposExtraRiesgo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);

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