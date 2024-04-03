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
    public class FrecuenciaController : Controller
    {
        private SICIEntities db = new SICIEntities();


        #region Index

        public ActionResult Index(int? id)
        {
           var cumpli =  db.c_frecuencia.ToList();
            return View(cumpli);

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
        public ActionResult Create(c_frecuencia model)
        {
            if(ModelState.IsValid)
            {
                db.c_frecuencia.Add(model);
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

            c_frecuencia c_Frecuencia = db.c_frecuencia.Find(id);

            if(c_Frecuencia == null)
            {
                return HttpNotFound();
            }

            return View(c_Frecuencia);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit([Bind(Include = "id_frecuencia,cl_frecuencia,nb_frecuencia")]c_frecuencia c_frecuencia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_frecuencia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_frecuencia);
        }
        #endregion

        #region Delete
        // GET: PruebaAutoEvaluacion/Delete/5
        public ActionResult Delete(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_frecuencia c_frecuencia = db.c_frecuencia.Find(id);
            if( c_frecuencia == null) 
            { 
                return HttpNotFound(); 
            }
            return View(c_frecuencia);
        }


        // POST: PruebaAutoEvaluacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_frecuencia c_frecuencia = db.c_frecuencia.Find(id);
            db.c_frecuencia.Remove(c_frecuencia);
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