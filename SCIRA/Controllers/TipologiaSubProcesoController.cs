﻿using SCIRA.Models;
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
    [Access(Funcion = "TipologiaSP", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class TipologiaSubProcesoController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: TipologiaSubProceso
        public ActionResult Index()
        {
            return View(db.c_tipologia_sub_proceso.ToList());
        }

        // GET: TipologiaSubProceso/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TipologiaSubProceso/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create([Bind(Include = "id_tipologia_sub_proceso,cl_tipologia_sub_proceso,nb_tipologia_sub_proceso")] c_tipologia_sub_proceso c_tipologia_sub_proceso)
        {
            if (ModelState.IsValid)
            {
                db.c_tipologia_sub_proceso.Add(c_tipologia_sub_proceso);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_tipologia_sub_proceso);
        }

        // GET: TipologiaSubProceso/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_tipologia_sub_proceso c_tipologia_sub_proceso = db.c_tipologia_sub_proceso.Find(id);
            if (c_tipologia_sub_proceso == null)
            {
                return HttpNotFound();
            }
            return View(c_tipologia_sub_proceso);
        }

        // POST: TipologiaSubProceso/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit([Bind(Include = "id_tipologia_sub_proceso,cl_tipologia_sub_proceso,nb_tipologia_sub_proceso")] c_tipologia_sub_proceso c_tipologia_sub_proceso)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_tipologia_sub_proceso).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_tipologia_sub_proceso);
        }

        // GET: TipologiaSubProceso/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_tipologia_sub_proceso c_tipologia_sub_proceso = db.c_tipologia_sub_proceso.Find(id);
            if (c_tipologia_sub_proceso == null)
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

            //solo incluiremos sub procesos
            var r_sub_proceso = db.c_sub_proceso.Where(b => b.id_tipologia_sub_proceso == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_sub_proceso.Count > 0)
            {
                foreach (var sp in r_sub_proceso)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Sub Procesos";
                    rr.cl_registro = sp.cl_sub_proceso;
                    rr.nb_registro = sp.nb_sub_proceso;
                    rr.accion = "Delete";
                    rr.controlador = "SubProceso";
                    rr.id_registro = sp.id_sub_proceso.ToString();

                    RR.Add(rr);
                }
            }

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;

            return View(c_tipologia_sub_proceso);
        }

        // POST: TipologiaSubProceso/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_tipologia_sub_proceso c_tipologia_sub_proceso = db.c_tipologia_sub_proceso.Find(id);
            db.c_tipologia_sub_proceso.Remove(c_tipologia_sub_proceso);
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
