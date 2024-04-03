using SCIRA.Models;
using SCIRA.Validaciones;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "CuentasContables", ModuleCode = "MSICI005")]
    [CustomErrorHandler]
    public class CuentaContableController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: CuentaContable
        public ActionResult Index()
        {
            return View(db.c_entidad.ToList());
        }

        #region Grupos
        public ActionResult Groups(int id)
        {
            var en = db.c_entidad.Find(id);
            ViewBag.en = en;

            var Groups = en.c_grupo_cuenta_contable.Where(r => r.esta_activo ?? false).ToList();

            return PartialView("Groups/Index", Groups);
        }

        #region CreateGroup

        public ActionResult CreateGroup(int id)
        {
            var en = db.c_entidad.Find(id);
            ViewBag.en = en;

            var model = new c_grupo_cuenta_contable()
            {
                id_entidad = id
            };

            return PartialView("Groups/Create", model);
        }

        [HttpPost, NotOnlyRead, ValidateAntiForgeryToken]
        public ActionResult CreateGroup(c_grupo_cuenta_contable model)
        {
            if (ModelState.IsValid)
            {
                model.esta_activo = true;
                db.c_grupo_cuenta_contable.Add(model);
                db.SaveChanges();

                return null;
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        #endregion

        #region EditGroup
        public ActionResult EditGroup(int id)
        {
            var model = db.c_grupo_cuenta_contable.Find(id);
            var en = model.c_entidad;
            ViewBag.en = en;

            return PartialView("Groups/Edit", model);
        }

        [HttpPost, NotOnlyRead, ValidateAntiForgeryToken]
        public ActionResult EditGroup(c_grupo_cuenta_contable model)
        {
            if (ModelState.IsValid)
            {
                model.esta_activo = true;
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

        #region DeleteGroup

        [HttpPost, NotOnlyRead]
        public int DeleteGroup(int id)
        {
            var model = db.c_grupo_cuenta_contable.Find(id);

            Utilidades.DeleteActions.DeleteGrupoCuentaContableROObjects(model, db);
            model.esta_activo = false;

            db.SaveChanges();

            return id;
        }

        #endregion


        #endregion

        #region Cuentas Contables
        public ActionResult Accounts(int id)
        {
            var gr = db.c_grupo_cuenta_contable.Find(id);
            ViewBag.gr = gr;

            var Accounts = gr.c_cuenta_contable.Where(r => r.esta_activo ?? false).ToList();

            return PartialView("Accounts/Index", Accounts);
        }

        #region Create Cuenta

        public ActionResult CreateAccount(int id)
        {
            var gr = db.c_grupo_cuenta_contable.Find(id);
            ViewBag.gr = gr;

            var model = new c_cuenta_contable()
            {
                id_grupo_cuenta_contable = id
            };

            return PartialView("Accounts/Create", model);
        }

        [HttpPost, NotOnlyRead, ValidateAntiForgeryToken]
        public ActionResult CreateAccount(c_cuenta_contable model)
        {
            if (ModelState.IsValid)
            {
                model.esta_activo = true;
                db.c_cuenta_contable.Add(model);
                db.SaveChanges();

                return null;
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        #endregion

        #region EditGroup
        public ActionResult EditAccount(int id)
        {
            var model = db.c_cuenta_contable.Find(id);
            var gr = model.c_grupo_cuenta_contable;
            ViewBag.gr = gr;

            return PartialView("Accounts/Edit", model);
        }

        [HttpPost, NotOnlyRead, ValidateAntiForgeryToken]
        public ActionResult EditAccount(c_cuenta_contable model)
        {
            if (ModelState.IsValid)
            {
                model.esta_activo = true;
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

        #region DeleteGroup

        [HttpPost, NotOnlyRead]
        public int DeleteAccount(int id)
        {
            var model = db.c_cuenta_contable.Find(id);

            model.esta_activo = false;
            db.SaveChanges();

            return id;
        }

        #endregion


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
