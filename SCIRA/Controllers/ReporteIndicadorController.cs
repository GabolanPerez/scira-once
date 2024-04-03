using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "RepIndicadores", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ReporteIndicadorController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: ReporteGeneralBDEI
        public ActionResult Index()
        {
            var user = ((IdentityPersonalizado)User.Identity);
            var indicadores = Utilidades.Utilidades.RTCObject(db.c_usuario.Find(user.Id_usuario), db, "c_indicador").Cast<c_indicador>();

            List<k_evaluacion> evals = new List<k_evaluacion>();

            foreach (var ind in indicadores)
            {
                evals.AddRange(ind.k_evaluacion.ToList());
            }

            return View(evals);
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