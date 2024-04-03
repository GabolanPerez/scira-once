using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "ActividadCosteo", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ActividadCosteoController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: c_area_costeo
        public ActionResult Index()
        {
            return View(db.c_area_costeo.ToList());
        }

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
        public ActionResult Create([Bind(Include = "id_area_costeo,cl_area_costeo,nb_area_costeo")] c_area_costeo c_area_costeo)
        {
            if (ModelState.IsValid)
            {
                db.c_area_costeo.Add(c_area_costeo);
                db.SaveChanges();

                Task.Run(() => Utilidades.Utilidades.AgregarActividadASubProcesos(c_area_costeo.id_area_costeo));

                return RedirectToAction("Index");
            }

            return View(c_area_costeo);
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
            c_area_costeo c_area_costeo = db.c_area_costeo.Find(id);
            if (c_area_costeo == null)
            {
                return HttpNotFound();
            }
            return View(c_area_costeo);
        }

        // POST: c_area_costeo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit([Bind(Include = "id_area_costeo,cl_area_costeo,nb_area_costeo")] c_area_costeo c_area_costeo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_area_costeo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_area_costeo);
        }
        #endregion

        #region Delete

        // GET: c_area_costeo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_area_costeo c_area_costeo = db.c_area_costeo.Find(id);
            if (c_area_costeo == null)
            {
                return HttpNotFound();
            }
            return View(c_area_costeo);
        }

        // POST: c_area_costeo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_area_costeo c_area_costeo = db.c_area_costeo.Find(id);

            Utilidades.DeleteActions.DeleteActividadesCosteoObjects(c_area_costeo, db);

            db.c_area_costeo.Remove(c_area_costeo);
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

        #region LVL2

        public ActionResult LVL2(int id)
        {
            var ac = db.c_area_costeo.Find(id);
            ViewBag.ac = ac;

            var areasLVL2 = ac.c_area_costeo_n2.ToList();

            return PartialView("N2/Index", areasLVL2);
        }

        #region CreateLVL2

        public ActionResult CreateLVL2(int id)
        {
            var ac = db.c_area_costeo.Find(id);
            ViewBag.ac = ac;

            var model = new c_area_costeo_n2()
            {
                id_area_costeo = id
            };

            return PartialView("N2/Create", model);
        }

        [HttpPost, NotOnlyRead, ValidateAntiForgeryToken]
        public ActionResult CreateLVL2(c_area_costeo_n2 model)
        {
            if (ModelState.IsValid)
            {
                db.c_area_costeo_n2.Add(model);
                db.SaveChanges();

                return null;
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        #endregion

        #region EditLVL2
        public ActionResult EditLVL2(int id)
        {
            var model = db.c_area_costeo_n2.Find(id);
            var ac = model.c_area_costeo;
            ViewBag.ac = ac;

            return PartialView("N2/Edit", model);
        }

        [HttpPost, NotOnlyRead, ValidateAntiForgeryToken]
        public ActionResult EditLVL2(c_area_costeo_n2 model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                return null;
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        #endregion

        #region DeleteLVL2

        [HttpPost, NotOnlyRead]
        public int DeleteLVL2(int id)
        {
            var model = db.c_area_costeo_n2.Find(id);

            Utilidades.DeleteActions.DeleteActividadesCosteoLVL2Objects(model, db);

            db.c_area_costeo_n2.Remove(model);
            db.SaveChanges();

            return id;
        }

        #endregion

        #endregion

        #region LVL3

        public ActionResult LVL3(int id)
        {
            var ac = db.c_area_costeo_n2.Find(id);
            ViewBag.ac = ac;

            var areasLVL3 = ac.c_area_costeo_n3.ToList();

            return PartialView("N3/Index", areasLVL3);
        }

        #region CreateLVL3

        public ActionResult CreateLVL3(int id)
        {
            var ac = db.c_area_costeo_n2.Find(id);
            ViewBag.ac = ac;

            var model = new c_area_costeo_n3()
            {
                id_area_costeo_n2 = id
            };

            return PartialView("N3/Create", model);
        }

        [HttpPost, NotOnlyRead, ValidateAntiForgeryToken]
        public ActionResult CreateLVL3(c_area_costeo_n3 model)
        {
            if (ModelState.IsValid)
            {
                db.c_area_costeo_n3.Add(model);
                db.SaveChanges();

                return null;
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        #endregion

        #region EditLVL3
        public ActionResult EditLVL3(int id)
        {
            var model = db.c_area_costeo_n3.Find(id);
            var ac = model.c_area_costeo_n2;
            ViewBag.ac = ac;

            return PartialView("N3/Edit", model);
        }

        [HttpPost, NotOnlyRead, ValidateAntiForgeryToken]
        public ActionResult EditLVL3(c_area_costeo_n3 model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                return null;
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        #endregion

        #region DeleteLVL3

        [HttpPost, NotOnlyRead]
        public int DeleteLVL3(int id)
        {
            var model = db.c_area_costeo_n3.Find(id);

            Utilidades.DeleteActions.DeleteActividadesCosteoLVL3Objects(model, db);

            db.c_area_costeo_n3.Remove(model);
            db.SaveChanges();

            return id;
        }

        #endregion

        #endregion

        #region Otros
        public void Normalize()
        {
            var sps = db.c_sub_proceso.ToList();
            var areas_costeo = db.c_area_costeo.ToList();

            var user = (IdentityPersonalizado)User.Identity;
            int i = 1;


            foreach (var sp in sps)
            {
                foreach (var ac in areas_costeo)
                {
                    if (!sp.c_area_costeo_sub_proceso.Select(acsp => acsp.id_area_costeo).Contains(ac.id_area_costeo))
                    {
                        var acsp = new c_area_costeo_sub_proceso() { id_sub_proceso = sp.id_sub_proceso, id_area_costeo = ac.id_area_costeo };
                        db.c_area_costeo_sub_proceso.Add(acsp);
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


        private async void AgregarActividadASubProcesos(int id_area_costeo)
        {
            var auxDB = new SICIEntities();
            var sps = db.c_sub_proceso.ToList();
            int counter = 0;
            foreach (var sp in sps)
            {
                var acsp = new c_area_costeo_sub_proceso { id_area_costeo = id_area_costeo, id_sub_proceso = sp.id_sub_proceso };
                db.c_area_costeo_sub_proceso.Add(acsp);

                Debug.WriteLine("Agregada al sp: " + sp.cl_sub_proceso +"("+counter+")");
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
    }
}
