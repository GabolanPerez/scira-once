using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "RepPersonal", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ReportePersonalController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: ReporteGeneralBDEI
        public ActionResult Index()
        {
            var user = ((IdentityPersonalizado)User.Identity);
            var us = db.c_usuario.Find(user.Id_usuario);

            var model = new List<c_usuario>();

            if (user.Es_super_usuario)
            {
                model = db.c_usuario.ToList();
            }
            else
            {
                model = Utilidades.Utilidades.TramoControlInferior(us.id_usuario, db);
                model.Add(us);
            }

            return View(model);
        }

        public ActionResult DetailsMP(int id)
        {
            return PartialView("DetailViews/DetailsMP", db.c_usuario.Find(id).c_macro_proceso.ToList());
        }

        public ActionResult DetailsP(int id)
        {
            return PartialView("DetailViews/DetailsP", db.c_usuario.Find(id).c_proceso.ToList());
        }

        public ActionResult DetailsSP(int id)
        {
            return PartialView("DetailViews/DetailsSP", db.c_usuario.Find(id).c_sub_proceso.ToList());
        }

        public ActionResult DetailsCTR(int id)
        {
            return PartialView("DetailViews/DetailsCTR", db.c_usuario.Find(id).k_control1.ToList());
        }

        public ActionResult DetailsIND(int id)
        {
            return PartialView("DetailViews/DetailsIND", db.c_usuario.Find(id).c_indicador.ToList());
        }

        public ActionResult DetailsOFC(int id)
        {
            return PartialView("DetailViews/DetailsOFC", db.c_usuario.Find(id).k_objeto.Where(o => o.tipo_objeto == 1).ToList());
        }

        public ActionResult DetailsINF(int id)
        {
            return PartialView("DetailViews/DetailsINF", db.c_usuario.Find(id).k_objeto.Where(o => o.tipo_objeto == 2 || o.tipo_objeto == 3).ToList());
        }

        public ActionResult DetailsINC(int id)
        {
            return PartialView("DetailViews/DetailsINC", db.c_usuario.Find(id).k_incidencia.ToList());
        }

        public ActionResult DetailsPLN(int id)
        {
            return PartialView("DetailViews/DetailsPLN", db.c_usuario.Find(id).k_plan.ToList());
        }

        public ActionResult DetailsFIC(int id)
        {
            return PartialView("DetailViews/DetailsFIC", db.c_usuario.Find(id).r_evento.ToList());
        }

        public ActionResult DetailsTIME(int id)
        {
            return PartialView("DetailViews/DetailsTIME", db.c_usuario.Find(id).c_usuario_sub_proceso.ToList());
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