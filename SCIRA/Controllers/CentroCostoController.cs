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
    [Access(Funcion = "CentrosCosto", ModuleCode = "MSICI005")]
    [CustomErrorHandler]
    public class CentroCostoController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: CentroCosto
        public ActionResult Index()
        {
            return View(db.c_centro_costo.Where(r => r.esta_activo ?? false).ToList());
        }

        // GET: CentroCosto/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_centro_costo c_centro_costo = db.c_centro_costo.Find(id);
            if (c_centro_costo == null)
            {
                return HttpNotFound();
            }
            return View(c_centro_costo);
        }

        // GET: CentroCosto/Create
        public ActionResult Create()
        {
            ViewBag.id_entidad = new SelectList(db.c_entidad, "id_entidad", "nb_entidad");
            return View();
        }

        // POST: CentroCosto/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create(c_centro_costo c_centro_costo)
        {
            if (ModelState.IsValid)
            {
                c_centro_costo.esta_activo = true;
                db.c_centro_costo.Add(c_centro_costo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_entidad = new SelectList(db.c_entidad, "id_entidad", "nb_entidad", c_centro_costo.id_entidad);
            return View(c_centro_costo);
        }

        // GET: CentroCosto/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_centro_costo c_centro_costo = db.c_centro_costo.Find(id);
            if (c_centro_costo == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_entidad = new SelectList(db.c_entidad, "id_entidad", "nb_entidad", c_centro_costo.id_entidad);
            return View(c_centro_costo);
        }

        // POST: CentroCosto/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit([Bind(Include = "id_centro_costo,id_entidad,cl_centro_costo,nb_centro_costo")] c_centro_costo c_centro_costo)
        {
            if (ModelState.IsValid)
            {
                c_centro_costo.esta_activo = true;
                db.Entry(c_centro_costo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_entidad = new SelectList(db.c_entidad, "id_entidad", "nb_entidad", c_centro_costo.id_entidad);
            return View(c_centro_costo);
        }

        // GET: CentroCosto/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_centro_costo c_centro_costo = db.c_centro_costo.Find(id);
            if (c_centro_costo == null)
            {
                return HttpNotFound();
            }

            Utilidades.DeleteActions.checkRedirect(redirect);

            //Obtener todos los elementos a los que puede estar ligado este elemento.
            //creamos la lista que contendra a todos los registros relacionados
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();

            ////solo incluiremos bdei
            //var r_bdei = db.k_bdei.Where(b => b.id_centro_costo == id).ToList();

            ////creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            //if (r_bdei.Count > 0)
            //{
            //    foreach (var bdei in r_bdei)
            //    {
            //        RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
            //        rr.nb_catalogo = "BDEI";
            //        rr.cl_registro = bdei.id_bdei.ToString();
            //        rr.nb_registro = "BDEI ligado a la entidad: " + bdei.c_entidad.nb_entidad;
            //        rr.accion = "Delete";
            //        rr.controlador = "BDEI";
            //        rr.id_registro = bdei.id_bdei.ToString();

            //        RR.Add(rr);
            //    }
            //}

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;

            return View(c_centro_costo);
        }

        // POST: CentroCosto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_centro_costo c_centro_costo = db.c_centro_costo.Find(id);
            c_centro_costo.esta_activo = false;
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
