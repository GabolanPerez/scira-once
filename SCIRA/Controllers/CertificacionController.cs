using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "Certificacion", ModuleCode = "MSICI004")]
    [CustomErrorHandler]
    public class CertificacionController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            try
            {
                var user = (IdentityPersonalizado)User.Identity;
                var us = db.c_usuario.Find(user.Id_usuario);

                var controles = Utilidades.Utilidades.RTCObject(us, db, "k_control", "1").Cast<k_control>().ToList();

                //var controles = new List<k_control>();

                //if (us.es_super_usuario)
                //{
                //    controles = db.k_control.ToList();
                //    ViewBag.su = 1;
                //}
                //else
                //{
                //    controles = us.k_control1.ToList();
                //}

                return View(controles);
            }
            catch
            {
                return View("Error");
            }

        }

        #region Create
        public ActionResult Certificados(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_control k_control = db.k_control.Find(id);
            if (k_control == null)
            {
                return HttpNotFound();
            }
            ViewBag.Control = k_control.relacion_control;
            ViewBag.id_control = k_control.id_control;

            return View(db.k_certificacion_control.Include(c => c.c_periodo_certificacion).Where(c => c.id_control == k_control.id_control).ToList());
        }


        // GET: Certificacion/Certificar
        //el id pertenece a un control
        public ActionResult Certificar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_control k_control = db.k_control.Find(id);
            if (k_control == null)
            {
                return HttpNotFound();
            }

            c_naturaleza_control naturaleza = k_control.c_naturaleza_control;
            c_frecuencia_control frecuencia = k_control.c_frecuencia_control;

            int id_naturaleza = naturaleza.id_naturaleza_control;
            int id_frecuencia = frecuencia.id_frecuencia_control;

            var c_prueba_auto_eval = db.c_prueba_auto_eval.Where(p => p.id_frecuencia_control == id_frecuencia && p.id_naturaleza_control == id_naturaleza).First();

            CertificacionControlViewModel model = new CertificacionControlViewModel();
            model.id_control = k_control.id_control;
            model.no_partidas_minimo = c_prueba_auto_eval.no_partidas_minimo;
            model.no_partidas_semestre1 = c_prueba_auto_eval.no_partidas_semestre1;
            model.no_partidas_semestre2 = c_prueba_auto_eval.no_partidas_semestre2;

            try
            {
                c_periodo_certificacion c_periodo_certificacion = db.c_periodo_certificacion.Where(p => p.esta_activo == true).First();
                ViewBag.nb_periodo_certificacion = c_periodo_certificacion.nb_periodo_certificacion + " - " + c_periodo_certificacion.anio;
                model.id_periodo_certificacion = c_periodo_certificacion.id_periodo_certificacion;
            }
            catch
            {
                ViewBag.nb_periodo_certificacion = Strings.getMSG("EvaluacionIndicadorCreate006");
                ViewBag.Error = Strings.getMSG("CertificacionCertificar034");
            }

            ViewBag.id_tipo_evaluacion = new SelectList(db.c_tipo_evaluacion, "id_tipo_evaluacion", "nb_tipo_evaluacion");
            ViewBag.nb_control = k_control.relacion_control;

            //Mandamos las listas necesarias para la creacion de la incidencia en caso de ser necesaria
            ViewBag.id_responsable_i = Utilidades.DropDown.Usuario();
            ViewBag.id_clasificacion_incidencia = Utilidades.DropDown.ClasificacionIncidencia();

            ViewBag.rrData = RRData(k_control);

            return View(model);
        }

        // POST: Certificacion/Certificar
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Certificar(CertificacionControlViewModel model, HttpPostedFileBase file1, HttpPostedFileBase file2, HttpPostedFileBase file3, HttpPostedFileBase file4, HttpPostedFileBase file5)
        {
            k_control k_control = db.k_control.Find(model.id_control);

            c_naturaleza_control naturaleza = k_control.c_naturaleza_control;
            c_frecuencia_control frecuencia = k_control.c_frecuencia_control;

            int id_naturaleza = naturaleza.id_naturaleza_control;
            int id_frecuencia = frecuencia.id_frecuencia_control;

            var c_prueba_auto_eval = db.c_prueba_auto_eval.Where(p => p.id_frecuencia_control == id_frecuencia && p.id_naturaleza_control == id_naturaleza).First();

            model.no_partidas_minimo = c_prueba_auto_eval.no_partidas_minimo;
            model.no_partidas_semestre1 = c_prueba_auto_eval.no_partidas_semestre1;
            model.no_partidas_semestre2 = c_prueba_auto_eval.no_partidas_semestre2;
            model.fe_registro = DateTime.Now;

            bool valid = ValidarIncidencia(ModelState, model);

            if (ModelState.IsValid && valid)
            {

                //llenar k_certificacion_control a partir de un View Model
                var certificacion = getCertificacionFromViewModel(model);


                db.k_certificacion_control.Add(certificacion);
                db.SaveChanges();
                SaveFiles(certificacion, file1, file2, file3, file4, file5);

                if (!certificacion.tiene_funcionamiento_efectivo || !certificacion.tiene_disenio_efectivo)
                {
                    //Agregamos la Incidencia
                    var incidencia = new k_incidencia()
                    {
                        id_certificacion_control = certificacion.id_certificacion_control,
                        id_responsable = (int)model.id_responsable_i,
                        id_clasificacion_incidencia = (int)model.id_clasificacion_incidencia,
                        ds_incidencia = model.ds_incidencia,
                        requiere_plan = model.requiere_plan,
                        js_incidencia = model.js_incidencia
                    };

                    db.k_incidencia.Add(incidencia);
                    db.SaveChanges();
                    Utilidades.Notification.IncidenciaAsignada(incidencia);
                }


                Utilidades.Utilidades.refreshNotifCount((int)k_control.id_responsable);
                Utilidades.Utilidades.removeRow(1, k_control.id_control, (int)k_control.id_responsable);

                return RedirectToAction("Certificados", new { id = model.id_control });
            }

            try
            {
                c_periodo_certificacion c_periodo_certificacion = db.c_periodo_certificacion.Where(p => p.esta_activo == true).First();
                ViewBag.nb_periodo_certificacion = c_periodo_certificacion.nb_periodo_certificacion + " - " + c_periodo_certificacion.anio;
                model.id_periodo_certificacion = c_periodo_certificacion.id_periodo_certificacion;
            }
            catch
            {
                ViewBag.nb_periodo_certificacion = "No se encontró ningún periodo activo";
                ViewBag.Error = "No se puede certificar el control, ya que no existen periodos activos";
            }

            model.id_control = k_control.id_control;
            ViewBag.id_tipo_evaluacion = new SelectList(db.c_tipo_evaluacion, "id_tipo_evaluacion", "nb_tipo_evaluacion");
            ViewBag.nb_control = k_control.relacion_control;

            //Mandamos las listas necesarias para la creacion de la incidencia en caso de ser necesaria
            ViewBag.id_responsable_i = Utilidades.DropDown.Usuario();
            ViewBag.id_clasificacion_incidencia = Utilidades.DropDown.ClasificacionIncidencia();

            ViewBag.rrData = RRData(k_control);

            return View(model);
        }
        #endregion

        #region Edit
        // GET: Certificacion/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_certificacion_control k_certificacion_control = db.k_certificacion_control.Find(id);
            if (k_certificacion_control == null)
            {
                return HttpNotFound();
            }

            c_periodo_certificacion periodo = db.c_periodo_certificacion.Find(k_certificacion_control.id_periodo_certificacion);

            ViewBag.nb_periodo_certificacion = periodo.nb_periodo_certificacion + " - " + periodo.anio;
            ViewBag.id_tipo_evaluacion = new SelectList(db.c_tipo_evaluacion, "id_tipo_evaluacion", "nb_tipo_evaluacion", k_certificacion_control.id_tipo_evaluacion);
            ViewBag.nb_control = k_certificacion_control.k_control.relacion_control;

            ViewBag.nb_a1 = k_certificacion_control.nb_archivo_1;
            ViewBag.nb_a2 = k_certificacion_control.nb_archivo_2;
            ViewBag.nb_a3 = k_certificacion_control.nb_archivo_3;
            ViewBag.nb_a4 = k_certificacion_control.nb_archivo_4;
            ViewBag.nb_a5 = k_certificacion_control.nb_archivo_5;

            return View(k_certificacion_control);
        }

        // POST: Certificacion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(k_certificacion_control model, HttpPostedFileBase file1, HttpPostedFileBase file2, HttpPostedFileBase file3, HttpPostedFileBase file4, HttpPostedFileBase file5)
        {
            if (ModelState.IsValid)
            {
                model.fe_registro = DateTime.Now;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                SaveFiles(model, file1, file2, file3, file4, file5, true);
                return RedirectToAction("Certificados", new { id = model.id_control });
            }

            ViewBag.id_tipo_evaluacion = new SelectList(db.c_tipo_evaluacion, "id_tipo_evaluacion", "nb_tipo_evaluacion", model.id_tipo_evaluacion);

            ViewBag.nb_control = db.k_control.Find(model.id_control).relacion_control;

            ViewBag.nb_a1 = model.nb_archivo_1;
            ViewBag.nb_a2 = model.nb_archivo_2;
            ViewBag.nb_a3 = model.nb_archivo_3;
            ViewBag.nb_a4 = model.nb_archivo_4;
            ViewBag.nb_a5 = model.nb_archivo_5;

            return View(model);
        }
        #endregion

        #region Delete
        // GET: Certificacion/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_certificacion_control k_certificacion_control = db.k_certificacion_control.Find(id);
            if (k_certificacion_control == null)
            {
                return HttpNotFound();
            }

            Utilidades.DeleteActions.checkRedirect(redirect); //subrutina para saber si hay que volver a alguna pantalla

            ViewBag.nb_control = k_certificacion_control.k_control.relacion_control;
            return View(k_certificacion_control);
        }

        // POST: Certificacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            k_certificacion_control k_certificacion_control = db.k_certificacion_control.Find(id);
            int id_control = k_certificacion_control.id_control;
            bool objects = Utilidades.DeleteActions.DeleteCertificacionObjects(k_certificacion_control, db);
            if (objects) try
                {
                    db.k_certificacion_control.Remove(k_certificacion_control);
                    db.SaveChanges();

                }
                catch (Exception e)
                {
                    Debug.Write(e.Message);
                    return RedirectToAction("CantErase", "Error", null);
                }

            //En caso de que el registro se haya eliminado correctamente, redireccionar dependiendo desde donde se haya accesado al menú de eliminar
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
                return RedirectToAction("Certificados", new { id = id_control });

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
                    return RedirectToAction("Certificados", new { id = id_control });
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




        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Otros
        private bool ValidarIncidencia(ModelStateDictionary modelState, CertificacionControlViewModel m1)
        {
            bool valid = true;

            //La incidencia se valida solo en caso de no contar con diseño o funcionamiento efectivos
            if (!m1.tiene_disenio_efectivo || !m1.tiene_funcionamiento_efectivo)
            {
                if (m1.ds_incidencia == null)
                {
                    modelState.AddModelError("ds_incidencia", Strings.getMSG("IndicadorDiarioCreate004"));
                    valid = false;
                }
                if (m1.id_responsable_i <= 0 || m1.id_responsable_i == null)
                {
                    modelState.AddModelError("id_responsable_i", Strings.getMSG("RiesgoCreate065"));
                    valid = false;
                }
                if (m1.id_clasificacion_incidencia <= 0 || m1.id_clasificacion_incidencia == null)
                {
                    modelState.AddModelError("id_clasificacion_incidencia", Strings.getMSG("RiesgoCreate066"));
                    valid = false;
                }
                if (!m1.requiere_plan)
                {
                    if (m1.js_incidencia == null)
                    {
                        modelState.AddModelError("js_incidencia", Strings.getMSG("IndicadorDiarioCreate004"));
                        valid = false;
                    }
                }
            }

            return valid;
        }

        public k_certificacion_control getCertificacionFromViewModel(CertificacionControlViewModel vModel)
        {
            var certificacion = new k_certificacion_control();
            certificacion.cl_certificacion_control = vModel.cl_certificacion_control;
            certificacion.id_control = vModel.id_control;
            certificacion.id_periodo_certificacion = vModel.id_periodo_certificacion;
            certificacion.ds_procedimiento_certificacion = vModel.ds_procedimiento_certificacion;
            certificacion.id_tipo_evaluacion = vModel.id_tipo_evaluacion;
            certificacion.no_partidas_minimo = vModel.no_partidas_minimo;
            certificacion.no_partidas_semestre1 = vModel.no_partidas_semestre1;
            certificacion.no_partidas_semestre2 = vModel.no_partidas_semestre2;
            certificacion.no_pruebas_realizadas = vModel.no_pruebas_realizadas;
            certificacion.tiene_funcionamiento_efectivo = vModel.tiene_funcionamiento_efectivo;
            certificacion.tiene_disenio_efectivo = vModel.tiene_disenio_efectivo;
            certificacion.ds_plan_remediacion = vModel.ds_plan_remediacion;
            certificacion.fe_registro = vModel.fe_registro;


            return certificacion;
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_certificacion_control k_certificacion_control = db.k_certificacion_control.Find(id);
            if (k_certificacion_control == null)
            {
                return HttpNotFound();
            }

            ViewBag.nb_control = k_certificacion_control.k_control.relacion_control;
            return View(k_certificacion_control);
        }

        private bool SaveFiles(k_certificacion_control certificacion, HttpPostedFileBase file1, HttpPostedFileBase file2, HttpPostedFileBase file3, HttpPostedFileBase file4, HttpPostedFileBase file5, bool edit = false)
        {
            Type m_tipo = certificacion.GetType();
            PropertyInfo[] m_propiedades = m_tipo.GetProperties();
            HttpPostedFileBase[] files = { file1, file2, file3, file4, file5 };
            //m_propiedades[13] archivo 1



            for (int i = 1; i < 6; i++)
            {
                string nombre = "ac" + i + "-" + certificacion.id_certificacion_control;
                var prop = m_propiedades.Where(p => p.Name == "nb_archivo_" + i).First();

                if (files[i - 1] != null)
                {
                    files[i - 1].SaveAs(Server.MapPath("~/App_Data/Certificacion/" + nombre));
                    prop.SetValue(certificacion, files[i - 1].FileName);
                }
                else
                {
                    if (edit)
                    {
                        if (prop.GetValue(certificacion, null) == null)
                        {
                            string path = Server.MapPath("~/App_Data/Certificacion/" + nombre);
                            System.IO.File.Delete(path);
                        }
                    }
                }
            }

            db.SaveChanges();

            return true;
        }

        [NotOnlyRead]
        public ActionResult DescargaArchivo(int id, int index)
        {
            k_certificacion_control certificacion = db.k_certificacion_control.Find(id);
            Type m_tipo = certificacion.GetType();
            PropertyInfo[] m_propiedades = m_tipo.GetProperties();
            var prop = m_propiedades.Where(p => p.Name == "nb_archivo_" + index).First();


            string contentType = System.Net.Mime.MediaTypeNames.Application.Octet;
            return new FilePathResult("~/App_Data/Certificacion/ac" + index + "-" + id, contentType)
            {
                FileDownloadName = (string)prop.GetValue(certificacion, null),
            };
        }


        private string[] RRData(k_control control)
        {
            string[] data = new string[2];
            if (control.k_riesgo_residual.Count > 0)
            {
                var Model = db.a_campo_cobertura_control.ToList();
                var rr = control.k_riesgo_residual.First();


                decimal aux11 = 0; a_campo_cobertura_control campo11 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };
                decimal aux12 = 0; a_campo_cobertura_control campo12 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };
                decimal aux13 = 0; a_campo_cobertura_control campo13 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };
                decimal aux14 = 0; a_campo_cobertura_control campo14 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };
                decimal aux15 = 0; a_campo_cobertura_control campo15 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };
                decimal aux16 = 0; a_campo_cobertura_control campo16 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };
                decimal aux17 = 0; a_campo_cobertura_control campo17 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };
                decimal aux18 = 0; a_campo_cobertura_control campo18 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };
                decimal aux19 = 0; a_campo_cobertura_control campo19 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };

                decimal aux21 = 0; a_campo_cobertura_control campo21 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };
                decimal aux22 = 0; a_campo_cobertura_control campo22 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };
                decimal aux23 = 0; a_campo_cobertura_control campo23 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };
                decimal aux24 = 0; a_campo_cobertura_control campo24 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };
                decimal aux25 = 0; a_campo_cobertura_control campo25 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };

                if (Model.Any(c => c.cl_catalogo == 1 && c.cl_campo == 1))
                {
                    aux11 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 1).Max(c => c.valor);
                    campo11 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 1 && c.valor == aux11).First();
                }


                if (Model.Any(c => c.cl_catalogo == 1 && c.cl_campo == 2))
                {
                    aux12 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 2).Max(c => c.valor);
                    campo12 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 2 && c.valor == aux12).First();
                }

                if (Model.Any(c => c.cl_catalogo == 1 && c.cl_campo == 3))
                {
                    aux13 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 3).Max(c => c.valor);
                    campo13 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 3 && c.valor == aux13).First();
                }

                if (Model.Any(c => c.cl_catalogo == 1 && c.cl_campo == 4))
                {
                    aux14 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 4).Max(c => c.valor);
                    campo14 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 4 && c.valor == aux14).First();
                }

                if (Model.Any(c => c.cl_catalogo == 1 && c.cl_campo == 5))
                {
                    aux15 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 5).Max(c => c.valor);
                    campo15 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 5 && c.valor == aux15).First();
                }

                if (Model.Any(c => c.cl_catalogo == 1 && c.cl_campo == 6))
                {
                    aux16 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 6).Max(c => c.valor);
                    campo16 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 6 && c.valor == aux16).First();
                }

                if (Model.Any(c => c.cl_catalogo == 1 && c.cl_campo == 7))
                {
                    aux17 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 7).Max(c => c.valor);
                    campo17 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 7 && c.valor == aux17).First();
                }

                if (Model.Any(c => c.cl_catalogo == 1 && c.cl_campo == 8))
                {
                    aux18 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 8).Max(c => c.valor);
                    campo18 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 8 && c.valor == aux18).First();
                }

                if (Model.Any(c => c.cl_catalogo == 1 && c.cl_campo == 9))
                {
                    aux19 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 9).Max(c => c.valor);
                    campo19 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 9 && c.valor == aux19).First();
                }

                if (Model.Any(c => c.cl_catalogo == 2 && c.cl_campo == 1))
                {
                    aux21 = Model.Where(c => c.cl_catalogo == 2 && c.cl_campo == 1).Max(c => c.valor);
                    campo21 = Model.Where(c => c.cl_catalogo == 2 && c.cl_campo == 1 && c.valor == aux21).First();
                }

                if (Model.Any(c => c.cl_catalogo == 2 && c.cl_campo == 2))
                {
                    aux22 = Model.Where(c => c.cl_catalogo == 2 && c.cl_campo == 2).Max(c => c.valor);
                    campo22 = Model.Where(c => c.cl_catalogo == 2 && c.cl_campo == 2 && c.valor == aux22).First();
                }

                if (Model.Any(c => c.cl_catalogo == 2 && c.cl_campo == 3))
                {
                    aux23 = Model.Where(c => c.cl_catalogo == 2 && c.cl_campo == 3).Max(c => c.valor);
                    campo23 = Model.Where(c => c.cl_catalogo == 2 && c.cl_campo == 3 && c.valor == aux23).First();
                }

                if (Model.Any(c => c.cl_catalogo == 2 && c.cl_campo == 4))
                {
                    aux24 = Model.Where(c => c.cl_catalogo == 2 && c.cl_campo == 4).Max(c => c.valor);
                    campo24 = Model.Where(c => c.cl_catalogo == 2 && c.cl_campo == 4 && c.valor == aux24).First();
                }

                if (Model.Any(c => c.cl_catalogo == 2 && c.cl_campo == 5))
                {
                    aux25 = Model.Where(c => c.cl_catalogo == 2 && c.cl_campo == 5).Max(c => c.valor);
                    campo25 = Model.Where(c => c.cl_catalogo == 2 && c.cl_campo == 5 && c.valor == aux25).First();
                }

                var total1 = aux11 + aux12 + aux13 + aux14 + aux15 + aux16 + aux17 + aux18 + aux19;
                var total2 = aux21 + aux22 + aux23 + aux24 + aux25;

                var totalr1 = rr.a_campo_cobertura_control.valor
                    + rr.a_campo_cobertura_control1.valor
                    + rr.a_campo_cobertura_control2.valor
                    + rr.a_campo_cobertura_control3.valor
                    + rr.a_campo_cobertura_control4.valor
                    + rr.a_campo_cobertura_control5.valor
                    + rr.a_campo_cobertura_control6.valor
                    + rr.a_campo_cobertura_control7.valor
                    + rr.a_campo_cobertura_control8.valor;

                var totalr2 = rr.a_campo_cobertura_control9.valor
                    + rr.a_campo_cobertura_control10.valor
                    + rr.a_campo_cobertura_control11.valor
                    + rr.a_campo_cobertura_control12.valor
                    + rr.a_campo_cobertura_control13.valor;

                data[0] = string.Format((Strings.getMSG("CertificacionCertificar035")), totalr1, total1);
                data[1] = string.Format((Strings.getMSG("CertificacionCertificar036")), totalr2, total2);
            }
            else
            {
                data[0] = null;
                data[1] = null;
            }

            return data;
        }
        #endregion
    }
}
