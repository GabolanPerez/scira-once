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
    [Access(Funcion = "SCTipologiaRiesgo", ModuleCode = "MSICI003")]
    [CustomErrorHandler]
    public class SubClaseTipologiaRiesgoController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: SubClaseTipologiaRiesgo
        public ActionResult Index()
        {
            var c_sub_clase_tipologia_riesgo = db.c_sub_clase_tipologia_riesgo.Include(c => c.c_clase_tipologia_riesgo);
            return View(c_sub_clase_tipologia_riesgo.ToList());
        }

        // GET: SubClaseTipologiaRiesgo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_sub_clase_tipologia_riesgo c_sub_clase_tipologia_riesgo = db.c_sub_clase_tipologia_riesgo.Find(id);
            if (c_sub_clase_tipologia_riesgo == null)
            {
                return HttpNotFound();
            }
            return View(c_sub_clase_tipologia_riesgo);
        }

        // GET: SubClaseTipologiaRiesgo/Create
        public ActionResult Create()
        {
            ViewBag.id_clase_tipologia_riesgo = new SelectList(db.c_clase_tipologia_riesgo.OrderBy(x => x.cl_clase_tipologia_riesgo), "id_clase_tipologia_riesgo", "nb_clase_tipologia_riesgo");
            return View();
        }

        // POST: SubClaseTipologiaRiesgo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create([Bind(Include = "id_sub_clase_tipologia_riesgo,id_clase_tipologia_riesgo,cl_sub_clase_tipologia_riesgo,nb_sub_clase_tipologia_riesgo")] c_sub_clase_tipologia_riesgo c_sub_clase_tipologia_riesgo)
        {
            if (ModelState.IsValid)
            {
                db.c_sub_clase_tipologia_riesgo.Add(c_sub_clase_tipologia_riesgo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_clase_tipologia_riesgo = new SelectList(db.c_clase_tipologia_riesgo.OrderBy(x => x.cl_clase_tipologia_riesgo), "id_clase_tipologia_riesgo", "nb_clase_tipologia_riesgo", c_sub_clase_tipologia_riesgo.id_clase_tipologia_riesgo);
            return View(c_sub_clase_tipologia_riesgo);
        }

        // GET: SubClaseTipologiaRiesgo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_sub_clase_tipologia_riesgo c_sub_clase_tipologia_riesgo = db.c_sub_clase_tipologia_riesgo.Find(id);
            if (c_sub_clase_tipologia_riesgo == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_clase_tipologia_riesgo = new SelectList(db.c_clase_tipologia_riesgo.OrderBy(x => x.cl_clase_tipologia_riesgo), "id_clase_tipologia_riesgo", "nb_clase_tipologia_riesgo", c_sub_clase_tipologia_riesgo.id_clase_tipologia_riesgo);
            return View(c_sub_clase_tipologia_riesgo);
        }

        // POST: SubClaseTipologiaRiesgo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit([Bind(Include = "id_sub_clase_tipologia_riesgo,id_clase_tipologia_riesgo,cl_sub_clase_tipologia_riesgo,nb_sub_clase_tipologia_riesgo")] c_sub_clase_tipologia_riesgo c_sub_clase_tipologia_riesgo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_sub_clase_tipologia_riesgo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_clase_tipologia_riesgo = new SelectList(db.c_clase_tipologia_riesgo.OrderBy(x => x.cl_clase_tipologia_riesgo), "id_clase_tipologia_riesgo", "nb_clase_tipologia_riesgo", c_sub_clase_tipologia_riesgo.id_clase_tipologia_riesgo);
            return View(c_sub_clase_tipologia_riesgo);
        }

        // GET: SubClaseTipologiaRiesgo/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_sub_clase_tipologia_riesgo c_sub_clase_tipologia_riesgo = db.c_sub_clase_tipologia_riesgo.Find(id);
            if (c_sub_clase_tipologia_riesgo == null)
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

            //solo incluiremos tipologia de riesgo
            var r_tipologia_riesgo = db.c_tipologia_riesgo.Where(b => b.id_sub_clase_tipologia_riesgo == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_tipologia_riesgo.Count > 0)
            {
                foreach (var tipologia_riesgo in r_tipologia_riesgo)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Tipología de Riesgo";
                    rr.cl_registro = tipologia_riesgo.cl_tipologia_riesgo;
                    rr.nb_registro = tipologia_riesgo.nb_tipologia_riesgo;
                    rr.accion = "Delete";
                    rr.controlador = "TipologiaRiesgo";
                    rr.id_registro = tipologia_riesgo.id_tipologia_riesgo.ToString();

                    RR.Add(rr);
                }
            }

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;


            return View(c_sub_clase_tipologia_riesgo);
        }

        // POST: SubClaseTipologiaRiesgo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_sub_clase_tipologia_riesgo c_sub_clase_tipologia_riesgo = db.c_sub_clase_tipologia_riesgo.Find(id);
            db.c_sub_clase_tipologia_riesgo.Remove(c_sub_clase_tipologia_riesgo);
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
