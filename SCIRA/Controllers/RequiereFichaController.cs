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
using System.Web;
using System.Web.Services.Protocols;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "Estructura", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class RequiereFichaController : Controller
    {
        private SICIEntities db = new SICIEntities();


        #region Index

        public ActionResult Index(int? id)
        {
           var req = db.c_requiere_ficha.ToList();
            return View(req);
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
        public ActionResult Create(c_requiere_ficha model)
        {
           if (ModelState.IsValid)
            {
                db.c_requiere_ficha.Add(model);
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_requiere_ficha c_requiere_ficha = db.c_requiere_ficha.Find(id);

            if(c_requiere_ficha == null)
            {
                return HttpNotFound();
            }
            return View(c_requiere_ficha);
            
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit([Bind(Include = "id_requiere_ficha,cl_requiere_ficha,nb_requiere_ficha")]c_requiere_ficha model)
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
           if(!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            c_requiere_ficha c_requiere_ficha = db.c_requiere_ficha.Find(id);

            if(c_requiere_ficha != null)
            {
                return HttpNotFound();
            }

            return View(c_requiere_ficha);
        }


        // POST: PruebaAutoEvaluacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_requiere_ficha c_requiere_ficha = db.c_requiere_ficha.Find(id);
            db.c_requiere_ficha.Remove(c_requiere_ficha);
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