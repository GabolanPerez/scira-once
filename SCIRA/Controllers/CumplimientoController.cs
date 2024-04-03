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
    public class CumplimientoController : Controller
    {
        private SICIEntities db = new SICIEntities();


        #region Index

        public ActionResult Index(int? id)
        {
            var user = ((IdentityPersonalizado)User.Identity);

            var enti = Utilidades.Utilidades.RTCObject(db.c_usuario.Find(user.Id_usuario), db, "c_rango").Cast<c_rango>().ToList();

            var aux = db.c_cumplimiento.ToList();

            return View(aux);

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
        public ActionResult Create(c_cumplimiento model)
        {         
            if(ModelState.IsValid) 
            {
            db.c_cumplimiento.Add(model);

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
            c_cumplimiento c_cumplimiento = db.c_cumplimiento.Find(id);
            if (c_cumplimiento == null)
            {
                return HttpNotFound();
            }
            return View(c_cumplimiento);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit([Bind(Include = "id_cumplimiento,cl_cumplimiento,nb_cumplimiento")]c_cumplimiento c_cumplimiento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_cumplimiento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
           
            return View(c_cumplimiento);
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
          c_cumplimiento c_cumpliento = db.c_cumplimiento.Find(id);
            if(c_cumpliento == null) 
            { 
                return HttpNotFound();
             
            }
            return View(c_cumpliento);
        }


        // POST: PruebaAutoEvaluacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
                  
                c_cumplimiento c_cumplimiento = db.c_cumplimiento.Find(id);
                db.c_cumplimiento.Remove(c_cumplimiento);
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