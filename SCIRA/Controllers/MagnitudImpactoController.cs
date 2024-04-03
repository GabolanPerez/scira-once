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
    [Access(Funcion = "MagnitudImpacto", ModuleCode = "MSICI003")]
    [CustomErrorHandler]
    public class MagnitudImpactoController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: MagnitudImpacto
        public ActionResult Index()
        {
            return View(db.c_magnitud_impacto.ToList());
        }

        // GET: MagnitudImpacto/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MagnitudImpacto/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create([Bind(Include = "id_magnitud_impacto,cl_magnitud_impacto,nb_magnitud_impacto,magnitud_impacto")] c_magnitud_impacto c_magnitud_impacto)
        {
            if (ModelState.IsValid)
            {
                db.c_magnitud_impacto.Add(c_magnitud_impacto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_magnitud_impacto);
        }

        // GET: MagnitudImpacto/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_magnitud_impacto c_magnitud_impacto = db.c_magnitud_impacto.Find(id);
            if (c_magnitud_impacto == null)
            {
                return HttpNotFound();
            }
            return View(c_magnitud_impacto);
        }

        // POST: MagnitudImpacto/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit([Bind(Include = "id_magnitud_impacto,cl_magnitud_impacto,nb_magnitud_impacto,magnitud_impacto")] c_magnitud_impacto c_magnitud_impacto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_magnitud_impacto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_magnitud_impacto);
        }

        // GET: MagnitudImpacto/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_magnitud_impacto c_magnitud_impacto = db.c_magnitud_impacto.Find(id);
            if (c_magnitud_impacto == null)
            {
                return HttpNotFound();
            }

            if (redirect != null)
            {
                if (redirect != "bfo")
                {
                    //obtenemos el valor del numero de salto
                    int ns;
                    try
                    {
                        ns = (int)HttpContext.Session["JumpCounter"];
                    }
                    catch
                    {
                        ns = 0;
                    }
                    //Si ns es 0, creamos un nuevo array, agregamos la direccion actual y lo asignamos a la variable "Directions" y establecemos "JumpCounter" = 1
                    if (ns == 0)
                    {
                        List<string> directions = new List<string>();
                        directions.Add(redirect);
                        HttpContext.Session["JumpCounter"] = 1;
                        HttpContext.Session["Directions"] = directions;

                    }//En caso de que ns sea distinto a 0, obtenemos el Array "Directions", agregamos la direccion actual, aumentamos el contador y salvamos ambas variables globales
                    else
                    {
                        ns++;
                        List<string> directions = (List<string>)HttpContext.Session["Directions"];
                        directions.Add(redirect);
                        HttpContext.Session["JumpCounter"] = ns;
                        HttpContext.Session["Directions"] = directions;
                    }
                }
            }
            else
            {
                HttpContext.Session["JumpCounter"] = null;
                HttpContext.Session["Directions"] = null;
            }

            //Obtener todos los elementos a los que puede estar ligado este elemento.
            //creamos la lista que contendra a todos los registros relacionados
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();

            //solo incluiremos riesgos
            var r_riesgo = db.k_riesgo.Where(b => b.id_magnitud_impacto == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_riesgo.Count > 0)
            {
                foreach (var riesgo in r_riesgo)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Riesgos";
                    rr.cl_registro = riesgo.nb_riesgo;
                    rr.nb_registro = riesgo.evento;
                    rr.accion = "Delete";
                    rr.controlador = "Riesgo";
                    rr.id_registro = riesgo.id_riesgo.ToString();

                    RR.Add(rr);
                }
            }

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;
            if (c_magnitud_impacto.r_riesgo.Count() > 0) ViewBag.LinkedToR = true;
            return View(c_magnitud_impacto);
        }

        // POST: MagnitudImpacto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_magnitud_impacto c_magnitud_impacto = db.c_magnitud_impacto.Find(id);

            Utilidades.DeleteActions.DeleteMagnitudImpactoObjects(c_magnitud_impacto, db);
            db.c_magnitud_impacto.Remove(c_magnitud_impacto);
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
