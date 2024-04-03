using SCIRA.Models;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "UsuarioNR", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ArchivosController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: Prueba de archivos
        public ActionResult Index()
        {

            return PartialView();
        }

        public int Upload(HttpPostedFileBase file, string uuid)
        {
            if (file != null)
            {
                var archivo = new c_archivo();
                archivo.uuid = uuid;
                archivo.fe_alta = DateTime.Now;
                string nbarchivo = file.FileName;
                archivo.nb_archivo = nbarchivo.Substring(0, nbarchivo.LastIndexOf('.'));
                archivo.extension = nbarchivo.Substring(nbarchivo.LastIndexOf('.') + 1);

                db.c_archivo.Add(archivo);
                db.SaveChanges();


                string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Archivos/a" + archivo.id_archivo);
                //string path = HttpRuntime.AppDomainAppPath + "App_Data/Archivos/a" + archivo.id_archivo;

                file.SaveAs(path);

                return archivo.id_archivo;
            }
            return 0;
        }

        public int Delete(string uuid)
        {
            c_archivo archivo;
            try
            {
                archivo = db.c_archivo.Where(a => a.uuid == uuid).First();
            }
            catch
            {
                return 0;
            }

            int id = archivo.id_archivo;

            //eliminamos fisicamente el archivo
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Archivos/a" + archivo.id_archivo);
            //string path = HttpRuntime.AppDomainAppPath + "App_Data/Archivos/a" + archivo.id_archivo;
            System.IO.File.Delete(path);

            //eliminamos el registro de la db
            db.c_archivo.Remove(archivo);
            db.SaveChanges();

            return id;
        }

        [NotOnlyRead]
        public int DeleteByUser(int id)
        {
            c_archivo archivo = db.c_archivo.Find(id);

            string nombre = archivo.nb_archivo;
            //eliminamos fisicamente el archivo
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Archivos/a" + archivo.id_archivo);
            //string path = HttpRuntime.AppDomainAppPath + "App_Data/Archivos/a" + archivo.id_archivo;
            System.IO.File.Delete(path);

            //eliminamos el registro de la db
            Utilidades.DeleteActions.DeleteArchivoObjects(archivo, db);
            db.c_archivo.Remove(archivo);
            db.SaveChanges();

            return id;
        }

        public ActionResult Download(int id)
        {
            var archivo = db.c_archivo.Find(id);
            string contentType = System.Net.Mime.MediaTypeNames.Application.Octet;

            string filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Archivos/a" + id);

            return new FilePathResult("~/App_Data/Archivos/a" + id, contentType)
            {
                FileDownloadName = archivo.nb_archivo + "." + archivo.extension
            };
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
