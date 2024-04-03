using Newtonsoft.Json;
using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "Manuales", ModuleCode = "MSICI010")]
    [CustomErrorHandler]
    public class ManualesController : Controller
    {
        private SICIEntities db = new SICIEntities();


        public ActionResult Index()
        {
            var model = db.c_contenido_manual.Where(c => c.id_contenido_manual_padre == null).ToList();
            ViewBag.su = ((IdentityPersonalizado)User.Identity).Es_super_usuario ? 1 : 0;


            return View(model);
        }

        #region Create  
        public ActionResult Create()
        {
            ViewBag.id_estructuraL = Utilidades.DropDown.EstructuraManual(needLevels: true);

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create(c_contenido_manual c_contenido_manual, int? id_estructura, HttpPostedFileBase archivo)
        {
            if (id_estructura == null)
            {
                ModelState.AddModelError("id_nivel_manual", Strings.getMSG("ManualesCreate018"));
            }
            else
            {
                var estructura = db.c_estructura_manual.Find(id_estructura);

                c_contenido_manual.id_nivel_manual = estructura.c_nivel_manual.First().id_nivel_manual;
            }


            if (ModelState.IsValid)
            {
                db.c_contenido_manual.Add(c_contenido_manual);
                db.SaveChanges();

                //agregar el archivo en caso de existir
                AddImage(archivo, c_contenido_manual.id_contenido_manual);

                return RedirectToAction("Index");
            }

            ViewBag.id_estructuraL = Utilidades.DropDown.EstructuraManual(selected: id_estructura ?? 0, needLevels: true);

            return View(c_contenido_manual);
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_contenido_manual c_contenido_manual = db.c_contenido_manual.Find(id);
            if (c_contenido_manual == null)
            {
                return HttpNotFound();
            }
            ViewBag.nb_contenido = c_contenido_manual.cl_contenido_manual;
            ViewBag.id = c_contenido_manual.id_contenido_manual;

            //ViewBag.data = Utilidades.Utilidades.getChildData(c_contenido_manual, db);

            var contenidos = Utilidades.Utilidades.manualesInferiores(c_contenido_manual);
            contenidos.Add(c_contenido_manual);
            ViewBag.JsonData = JsonData(contenidos);

            var user = (IdentityPersonalizado)User.Identity;

            if (user.Es_super_usuario) ViewBag.su = 1;

            return View(c_contenido_manual);
        }

        #region Clase para Datos
        private class OrgChart
        {
            public int id { get; set; }
            public int pid { get; set; }
            public string Nombre { get; set; }
            public string Clave { get; set; }
            public string Descripcion { get; set; }
            public int noOrden { get; set; }
            public string canEdit { get; set; }
        }

        private string JsonData(List<c_contenido_manual> contenidos)
        {
            var data = new List<OrgChart>();

            var user = (IdentityPersonalizado)User.Identity;
            var userInDb = db.c_usuario.Find(user.Id_usuario);

            foreach (var contenido in contenidos)
            {
                int pid = 0;
                try
                {
                    pid = (int)contenido.id_contenido_manual_padre;
                }
                catch
                {

                }

                int noOrden = 0;
                string canEdit = "0";
                if (contenido.no_orden != null) noOrden = (int)contenido.no_orden;


                if (contenido.c_usuario.Select(c => c.id_usuario).ToList().Contains(userInDb.id_usuario) || user.Es_super_usuario) canEdit = "1";

                var org = new OrgChart
                {
                    id = contenido.id_contenido_manual,
                    pid = pid,
                    Nombre = contenido.cl_contenido_manual,
                    Clave = contenido.c_nivel_manual.nb_nivel_manual,
                    Descripcion = contenido.ds_contenido_manual,
                    noOrden = noOrden,
                    canEdit = canEdit
                };
                data.Add(org);
            }

            return JsonConvert.SerializeObject(data);
        }
        #endregion

        #region AddSoon
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult AddSoon(c_contenido_manual contenido, HttpPostedFileBase archivo)
        {
            if (ModelState.ContainsKey("id_contenido_manual"))
                ModelState["id_contenido_manual"].Errors.Clear();

            var hermanos = db.c_contenido_manual.Where(c => c.id_contenido_manual_padre == contenido.id_contenido_manual_padre);

            int noOrden = hermanos.Count();

            contenido.no_orden = noOrden + 1;

            if (ModelState.IsValid)
            {


                db.c_contenido_manual.Add(contenido);
                db.SaveChanges();

                var user = (IdentityPersonalizado)User.Identity;
                var userInDb = db.c_usuario.Find(user.Id_usuario);
                string canEdit = "0";
                if (contenido.no_orden != null) noOrden = (int)contenido.no_orden;

                if (contenido.c_usuario.Select(c => user.Id_usuario).ToList().Contains(userInDb.id_usuario) || user.Es_super_usuario) canEdit = "1";

                string Json = JsonConvert.SerializeObject(new OrgChart()
                {
                    id = contenido.id_contenido_manual,
                    pid = (int)contenido.id_contenido_manual_padre,
                    Nombre = contenido.cl_contenido_manual,
                    Clave = db.c_nivel_manual.Find(contenido.id_nivel_manual).nb_nivel_manual,
                    Descripcion = contenido.ds_contenido_manual,
                    noOrden = noOrden,
                    canEdit = canEdit
                });

                ViewBag.data = Json;

                //agregar el archivo en caso de existir
                AddImage(archivo, contenido.id_contenido_manual);

                return PartialView("LoadData2");
            }

            return null;
        }
        #endregion

        #region EditContent
        [HttpPost]
        [NotOnlyRead]
        public ActionResult EditContent(int id_contenido_manual, string cl_contenido_manual, string ds_contenido_manual, HttpPostedFileBase archivo)
        {
            var user = (IdentityPersonalizado)User.Identity;
            var userInDb = db.c_usuario.Find(user.Id_usuario);

            if (CanEdit(id_contenido_manual, user.Id_usuario) != "1") return null;

            try
            {
                var contenido = db.c_contenido_manual.Find(id_contenido_manual);
                contenido.cl_contenido_manual = cl_contenido_manual;
                contenido.ds_contenido_manual = ds_contenido_manual;
                db.SaveChanges();

                int noOrden = 0;
                if (contenido.no_orden != null) noOrden = (int)contenido.no_orden;


                string canEdit = "0";
                if (contenido.no_orden != null) noOrden = (int)contenido.no_orden;
                if (contenido.c_usuario.Select(c => user.Id_usuario).ToList().Contains(userInDb.id_usuario) || user.Es_super_usuario) canEdit = "1";


                string Json = JsonConvert.SerializeObject(new OrgChart()
                {
                    id = contenido.id_contenido_manual,
                    pid = contenido.id_contenido_manual_padre ?? 0,
                    Nombre = contenido.cl_contenido_manual,
                    Clave = contenido.c_nivel_manual.nb_nivel_manual,
                    Descripcion = contenido.ds_contenido_manual,
                    noOrden = noOrden,
                    canEdit = canEdit
                });

                ViewBag.data = Json;

                //agregar el archivo en caso de existir
                AddImage(archivo, contenido.id_contenido_manual);

                return PartialView("LoadData3");
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.StackTrace);
                return null;
            }
        }
        #endregion

        #region DeleteContent
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteContent(int id)
        {
            var contenido = db.c_contenido_manual.Find(id);
            List<int> ids = new List<int>();
            ids.Add(contenido.id_contenido_manual);
            var contenidosInferiores = Utilidades.Utilidades.manualesInferiores(contenido);
            foreach (var p in contenidosInferiores)
            {
                ids.Add(p.id_contenido_manual);
            }

            var padre = db.c_contenido_manual.Find(contenido.id_contenido_manual_padre);


            Utilidades.DeleteActions.DeleteContenidoManualObjects(contenido, db);
            db.c_contenido_manual.Remove(contenido);
            db.SaveChanges();

            NormalizeOne(padre);

            var hijos = db.c_contenido_manual.Where(c => c.id_contenido_manual_padre == padre.id_contenido_manual).OrderBy(c => c.no_orden).ToList();

            string Json = JsonConvert.SerializeObject(ids);
            ViewBag.data = Json;
            ViewBag.JsonData = JsonData(hijos);

            return PartialView("LoadData4");
        }
        #endregion

        #region Asignar Sub procesos
        public ActionResult AssignSP(int id)
        {
            AsignaUsuarioPuestoViewModel model = new AsignaUsuarioPuestoViewModel();
            model.id_puesto = id; //id_contenido

            var contenido = db.c_contenido_manual.Find(id);
            var sps = contenido.c_sub_proceso.ToList();

            int[] idUs = new int[sps.Count];

            for (int i = 0; i < sps.Count; i++)
            {
                idUs[i] = sps.ElementAt(i).id_sub_proceso;
            }

            model.users = idUs;

            ViewBag.sps = (MultiSelectList)Utilidades.DropDown.SubProcesosMS(idUs);
            ViewBag.id = id;
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult AssignSP(AsignaUsuarioPuestoViewModel model)
        {
            var contenido = db.c_contenido_manual.Find(model.id_puesto);

            if (model.users != null)
            {
                contenido.c_sub_proceso.Clear();
                foreach (var intU in model.users)
                {
                    var sp = db.c_sub_proceso.Find(intU);
                    sp.c_contenido_manual.Add(contenido);
                }
            }
            else
            {
                contenido.c_sub_proceso.Clear();
            }

            db.SaveChanges();

            return null;
        }
        #endregion

        #region Asignar Usuarios
        public ActionResult AssignU(int id)
        {
            AsignaUsuarioPuestoViewModel model = new AsignaUsuarioPuestoViewModel();
            model.id_puesto = id; //id_contenido

            var contenido = db.c_contenido_manual.Find(id);
            var sps = contenido.c_usuario.ToList();

            int[] idUs = new int[sps.Count];

            for (int i = 0; i < sps.Count; i++)
            {
                idUs[i] = sps.ElementAt(i).id_usuario;
            }

            model.users = idUs;

            ViewBag.sps = (MultiSelectList)Utilidades.DropDown.UsuariosMS(idUs);
            ViewBag.id = id;
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult AssignU(AsignaUsuarioPuestoViewModel model)
        {
            var contenido = db.c_contenido_manual.Find(model.id_puesto);

            if (model.users != null)
            {
                contenido.c_usuario.Clear();
                foreach (var intU in model.users)
                {
                    var sp = db.c_usuario.Find(intU);
                    sp.c_contenido_manual.Add(contenido);
                }
            }
            else
            {
                contenido.c_usuario.Clear();
            }

            db.SaveChanges();

            return null;
        }
        #endregion

        #region CanEdit
        public string CanEdit(int id, int UserId)
        {
            var user = (IdentityPersonalizado)User.Identity;
            if (user.Es_super_usuario) return "1";
            return db.c_contenido_manual.Find(id).c_usuario.Select(u => u.id_usuario).ToList().Contains(UserId) ? "1" : "0";
        }

        #endregion

        #endregion

        #region Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_contenido_manual c_contenido_manual = db.c_contenido_manual.Find(id);
            if (c_contenido_manual == null)
            {
                return HttpNotFound();
            }

            return View(c_contenido_manual);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_contenido_manual c_contenido_manual = db.c_contenido_manual.Find(id);

            if (Utilidades.DeleteActions.DeleteContenidoManualObjects(c_contenido_manual, db))
            {
                db.c_contenido_manual.Remove(c_contenido_manual);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region Otros

        private void AddImage(HttpPostedFileBase archivo,int idContenido)
        {
            if (archivo != null)
            {
                try
                {
                    var fName = Server.MapPath("~/App_Data/Manuales/c" + idContenido + ".png");

                    if (System.IO.File.Exists(fName))
                        System.IO.File.Delete(fName);

                    archivo.SaveAs(fName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public FileResult DisplayIMG(int id,string rid)
        {
            string path = "~/App_Data/Manuales/c" + id + ".png";
            var manual = File(path, "image/jpeg");
            return manual;
        }

        public string DeleteIMG(int id)
        {
            var fName = Server.MapPath("~/App_Data/Manuales/c" + id + ".png");
            if (System.IO.File.Exists(fName))
                System.IO.File.Delete(fName);
            return "ok";
        }

        public string GetNextLevel(int id)
        {
            var contenido = db.c_contenido_manual.Find(id);
            var nivelActual = contenido.c_nivel_manual;
            var estructura = nivelActual.c_estructura_manual;
            var nivelSiguiente = estructura.c_nivel_manual.Where(n => n.no_orden == nivelActual.no_orden + 1).First();

            return nivelSiguiente.nb_nivel_manual;
        }

        public string GetNextLevelID(int id)
        {
            var contenido = db.c_contenido_manual.Find(id);
            var nivelActual = contenido.c_nivel_manual;
            var estructura = nivelActual.c_estructura_manual;
            var nivelSiguiente = estructura.c_nivel_manual.Where(n => n.no_orden == nivelActual.no_orden + 1).First();

            return nivelSiguiente.id_nivel_manual.ToString();
        }


        public string GetLevel(int id)
        {
            var contenido = db.c_contenido_manual.Find(id);
            var nivelActual = contenido.c_nivel_manual;

            return nivelActual.nb_nivel_manual;
        }

        public string GetContent(int id)
        {
            var contenido = db.c_contenido_manual.Find(id);

            content res = new content();

            string path = Server.MapPath("~/App_Data/Manuales/c" + id + ".png");

            res.tieneImagen = System.IO.File.Exists(path);
            res.contenido = contenido.ds_contenido_manual;

            return JsonConvert.SerializeObject(res);
        }

        public class content
        {
            public string contenido { get; set; }
            public bool tieneImagen { get; set; }
        }

        public ActionResult MoveContent(int id, int LoR)
        {
            var contenido = db.c_contenido_manual.Find(id);
            bool tiene_padre = contenido.id_contenido_manual_padre != null;
            c_contenido_manual padre = new c_contenido_manual();

            List<c_contenido_manual> contenidos = new List<c_contenido_manual>();

            if (tiene_padre)
            {
                padre = db.c_contenido_manual.Find(contenido.id_contenido_manual_padre);
                var hermanos = db.c_contenido_manual.Where(c => c.id_contenido_manual_padre == padre.id_contenido_manual);
                var NoHermanos = hermanos.Count();
                var posAux = 0;

                if (LoR == 0) //Izquierda
                {
                    if (contenido.no_orden == 1) //No se puede mover a la izquierda
                    {
                        //Mensaje de que no se puede mover
                        ViewBag.msg = Strings.getMSG("AvisoError004");
                    }
                    else //mover a la izquierda
                    {
                        var hermano = db.c_contenido_manual.Where(c => c.id_contenido_manual_padre == padre.id_contenido_manual && c.no_orden == contenido.no_orden - 1).First();
                        posAux = (int)hermano.no_orden;
                        hermano.no_orden = contenido.no_orden;
                        contenido.no_orden = posAux;

                        contenidos.Add(contenido);
                        contenidos.Add(hermano);
                    }
                }
                if (LoR == 1) //Derecha
                {
                    if (contenido.no_orden == NoHermanos) //No se puede mover a la derecha
                    {
                        //mensaje de que no se puede mover
                        ViewBag.msg = Strings.getMSG("No se puede mover a la derecha.");
                    }
                    else //mover a la derecha
                    {
                        var hermano = db.c_contenido_manual.Where(c => c.id_contenido_manual_padre == padre.id_contenido_manual && c.no_orden == contenido.no_orden + 1).First();
                        posAux = (int)hermano.no_orden;
                        hermano.no_orden = contenido.no_orden;
                        contenido.no_orden = posAux;

                        contenidos.Add(contenido);
                        contenidos.Add(hermano);
                    }
                }

                db.SaveChanges();

                //regresar la informacion de los 2 campos que se cambiaron
                ViewBag.JsonData = JsonData(contenidos);

                //return PartialView("LoadData");
            }
            else //si no tiene padre, no se puede mover
            {
                //regresar mensaje de que no se puede mover
                ViewBag.msg = Strings.getMSG("ManualesCreate007");
                ViewBag.JsonData = JsonData(contenidos);
            }

            return PartialView("LoadData5");
        }
        #endregion

        #region Generacion de Documentos
        public FileResult GetPDF(int id, int sp = 0)
        {
            var rootContent = db.c_contenido_manual.Find(id);
            var bytes = Utilidades.GenerateDoc.Manual(rootContent, sp);

            var name = Utilidades.Utilidades.NormalizarNombreArchivo(rootContent.c_nivel_manual.nb_nivel_manual + " " + rootContent.cl_contenido_manual);

            return File(bytes, "application/pdf");
            //return File(bytes, "application/pdf", name + ".pdf");
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

        public void NormalizeContent()
        {
            var cl = db.c_contenido_manual.Where(c => c.id_contenido_manual_padre == null).ToList();

            foreach (var root in cl)
            {
                NormalizeOne(root, true);
            }
        }

        private void NormalizeOne(c_contenido_manual cont, bool descendent = false)
        {
            var hijos = db.c_contenido_manual.Where(c => c.id_contenido_manual_padre == cont.id_contenido_manual).OrderBy(c => c.no_orden).ToList();

            var NoHijos = hijos.Count;

            c_contenido_manual aux;

            for (int i = 1; i <= NoHijos; i++)
            {
                aux = hijos.ElementAt(i - 1);
                aux.no_orden = i;
                db.SaveChanges();
                if (descendent)
                {
                    NormalizeOne(aux, descendent);
                }
            }
        }
    }
}
