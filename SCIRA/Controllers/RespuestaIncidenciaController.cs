using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "IncResp", ModuleCode = "MSICI006")]
    [CustomErrorHandler]
    public class RespuestaIncidenciaController : Controller
    {
        private SICIEntities db = new SICIEntities();


        public ActionResult Index()
        {
            var User = (IdentityPersonalizado)HttpContext.User.Identity;
            int id = User.Id_usuario;
            var usuario = db.c_usuario.Find(id);

            //var objetos = Utilidades.Utilidades.RTCObject(usuario, db, "k_objeto").Cast<k_objeto>().ToList();

            //objetos = objetos.Where(o => (o.tipo_objeto == 1 || o.tipo_objeto == 2 || o.tipo_objeto == 3)).ToList();
            List<k_incidencia> Incidencias = new List<k_incidencia>();

            //foreach (var objeto in objetos)
            //{
            //    var incs = objeto.k_incidencia.ToList();
            //    foreach (var inc in incs)
            //    {
            //        if (inc.id_responsable == usuario.id || usuario.Es_super_usuario)
            //            incidencias.Add(inc);
            //    }
            //}

            var incidencias = Utilidades.Utilidades.RTCObject(usuario, db, "k_incidencia").Cast<k_incidencia>().ToList();
            
            foreach(var incidencia in incidencias)
            {
                var objeto = incidencia.k_objeto;
                if (objeto != null)
                {
                    var tipo = objeto.tipo_objeto;
                    if(tipo == 1 || tipo == 2 || tipo == 3)
                    {
                        Incidencias.Add(incidencia);
                    }
                }
            }


            if (usuario.es_super_usuario) ViewBag.su = 1;

            return View(Incidencias);
        }



        public ActionResult ResponderIncidencia(int id)
        {
            var model = new r_respuesta();
            var incidencia = db.k_incidencia.Find(id);

            model.id_incidencia = id;
            ViewBag.Incidencia = incidencia;

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult ResponderIncidencia(r_respuesta model, HttpPostedFileBase file1, HttpPostedFileBase file2, HttpPostedFileBase file3)
        {

            model.fe_solucion = DateTime.Now;
            model.observaciones = model.observaciones ?? "";
            var incidencia = db.k_incidencia.Find(model.id_incidencia);

            if (ModelState.IsValid)
            {
                db.r_respuesta.Add(model);
                db.SaveChanges();
                SaveFiles(model, file1, file2, file3);



                Utilidades.Utilidades.refreshNotifCount(incidencia.id_responsable);
                Utilidades.Utilidades.removeRow(5, incidencia.id_incidencia, incidencia.id_responsable);
                return RedirectToAction("Index");
            }


            ViewBag.Incidencia = incidencia;
            return View(model);
        }


        public ActionResult Edit(int id)
        {
            var model = db.r_respuesta.Find(id);
            var incidencia = model.k_incidencia;

            ViewBag.Incidencia = incidencia;
            ViewBag.nb_a1 = model.archivo_1;
            ViewBag.nb_a2 = model.archivo_2;
            ViewBag.nb_a3 = model.archivo_3;


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(r_respuesta model, HttpPostedFileBase file1, HttpPostedFileBase file2, HttpPostedFileBase file3)
        {
            //Si se edita la respuesta, la fecha de respuesta cambia
            model.fe_solucion = DateTime.Now;
            model.observaciones = model.observaciones ?? "";

            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                SaveFiles(model, file1, file2, file3, true);
                return RedirectToAction("Index");
            }

            var incidencia = db.k_incidencia.Find(model.id_incidencia);
            ViewBag.Incidencia = incidencia;
            ViewBag.nb_a1 = model.archivo_1;
            ViewBag.nb_a2 = model.archivo_2;
            ViewBag.nb_a3 = model.archivo_3;
            return View(model);
        }

        public ActionResult Delete(int? id)
        {
            var model = db.r_respuesta.Find(id);
            var incidencia = model.k_incidencia;

            ViewBag.Incidencia = incidencia;
            ViewBag.nb_a1 = model.archivo_1;
            ViewBag.nb_a2 = model.archivo_2;
            ViewBag.nb_a3 = model.archivo_3;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Delete(int id)
        {
            var model = db.r_respuesta.Find(id);

            bool del = Utilidades.DeleteActions.DeleteRespuestaIncidenciaObjects(model, db);
            if (del)
            {
                db.r_respuesta.Remove(model);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        private bool SaveFiles(r_respuesta respuesta, HttpPostedFileBase file1, HttpPostedFileBase file2, HttpPostedFileBase file3, bool edit = false)
        {
            Type m_tipo = respuesta.GetType();
            PropertyInfo[] m_propiedades = m_tipo.GetProperties();
            HttpPostedFileBase[] files = { file1, file2, file3 };

            for (int i = 1; i < 4; i++)
            {
                var prop = m_propiedades.Where(p => p.Name == "archivo_" + i).First();

                string nombre = "ac" + i + "-" + respuesta.id_respuesta;
                if (files[i - 1] != null)
                {
                    files[i - 1].SaveAs(Server.MapPath("~/App_Data/RIncidencias/" + nombre));
                    prop.SetValue(respuesta, files[i - 1].FileName);
                }
                else
                {
                    if (edit)
                    {
                        if (prop.GetValue(respuesta, null) == null)
                        {
                            string path = Server.MapPath("~/App_Data/RIncidencias/" + nombre);
                            System.IO.File.Delete(path);
                        }
                    }
                }
            }
            db.SaveChanges();
            return true;
        }

        [NotOnlyRead]
        public ActionResult DescargaArchivo(int id, int index)
        {
            r_respuesta respuesta = db.r_respuesta.Find(id);
            Type m_tipo = respuesta.GetType();
            PropertyInfo[] m_propiedades = m_tipo.GetProperties();
            var prop = m_propiedades.Where(p => p.Name == "archivo_" + index).First();


            string contentType = System.Net.Mime.MediaTypeNames.Application.Octet;
            return new FilePathResult("~/App_Data/RIncidencias/ac" + index + "-" + id, contentType)
            {
                FileDownloadName = (string)prop.GetValue(respuesta, null),
            };
        }

    }
}
