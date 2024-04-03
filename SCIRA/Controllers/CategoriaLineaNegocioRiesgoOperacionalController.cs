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
    [Access(Funcion = "CLNRO", ModuleCode = "MSICI005")]
    [CustomErrorHandler]
    public class CategoriaLineaNegocioRiesgoOperacionalController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            return View(db.c_categoria_linea_negocio_riesgo_operacional.Where(r => r.esta_activo ?? false).ToList());
        }

        #region Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create(c_categoria_linea_negocio_riesgo_operacional c_categoria_linea_negocio_riesgo_operacional)
        {
            if (ModelState.IsValid)
            {
                c_categoria_linea_negocio_riesgo_operacional.esta_activo = true;
                db.c_categoria_linea_negocio_riesgo_operacional.Add(c_categoria_linea_negocio_riesgo_operacional);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_categoria_linea_negocio_riesgo_operacional);
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_categoria_linea_negocio_riesgo_operacional c_categoria_linea_negocio_riesgo_operacional = db.c_categoria_linea_negocio_riesgo_operacional.Find(id);
            if (c_categoria_linea_negocio_riesgo_operacional == null)
            {
                return HttpNotFound();
            }
            return View(c_categoria_linea_negocio_riesgo_operacional);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(c_categoria_linea_negocio_riesgo_operacional c_categoria_linea_negocio_riesgo_operacional)
        {
            if (ModelState.IsValid)
            {
                c_categoria_linea_negocio_riesgo_operacional.esta_activo = true;
                db.Entry(c_categoria_linea_negocio_riesgo_operacional).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_categoria_linea_negocio_riesgo_operacional);
        }
        #endregion

        #region Delete
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_categoria_linea_negocio_riesgo_operacional c_categoria_linea_negocio_riesgo_operacional = db.c_categoria_linea_negocio_riesgo_operacional.Find(id);
            if (c_categoria_linea_negocio_riesgo_operacional == null)
            {
                return HttpNotFound();
            }

            Utilidades.DeleteActions.checkRedirect(redirect);

            //Obtener todos los elementos a los que puede estar ligado este elemento.
            //creamos la lista que contendra a todos los registros relacionados
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();

            ////solo incluiremos sub tipo riesgo operacional
            //var r_linea_negocio_riesgo_operacional = db.c_linea_negocio_riesgo_operacional.Where(b => b.id_categoria_linea_negocio_riesgo_operacional == id).ToList();

            ////creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            //if (r_linea_negocio_riesgo_operacional.Count > 0)
            //{
            //    foreach (var sctr in r_linea_negocio_riesgo_operacional)
            //    {
            //        RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
            //        rr.nb_catalogo = "Línea de Negocio de Riesgo Operacional";
            //        rr.cl_registro = sctr.cl_linea_negocio_riesgo_operacional;
            //        rr.nb_registro = sctr.nb_linea_negocio_riesgo_operacional;
            //        rr.accion = "Delete";
            //        rr.controlador = "LineaNegocioRiesgoOperacional";
            //        rr.id_registro = sctr.id_linea_negocio_riesgo_operacional.ToString();

            //        RR.Add(rr);
            //    }
            //}

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;

            return View(c_categoria_linea_negocio_riesgo_operacional);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_categoria_linea_negocio_riesgo_operacional c_categoria_linea_negocio_riesgo_operacional = db.c_categoria_linea_negocio_riesgo_operacional.Find(id);
            Utilidades.DeleteActions.DeleteCategoriaLineaNegocioROObjects(c_categoria_linea_negocio_riesgo_operacional, db);
            c_categoria_linea_negocio_riesgo_operacional.esta_activo = false;

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
            string pathPDF = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/BDEIAuxInfo/categoriaLN.pdf");
            string pathPNG = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/BDEIAuxInfo/categoriaLN.png");

            ViewBag.path = "categoriaLN";

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
                if (System.IO.File.Exists(Server.MapPath("~/App_Data/BDEIAuxInfo/categoriaLN.pdf")))
                    System.IO.File.Delete(Server.MapPath("~/App_Data/BDEIAuxInfo/categoriaLN.pdf"));
                if (System.IO.File.Exists(Server.MapPath("~/App_Data/BDEIAuxInfo/categoriaLN.png")))
                    System.IO.File.Delete(Server.MapPath("~/App_Data/BDEIAuxInfo/categoriaLN.png"));

                archivo.SaveAs(Server.MapPath("~/App_Data/BDEIAuxInfo/categoriaLN." + ext));
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
            string path = "~/App_Data/BDEIAuxInfo/categoriaLN.pdf";
            var manual = File(path, "application/pdf");
            return manual;
        }

        public FileResult DisplayIMG()
        {
            string path = "~/App_Data/BDEIAuxInfo/categoriaLN.png";
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
