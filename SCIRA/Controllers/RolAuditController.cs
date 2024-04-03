using SCIRA.Models;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [AccessAudit(Funcion = "adminAudit")]
    [CustomErrorHandler]
    public class RolAuditController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: Rol
        public ActionResult Index()
        {
            List<c_rol> rolesAudit = new List<c_rol>();

            var roles = db.c_rol;

            foreach (var rol in roles)
            {
                var funcion = rol.c_funcion.FirstOrDefault();
                if (funcion != null)
                {
                    if (funcion.c_menu_funcion.cl_menu_funcion == "AUDI")
                        rolesAudit.Add(rol);
                }
                else
                {
                    rol.c_usuario.Clear();
                    db.c_rol.Remove(rol);
                    db.SaveChanges();
                }
            }

            return View(rolesAudit);
        }


        // GET: Rol/Create
        public ActionResult Agregar()
        {
            ViewBag.funciones = Utilidades.DropDown.FuncionesMS(audit: true);
            return View();
        }

        // POST: Rol/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Agregar([Bind(Include = "id_rol,cl_rol,nb_rol,id_funcion")] AgregarRolViewModel Rol)
        {
            c_rol c_rol = new c_rol()
            {
                cl_rol = Rol.cl_rol,
                nb_rol = Rol.nb_rol
            };
            if (ModelState.IsValid && Rol.id_funcion != null)
            {
                db.c_rol.Add(c_rol);
                if (Rol.id_funcion != null)
                {
                    foreach (int id_func in Rol.id_funcion)
                    {
                        c_funcion f = db.c_funcion.Find(id_func);
                        c_rol.c_funcion.Add(f);
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.funciones = Utilidades.DropDown.FuncionesMS(Rol.id_funcion, true);
            if (Rol.id_funcion == null)
                ViewBag.error = Strings.getMSG("RolCreate005");



            return View(Rol);
        }

        private static c_rol GetC_rol(c_rol c_rol)
        {
            return c_rol;
        }

        // GET: Rol/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AgregarRolViewModel rol = new AgregarRolViewModel();
            c_rol c_rol = db.c_rol.Find(id);
            if (c_rol == null)
            {
                return HttpNotFound();
            }
            rol.id_rol = c_rol.id_rol;
            rol.cl_rol = c_rol.cl_rol;
            rol.nb_rol = c_rol.nb_rol;

            string sql = "select id_funcion from c_funcion_rol where id_rol = " + rol.id_rol;
            var funciones = db.Database.SqlQuery<int>(sql).ToArray();
            ViewBag.funciones = Utilidades.DropDown.FuncionesMS(funciones, true);


            return View(rol);
        }

        // POST: Rol/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit([Bind(Include = "id_rol,cl_rol,nb_rol,id_funcion")] AgregarRolViewModel Rol)
        {
            c_rol c_rol = db.c_rol.Find(Rol.id_rol);

            if (ModelState.IsValid && Rol.id_funcion != null)
            {
                c_rol.cl_rol = Rol.cl_rol;
                c_rol.nb_rol = Rol.nb_rol;
                c_rol.c_funcion.Clear();
                if (Rol.id_funcion != null)
                {
                    foreach (int id_func in Rol.id_funcion)
                    {
                        c_funcion f = db.c_funcion.Find(id_func);
                        c_rol.c_funcion.Add(f);
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            string sql = "select id_funcion from c_funcion_rol where id_rol = " + Rol.id_rol;
            var funciones = db.Database.SqlQuery<int>(sql).ToArray();
            ViewBag.funciones = Utilidades.DropDown.FuncionesMS(funciones, true);

            if (Rol.id_funcion == null)
                ViewBag.error = Strings.getMSG("RolCreate005");


            return View(Rol);
        }

        // GET: Rol/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_rol c_rol = db.c_rol.Find(id);
            if (c_rol == null)
            {
                return HttpNotFound();
            }
            return View(c_rol);
        }

        // POST: Rol/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_rol c_rol = db.c_rol.Find(id);
            c_rol.c_funcion.Clear();
            c_rol.c_usuario.Clear();
            db.c_rol.Remove(c_rol);
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

        public ActionResult AsignaUsuarios(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_rol c_rol = db.c_rol.Find(id);
            if (c_rol == null)
            {
                return HttpNotFound();
            }
            AsignaUsuarioRolViewModel Rol = new AsignaUsuarioRolViewModel();
            Rol.id_rol = c_rol.id_rol;
            ViewBag.nb_rol = c_rol.nb_rol;
            string sql = "select id_usuario from c_rol_usuario where id_rol = " + Rol.id_rol;
            var usuarios = db.Database.SqlQuery<int>(sql).ToArray();
            ViewBag.usuarios = new MultiSelectList(db.c_usuario.Where(u => u.es_auditor).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", usuarios);
            return View(Rol);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult AsignaUsuarios([Bind(Include = "id_rol,id_usuario")] AsignaUsuarioRolViewModel Rol)
        {
            c_rol c_rol = db.c_rol.Find(Rol.id_rol);
            if (c_rol == null)
            {
                return HttpNotFound();
            }

            try
            {
                c_rol.c_usuario.Clear();
                if (Rol.id_usuario == null)
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                foreach (int id_usr in Rol.id_usuario)
                {
                    c_usuario u = db.c_usuario.Find(id_usr);
                    c_rol.c_usuario.Add(u);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.nb_rol = c_rol.nb_rol;
                string sql = "select id_usuario from c_rol_usuario where id_rol = " + Rol.id_rol;
                var usuarios = db.Database.SqlQuery<int>(sql).ToArray();
                ViewBag.usuarios = new MultiSelectList(db.c_usuario.Where(u => u.es_auditor).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", usuarios);
                return View(Rol);
            }
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
