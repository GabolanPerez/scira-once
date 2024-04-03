using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "RepRR", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ReporteRRController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            var user = ((IdentityPersonalizado)User.Identity);
            var controles = Utilidades.Utilidades.RTCObject(db.c_usuario.Find(user.Id_usuario), db, "k_control", "1").Cast<k_control>();


            List<k_control> conRR = new List<k_control>();
            List<k_control> sinRR = new List<k_control>();


            foreach (var control in controles)
            {
                //evitar que se usen controles sin riesgo
                if (control.k_riesgo.Count() > 0)
                {
                    if (control.k_riesgo_residual.Count() > 0)
                    {
                        conRR.Add(control);
                    }
                    else
                    {
                        string prefijo = control.c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso.Substring(0, 2);
                        if (prefijo == "MP")
                            sinRR.Add(control);
                    }
                }
            }

            ViewBag.CRR = conRR;
            ViewBag.SRR = sinRR;
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