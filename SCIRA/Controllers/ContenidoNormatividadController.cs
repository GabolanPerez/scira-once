using SCIRA.Models;
using SCIRA.Utilidades;
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
    [Access(Funcion = "CN", ModuleCode = "MSICI002")]
    [CustomErrorHandler]
    public class ContenidoNormatividadController : Controller
    {
        private SICIEntities db = new SICIEntities();

        #region Index

        // GET: Normatividad
        public ActionResult Index(int? id)
        {
            c_contenido_normatividad contenido;
            string sql;
            List<c_contenido_normatividad> contenidos;
            List<ContenidoNormatividadViewModel> Contenidos;

            if (id == null)
            {
                contenido = db.c_contenido_normatividad.Where(c => c.id_contenido_normatividad_padre == null).FirstOrDefault();
                ViewBag.nb_nivel = contenido.c_nivel_normatividad.nb_nivel_normatividad;
                ViewBag.orden_nivel = contenido.c_nivel_normatividad.no_orden;
                sql = "select * from c_contenido_normatividad where id_contenido_normatividad_padre is NULL";
                contenidos = db.c_contenido_normatividad.SqlQuery(sql).ToList();
                Contenidos = new List<ContenidoNormatividadViewModel>();

                foreach (var cont in contenidos)
                {
                    c_nivel_normatividad nivelActual = db.c_nivel_normatividad.Find(cont.id_nivel_normatividad);
                    c_normatividad normatividad = db.c_normatividad.Find(nivelActual.id_normatividad);
                    int maxLevel = normatividad.c_nivel_normatividad.Max(n => n.no_orden);

                    ContenidoNormatividadViewModel content = new ContenidoNormatividadViewModel();
                    content.cl_contenido_normatividad = cont.cl_contenido_normatividad;
                    content.ds_contenido_normatividad = cont.ds_contenido_normatividad;
                    content.id_contenido_normatividad = cont.id_contenido_normatividad;
                    content.id_contenido_normatividad_padre = cont.id_contenido_normatividad_padre;
                    content.id_nivel_normatividad = cont.id_nivel_normatividad;
                    content.nb_nivel_normatividad = cont.c_nivel_normatividad.nb_nivel_normatividad;
                    //content.aparece_en_reporte = cont.aparece_en_reporte ? "Si aparece" : "No aparece";
                    if (maxLevel > cont.c_nivel_normatividad.no_orden)
                    {
                        c_nivel_normatividad sigLevel = db.c_nivel_normatividad.Where(n => n.no_orden == (cont.c_nivel_normatividad.no_orden + 1) && n.id_normatividad == normatividad.id_normatividad).First();
                        content.sig_nivel = sigLevel.nb_nivel_normatividad;
                    }
                    else
                    {
                        content.sig_nivel = "null";
                    }
                    Contenidos.Add(content);
                }

                return View(Contenidos);
            }

            //necesitamos id y el nombre del padre del padre del contenido actual, hasta llegar a un contenido sin padre
            int? id_padre = id;
            List<int?> aux = new List<int?>();
            List<string> aux2 = new List<string>();
            int auxCont = 0;
            while (id_padre != null)
            {
                c_contenido_normatividad aux3 = db.c_contenido_normatividad.Find(id_padre);
                aux.Add(aux3.id_contenido_normatividad_padre);
                aux2.Add(aux3.cl_contenido_normatividad);
                id_padre = aux3.id_contenido_normatividad_padre;
                auxCont++;
            }


            ViewBag.IDs = aux;
            ViewBag.Claves = aux2;
            ViewBag.Contador = auxCont;

            contenido = db.c_contenido_normatividad.Find(id);
            ViewBag.idAnterior = contenido.id_contenido_normatividad_padre;

            contenido = db.c_contenido_normatividad.Where(c => c.id_contenido_normatividad_padre == id).FirstOrDefault();
            if (contenido != null)
            {
                ViewBag.nb_nivel = contenido.c_nivel_normatividad.nb_nivel_normatividad;
                ViewBag.orden_nivel = contenido.c_nivel_normatividad.no_orden;
            }


            sql = "select * from c_contenido_normatividad where id_contenido_normatividad_padre = " + id;
            contenidos = db.c_contenido_normatividad.SqlQuery(sql).ToList();
            Contenidos = new List<ContenidoNormatividadViewModel>();

            foreach (var cont in contenidos)
            {
                c_nivel_normatividad nivelActual = db.c_nivel_normatividad.Find(cont.id_nivel_normatividad);
                c_normatividad normatividad = db.c_normatividad.Find(nivelActual.id_normatividad);
                int maxLevel = normatividad.c_nivel_normatividad.Max(n => n.no_orden);

                ContenidoNormatividadViewModel content = new ContenidoNormatividadViewModel();
                content.cl_contenido_normatividad = cont.cl_contenido_normatividad;
                content.ds_contenido_normatividad = cont.ds_contenido_normatividad;
                content.id_contenido_normatividad = cont.id_contenido_normatividad;
                content.id_contenido_normatividad_padre = cont.id_contenido_normatividad_padre;
                content.id_nivel_normatividad = cont.id_nivel_normatividad;
                content.nb_nivel_normatividad = cont.c_nivel_normatividad.nb_nivel_normatividad;
                //content.aparece_en_reporte = cont.aparece_en_reporte ? "Si aparece" : "No aparece";
                if (maxLevel > cont.c_nivel_normatividad.no_orden)
                {
                    c_nivel_normatividad sigLevel = db.c_nivel_normatividad.Where(n => n.no_orden == (cont.c_nivel_normatividad.no_orden + 1) && n.id_normatividad == normatividad.id_normatividad).First();
                    content.sig_nivel = sigLevel.nb_nivel_normatividad;
                }
                else
                {
                    content.sig_nivel = "null";
                }
                Contenidos.Add(content);
            }

            return View(Contenidos);
        }
        #endregion

        #region Create

        // GET: Normatividad/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_contenido_normatividad c_contenido_normatividad = db.c_contenido_normatividad.Find(id);
            if (c_contenido_normatividad == null)
            {
                return HttpNotFound();
            }

            c_contenido_normatividad model = new c_contenido_normatividad();
            c_nivel_normatividad nivel = db.c_nivel_normatividad.Where(n => n.no_orden == (c_contenido_normatividad.c_nivel_normatividad.no_orden + 1) && n.id_normatividad == c_contenido_normatividad.c_nivel_normatividad.id_normatividad).First();

            model.id_contenido_normatividad_padre = c_contenido_normatividad.id_contenido_normatividad;

            model.id_nivel_normatividad = nivel.id_nivel_normatividad;

            ViewBag.nb_nivel = nivel.nb_nivel_normatividad;
            ViewBag.padre = c_contenido_normatividad.cl_contenido_normatividad + " - " + c_contenido_normatividad.ds_contenido_normatividad;
            ViewBag.abuelo = db.c_contenido_normatividad.Find(model.id_contenido_normatividad_padre).id_contenido_normatividad_padre;

            return View(model);
        }

        // POST: Normatividad/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create(c_contenido_normatividad model)
        {
            if (ModelState.IsValid)
            {
                db.c_contenido_normatividad.Add(model);
                db.SaveChanges();
                var sp_normatividad = db.c_sub_proceso_normatividad.Where(spn => spn.id_contenido_normatividad == model.id_contenido_normatividad_padre).ToList();
                //Agregar todos los sub procesos a los que esté ligado el contenido de normatividad padre 
                foreach (var spn in sp_normatividad)
                {
                    var SPN = new c_sub_proceso_normatividad();
                    SPN.id_contenido_normatividad = model.id_contenido_normatividad;
                    SPN.id_sub_proceso = spn.id_sub_proceso;
                    SPN.es_raiz = false;

                    db.c_sub_proceso_normatividad.Add(SPN);
                }
                db.SaveChanges();
                return RedirectToAction("Index", new { id = model.id_contenido_normatividad_padre });
            }

            c_nivel_normatividad nivel = db.c_nivel_normatividad.Find(model.id_nivel_normatividad);
            c_contenido_normatividad padre = db.c_contenido_normatividad.Find(model.id_contenido_normatividad_padre);

            ViewBag.nb_nivel = nivel.nb_nivel_normatividad;
            ViewBag.padre = padre.cl_contenido_normatividad + " - " + padre.ds_contenido_normatividad;

            return View(model);
        }
        #endregion

        #region Edit

        // GET: Normatividad/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_contenido_normatividad c_contenido_normatividad = db.c_contenido_normatividad.Find(id);
            if (c_contenido_normatividad == null)
            {
                return HttpNotFound();
            }
            return View(c_contenido_normatividad);
        }

        // POST: Normatividad/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(c_contenido_normatividad model)
        {
            if (ModelState.IsValid)
            {

                db.Entry(model).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index", new { id = model.id_contenido_normatividad_padre });
            }
            return View(model);
        }
        #endregion

        #region Delete

        // GET: Normatividad/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_contenido_normatividad c_contenido_normatividad = db.c_contenido_normatividad.Find(id);
            if (c_contenido_normatividad == null)
            {
                return HttpNotFound();
            }
            if (c_contenido_normatividad.id_contenido_normatividad_padre == null)
            {
                ViewBag.Error = "No se puede eliminar un contenido raíz, se debe borrar toda la normatividad en en la sección \"Normatividades\"";
            }
            return View(c_contenido_normatividad);
        }

        // POST: Normatividad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_contenido_normatividad contenido = db.c_contenido_normatividad.Find(id);
            //Borramos todos los niveles inferiores
            int id_padre = (int)contenido.id_contenido_normatividad_padre;

            if (contenido.id_contenido_normatividad_padre == null)
            {
                return View(contenido);
            }
            Utilidades.DeleteActions.DeleteContenidoNormatividadObjects(contenido, db);
            db.SaveChanges();

            return RedirectToAction("Index", new { id = id_padre });
        }
        #endregion

        #region Otros
        public ActionResult LigarSP(int id)
        {
            var cont = db.c_contenido_normatividad.Find(id);


            var selected = cont.c_sub_proceso_normatividad.Select(spn => spn.id_sub_proceso).ToArray();

            var norm = Utilidades.Utilidades.getRoot(db, cont);

            if (cont.id_contenido_normatividad_padre != null)
            {
                ViewBag.title = Strings.getMSG("Sub Procesos ligados con") + cont.cl_contenido_normatividad + Strings.getMSG("de la normatividad") + norm.ds_contenido_normatividad;
            }
            else
            {
                ViewBag.title = Strings.getMSG("Sub Procesos ligados con la normatividad") + norm.ds_contenido_normatividad;
            }
            ViewBag.id_contenido = id;
            ViewBag.tspL = Utilidades.Utilidades.SubProcesosLigados(cont);
            ViewBag.spL = Utilidades.DropDown.SubProcesosMS(selected);

            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public int LigarSP(int id_contenido, int[] sps)
        {
            var cont = db.c_contenido_normatividad.Find(id_contenido);
            var ligas = cont.c_sub_proceso_normatividad.ToList();


            foreach (var liga in ligas)
            {
                db.c_sub_proceso_normatividad.Remove(liga);
            }

            db.SaveChanges();

            if (sps != null)
            {
                foreach (var idsp in sps)
                {
                    var spn = new c_sub_proceso_normatividad()
                    {
                        id_contenido_normatividad = id_contenido,
                        id_sub_proceso = idsp,
                        es_raiz = true
                    };

                    db.c_sub_proceso_normatividad.Add(spn);
                }
            }

            db.SaveChanges();

            return id_contenido;
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
