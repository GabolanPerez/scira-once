using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "CertificacionEN", ModuleCode = "MSICI004")]
    [CustomErrorHandler]
    public class CertificacionEntidadController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: Actividad
        public ActionResult Index()
        {
            var user = (IdentityPersonalizado)User.Identity;
            var us = db.c_usuario.Find(user.Id_usuario);

            //regresa las entidades pro  tramo de control
            var entidades = Utilidades.Utilidades.RTCObject(us, db, "c_entidad").Cast<c_entidad>().OrderBy(x => x.cl_entidad).ToList();

            return View(entidades);
        }


        #region Certificación
        public ActionResult Certificados(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_entidad c_entidad = db.c_entidad.Find(id);
            if (c_entidad == null)
            {
                return HttpNotFound();
            }
            ViewBag.Entidad = c_entidad;

            c_periodo_certificacion c_periodo_certificacion = db.c_periodo_certificacion.Where(p => p.esta_activo == true).FirstOrDefault();

            var macroProcesos = c_entidad.c_macro_proceso;
            bool puedeCertificar = true;
            var noMP = macroProcesos.Count();
            var noMPC = 0; //Número de macroprocesos certificados

            if (c_periodo_certificacion != null)
            {
                //Encontrar cuantos macro procesos están certificados en el periodo actual
                foreach (var mp in macroProcesos)
                {
                    if (mp.k_certificacion_estructura.Any(c => c.id_periodo_certificacion == c_periodo_certificacion.id_periodo_certificacion))
                        noMPC++;
                }
            }

            bool CertificacionSegura = Utilidades.Utilidades.GetBoolSecurityProp("CertificacionSegura", "true");

            if (CertificacionSegura)
            {
                //si la certificación está en módo segura
                //Solo se puede certificar una vez se haya certificado todo su nivel inferior
                puedeCertificar = noMP == noMPC;
            }

            puedeCertificar = puedeCertificar && c_periodo_certificacion != null;

            ViewBag.mpc = noMPC; //macro procesos certificados
            ViewBag.mpnc = noMP - noMPC; //macro procesos no certificados
            ViewBag.periodo = c_periodo_certificacion; //enviar periodo actual (si existiera)
            ViewBag.puedeCertificar = puedeCertificar;
            ViewBag.CertificacionSegura = CertificacionSegura;

            return View(c_entidad.k_certificacion_estructura);
        }
        #endregion


        #region Create
        public ActionResult Certificar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_entidad c_entidad = db.c_entidad.Find(id);
            if (c_entidad == null)
            {
                return HttpNotFound();
            }

            c_periodo_certificacion c_periodo_certificacion = db.c_periodo_certificacion.Where(p => p.esta_activo == true).FirstOrDefault();

            var macroProcesos = c_entidad.c_macro_proceso;
            bool puedeCertificar = true;
            var noMP = macroProcesos.Count();
            var noMPC = 0; //Número de macroprocesos certificados

            var nb_periodo = Strings.getMSG("EvaluacionIndicadorCreate006");

            if (c_periodo_certificacion != null)
            {
                //Encontrar cuantos macro procesos están certificados en el periodo actual
                foreach (var mp in macroProcesos)
                {
                    if (mp.k_certificacion_estructura.Any(c => c.id_periodo_certificacion == c_periodo_certificacion.id_periodo_certificacion))
                        noMPC++;
                }

                nb_periodo = c_periodo_certificacion.nb_periodo_certificacion + " - " + c_periodo_certificacion.anio;
            }
            else
            {
                ViewBag.Error = Strings.getMSG("CertificacionCertificar034");
            }



            bool CertificacionSegura = Utilidades.Utilidades.GetBoolSecurityProp("CertificacionSegura", "true");

            if (CertificacionSegura)
            {
                //si la certificación está en módo segura
                //Solo se puede certificar una vez se haya certificado todo su nivel inferior
                puedeCertificar = noMP == noMPC;
            }

            puedeCertificar = puedeCertificar && c_periodo_certificacion != null;

            if (!puedeCertificar)
                return RedirectToAction("Certificados", new { id = id });

            k_certificacion_estructura model = new k_certificacion_estructura();
            model.id_entidad = id;
            model.id_periodo_certificacion = c_periodo_certificacion.id_periodo_certificacion;
            model.leyenda_certificacion_estructura = Utilidades.Utilidades.GetSecurityProp("LeyendaCE", "Me he asegurado que los Macroprocesos que integran la " +
                "Entidad “XXXXX”, fueron certificados por cada uno " +
                "de sus Responsables y en el caso de existir incidencias " +
                "fueron señaladas oportunamente, además confirmo que " +
                "en esta Entidad no existen mas Macroprocesos relevantes " +
                "que no se hayan identificado y reflejado en la Matriz " +
                "de Riesgos y Controles correspondiente.");
            ViewBag.nb_periodo_certificacion = nb_periodo;
            ViewBag.entidad = c_entidad;

            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        public ActionResult Certificar(k_certificacion_estructura model, int[] files)
        {
            if (ModelState.IsValid)
            {
                model.cl_certificacion_estructura = "E";
                db.k_certificacion_estructura.Add(model);
                db.SaveChanges();


                model = db.k_certificacion_estructura.Find(model.id_certificacion_estructura);

                //agregar los archivos
                if (files != null)
                {
                    foreach (int file in files)
                    {
                        c_archivo archivo = db.c_archivo.Find(file);

                        model.c_archivo.Add(archivo);
                    }

                    db.SaveChanges();
                }

                var reg = db.c_entidad.Find(model.id_entidad);
                Utilidades.Utilidades.refreshNotifCount(reg.id_responsable);
                Utilidades.Utilidades.removeRow(12, model.id_entidad ?? 0, reg.id_responsable);

                return RedirectToAction("Certificados", new { id = model.id_entidad });
            }


            var periodo = db.c_periodo_certificacion.Find(model.id_periodo_certificacion);
            ViewBag.nb_periodo_certificacion = periodo.nb_periodo_certificacion + " - " + periodo.anio;
            ViewBag.entidad = db.c_entidad.Find(model.id_entidad);

            return View(model);
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_certificacion_estructura k_certificacion_estructura = db.k_certificacion_estructura.Find(id);
            if (k_certificacion_estructura == null)
            {
                return HttpNotFound();
            }

            c_periodo_certificacion c_periodo_certificacion = db.c_periodo_certificacion.Where(p => p.esta_activo == true).FirstOrDefault();

            if (c_periodo_certificacion != null)
            {
                if (k_certificacion_estructura.id_periodo_certificacion != c_periodo_certificacion.id_periodo_certificacion)
                    return RedirectToAction("Certificados", new { id = k_certificacion_estructura.id_entidad });
            }
            else
            {
                return RedirectToAction("Certificados", new { id = k_certificacion_estructura.id_entidad });
            }

            ViewBag.nb_periodo_certificacion = k_certificacion_estructura.c_periodo_certificacion.nb_periodo_certificacion + " - " + k_certificacion_estructura.c_periodo_certificacion.anio;
            ViewBag.entidad = k_certificacion_estructura.c_entidad;

            return View(k_certificacion_estructura);
        }


        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        public ActionResult Edit(k_certificacion_estructura model, int[] files)
        {
            if (ModelState.IsValid)
            {
                model.cl_certificacion_estructura = "E";
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();


                model = db.k_certificacion_estructura.Find(model.id_certificacion_estructura);

                //agregar los archivos
                if (files != null)
                {
                    foreach (int file in files)
                    {
                        c_archivo archivo = db.c_archivo.Find(file);

                        model.c_archivo.Add(archivo);
                    }

                    db.SaveChanges();
                }

                return RedirectToAction("Certificados", new { id = model.id_entidad });
            }


            var periodo = db.c_periodo_certificacion.Find(model.id_periodo_certificacion);
            var entidad = db.c_entidad.Find(model.id_entidad);
            ViewBag.nb_periodo_certificacion = periodo.nb_periodo_certificacion + " - " + periodo.anio;
            ViewBag.entidad = entidad;

            return View(model);
        }
        #endregion

        #region Delete
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_certificacion_estructura k_certificacion_estructura = db.k_certificacion_estructura.Find(id);
            if (k_certificacion_estructura == null)
            {
                return HttpNotFound();
            }

            Utilidades.DeleteActions.checkRedirect(redirect); //subrutina para saber si hay que volver a alguna pantalla

            return View(k_certificacion_estructura);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            k_certificacion_estructura k_certificacion_estructura = db.k_certificacion_estructura.Find(id);
            var id_entidad = k_certificacion_estructura.id_entidad;
            Utilidades.DeleteActions.DeleteCertificacionObjects(k_certificacion_estructura, db);
            db.k_certificacion_estructura.Remove(k_certificacion_estructura);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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
                return RedirectToAction("Certificados", new { id = id_entidad });

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
                    return RedirectToAction("Certificados", new { id = id_entidad });
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

        #region Info
        public ActionResult mpc(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_entidad c_entidad = db.c_entidad.Find(id);
            if (c_entidad == null)
            {
                return HttpNotFound();
            }

            c_periodo_certificacion c_periodo_certificacion = db.c_periodo_certificacion.Where(p => p.esta_activo == true).FirstOrDefault();

            var macroProcesos = c_entidad.c_macro_proceso;

            List<c_macro_proceso> mpc = new List<c_macro_proceso>();

            int idPeriodo = 0;

            if (c_periodo_certificacion != null)
            {
                //Encontrar cuantos macro procesos están certificados en el periodo actual
                foreach (var mp in macroProcesos)
                {
                    if (mp.k_certificacion_estructura.Any(c => c.id_periodo_certificacion == c_periodo_certificacion.id_periodo_certificacion))
                        mpc.Add(mp);
                }
                idPeriodo = c_periodo_certificacion.id_periodo_certificacion;
            }

            ViewBag.type = "certificados";
            ViewBag.idPeriodoC = idPeriodo;

            return PartialView("DetailViews/DetailsMP", mpc);
        }


        public ActionResult mpnc(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_entidad c_entidad = db.c_entidad.Find(id);
            if (c_entidad == null)
            {
                return HttpNotFound();
            }

            c_periodo_certificacion c_periodo_certificacion = db.c_periodo_certificacion.Where(p => p.esta_activo == true).FirstOrDefault();

            var macroProcesos = c_entidad.c_macro_proceso;

            List<c_macro_proceso> mpnc = macroProcesos.ToList();

            if (c_periodo_certificacion != null)
            {
                //Encontrar cuantos macro procesos están certificados en el periodo actual
                foreach (var mp in macroProcesos)
                {
                    if (mp.k_certificacion_estructura.Any(c => c.id_periodo_certificacion == c_periodo_certificacion.id_periodo_certificacion))
                        mpnc.Remove(mp);
                }
            }


            return PartialView("DetailViews/DetailsMP", mpnc);
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
