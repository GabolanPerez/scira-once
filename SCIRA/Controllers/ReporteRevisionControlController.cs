using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "RepRC", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ReporteRevisionControlController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            var user = ((IdentityPersonalizado)User.Identity);
            var us = db.c_usuario.Find(user.Id_usuario);

            var controles = Utilidades.Utilidades.RTCObject(us, db, "k_control", "1").Cast<k_control>().ToList();

            var model = controles.Where(c => !c.tiene_accion_correctora).ToList();

            //no añadir controles que provengan de un MG
            model = model.Where(c => c.c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso.Substring(0, 2) != "MG").ToList();

            //controles sin revision
            var controlesSR = model.Where(c => c.k_revision_control.Count() == 0).ToList();
            //var controlesCR = controles.Where(c => c.k_revision_control.Count() > 0).ToList();
            List<k_revision_control> revisiones = new List<k_revision_control>();

            foreach (var c in controles)
            {
                revisiones.AddRange(c.k_revision_control.ToList());
            }

            ViewBag.controlesSR = controlesSR;
            //ViewBag.controlesCR = controlesCR;
            ViewBag.revisiones = revisiones;


            return View();
        }

        #region Generacion de Documentos
        public FileResult GetPDF(int id)
        {
            var revision = db.k_revision_control.Find(id);
            var bytes = Utilidades.GenerateDoc.RevisionControl(revision);

            var name = Utilidades.Utilidades.NormalizarNombreArchivo("Revision del Control: " + revision.k_control.relacion_control + " " + ((DateTime)revision.dg_fe_revision).ToShortDateString());

            //var file = File(bytes, "application/pdf");
            //file.FileDownloadName = name;

            return File(bytes, "application/pdf");
            //return File(bytes, "application/pdf", name + ".pdf");
        }
        #endregion

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