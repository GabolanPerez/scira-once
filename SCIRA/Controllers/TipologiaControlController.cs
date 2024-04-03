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
    [Access(Funcion = "TipologiaControl", ModuleCode = "MSICI003")]
    [CustomErrorHandler]
    public class TipologiaControlController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: TipologiaControl
        public ActionResult Index()
        {
            return View(db.c_tipologia_control.ToList());
        }

        // GET: TipologiaControl/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_tipologia_control c_tipologia_control = db.c_tipologia_control.Find(id);
            if (c_tipologia_control == null)
            {
                return HttpNotFound();
            }
            return View(c_tipologia_control);
        }

        // GET: TipologiaControl/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TipologiaControl/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create([Bind(Include = "id_tipologia_control,cl_tipologia_control,nb_tipologia_control")] c_tipologia_control c_tipologia_control)
        {
            if (ModelState.IsValid)
            {
                db.c_tipologia_control.Add(c_tipologia_control);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_tipologia_control);
        }

        // GET: TipologiaControl/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_tipologia_control c_tipologia_control = db.c_tipologia_control.Find(id);
            if (c_tipologia_control == null)
            {
                return HttpNotFound();
            }
            return View(c_tipologia_control);
        }

        // POST: TipologiaControl/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit([Bind(Include = "id_tipologia_control,cl_tipologia_control,nb_tipologia_control")] c_tipologia_control c_tipologia_control)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_tipologia_control).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_tipologia_control);
        }

        // GET: TipologiaControl/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_tipologia_control c_tipologia_control = db.c_tipologia_control.Find(id);
            if (c_tipologia_control == null)
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

            //solo incluiremos controles
            var r_control = db.k_control.Where(b => b.id_tipologia_control == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_control.Count > 0)
            {
                foreach (var control in r_control)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Controles";
                    rr.cl_registro = control.relacion_control;
                    rr.nb_registro = control.tiene_accion_correctora ? control.accion_correctora : control.actividad_control;
                    rr.accion = "Delete";
                    rr.controlador = "Control";
                    rr.id_registro = control.id_control.ToString();

                    RR.Add(rr);
                }
            }

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;
            if (c_tipologia_control.r_control.Count() > 0) ViewBag.LinkedToR = true;
            return View(c_tipologia_control);
        }

        // POST: TipologiaControl/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_tipologia_control c_tipologia_control = db.c_tipologia_control.Find(id);

            Utilidades.DeleteActions.DeleteTipologiaControlObjects(c_tipologia_control, db);
            db.c_tipologia_control.Remove(c_tipologia_control);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("CantErase", "Error", null);
            }
            // En caso de que el registro se haya eliminado correctamente, redireccionar dependiendo desde donde se haya accesado al menú de eliminar
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
