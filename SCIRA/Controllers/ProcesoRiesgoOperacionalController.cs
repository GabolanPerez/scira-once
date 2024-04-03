using SCIRA.Models;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "ProcesoRO", ModuleCode = "MSICI005")]
    [CustomErrorHandler]
    public class ProcesoRiesgoOperacionalController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            return View(db.c_proceso_riesgo_operacional.Where(r => r.esta_activo ?? false).ToList());
        }

        #region Create
        public ActionResult Create()
        {
            ViewBag.id_ambito_riesgo_operacionalL = Utilidades.DropDown.AmbitoRiesgoOperacional();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create(c_proceso_riesgo_operacional c_proceso_riesgo_operacional)
        {
            if (ModelState.IsValid)
            {
                c_proceso_riesgo_operacional.esta_activo = true;
                db.c_proceso_riesgo_operacional.Add(c_proceso_riesgo_operacional);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_ambito_riesgo_operacionalL = Utilidades.DropDown.AmbitoRiesgoOperacional(c_proceso_riesgo_operacional.id_ambito_riesgo_operacional);
            return View(c_proceso_riesgo_operacional);
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_proceso_riesgo_operacional c_proceso_riesgo_operacional = db.c_proceso_riesgo_operacional.Find(id);
            if (c_proceso_riesgo_operacional == null)
            {
                return HttpNotFound();
            }
            return View(c_proceso_riesgo_operacional);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(c_proceso_riesgo_operacional c_proceso_riesgo_operacional)
        {
            if (ModelState.IsValid)
            {
                c_proceso_riesgo_operacional.esta_activo = true;
                db.Entry(c_proceso_riesgo_operacional).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_proceso_riesgo_operacional);
        }
        #endregion

        #region Delete
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_proceso_riesgo_operacional c_proceso_riesgo_operacional = db.c_proceso_riesgo_operacional.Find(id);
            if (c_proceso_riesgo_operacional == null)
            {
                return HttpNotFound();
            }

            Utilidades.DeleteActions.checkRedirect(redirect);

            //Obtener todos los elementos a los que puede estar ligado este elemento.
            //creamos la lista que contendra a todos los registros relacionados
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();


            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;
            return View(c_proceso_riesgo_operacional);
        }

        // POST: TipoRiesgo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_proceso_riesgo_operacional c_proceso_riesgo_operacional = db.c_proceso_riesgo_operacional.Find(id);

            c_proceso_riesgo_operacional.esta_activo = false;

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


        #region Archivo auxiliar

        public ActionResult AuxInfo()
        {
            //Comprobar si existe el archivo
            string pathPDF = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/BDEIAuxInfo/proceso.pdf");
            string pathPNG = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/BDEIAuxInfo/proceso.png");

            ViewBag.path = "proceso";

            if (System.IO.File.Exists(pathPDF))
            {
                ViewBag.type = "pdf";
            }
            else if (System.IO.File.Exists(pathPNG))
            {
                ViewBag.type = "png";
            }
            else
            {
                ViewBag.type = "none";
            }

            return View();
        }

        [HttpPost]
        [NotOnlyRead]
        public ActionResult AuxInfoP(HttpPostedFileBase archivo)
        {
            if (archivo == null)
            {
                return RedirectToAction("Index");
            }


            //obtener la extensión
            var ext = archivo.FileName.Split('.').ToList().Last();

            if (ext != "pdf")
                ext = "png";

            try
            {
                if (System.IO.File.Exists(Server.MapPath("~/App_Data/BDEIAuxInfo/proceso.pdf")))
                    System.IO.File.Delete(Server.MapPath("~/App_Data/BDEIAuxInfo/proceso.pdf"));
                if (System.IO.File.Exists(Server.MapPath("~/App_Data/BDEIAuxInfo/proceso.png")))
                    System.IO.File.Delete(Server.MapPath("~/App_Data/BDEIAuxInfo/proceso.png"));

                archivo.SaveAs(Server.MapPath("~/App_Data/BDEIAuxInfo/proceso." + ext));
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return RedirectToAction("Index");
            }
        }


        public FileResult DisplayPDF()
        {
            string path = "~/App_Data/BDEIAuxInfo/proceso.pdf";
            var manual = File(path, "application/pdf");
            return manual;
        }

        public FileResult DisplayIMG()
        {
            string path = "~/App_Data/BDEIAuxInfo/proceso.png";
            var manual = File(path, "image/jpeg");
            return manual;
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
