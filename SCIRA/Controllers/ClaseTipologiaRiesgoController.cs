using SCIRA.Models;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "ClaseTipologiaRiesgo", ModuleCode = "MSICI003")]
    [CustomErrorHandler]
    public class ClaseTipologiaRiesgoController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: ClaseTipologiaRiesgo
        public ActionResult Index()
        {
            return View(db.c_clase_tipologia_riesgo.ToList());
        }

        #region Create
        // GET: ClaseTipologiaRiesgo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClaseTipologiaRiesgo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create([Bind(Include = "id_clase_tipologia_riesgo,cl_clase_tipologia_riesgo,nb_clase_tipologia_riesgo")] c_clase_tipologia_riesgo c_clase_tipologia_riesgo)
        {
            if (ModelState.IsValid)
            {
                db.c_clase_tipologia_riesgo.Add(c_clase_tipologia_riesgo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_clase_tipologia_riesgo);
        }
        #endregion

        #region Edit
        // GET: ClaseTipologiaRiesgo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_clase_tipologia_riesgo c_clase_tipologia_riesgo = db.c_clase_tipologia_riesgo.Find(id);
            if (c_clase_tipologia_riesgo == null)
            {
                return HttpNotFound();
            }
            return View(c_clase_tipologia_riesgo);
        }

        // POST: ClaseTipologiaRiesgo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit([Bind(Include = "id_clase_tipologia_riesgo,cl_clase_tipologia_riesgo,nb_clase_tipologia_riesgo")] c_clase_tipologia_riesgo c_clase_tipologia_riesgo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_clase_tipologia_riesgo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_clase_tipologia_riesgo);
        }
        #endregion

        #region Delete
        // GET: ClaseTipologiaRiesgo/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_clase_tipologia_riesgo c_clase_tipologia_riesgo = db.c_clase_tipologia_riesgo.Find(id);
            if (c_clase_tipologia_riesgo == null)
            {
                return HttpNotFound();
            }

            Utilidades.DeleteActions.checkRedirect(redirect);

            //Obtener todos los elementos a los que puede estar ligado este elemento.
            //creamos la lista que contendra a todos los registros relacionados
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();

            //solo incluiremos benchmark
            var r_sub_clase_tipologia_riesgo = db.c_sub_clase_tipologia_riesgo.Where(b => b.id_clase_tipologia_riesgo == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_sub_clase_tipologia_riesgo.Count > 0)
            {
                foreach (var sctr in r_sub_clase_tipologia_riesgo)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Sub Clase de Tipología de Riesgo";
                    rr.cl_registro = sctr.cl_sub_clase_tipologia_riesgo;
                    rr.nb_registro = sctr.nb_sub_clase_tipologia_riesgo;
                    rr.accion = "Delete";
                    rr.controlador = "SubClaseTipologiaRiesgo";
                    rr.id_registro = sctr.id_sub_clase_tipologia_riesgo.ToString();

                    RR.Add(rr);
                }
            }

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;

            return View(c_clase_tipologia_riesgo);
        }

        // POST: ClaseTipologiaRiesgo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_clase_tipologia_riesgo c_clase_tipologia_riesgo = db.c_clase_tipologia_riesgo.Find(id);
            db.c_clase_tipologia_riesgo.Remove(c_clase_tipologia_riesgo);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("CantErase", "Error", null);
            }
            //En caso de que el registro se haya eliminado correctamente, redireccionar dependiendo desde donde se haya accesado al menú de eliminar
            int ns;
            try
            {
                ns = (int)HttpContext.Session["JumpCounter"];
            }
            catch
            {
                ns = 0;
            }
            //Si ns es 0 redireccionamos al index de este controlador
            if (ns == 0)
            {
                return RedirectToAction("Index");

            }//En caso de que ns sea distinto a 0, obtenemos el Array "Directions", agregamos la direccion actual, aumentamos el contador y salvamos ambas variables globales
            else
            {
                List<string> directions = new List<string>();
                try
                {
                    directions = (List<string>)HttpContext.Session["Directions"];
                }
                catch
                {
                    directions = null;
                }

                if (directions == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    string direction = directions.Last();
                    DirectionViewModel dir = Utilidades.Utilidades.getDirection(direction);
                    //disminuimos ns y eliminamos el ultimo elemento de directions
                    ns--;
                    directions.RemoveAt(ns);

                    //Guardamos ambas variables de sesion para seguir trabajando
                    HttpContext.Session["JumpCounter"] = ns;
                    HttpContext.Session["Directions"] = directions;

                    return RedirectToAction(dir.Action, dir.Controller, new { id = dir.Id, redirect = "bfo" });
                }

            }
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
