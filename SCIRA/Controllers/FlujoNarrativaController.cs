using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "FlujosNarrativas", ModuleCode = "MSICI013")]
    [CustomErrorHandler]
    public class FlujoNarrativaController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: FlujoNarrativa
        public ActionResult Index()
        {
            var user = (IdentityPersonalizado)ControllerContext.HttpContext.User.Identity;
            var us = db.c_usuario.Find(user.Id_usuario);

            var subProcesos = Utilidades.Utilidades.RTCObject(us, db, "c_sub_proceso").Cast<c_sub_proceso>().ToList();

            return View(subProcesos);
        }

        public ActionResult Flujo(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            c_sub_proceso c_sub_proceso = db.c_sub_proceso.Find(id); //buscar subproceso por id

            ViewBag.nb_sub_proceso = c_sub_proceso.nb_sub_proceso;  //mandamos el nombre del subproceso
            ViewBag.id = id;                                          //mandamos el id del subproceso


            ViewBag.nb_archivo_flujo = c_sub_proceso.nb_archivo_flujo; //mandamos el nombre del archivo de flujo
            return View();
        }

        public ActionResult Narrativa(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            c_sub_proceso c_sub_proceso = db.c_sub_proceso.Find(id); //buscar subproceso por id

            ViewBag.nb_sub_proceso = c_sub_proceso.nb_sub_proceso;  //mandamos el nombre del subproceso
            ViewBag.id = id;                                          //mandamos el id del subproceso


            ViewBag.nb_archivo_manual = c_sub_proceso.nb_archivo_manual; //mandamos el nombre del archivo de flujo
            return View();
        }

        [HttpPost]
        [NotOnlyRead]
        public ActionResult Subir(HttpPostedFileBase archivo, int id, int fon) // fon => flujo o narrativa
        {
            c_sub_proceso c_sub_proceso = db.c_sub_proceso.Find(id); //buscar subproceso por id
            ViewBag.id = id;
            ViewBag.nb_sub_proceso = c_sub_proceso.nb_sub_proceso;  //mandamos el nombre del subproceso
            if (fon == 1)
            {
                ViewBag.fon = Strings.getMSG("RiesgoCreate080");
            }
            else
            {
                ViewBag.fon = Strings.getMSG("RiesgoCreate081");
            }

            if (archivo == null)
            {
                ViewBag.carga = Strings.getMSG("Carga Fallida");
                ViewBag.estado = Strings.getMSG("No se cargó el archivo:");
                ViewBag.mensaje = Strings.getMSG("No se seleccionó ningún archivo");
                return View();
            }

            string nbarchivo;
            if (fon == 1)
            {
                nbarchivo = ("f" + id);
            }
            else
            {
                nbarchivo = ("n" + id);
            }

            try
            {
                archivo.SaveAs(Server.MapPath("~/App_Data/" + nbarchivo));
                ViewBag.nb_archivo = Path.GetFileName(archivo.FileName);

                if (fon == 1)
                {
                    c_sub_proceso.nb_archivo_flujo = Path.GetFileName(archivo.FileName);       //asignar el nombre del archivo en la base de datos
                    //ViewBag.mensaje = archivo.FileName;  //mandamos el nombre del subproceso
                    ViewBag.mensaje = Path.GetFileName(archivo.FileName);
                }
                else
                {
                    c_sub_proceso.nb_archivo_manual = Path.GetFileName(archivo.FileName);       //asignar el nombre del archivo en la base de datos
                    ViewBag.mensaje = Path.GetFileName(archivo.FileName);  //mandamos el nombre del subproceso
                }

                db.SaveChanges();
                ViewBag.estado = Strings.getMSG("Se cargó el archivo:");
                ViewBag.carga = Strings.getMSG("Carga Exitosa");
                return View();
            }
            catch (Exception e)
            {
                ViewBag.mensaje = Strings.getMSG("Falla al guardar en el servidor");
                ViewBag.mensaje = e.Message;
                ViewBag.estado = Strings.getMSG("No se cargó el archivo:");
                ViewBag.carga = Strings.getMSG("Carga Fallida");
                return View();
            }
        }

        [NotOnlyRead]
        public ActionResult DescargaFlujo(int id)
        {
            c_sub_proceso c_sub_proceso = db.c_sub_proceso.Find(id); //buscar subproceso por id
            string contentType = System.Net.Mime.MediaTypeNames.Application.Octet;
            return new FilePathResult("~/App_Data/f" + id, contentType)
            {
                FileDownloadName = c_sub_proceso.nb_archivo_flujo,
            };
        }

        [NotOnlyRead]
        public ActionResult DescargaNarrativa(int id)
        {
            c_sub_proceso c_sub_proceso = db.c_sub_proceso.Find(id); //buscar subproceso por id
            string contentType = System.Net.Mime.MediaTypeNames.Application.Octet;
            return new FilePathResult("~/App_Data/n" + id, contentType)
            {
                FileDownloadName = c_sub_proceso.nb_archivo_manual,
            };
        }

        public ActionResult EliminarFlujo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_sub_proceso c_sub_proceso = db.c_sub_proceso.Find(id);
            if (c_sub_proceso == null)
            {
                return HttpNotFound();
            }
            ViewBag.nb_sub_proceso = c_sub_proceso.nb_sub_proceso;
            ViewBag.nb_archivo_flujo = c_sub_proceso.nb_archivo_flujo;

            return View(c_sub_proceso);
        }

        [HttpPost, ActionName("EliminarFlujo")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmedf(int id)
        {
            c_sub_proceso c_sub_proceso = db.c_sub_proceso.Find(id);
            c_sub_proceso.nb_archivo_flujo = null;
            db.SaveChanges();

            string name = "f" + id;
            string path = Server.MapPath("~/App_Data");
            string fullpath = Path.Combine(path, name);
            System.IO.File.Delete(fullpath);

            return RedirectToAction("Index");
        }

        public ActionResult EliminarNarrativa(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_sub_proceso c_sub_proceso = db.c_sub_proceso.Find(id);
            if (c_sub_proceso == null)
            {
                return HttpNotFound();
            }
            ViewBag.nb_sub_proceso = c_sub_proceso.nb_sub_proceso;
            ViewBag.nb_archivo_manual = c_sub_proceso.nb_archivo_manual;

            return View(c_sub_proceso);
        }

        [HttpPost, ActionName("EliminarNarrativa")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmedn(int id)
        {
            c_sub_proceso c_sub_proceso = db.c_sub_proceso.Find(id);
            c_sub_proceso.nb_archivo_manual = null;
            db.SaveChanges();

            string name = "n" + id;
            string path = Server.MapPath("~/App_Data");
            string fullpath = Path.Combine(path, name);
            System.IO.File.Delete(fullpath);

            return RedirectToAction("Index");
        }

        public FileResult DisplayPDF(int id, string fon)
        {
            string path = "~/App_Data/" + fon + id;
            return File(path, "application/pdf");
        }
    }
}