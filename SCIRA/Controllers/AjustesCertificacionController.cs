using SCIRA.Models;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "AjustesCert", ModuleCode = "MSICI004")]
    [CustomErrorHandler]
    public class AjustesCertificacionController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: Actividad
        public ActionResult Index()
        {
            var model = new AjustesCertificacionViewModel();

            model.CertificacionSegura = Utilidades.Utilidades.GetBoolSecurityProp("CertificacionSegura", "true");
            model.LeyendaCertificacionE = Utilidades.Utilidades.GetSecurityProp("LeyendaCE", "Me he asegurado que los Macroprocesos que integran la " +
                "Entidad “XXXXX”, fueron certificados por cada uno " +
                "de sus Responsables y en el caso de existir incidencias " +
                "fueron señaladas oportunamente, además confirmo que " +
                "en esta Entidad no existen mas Macroprocesos relevantes " +
                "que no se hayan identificado y reflejado en la Matriz " +
                "de Riesgos y Controles correspondiente.");
            model.LeyendaCertificacionM = Utilidades.Utilidades.GetSecurityProp("LeyendaCM", "Me he asegurado que los Procesos que integran el " +
                "Macro Proceso “XXXXX”, fueron certificados por cada uno " +
                "de sus Responsables y en el caso de existir incidencias " +
                "fueron señaladas oportunamente, además confirmo que " +
                "en este Macro Proceso no existen mas Procesos relevantes " +
                "que no se hayan identificado y reflejado en la Matriz " +
                "de Riesgos y Controles correspondiente.");
            model.LeyendaCertificacionP = Utilidades.Utilidades.GetSecurityProp("LeyendaCP", "Me he asegurado que los Subprocesos que integran el " +
                "Proceso “XXXXX”, fueron certificados por cada uno de sus " +
                "Responsables y en el caso de existir incidencias " +
                "fueron señaladas oportunamente, además confirmo que " +
                "en este Proceso no existen mas Subprocesos relevantes que no se hayan identificado y reflejado en la Matriz " +
                "de Riesgos y Controles correspondiente.");
            model.LeyendaCertificacionS = Utilidades.Utilidades.GetSecurityProp("LeyendaCS", "Me he asegurado que los controles que mitigan " +
                "los riesgos identificados en el Subproceso “XXXXX”, " +
                "fueron certificados por cada uno de sus responsables " +
                "y en el caso de existir incidencias fueron señaladas " +
                "oportunamente, además confirmo que en este " +
                "subproceso no existen mas riesgos relevantes que " +
                "no se hayan identificado y reflejado en la Matriz " +
                "de Riesgos y Controles correspondiente");

            return View(model);
        }


        // POST: AjustesSeguridad/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Index(AjustesCertificacionViewModel model)
        {
            if (ModelState.IsValid)
            {
                Utilidades.Utilidades.SetSecurityProp("CertificacionSegura", model.CertificacionSegura.ToString());
                Utilidades.Utilidades.SetSecurityProp("LeyendaCE", model.LeyendaCertificacionE.ToString());
                Utilidades.Utilidades.SetSecurityProp("LeyendaCM", model.LeyendaCertificacionM.ToString());
                Utilidades.Utilidades.SetSecurityProp("LeyendaCP", model.LeyendaCertificacionP.ToString());
                Utilidades.Utilidades.SetSecurityProp("LeyendaCS", model.LeyendaCertificacionS.ToString());

                ViewBag.Mensaje = Strings.getMSG("CosteoIndex008");
                return View(model);
            }
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
