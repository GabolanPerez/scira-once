using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [AccessAudit(Funcion = "SegAud-Stats")]
    [CustomErrorHandler]
    public class GraphicsSeguimientoAuditoriaController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            var auditorias = db.c_auditoria;

            var kAuditorias = db.k_auditoria;

            #region Encontrar años
            List<int> años = new List<int>();

            foreach (var item in kAuditorias)
            {
                if (item.fe_inicial_planeada.HasValue)
                {
                    var año = item.fe_inicial_planeada.Value.Year;

                    if (!años.Contains(año))
                        años.Add(año);
                }
            }

            if (años.Count == 0)
            {
                años.Add(DateTime.Now.Year);
            }

            años = años.OrderBy(a => a).ToList();

            ViewBag.años = años;
            #endregion

            return View(auditorias);
        }



        #region Detalles

        public ActionResult Details(int id)
        {
            var model = db.k_auditoria.Find(id);

            var files = Directory.GetFiles(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/InformesAuditoria/"));

            var exist = false;

            foreach (var file in files)
            {
                if (file.Split(new char[] { '\\' }).Last() == id.ToString())
                {
                    exist = true;
                    break;
                }
            }

            ViewBag.hasFile = exist;
            return PartialView("DetailViews/DetailsINFAUD", model);
        }

        public FileResult GetInformeDef(int id)
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/InformesAuditoria/" + id);
            var bytes = System.IO.File.ReadAllBytes(path);


            return File(bytes, "application/pdf");
        }
        #endregion


        public ActionResult DetailsCosVista(int? id)
        {
            var model = db.c_auditoria.Find(id);


            return PartialView("DetailViews/DetailsCosVista", model);
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
