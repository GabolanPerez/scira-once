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
    public class ImpactoMonetarioController : Controller
    {
        private SICIEntities db = new SICIEntities();


        #region Index

        public ActionResult Index()
        {
           var imp = db.c_impacto_monetario.ToList();
            return View(imp);
        }
        #endregion

        #region Create

        // GET: c_area_costeo/Create
        public ActionResult Create()
        {
          return View();
           
        }

        // POST: c_area_costeo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create(c_impacto_monetario model)
        {
            if (ModelState.IsValid)
            {
                db.c_impacto_monetario.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        #endregion

        #region Edit
        // GET: c_cumplimiento/Edit/5
        public ActionResult Edit(int? id)
        {
          if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
          c_impacto_monetario impacto_monetario = db.c_impacto_monetario.Find(id);
            if(impacto_monetario == null)
            {
                return HttpNotFound();
            }
            return View(impacto_monetario);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit([Bind(Include = "id_impacto_monetario,cl_impacto_monetario,nb_impacto_monetario")]c_impacto_monetario model)
        {
         if(ModelState.IsValid)
            {
                
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
         return View(model);
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
            c_impacto_monetario c_impacto_monetario = db.c_impacto_monetario.Find(id);
            if(c_impacto_monetario == null)
            {
                return HttpNotFound();
            }
            return View(c_impacto_monetario);
        }


        // POST: PruebaAutoEvaluacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {

            c_impacto_monetario c_impacto_monetario = db.c_impacto_monetario.Find(id);

            Utilidades.DeleteActions.DeleteMontoImpactoObjects(c_impacto_monetario, db);

            db.c_impacto_monetario.Remove(c_impacto_monetario);
            db.SaveChanges();
            return RedirectToAction("Index");
                
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