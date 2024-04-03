using Newtonsoft.Json;
using SCIRA.Models;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "Normatividad", ModuleCode = "MSICI002")]
    [CustomErrorHandler]
    public class NormatividadControllerBKP : Controller
    {
        private SICIEntities db = new SICIEntities();


        #region Index
        // GET: Normatividad
        public ActionResult Index()
        {
            var c_normatividad = db.c_normatividad.Include(c => c.c_tipo_normatividad);
            return View(c_normatividad.ToList());
        }
        #endregion

        #region Details
        // GET: Normatividad/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_normatividad c_normatividad = db.c_normatividad.Find(id);
            if (c_normatividad == null)
            {
                return HttpNotFound();
            }
            return View(c_normatividad);
        }
        #endregion

        #region Create
        // GET: Normatividad/Create
        public ActionResult Create()
        {
            AgregarNormatividadViewModel model = new AgregarNormatividadViewModel();
            ViewBag.id_tipo_normatividad = new SelectList(db.c_tipo_normatividad, "id_tipo_normatividad", "cl_tipo_normatividad");
            ViewBag.DateFormat = GetDateFormat();
            return View(model);
        }

        // POST: Normatividad/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create(AgregarNormatividadViewModel normatividad)
        {
            // crear instancias de c_normatividad c_nivel_normatividad y c_contenido_normatividad
            // Después aniadir cada uno a la bd
            c_normatividad c_normatividad = new c_normatividad();
            c_nivel_normatividad c_nivel_normatividad = new c_nivel_normatividad();
            c_contenido_normatividad c_contenido_normatividad = new c_contenido_normatividad();


            if (normatividad.fe_publicacion_dof != null)
            {
                ModelState["fe_publicacion_dof"].Errors.Clear();
            }


            if (ModelState.IsValid)
            {
                c_normatividad.id_normatividad = 0;
                c_normatividad.cl_normatividad = normatividad.cl_normatividad;
                c_normatividad.nb_normatividad = normatividad.nb_normatividad;
                c_normatividad.ds_normatividad = normatividad.ds_normatividad;
                c_normatividad.fe_publicacion_dof = normatividad.fe_publicacion_dof;
                c_normatividad.ds_sectores = normatividad.ds_sectores;
                c_normatividad.id_tipo_normatividad = normatividad.id_tipo_normatividad;
                db.c_normatividad.Add(c_normatividad);
                db.SaveChanges();

                c_nivel_normatividad.id_normatividad = c_normatividad.id_normatividad;
                c_nivel_normatividad.cl_nivel_normatividad = normatividad.cl_nivel_normatividad;
                c_nivel_normatividad.nb_nivel_normatividad = normatividad.nb_nivel_normatividad;
                c_nivel_normatividad.no_orden = 0;
                db.c_nivel_normatividad.Add(c_nivel_normatividad);
                db.SaveChanges();

                c_contenido_normatividad.cl_contenido_normatividad = normatividad.cl_normatividad;
                c_contenido_normatividad.ds_contenido_normatividad = normatividad.nb_normatividad;
                c_contenido_normatividad.id_contenido_normatividad_padre = null;
                c_contenido_normatividad.id_nivel_normatividad = c_nivel_normatividad.id_nivel_normatividad;
                c_contenido_normatividad.aparece_en_reporte = normatividad.aparece_en_reporte;
                db.c_contenido_normatividad.Add(c_contenido_normatividad);
                db.SaveChanges();

                c_normatividad.id_root_contenido = c_contenido_normatividad.id_contenido_normatividad;
                db.Entry(c_normatividad).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.id_tipo_normatividad = new SelectList(db.c_tipo_normatividad, "id_tipo_normatividad", "cl_tipo_normatividad", c_normatividad.id_tipo_normatividad);
            ViewBag.DateFormat = GetDateFormat();
            return View(normatividad);
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
            c_normatividad c_normatividad = db.c_normatividad.Find(id);
            if (c_normatividad == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_tipo_normatividad = new SelectList(db.c_tipo_normatividad, "id_tipo_normatividad", "cl_tipo_normatividad", c_normatividad.id_tipo_normatividad);
            ViewBag.DateFormat = GetDateFormat();
            return View(c_normatividad);
        }

        // POST: Normatividad/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(c_normatividad c_normatividad)
        {
            c_contenido_normatividad c_contenido_normatividad = db.c_contenido_normatividad.Find(c_normatividad.id_root_contenido);

            if (ModelState.IsValid)
            {

                db.Entry(c_normatividad).State = EntityState.Modified;
                c_contenido_normatividad.cl_contenido_normatividad = c_normatividad.cl_normatividad;
                c_contenido_normatividad.ds_contenido_normatividad = c_normatividad.nb_normatividad;
                db.Entry(c_contenido_normatividad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_tipo_normatividad = new SelectList(db.c_tipo_normatividad, "id_tipo_normatividad", "cl_tipo_normatividad", c_normatividad.id_tipo_normatividad);
            ViewBag.DateFormat = GetDateFormat();
            return View(c_normatividad);
        }
        #endregion

        #region Delete
        // GET: Normatividad/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_normatividad c_normatividad = db.c_normatividad.Find(id);
            if (c_normatividad == null)
            {
                return HttpNotFound();
            }

            Utilidades.DeleteActions.checkRedirect(redirect);

            return View(c_normatividad);
        }

        // POST: Normatividad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            //obtenemos la normatividad
            c_normatividad c_normatividad = db.c_normatividad.Find(id);

            if (Utilidades.DeleteActions.DeleteNormatividadObjects(c_normatividad, db))
            {
                db.c_normatividad.Remove(c_normatividad);
                db.SaveChanges();
            }
            // En caso de que el registro se haya eliminado correctamente, redireccionar dependiendo desde donde se haya accesado al menú de eliminar
            int ns;
            try
            {
                ns = (int)HttpContext.Session["JumpCounter"];
            }
            catch
            {
                ns = 0;
            }
            //Si ns es 0 redireccionamos al index de este controlador
            if (ns == 0)
            {
                return RedirectToAction("Index");

            }//En caso de que ns sea distinto a 0, obtenemos el Array "Directions", agregamos la direccion actual, aumentamos el contador y salvamos ambas variables globales
            else
            {
                List<string> directions = new List<string>();
                try
                {
                    directions = (List<string>)HttpContext.Session["Directions"];
                }
                catch
                {
                    directions = null;
                }

                if (directions == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    string direction = directions.Last();
                    DirectionViewModel dir = Utilidades.Utilidades.getDirection(direction);
                    //disminuimos ns y eliminamos el ultimo elemento de directions
                    ns--;
                    directions.RemoveAt(ns);

                    //Guardamos ambas variables de sesion para seguir trabajando
                    HttpContext.Session["JumpCounter"] = ns;
                    HttpContext.Session["Directions"] = directions;

                    return RedirectToAction(dir.Action, dir.Controller, new { id = dir.Id, redirect = "bfo" });
                }
            }
        }
        #endregion

        #region Otros
        private string GetDateFormat()
        {
            string lang = Request.UserLanguages[0];
            if (lang.ToLower().Contains("es"))
            {
                return "DD/MM/YYYY";
            }
            else if (lang.ToLower().Contains("en"))
            {
                return "MM/DD/YYYY";
            }
            else if (lang.ToLower().Contains("fr"))
            {
                return "DD/MM/YYYY";
            }
            else if (lang.ToLower().Contains("it"))
            {
                return "DD/MM/YYYY";
            }
            else
            {
                return "DD/MM/YYYY";
            }
        }
        #endregion

        #region Contenido Normatividad

        public ActionResult Content(int id)
        {
            //obtenemos el contenido normatividad y sus hijos
            var padre = db.c_contenido_normatividad.Find(id);

            ViewBag.JsonData = Data(padre);

            return View();
        }

        private string Data(c_contenido_normatividad padre)
        {
            var lista = new List<c_contenido_normatividad>();

            var raiz = padre.c_contenido_normatividad2;

            var hijos = padre.c_contenido_normatividad1;

            //Aquí se guardarán los datos para el organigrama
            var data = new List<OrgChart>();

            lista.AddRange(hijos);
            lista.Add(padre);

            while (raiz != null)
            {
                lista.Add(raiz);
                raiz = raiz.c_contenido_normatividad2;
            }

            foreach (var hijo in lista)
            {
                int pid = 0;
                try
                {
                    pid = (int)hijo.id_contenido_normatividad_padre;
                }
                catch
                {

                }

                data.Add(new OrgChart()
                {
                    id = hijo.id_contenido_normatividad,
                    pid = pid,
                    Nombre = hijo.ds_contenido_normatividad.Substring(0, hijo.ds_contenido_normatividad.Length > 25 ? 25 : hijo.ds_contenido_normatividad.Length),
                    Clave = hijo.cl_contenido_normatividad
                });
            }


            return JsonConvert.SerializeObject(data); ;
        }

        #region LoadData
        public ActionResult loadData(int id)
        {
            var Contenido = db.c_contenido_normatividad.Find(id);

            ViewBag.JsonData = Data(Contenido);

            return PartialView();
        }
        #endregion


        #region Clase para Datos
        private class OrgChart
        {
            public int id { get; set; }
            public int pid { get; set; }
            public string Nombre { get; set; }
            public string Clave { get; set; }
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
