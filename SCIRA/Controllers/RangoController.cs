using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using SCIRA.Utilidades;
using Microsoft.Ajax.Utilities;
using Org.BouncyCastle.Crypto;
using LinqKit;
using Syncfusion.EJ2.Linq;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "Estructura", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class RangoController : Controller
    {
        private SICIEntities db = new SICIEntities();


        #region Index

        public ActionResult Index(int? id)
        {
            var user = ((IdentityPersonalizado)User.Identity);

            var enti = Utilidades.Utilidades.RTCObject(db.c_usuario.Find(user.Id_usuario), db, "c_rango").Cast<c_rango>().ToList();

            var aux = db.c_rango.ToList();

            var rango = db.c_rango_costeo.ToList();

            List<RangoModel> costeo = new List<RangoModel>();

            foreach (var en in rango)
            {
                RangoModel item = new RangoModel();

                item.nb_rango = en.nb_rango_costeo;
                item.cl_rango = en.cl_rango_costeo;
                item.id_rango_costeo = en.id_rango_costeo;
                item.pr_costeo = en.pr_costeo;
                item.cl_rango_costeo = en.cl_rango_costeo;

                ViewBag.idcosteo = item.id_rango_costeo;
                ViewBag.clcosteo = item.cl_rango_costeo;
                costeo.Add(item);
            }

            //List<RangoModel> lista = new List<RangoModel>();

            // foreach (var en in aux)
            // {
            //  RangoModel item = new RangoModel();

            // item.cl_rango = en.cl_rango;
            // item.nb_rango = en.nb_rango;
            // item.valor = en.valor;
            // item.seg_cali = en.segmentacion_calificacion;

            // lista.Add(item);
            // }
            var rang = db.c_rango_costeo.Find(id);
            ViewBag.cos = rang;



            return View(aux);

        }
        #endregion

        #region Create

        // GET: c_area_costeo/Create
        public ActionResult Create()
        {
            ViewBag.Colores = Utilidades.Utilidades.ColoresMetaCampos();
            return View();
        }

        // POST: c_area_costeo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create(c_rango model)
        {
            if (ModelState.IsValid)
            {
                db.c_rango.Add(model);

                db.SaveChanges();


                return RedirectToAction("Index");
            }


            return View(model);
        }
        #endregion

        #region Edit
        // GET: c_area_costeo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_rango c_rango = db.c_rango.Find(id);
            if (c_rango == null)
            {
                return HttpNotFound();
            }
            ViewBag.Colores = Utilidades.Utilidades.ColoresMetaCampos();
            return View(c_rango);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit([Bind(Include = "id_rango,cl_rango,nb_rango,cl_color_campo,valor,segmentacion_calificacion,segmentacion_calificacion2")] c_rango c_rango)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_rango).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Colores = Utilidades.Utilidades.ColoresMetaCampos();
            return View(c_rango);
        }
        #endregion

        #region Delete
        // GET: PruebaAutoEvaluacion/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_rango c_rango = db.c_rango.Find(id);
            if (c_rango == null)
            {
                return HttpNotFound();
            }
            return View(c_rango);
        }


        // POST: PruebaAutoEvaluacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_rango c_rango = db.c_rango.Find(id);
            db.c_rango.Remove(c_rango);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion

        #region CreateCosteo

        // GET: c_area_costeo/Create
        public ActionResult CreateCosteo()
        {
            return View();
        }

        // POST: c_area_costeo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult CreateCosteo(c_rango_costeo c_rango_costeo)
        {
            if (ModelState.IsValid)
            {
                db.c_rango_costeo.Add(c_rango_costeo);

                db.SaveChanges();

                Task.Run(() => Utilidades.Utilidades.AgregarActividadAauditoria(c_rango_costeo.id_rango_costeo));

                return RedirectToAction("Index");
            }

            return View(c_rango_costeo);
        }

        #endregion

        #region EditCosteo
        // GET: c_area_costeo/Edit/5
        public ActionResult EditCosteo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_rango_costeo c_rango_costeo = db.c_rango_costeo.Find(id);
            if (c_rango_costeo == null)
            {
                return HttpNotFound();
            }

            return View(c_rango_costeo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult EditCosteo([Bind(Include = "id_rango_costeo,cl_rango_costeo,nb_rango_costeo,pr_costeo")] c_rango_costeo c_rango_costeo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_rango_costeo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_rango_costeo);
        }
        #endregion

        #region DeleteCosteo
        // GET: PruebaAutoEvaluacion/Delete/5
        public ActionResult DeleteCosteo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_rango_costeo c_rango_costeo = db.c_rango_costeo.Find(id);
            if (c_rango_costeo == null)
            {
                return HttpNotFound();
            }
            return View(c_rango_costeo);
        }

        // POST: PruebaAutoEvaluacion/Delete/5
        [HttpPost, ActionName("DeleteCosteo")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmedCosteo(int id)
        {
            c_rango_costeo c_rango_costeo = db.c_rango_costeo.Find(id);

            Utilidades.DeleteActions.DeleteActividadRangoCosteoAuditoriaObjects(c_rango_costeo, db);

            db.c_rango_costeo.Remove(c_rango_costeo);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("CantErase", "Error", null);
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region Otros

        public void Normalize()
        {
            var sps = db.c_auditoria.ToList();
            var areas_costeo = db.c_rango_costeo.ToList();

            var user = (IdentityPersonalizado)User.Identity;
            int i = 1;


            foreach (var sp in sps)
            {
                foreach (var ac in areas_costeo)
                {
                    if (!sp.c_rango_costeo_auditoria.Select(acsp => acsp.id_rango_costeo).Contains(ac.id_rango_costeo))
                    {
                        var acsp = new c_rango_costeo_auditoria() { id_auditoria = sp.id_auditoria, id_rango_costeo = ac.id_rango_costeo };
                        db.c_rango_costeo_auditoria.Add(acsp);
                    }
                }

                string msg = "";

                if (i % 50 == 0)
                {
                    db.SaveChanges();
                    msg = Strings.getMSG("CosteoIndex006");
                }

                Utilidades.Utilidades.notifyUser(user.Id_usuario, "sub proceso: no " + (i++) + "de " + sps.Count + msg, "info");
            }
            db.SaveChanges();
        }

        #endregion

        private async void AgregarActividadAauditoria(int id_rango_costeo)
        {
            var auxDB = new SICIEntities();
            var sps = db.c_auditoria.ToList();
            int counter = 0;
            foreach (var sp in sps)
            {
                var acsp = new c_rango_costeo_auditoria { id_rango_costeo = id_rango_costeo, id_auditoria = sp.id_auditoria };
                db.c_rango_costeo_auditoria.Add(acsp);

                Debug.WriteLine("Agregada al sp: " + sp.cl_auditoria + "(" + counter + ")");
                counter++;
            }

            await db.SaveChangesAsync();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: SubProceso/Costeo/5

        /// <summary>
        /// {
        ///     key : value,
        ///     var1 : "value1"
        ///     var2 : true,
        ///     var3 : 0
        /// }
        /// 
        /// class reciveinformacion {
        ///    public string key { get; set;}
        ///    public string var1 { get; set;}
        ///    public string var2 { get; set;}
        ///    public dinamyc var3 { get; set;}
        /// }
        /// 
        /// 
        /// </summary>
        /// <param name="rangos"></param>
        /// <returns></returns>
        [HttpPost]
        public string ActualizarNombres(List<RangoModel> rangos)
        {

            //return "no completas el 100%";
            foreach (var pro in rangos)
            {
                var item = db.c_rango_costeo.FirstOrDefault(c => c.id_rango_costeo == pro.id_rango_costeo);
                if (item != null)
                {
                    item.pr_costeo = pro.pr_costeo;
                    db.SaveChanges();

                }
                //db.c_rango.Select(s=> s.cl_color_campo)
                //RangoModel item  = db.c_rango.Select(s => new { s.segmentacion_calificacion, s.segmentacion_calificacion2, s.nb_rango }).First();

                //var res = db.c_rango.Select(s => new { s.segmentacion_calificacion, s.segmentacion_calificacion2, s.nb_rango }).First();

                //var item2 = new RangoModel(res.nb_rango,res.segmentacion_calificacion)


            }

            decimal suma = db.c_rango_costeo.Sum(item => item.pr_costeo);
            ViewBag.sumaL = suma.ToString();
            if (suma == 100)
            {

            }
            else
            {
                // Muestra un mensaje de error al usuario
                ViewBag.suma = suma.ToString();
                return "no completas el 100%";
            }


            return "true";
        }

    }
}