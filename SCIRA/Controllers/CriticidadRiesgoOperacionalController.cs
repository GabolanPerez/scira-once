using SCIRA.Models;
using SCIRA.Validaciones;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "CriticidadRO", ModuleCode = "MSICI005")]
    [CustomErrorHandler]
    public class CriticidadRiesgoOperacionalController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: CriticidadRO
        public ActionResult Index()
        {
            var FrecuenciaRO = db.c_frecuencia_riesgo_operacional.Where(r => r.esta_activo ?? false).ToList().OrderBy(fr => fr.cl_frecuencia_riesgo_operacional).ToList();
            var ImpactoRO = db.c_impacto_riesgo_operacional.Where(r => r.esta_activo ?? false).ToList().OrderBy(ir => ir.cl_impacto_riesgo_operacional).ToList();
            int NF = FrecuenciaRO.Count();
            int NI = ImpactoRO.Count();

            c_criticidad_ro[,] criticidades = new c_criticidad_ro[NI, NF];
            int i = 0;
            int j = 0;

            foreach (var MI in ImpactoRO)
            {
                foreach (var PO in FrecuenciaRO)
                {
                    //Si no existe la criticidad para este par de impacto y frecuencia, se creará una nueva
                    if (!db.c_criticidad_ro.Any(cr => cr.id_impacto_riesgo_operacional == MI.id_impacto_riesgo_operacional && cr.id_frecuencia_riesgo_operacional == PO.id_frecuencia_riesgo_operacional))
                    {
                        //Comprobar que exista un registro en c_criticidad_riesgo
                        Utilidades.Utilidades.ValidateCRR();
                        var criticidad = new c_criticidad_ro();
                        criticidad.id_impacto_riesgo_operacional = MI.id_impacto_riesgo_operacional;
                        criticidad.id_frecuencia_riesgo_operacional = PO.id_frecuencia_riesgo_operacional;
                        criticidad.id_criticidad_riesgo_ro = Utilidades.Utilidades.idOfFirsCritRO(db);
                        db.c_criticidad_ro.Add(criticidad);
                        criticidades[i, j] = criticidad;
                    }
                    else //Si la criticidad existe
                    {
                        criticidades[i, j] = db.c_criticidad_ro.Where(cr => cr.id_impacto_riesgo_operacional == MI.id_impacto_riesgo_operacional && cr.id_frecuencia_riesgo_operacional == PO.id_frecuencia_riesgo_operacional).First();
                    }
                    j++;
                }
                i++;
                j = 0;
            }

            db.SaveChanges();

            ViewBag.Colores = Utilidades.Utilidades.ColoresMetaCampos();
            ViewBag.FR = FrecuenciaRO;
            ViewBag.NF = NF;
            ViewBag.IM = ImpactoRO;
            ViewBag.NI = NI;
            ViewBag.CRS = criticidades;

            var c = Utilidades.Utilidades.ColoresMetaCampos();
            string cadena = c.First().Text;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult CreateCR(c_criticidad_riesgo_ro criticidad)
        {

            if (ModelState.IsValid)
            {
                var model = new c_criticidad_riesgo_ro();
                model.cl_criticidad_riesgo_ro = criticidad.cl_criticidad_riesgo_ro;
                model.nb_criticidad_riesgo_ro = criticidad.nb_criticidad_riesgo_ro;
                model.cl_color_campo = criticidad.cl_color_campo;
                db.c_criticidad_riesgo_ro.Add(model);
                db.SaveChanges();
                return RedirectToAction("CriticidadRiesgo");
            }
            return RedirectToAction("CriticidadRiesgo");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult EditCR(c_criticidad_riesgo_ro criticidad)
        {

            if (ModelState.IsValid)
            {

                db.Entry(criticidad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CriticidadRiesgo");
            }
            return RedirectToAction("CriticidadRiesgo");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteCR(c_criticidad_riesgo_ro criticidad)
        {
            var model = db.c_criticidad_riesgo_ro.Find(criticidad.id_criticidad_riesgo_ro);
            if (!db.c_criticidad_riesgo_ro.Any(cr => cr.id_criticidad_riesgo_ro != model.id_criticidad_riesgo_ro))
            {
                var CR = new c_criticidad_riesgo_ro();
                CR.cl_color_campo = "0";
                CR.cl_criticidad_riesgo_ro = "N";
                CR.nb_criticidad_riesgo_ro = "Ninguno";
                db.c_criticidad_riesgo_ro.Add(CR);
                db.SaveChanges();
            }
            var CRaux = db.c_criticidad_riesgo_ro.Where(cr => cr.id_criticidad_riesgo_ro != model.id_criticidad_riesgo_ro).First();
            var Criticidades = model.c_criticidad_ro.ToList();

            foreach (var cr in Criticidades)
            {
                cr.id_criticidad_riesgo_ro = CRaux.id_criticidad_riesgo_ro;
            }

            db.c_criticidad_riesgo_ro.Remove(model);
            db.SaveChanges();
            return RedirectToAction("CriticidadRiesgo", new { idA = model.id_criticidad_riesgo_ro, idS = CRaux.id_criticidad_riesgo_ro });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(int id_frecuencia_ro, int id_impacto_ro, int id_criticidad_riesgo_ro)
        {
            var criticidades = db.c_criticidad_ro.ToList();
            var criticidad = db.c_criticidad_ro.Where(c => c.id_impacto_riesgo_operacional == id_impacto_ro && c.id_frecuencia_riesgo_operacional == id_frecuencia_ro).First();
            criticidad.id_criticidad_riesgo_ro = id_criticidad_riesgo_ro;
            db.SaveChanges();
            return null;
        }

        public ActionResult CriticidadRiesgo(int idA = 0, int idS = 0)
        {
            if (idA != 0)
            {
                ViewBag.idcra = idA;
                ViewBag.idcr = idS;
            }

            var CR = db.c_criticidad_riesgo_ro.ToList().OrderBy(c => c.id_criticidad_riesgo_ro);

            return PartialView(CR);
        }
    }
}
