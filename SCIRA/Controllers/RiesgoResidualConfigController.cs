using SCIRA.Models;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "RRConfig", ModuleCode = "MSICI003")]
    [CustomErrorHandler]
    public class RiesgoResidualConfigController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create(a_campo_cobertura_control model)
        {
            var listaCampos = db.a_campo_cobertura_control.Where(a => a.cl_catalogo == model.cl_catalogo && a.cl_campo == model.cl_campo).ToList();
            model.no_orden = listaCampos.Count() + 1;

            if (ModelState.ContainsKey("no_orden"))
                ModelState["no_orden"].Errors.Clear();

            if (ModelState.IsValid)
            {
                db.a_campo_cobertura_control.Add(model);
                db.SaveChanges();

                return RedirectToAction("CampoCoberturaControl", new { cl_campo = model.cl_campo, catalogo = model.cl_catalogo });
            }

            string ErrorMessage = "";

            foreach (var value in ModelState.Values)
            {
                foreach (var error in value.Errors)
                {
                    ErrorMessage = ErrorMessage + "\n" + error.ErrorMessage;
                }
            }

            throw new ArgumentNullException(ErrorMessage);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(a_campo_cobertura_control model)
        {
            var campo = db.a_campo_cobertura_control.Where(a => a.cl_campo == model.cl_campo && a.cl_catalogo == model.cl_catalogo && a.no_orden == model.no_orden).First();

            campo.nb_campo = model.nb_campo;
            campo.ds_campo = model.ds_campo;
            campo.valor = model.valor;

            db.Entry(campo).State = EntityState.Modified;
            db.SaveChanges();

            campo = db.a_campo_cobertura_control.Find(campo.id_campo_cobertura_control);

            return PartialView("CampoCoberturaControl", db.a_campo_cobertura_control.Where(a => a.cl_campo == model.cl_campo && a.cl_catalogo == model.cl_catalogo).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            a_campo_cobertura_control campo = db.a_campo_cobertura_control.Find(id);
            if (campo == null)
            {
                return HttpNotFound();
            }

            db.a_campo_cobertura_control.Remove(campo);
            db.SaveChanges();

            return PartialView("CampoCoberturaControl", db.a_campo_cobertura_control.Where(a => a.cl_campo == campo.cl_campo && a.cl_catalogo == campo.cl_catalogo).ToList());
        }

        private string getFieldName(int cl_campo, int catalogo)
        {
            if (catalogo == 1)
            {
                if (cl_campo == 1) return Strings.getMSG("RiesgoResidualConfigIndex005");
                if (cl_campo == 2) return Strings.getMSG("RiesgoResidualConfigIndex006");
                if (cl_campo == 3) return Strings.getMSG("RiesgoResidualConfigIndex007");
                if (cl_campo == 4) return Strings.getMSG("RiesgoResidualConfigIndex008");
                if (cl_campo == 5) return Strings.getMSG("RiesgoResidualConfigIndex009");
                if (cl_campo == 6) return Strings.getMSG("RiesgoResidualConfigIndex010");
                if (cl_campo == 7) return Strings.getMSG("ControlEdit009");
                if (cl_campo == 8) return Strings.getMSG("RiesgoResidualConfigIndex012");
                if (cl_campo == 9) return Strings.getMSG("RiesgoResidualConfigIndex013");
            }
            else if (catalogo == 2)
            {
                if (cl_campo == 1) return Strings.getMSG("RiesgoResidualConfigIndex021");
                if (cl_campo == 2) return Strings.getMSG("RiesgoResidualConfigIndex022");
                if (cl_campo == 3) return Strings.getMSG("RiesgoResidualConfigIndex023");
                if (cl_campo == 4) return Strings.getMSG("RiesgoResidualConfigIndex024");
                if (cl_campo == 5) return Strings.getMSG("RiesgoResidualConfigIndex025");
            }
            return "";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult CampoCoberturaControl(int cl_campo, int catalogo, int isLoading = 0)
        {
            ViewBag.nb_campo = getFieldName(cl_campo, catalogo);
            ViewBag.id_card = "c" + catalogo.ToString() + cl_campo.ToString();
            ViewBag.id_collapse = "col" + catalogo.ToString() + cl_campo.ToString();
            ViewBag.cl_campo = cl_campo;
            ViewBag.catalogo = catalogo;

            if (isLoading == 1)
            {
                ViewBag.show = true;
            }

            var ListaCampos = db.a_campo_cobertura_control.Where(a => a.cl_campo == cl_campo && a.cl_catalogo == catalogo).ToList();

            return PartialView("CampoCoberturaControl", ListaCampos);
        }

        [OverloadAvoider]
        public ActionResult Summary()
        {

            var ListaCampos = db.a_campo_cobertura_control.ToList();

            return PartialView(ListaCampos);
        }
    }
}
