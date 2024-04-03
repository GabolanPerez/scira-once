using SCIRA.Models;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "Normatividad", ModuleCode = "MSICI002")]
    [CustomErrorHandler]
    public class NormatividadController : Controller
    {
        private SICIEntities db = new SICIEntities();


        #region Index
        // GET: Normatividad
        public ActionResult Index()
        {
            var tipos = db.c_tipo_normatividad.OrderBy(t => t.cl_tipo_normatividad).ToList();
            return View(tipos);
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







        #region Tipo de Normatividad

        #region Agregar

        public ActionResult CreateTN()
        {
            return View("TipoNormatividad/CreateTN");
        }

        // POST: TipoNormatividad/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult CreateTN([Bind(Include = "id_tipo_normatividad,cl_tipo_normatividad,nb_tipo_normatividad")] c_tipo_normatividad c_tipo_normatividad)
        {
            if (ModelState.IsValid)
            {
                db.c_tipo_normatividad.Add(c_tipo_normatividad);
                db.SaveChanges();
                return RedirectToAction("Index", "Normatividad");
            }

            return View("TipoNormatividad/CreateTN", c_tipo_normatividad);
        }
        #endregion

        #region Editar
        // GET: TipoNormatividad/Edit/5
        public ActionResult EditTN(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_tipo_normatividad c_tipo_normatividad = db.c_tipo_normatividad.Find(id);
            if (c_tipo_normatividad == null)
            {
                return HttpNotFound();
            }
            return View("TipoNormatividad/EditTN", c_tipo_normatividad);
        }

        // POST: TipoNormatividad/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult EditTN([Bind(Include = "id_tipo_normatividad,cl_tipo_normatividad,nb_tipo_normatividad")] c_tipo_normatividad c_tipo_normatividad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_tipo_normatividad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Normatividad");
            }
            return View("TipoNormatividad/CreateTN", c_tipo_normatividad);
        }
        #endregion

        #region Delete
        // GET: TipoNormatividad/Delete/5
        public ActionResult DeleteTN(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_tipo_normatividad c_tipo_normatividad = db.c_tipo_normatividad.Find(id);
            if (c_tipo_normatividad == null)
            {
                return HttpNotFound();
            }

            Utilidades.DeleteActions.checkRedirect(redirect);

            //Obtener todos los elementos a los que puede estar ligado este elemento.
            //creamos la lista que contendra a todos los registros relacionados
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();

            //solo incluiremos Normatividad
            var r_normatividad = db.c_normatividad.Where(b => b.id_tipo_normatividad == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_normatividad.Count > 0)
            {
                foreach (var normatividad in r_normatividad)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Normatividad";
                    rr.cl_registro = normatividad.cl_normatividad;
                    rr.nb_registro = normatividad.ds_normatividad;
                    rr.accion = "Delete";
                    rr.controlador = "Normatividad";
                    rr.id_registro = normatividad.id_normatividad.ToString();

                    RR.Add(rr);
                }
            }

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;

            return View("TipoNormatividad/DeleteTN", c_tipo_normatividad);
        }

        // POST: TipoNormatividad/Delete/5
        [HttpPost, ActionName("DeleteTN")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmedTN(int id)
        {
            c_tipo_normatividad c_tipo_normatividad = db.c_tipo_normatividad.Find(id);
            db.c_tipo_normatividad.Remove(c_tipo_normatividad);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("CantErase", "Error", null);
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
                    return RedirectToAction("Index", "Normatividad");
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

        #endregion

        #region Normatividad

        #region Create
        // GET: Normatividad/Create
        public ActionResult Create(int id)
        {
            AgregarNormatividadViewModel model = new AgregarNormatividadViewModel();
            ViewBag.DateFormat = GetDateFormat();

            var tipoN = db.c_tipo_normatividad.Find(id);
            ViewBag.nb_tipo = tipoN.nb_tipo_normatividad;

            model.id_tipo_normatividad = id;

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

                #region Añadir Normatividad

                c_normatividad.id_normatividad = 0;
                c_normatividad.cl_normatividad = normatividad.cl_normatividad;
                c_normatividad.nb_normatividad = normatividad.nb_normatividad;
                c_normatividad.ds_normatividad = normatividad.ds_normatividad;
                c_normatividad.fe_publicacion_dof = normatividad.fe_publicacion_dof;
                c_normatividad.ds_sectores = normatividad.ds_sectores;
                c_normatividad.id_tipo_normatividad = normatividad.id_tipo_normatividad;
                db.c_normatividad.Add(c_normatividad);
                db.SaveChanges();
                #endregion

                #region Añadir Nivel de Normatividad
                c_nivel_normatividad.id_normatividad = c_normatividad.id_normatividad;
                c_nivel_normatividad.cl_nivel_normatividad = normatividad.cl_nivel_normatividad;
                c_nivel_normatividad.nb_nivel_normatividad = normatividad.nb_nivel_normatividad;
                c_nivel_normatividad.no_orden = 0;
                db.c_nivel_normatividad.Add(c_nivel_normatividad);
                db.SaveChanges();
                #endregion

                #region Añadir Contenido de Normatividad
                c_contenido_normatividad.cl_contenido_normatividad = normatividad.cl_normatividad;
                c_contenido_normatividad.ds_contenido_normatividad = normatividad.nb_normatividad;
                c_contenido_normatividad.id_contenido_normatividad_padre = null;
                c_contenido_normatividad.id_nivel_normatividad = c_nivel_normatividad.id_nivel_normatividad;
                c_contenido_normatividad.aparece_en_reporte = normatividad.aparece_en_reporte;
                db.c_contenido_normatividad.Add(c_contenido_normatividad);
                db.SaveChanges();
                #endregion


                c_normatividad.id_root_contenido = c_contenido_normatividad.id_contenido_normatividad;
                db.Entry(c_normatividad).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

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

        #endregion

        #region Niveles de Normatividad
        public ActionResult IndexLVL(int id)
        {
            var norm = db.c_normatividad.Find(id);
            ViewBag.title = Strings.getMSG("Niveles de la Normatividad") + norm.nb_normatividad;
            var model = norm.c_nivel_normatividad.ToList();

            ViewBag.id_norm = id;

            return PartialView("Niveles/Index", model);
        }


        #region Create
        public ActionResult CreateLVL(int id)
        {
            c_normatividad c_normatividad = db.c_normatividad.Find(id);
            c_nivel_normatividad model = new c_nivel_normatividad();
            model.id_normatividad = c_normatividad.id_normatividad;
            ViewBag.nb_normatividad = c_normatividad.nb_normatividad;


            model.id_normatividad = id;

            return PartialView("Niveles/Create", model);
        }

        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        public int CreateLVLP(c_nivel_normatividad c_nivel_normatividad)
        {
            c_normatividad normatividad = db.c_normatividad.Find(c_nivel_normatividad.id_normatividad);

            c_nivel_normatividad.no_orden = (Int16)(normatividad.c_nivel_normatividad.Max(c => c.no_orden) + 1);

            db.c_nivel_normatividad.Add(c_nivel_normatividad);
            db.SaveChanges();

            return 1;
        }
        #endregion

        #region Edit
        public ActionResult EditLVL(int? id)
        {
            c_nivel_normatividad c_nivel_normatividad = db.c_nivel_normatividad.Find(id);

            ViewBag.nb_normatividad = c_nivel_normatividad.c_normatividad.nb_normatividad;

            return PartialView("Niveles/Edit", c_nivel_normatividad);
        }

        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        public int EditLVLP(c_nivel_normatividad c_nivel_normatividad)
        {
            db.Entry(c_nivel_normatividad).State = EntityState.Modified;
            db.SaveChanges();
            return 1;
        }


        #endregion

        #region Delete
        public ActionResult DeleteLVL(int? id)
        {
            c_normatividad norm = db.c_normatividad.Find(id);
            var maxOrder = norm.c_nivel_normatividad.Max(n => n.no_orden);
            var nivel = norm.c_nivel_normatividad.Where(n => n.no_orden == maxOrder).First();

            ViewBag.nb_normatividad = norm.nb_normatividad;
            if (nivel.no_orden == 0)
            {
                ViewBag.Error = "No se puede borrar el nivel 0 de la normatividad";
            }

            return PartialView("Niveles/Delete", nivel);
        }

        [HttpPost, ActionName("DeleteLVLP"), ValidateAntiForgeryToken, NotOnlyRead]
        public int DeletelvlConfirmed(int id)
        {
            c_nivel_normatividad c_nivel_normatividad = db.c_nivel_normatividad.Find(id);

            if (c_nivel_normatividad.no_orden == 0)
            {
                c_normatividad c_normatividad = db.c_normatividad.Find(c_nivel_normatividad.id_normatividad);
                ViewBag.nb_normatividad = c_normatividad.nb_normatividad;
                ViewBag.Error = "No se puede borrar el nivel 0 de la normatividad";
                return 0;
            }

            if (Utilidades.DeleteActions.DeleteNivelNormatividadObjects(c_nivel_normatividad, db))
            {
                db.c_nivel_normatividad.Remove(c_nivel_normatividad);
                db.SaveChanges();
            }
            return 1;
        }

        #endregion
        #endregion

        #region Contenido Normatividad
        public ActionResult Content(int id)
        {
            var nivel = db.c_nivel_normatividad.Find(id);
            ViewBag.title = Strings.getMSG("Contenidos del nivel") + nivel.nb_nivel_normatividad + Strings.getMSG("de la normatividad") + nivel.c_normatividad.nb_normatividad;
            var model = nivel.c_contenido_normatividad.ToList();

            ViewBag.nivelesL = Utilidades.DropDown.NivelesNormatividad(nivel.id_normatividad);

            return PartialView("Contenido/Index", model);
        }

        public ActionResult DescendantContent(int id)
        {
            var contenido = db.c_contenido_normatividad.Find(id);
            var root = Utilidades.Utilidades.getRoot(db, contenido);
            ViewBag.title = Strings.getMSG("ConsultaNormatividadIndex002") + contenido.cl_contenido_normatividad + Strings.getMSG("de la normatividad") + root.ds_contenido_normatividad;
            var model = contenido.c_contenido_normatividad1.ToList();

            if (contenido.id_contenido_normatividad_padre != null)
            {
                ViewBag.id_abuelo = contenido.id_contenido_normatividad_padre;
            }

            ViewBag.id_padre = id;

            ViewBag.nivelesL = Utilidades.DropDown.NivelesNormatividad(contenido.c_nivel_normatividad.id_normatividad);

            return PartialView("Contenido/Index", model);
        }

        #region Documentos
        public FileResult DescargaContenido(int id)
        {
            var cont = db.c_contenido_normatividad.Find(id);


            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Archivos/Temp" + id);
            var bytes = Utilidades.GenerateDoc.ContenidoNormatiidad(cont);


            return File(bytes, "application/pdf", "Contenido " + cont.cl_contenido_normatividad + ".pdf");
        }



        #endregion

        #region Create
        public ActionResult CreateC(int? id)
        {
            c_contenido_normatividad c_contenido_normatividad = db.c_contenido_normatividad.Find(id);

            c_contenido_normatividad model = new c_contenido_normatividad();
            c_nivel_normatividad nivel = db.c_nivel_normatividad.Where(n => n.no_orden == (c_contenido_normatividad.c_nivel_normatividad.no_orden + 1) && n.id_normatividad == c_contenido_normatividad.c_nivel_normatividad.id_normatividad).First();

            model.id_contenido_normatividad_padre = c_contenido_normatividad.id_contenido_normatividad;

            model.id_nivel_normatividad = nivel.id_nivel_normatividad;

            ViewBag.cumplimientoL = Utilidades.DropDown.CumplimientoNormatividad();
            ViewBag.areaL = Utilidades.DropDown.AreaNormatividad();
            ViewBag.frecuenciaL = Utilidades.DropDown.FrecuenciaNormatividad();
            ViewBag.comiteL = Utilidades.DropDown.ComiteNormatividad();
            ViewBag.obligacionL = Utilidades.DropDown.ObligacionNormatividad();

            ViewBag.id_impacto_monetario = new SelectList(db.c_impacto_monetario.OrderBy(c => c.cl_impacto_monetario), "id_impacto_monetario", "nb_impacto_monetario");
            ViewBag.id_factibilidad = new SelectList(db.c_factibilidad.OrderBy(c => c.cl_factibilidad), "id_factibilidad", "nb_factibilidad");

            ViewBag.requiere_ficha = Utilidades.DropDown.RequiereFicha();
            //ViewBag.requiere_ficha = new SelectList(db.c_requiere_ficha, "id_requiere_ficha", "nb_requiere_ficha");

            //Enviar todos los datos de la tabla de Criticidad
            ViewBag.Criticidad1 = db.c_criticidad1.ToList();

            ViewBag.nb_nivel = nivel.nb_nivel_normatividad;
            ViewBag.padre = c_contenido_normatividad.cl_contenido_normatividad + " - " + c_contenido_normatividad.ds_contenido_normatividad;
            ViewBag.abuelo = db.c_contenido_normatividad.Find(model.id_contenido_normatividad_padre).id_contenido_normatividad_padre;

            return PartialView("Contenido/Create", model);
        }

        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        public int CreateCP(c_contenido_normatividad model)
        {
            foreach (int id_func in model.id_cumplimiento)
            {
                c_cumplimiento f = db.c_cumplimiento.Find(id_func);
                model.c_cumplimiento.Add(f);
            }

            foreach (int id_func in model.id_area)
            {
                c_area f = db.c_area.Find(id_func);
                model.c_area.Add(f);
            }

            foreach (int id_func in model.id_frecuencia)
            {
                c_frecuencia f = db.c_frecuencia.Find(id_func);
                model.c_frecuencia.Add(f);
            }

            foreach (int id_func in model.id_comite)
            {
                c_comite f = db.c_comite.Find(id_func);
                model.c_comite.Add(f);
            }

            foreach (int id_func in model.id_obligacion)
            {
                c_obligacion f = db.c_obligacion.Find(id_func);
                model.c_obligacion.Add(f);
            }

            db.c_contenido_normatividad.Add(model);
            db.SaveChanges();

            ViewBag.cumplimientoL = Utilidades.DropDown.CumplimientoNormatividad();
            ViewBag.areaL = Utilidades.DropDown.AreaNormatividad();
            ViewBag.frecuenciaL = Utilidades.DropDown.FrecuenciaNormatividad();
            ViewBag.comiteL = Utilidades.DropDown.ComiteNormatividad();
            ViewBag.obligacionL = Utilidades.DropDown.ObligacionNormatividad();

            ViewBag.id_impacto_monetario = new SelectList(db.c_impacto_monetario.OrderBy(c => c.cl_impacto_monetario), "id_impacto_monetario", "nb_impacto_monetario");
            ViewBag.id_factibilidad = new SelectList(db.c_factibilidad.OrderBy(c => c.cl_factibilidad), "id_factibilidad", "nb_factibilidad");

            ViewBag.requiere_ficha = Utilidades.DropDown.RequiereFicha();
            //ViewBag.requiere_ficha = new SelectList(db.c_requiere_ficha, "id_requiere_ficha", "nb_requiere_ficha", model.requiere_ficha);

            //Enviar todos los datos de la tabla de Criticidad
            ViewBag.Criticidad1 = db.c_criticidad1.ToList();

            return 1;
        }

        #endregion

        #region Edit
        public ActionResult EditC(int? id)
        {
            c_contenido_normatividad c_contenido_normatividad = db.c_contenido_normatividad.Find(id);

            ViewBag.id_impacto_monetario = new SelectList(db.c_impacto_monetario.OrderBy(c => c.cl_impacto_monetario), "id_impacto_monetario", "nb_impacto_monetario",c_contenido_normatividad.id_contenido_normatividad);
            ViewBag.id_factibilidad = new SelectList(db.c_factibilidad.OrderBy(c => c.cl_factibilidad), "id_factibilidad", "nb_factibilidad",c_contenido_normatividad.id_contenido_normatividad);

            ViewBag.requiere_ficha = Utilidades.DropDown.RequiereFicha(c_contenido_normatividad.id_contenido_normatividad);
            //ViewBag.requiere_ficha = new SelectList(db.c_requiere_ficha, "id_requiere_ficha", "nb_requiere_ficha", c_contenido_normatividad.requiere_ficha);

            string sql = "select id_cumplimiento from c_contenido_normatividad_cumplimiento where id_contenido_normatividad = " + c_contenido_normatividad.id_contenido_normatividad;
            var funciones = db.Database.SqlQuery<int>(sql).ToArray();
            ViewBag.cumplimientoL = Utilidades.DropDown.CumplimientoNormatividad(funciones);

            string sq = "select id_area from c_contenido_normatividad_area where id_contenido_normatividad = " + c_contenido_normatividad.id_contenido_normatividad;
            var area = db.Database.SqlQuery<int>(sq).ToArray();
            ViewBag.areaL = Utilidades.DropDown.AreaNormatividad(area);

            string s = "select id_frecuencia from c_contenido_normatividad_frecuencia where id_contenido_normatividad = " + c_contenido_normatividad.id_contenido_normatividad;
            var frecuencia = db.Database.SqlQuery<int>(s).ToArray();
            ViewBag.frecuenciaL = Utilidades.DropDown.FrecuenciaNormatividad(frecuencia);

            string sqls = "select id_comite from c_contenido_normatividad_comite where id_contenido_normatividad = " + c_contenido_normatividad.id_contenido_normatividad;
            var comite = db.Database.SqlQuery<int>(sqls).ToArray();
            ViewBag.comiteL = Utilidades.DropDown.ComiteNormatividad(comite);

            string sqlss = "select id_obligacion from c_contenido_normatividad_obligacion where id_contenido_normatividad = " + c_contenido_normatividad.id_contenido_normatividad;
            var obligacion = db.Database.SqlQuery<int>(sqlss).ToArray();
            ViewBag.obligacionL = Utilidades.DropDown.ObligacionNormatividad(obligacion);

            //Enviar todos los datos de la tabla de Criticidad
            ViewBag.Criticidad1 = db.c_criticidad1.ToList();

            return PartialView("Contenido/Edit", c_contenido_normatividad);
        }

        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        public int EditCP(c_contenido_normatividad model)
        {
           
            db.Entry(model).State = EntityState.Modified;
                
            c_contenido_normatividad c_contenido_normatividad = db.c_contenido_normatividad.Find(model.id_contenido_normatividad);

            //Si se modifica el nivel raíz, Modificar la clave y nombre de la normatividad Completa
            if (model.id_contenido_normatividad_padre == null)
            {
                var norm = db.c_nivel_normatividad.Find(model.id_nivel_normatividad).c_normatividad;

                norm.cl_normatividad = model.cl_contenido_normatividad;
                norm.nb_normatividad = model.ds_contenido_normatividad;
                db.Entry(norm).State = EntityState.Modified;              
            }

            if (model.id_cumplimiento != null)
            {
                c_contenido_normatividad.c_cumplimiento.Clear();
                             
                foreach (int id_func in model.id_cumplimiento)
                {
                    c_cumplimiento f = db.c_cumplimiento.Find(id_func);
                    c_contenido_normatividad.c_cumplimiento.Add(f);              
                }
            }

            if (model.id_area != null)
            {              
                c_contenido_normatividad.c_area.Clear();

                foreach (int id_func in model.id_area)
                {
                    c_area f = db.c_area.Find(id_func);
                    c_contenido_normatividad.c_area.Add(f);                 
                }
            }

            if (model.id_frecuencia != null)
            {
                c_contenido_normatividad.c_frecuencia.Clear();

                foreach (int id_func in model.id_frecuencia)
                {
                    c_frecuencia f = db.c_frecuencia.Find(id_func);
                    c_contenido_normatividad.c_frecuencia.Add(f);
                }
            }

            if (model.id_comite != null)
            {
                c_contenido_normatividad.c_comite.Clear();

                foreach (int id_func in model.id_comite)
                {
                    c_comite f = db.c_comite.Find(id_func);
                    c_contenido_normatividad.c_comite.Add(f);
                }
            }

            if (model.id_obligacion != null)
            {
                c_contenido_normatividad.c_obligacion.Clear();

                foreach (int id_func in model.id_obligacion)
                {
                    c_obligacion f = db.c_obligacion.Find(id_func);
                    c_contenido_normatividad.c_obligacion.Add(f);
                }
            }

            db.Database.ExecuteSqlCommand("Delete from c_contenido_normatividad_area where id_contenido_normatividad = "+ model.id_contenido_normatividad);
            db.Database.ExecuteSqlCommand("Delete  from c_contenido_normatividad_cumplimiento where  id_contenido_normatividad = "+ model.id_contenido_normatividad);
            db.Database.ExecuteSqlCommand("Delete  from c_contenido_normatividad_frecuencia where  id_contenido_normatividad = " + model.id_contenido_normatividad);
            db.Database.ExecuteSqlCommand("Delete  from c_contenido_normatividad_comite where  id_contenido_normatividad = " + model.id_contenido_normatividad);
            db.Database.ExecuteSqlCommand("Delete  from c_contenido_normatividad_obligacion where  id_contenido_normatividad = " + model.id_contenido_normatividad);

            db.SaveChanges();        

            ViewBag.requiere_ficha = Utilidades.DropDown.RequiereFicha(model.id_contenido_normatividad);
            // ViewBag.requiere_ficha = new SelectList(db.c_requiere_ficha, "id_requiere_ficha", "nb_requiere_ficha", model.requiere_ficha);

            ViewBag.id_impacto_monetario = new SelectList(db.c_impacto_monetario.OrderBy(c => c.cl_impacto_monetario), "id_impacto_monetario", "nb_impacto_monetario",model.id_contenido_normatividad);
            ViewBag.id_factibilidad = new SelectList(db.c_factibilidad.OrderBy(c => c.cl_factibilidad), "id_factibilidad", "nb_factibilidad",model.id_contenido_normatividad);

            string sql = "select id_cumplimiento from c_contenido_normatividad_cumplimiento where id_contenido_normatividad = " + model.id_contenido_normatividad;
            var funciones = db.Database.SqlQuery<int>(sql).ToArray();
            ViewBag.cumplimientoL = Utilidades.DropDown.CumplimientoNormatividad(funciones);

            //Enviar todos los datos de la tabla de Criticidad
            ViewBag.Criticidad1 = db.c_criticidad1.ToList();

            return 1;
        }
        #endregion

        #region Delete
        public ActionResult DeleteC(int? id)
        {
            c_contenido_normatividad c_contenido_normatividad = db.c_contenido_normatividad.Find(id);

            if (c_contenido_normatividad.id_contenido_normatividad_padre == null)
            {
                ViewBag.Error = "No se puede eliminar un contenido raíz, se debe borrar toda la normatividad en en la tabla de \"Normatividades\"";
            }

            return PartialView("Contenido/Delete", c_contenido_normatividad);
        }

        [HttpPost, ActionName("DeleteCP"), ValidateAntiForgeryToken, NotOnlyRead]
        public int DeleteCConfirmed(int id)
        {
            c_contenido_normatividad contenido = db.c_contenido_normatividad.Find(id);

            contenido.c_cumplimiento.Clear();
            contenido.c_area.Clear();
            contenido.c_frecuencia.Clear();
            contenido.c_comite.Clear();
            contenido.c_obligacion.Clear();

            //Borramos todos los niveles inferiores
            int id_padre = (int)contenido.id_contenido_normatividad_padre;

            if (contenido.id_contenido_normatividad_padre == null)
            {
                return 0;
            }

           
            Utilidades.DeleteActions.DeleteContenidoNormatividadObjects(contenido, db);
            db.SaveChanges();

            return 1;
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

            return PartialView("Contenido/LigarSP");
        }

        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
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

        #endregion



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult infoMI()
        {
            return PartialView("DetailViews/infoMI");
        }
    }
}
