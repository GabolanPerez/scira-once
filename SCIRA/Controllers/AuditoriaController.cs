using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SCIRA.Utilidades;
using System.Threading.Tasks;
using System.Web.Services;
using System.ComponentModel;
using Microsoft.Ajax.Utilities;
using Org.BouncyCastle.Crypto;
using System.Drawing;

namespace SCIRA.Controllers
{
    [Authorize]
    //[Access(Funcion = "Areas", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class AuditoriaController : Controller
    {
        private SICIEntities db = new SICIEntities();
        private SeguridadUtilidades utilidades = new SeguridadUtilidades();

        #region ABC Periodo Auditoria (c_periodo_auditoria)
        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult IndexPeriodoAuditoria()
        {
            return View(db.c_periodo_auditoria.ToList());
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult CreatePeriodoAuditoria()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult CreatePeriodoAuditoria([Bind(Include = "id_periodo_auditoria,cl_periodo_auditoria,nb_periodo_auditoria")] c_periodo_auditoria c_periodo_auditoria)
        {
            if (ModelState.IsValid)
            {
                db.c_periodo_auditoria.Add(c_periodo_auditoria);
                db.SaveChanges();

                return RedirectToAction("IndexPeriodoAuditoria");
            }

            return View(c_periodo_auditoria);
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult EditPeriodoAuditoria(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            c_periodo_auditoria c_periodo_auditoria = db.c_periodo_auditoria.Find(id);
            if (c_periodo_auditoria == null)
            {
                return HttpNotFound();
            }

            return View(c_periodo_auditoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult EditPeriodoAuditoria([Bind(Include = "id_periodo_auditoria,cl_periodo_auditoria,nb_periodo_auditoria")] c_periodo_auditoria c_periodo_auditoria)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_periodo_auditoria).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexPeriodoAuditoria");
            }

            return View(c_periodo_auditoria);
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult DeletePeriodoAuditoria(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            c_periodo_auditoria c_periodo_auditoria = db.c_periodo_auditoria.Find(id);
            if (c_periodo_auditoria == null)
            {
                return HttpNotFound();
            }

            DeleteActions.checkRedirect(redirect);

            //Obtener todos los elementos a los que puede estar ligado este elemento.
            //creamos la lista que contendra a todos los registros relacionados
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();

            //Incluimos c_auditoria
            var c_auditoria = c_periodo_auditoria.c_auditoria.ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (c_auditoria.Count > 0)
            {
                foreach (var reg in c_auditoria)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "UniversoAuditable";
                    rr.cl_registro = reg.cl_auditoria;
                    rr.nb_registro = reg.nb_auditoria;
                    rr.accion = "DeleteUniversoAuditable";
                    rr.controlador = "Auditoria";
                    rr.id_registro = reg.id_auditoria.ToString();

                    RR.Add(rr);
                }
            }

            ViewBag.RR = RR;

            return View(c_periodo_auditoria);
        }

        [HttpPost, ActionName("DeletePeriodoAuditoria")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult DeleteConfirmedPeriodoAuditoria(int id)
        {
            c_periodo_auditoria c_periodo_auditoria = db.c_periodo_auditoria.Find(id);
            db.c_periodo_auditoria.Remove(c_periodo_auditoria);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("CantErase", "Error", null);
            }

            return RedirectToAction("IndexPeriodoAuditoria");
        }
        #endregion

        #region ABC Solicitante Auditoria (c_solicitante_auditoria)
        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult IndexSolicitanteAuditoria()
        {
            return View(db.c_solicitante_auditoria.ToList());
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult CreateSolicitanteAuditoria()
        {
            return View();
        }

        [AccessAudit(Funcion = "adminAudit")]
        [HttpPost] //, ValidateAntiForgeryToken, NotOnlyRead, OnlySuperUser]
        public ActionResult CreateSolicitanteAuditoria([Bind(Include = "id_solicitante_auditoria,cl_solicitante_auditoria,nb_solicitante_auditoria")] c_solicitante_auditoria c_solicitante_auditoria)
        {
            if (ModelState.IsValid)
            {
                db.c_solicitante_auditoria.Add(c_solicitante_auditoria);
                db.SaveChanges();

                return RedirectToAction("IndexSolicitanteAuditoria");
            }

            return View(c_solicitante_auditoria);
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult EditSolicitanteAuditoria(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            c_solicitante_auditoria c_solicitante_auditoria = db.c_solicitante_auditoria.Find(id);
            if (c_solicitante_auditoria == null)
            {
                return HttpNotFound();
            }

            return View(c_solicitante_auditoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessAudit(Funcion = "adminAudit")]
        [NotOnlyRead]
        public ActionResult EditSolicitanteAuditoria([Bind(Include = "id_solicitante_auditoria,cl_solicitante_auditoria,nb_solicitante_auditoria")] c_solicitante_auditoria c_solicitante_auditoria)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_solicitante_auditoria).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexSolicitanteAuditoria");
            }

            return View(c_solicitante_auditoria);
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult DeleteSolicitanteAuditoria(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            c_solicitante_auditoria c_solicitante_auditoria = db.c_solicitante_auditoria.Find(id);
            if (c_solicitante_auditoria == null)
            {
                return HttpNotFound();
            }

            DeleteActions.checkRedirect(redirect);

            //Obtener todos los elementos a los que puede estar ligado este elemento.
            //creamos la lista que contendra a todos los registros relacionados
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();

            //Incluimos c_auditoria
            var c_auditoria = c_solicitante_auditoria.c_auditoria.ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (c_auditoria.Count > 0)
            {
                foreach (var reg in c_auditoria)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "UniversoAuditable";
                    rr.cl_registro = reg.cl_auditoria;
                    rr.nb_registro = reg.nb_auditoria;
                    rr.accion = "DeleteUniversoAuditable";
                    rr.controlador = "Auditoria";
                    rr.id_registro = reg.id_auditoria.ToString();

                    RR.Add(rr);
                }
            }


            ViewBag.RR = RR;

            return View(c_solicitante_auditoria);
        }

        [HttpPost, ActionName("DeleteSolicitanteAuditoria")]
        [ValidateAntiForgeryToken]
        [AccessAudit(Funcion = "adminAudit")]
        [NotOnlyRead]
        public ActionResult DeleteConfirmedSolicitanteAuditoria(int id)
        {
            c_solicitante_auditoria c_solicitante_auditoria = db.c_solicitante_auditoria.Find(id);
            db.c_solicitante_auditoria.Remove(c_solicitante_auditoria);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("CantErase", "Error", null);
            }

            return RedirectToAction("IndexSolicitanteAuditoria");
        }
        #endregion

        #region ABC Rating Auditoria (c_rating_auditoria)
        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult IndexRatingAuditoria()
        {
            return View(db.c_rating_auditoria.ToList());
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult CreateRatingAuditoria()
        {
            ViewBag.Colores = Utilidades.Utilidades.ColoresMetaCampos();

            return View();
        }

        [AccessAudit(Funcion = "adminAudit")]
        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        public ActionResult CreateRatingAuditoria([Bind(Include = "id_rating_auditoria,nb_rating_auditoria,calificacion,cl_color_campo")] c_rating_auditoria c_rating_auditoria)
        {
            if (ModelState.IsValid)
            {
                db.c_rating_auditoria.Add(c_rating_auditoria);
                db.SaveChanges();

                return RedirectToAction("IndexRatingAuditoria");
            }

            ViewBag.Colores = Utilidades.Utilidades.ColoresMetaCampos();

            return View(c_rating_auditoria);
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult EditRatingAuditoria(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            c_rating_auditoria c_rating_auditoria = db.c_rating_auditoria.Find(id);
            if (c_rating_auditoria == null)
            {
                return HttpNotFound();
            }

            ViewBag.Colores = Utilidades.Utilidades.ColoresMetaCampos();
            return View(c_rating_auditoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessAudit(Funcion = "adminAudit")]
        [NotOnlyRead]
        public ActionResult EditRatingAuditoria([Bind(Include = "id_rating_auditoria,nb_rating_auditoria,calificacion,cl_color_campo")] c_rating_auditoria c_rating_auditoria)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_rating_auditoria).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexRatingAuditoria");
            }

            ViewBag.Colores = Utilidades.Utilidades.ColoresMetaCampos();
            return View(c_rating_auditoria);
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult DeleteRatingAuditoria(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            c_rating_auditoria c_rating_auditoria = db.c_rating_auditoria.Find(id);
            if (c_rating_auditoria == null)
            {
                return HttpNotFound();
            }

            return View(c_rating_auditoria);
        }

        [AccessAudit(Funcion = "adminAudit")]
        [HttpPost, ActionName("DeleteRatingAuditoria")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmedRatingAuditoria(int id)
        {
            c_rating_auditoria c_rating_auditoria = db.c_rating_auditoria.Find(id);

            DeleteActions.DeleteRatingAuditoriaObjects(c_rating_auditoria, db);

            db.c_rating_auditoria.Remove(c_rating_auditoria);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("CantErase", "Error", null);
            }

            return RedirectToAction("IndexRatingAuditoria");
        }
        #endregion

        #region ABC Estatus programa de trabajo (c_estatus_programa_trabajo)
        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult IndexEstatusPrograma()
        {
            return View(db.c_estatus_programa_trabajo.ToList());
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult CreateEstatusPrograma()
        {
            return View();
        }

        [AccessAudit(Funcion = "adminAudit")]
        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        public ActionResult CreateEstatusPrograma([Bind(Include = "id_estatus_programa_trabajo,cl_estatus_programa_trabajo,nb_estatus_programa_trabajo")] c_estatus_programa_trabajo c_estatus_programa_trabajo)
        {
            if (ModelState.IsValid)
            {
                db.c_estatus_programa_trabajo.Add(c_estatus_programa_trabajo);
                db.SaveChanges();

                return RedirectToAction("IndexEstatusPrograma");
            }

            return View(c_estatus_programa_trabajo);
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult EditEstatusPrograma(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            c_estatus_programa_trabajo c_estatus_programa_trabajo = db.c_estatus_programa_trabajo.Find(id);
            if (c_estatus_programa_trabajo == null)
            {
                return HttpNotFound();
            }

            return View(c_estatus_programa_trabajo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessAudit(Funcion = "adminAudit")]
        [NotOnlyRead]
        public ActionResult EditEstatusPrograma([Bind(Include = "id_estatus_programa_trabajo,cl_estatus_programa_trabajo,nb_estatus_programa_trabajo")] c_estatus_programa_trabajo c_estatus_programa_trabajo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_estatus_programa_trabajo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexEstatusPrograma");
            }

            return View(c_estatus_programa_trabajo);
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult DeleteEstatusPrograma(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            c_estatus_programa_trabajo c_estatus_programa_trabajo = db.c_estatus_programa_trabajo.Find(id);
            if (c_estatus_programa_trabajo == null)
            {
                return HttpNotFound();
            }

            return View(c_estatus_programa_trabajo);
        }

        [AccessAudit(Funcion = "adminAudit")]
        [HttpPost, ActionName("DeleteEstatusPrograma")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmedEstatusPrograma(int id)
        {
            c_estatus_programa_trabajo c_estatus_programa_trabajo = db.c_estatus_programa_trabajo.Find(id);

            //DeleteActions.DeleteEstatusProgramaObjects(c_rating_auditoria, db);

            db.c_estatus_programa_trabajo.Remove(c_estatus_programa_trabajo);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("CantErase", "Error", null);
            }

            return RedirectToAction("IndexEstatusPrograma");
        }
        #endregion

        #region ABC Division Auditoria (c_division_auditoria)
        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult IndexDivisionAuditoria()
        {
            return View(db.c_division_auditoria.Where(d => d.esta_activo).ToList());
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult CreateDivisionAuditoria()
        {
            return View();
        }

        [AccessAudit(Funcion = "adminAudit")]
        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        public ActionResult CreateDivisionAuditoria([Bind(Include = "id_division_auditoria,nb_division_auditoria,ds_division_auditoria")] c_division_auditoria c_division_auditoria)
        {
            if (ModelState.IsValid)
            {
                c_division_auditoria.esta_activo = true;
                db.c_division_auditoria.Add(c_division_auditoria);
                db.SaveChanges();

                return RedirectToAction("IndexDivisionAuditoria");
            }

            return View(c_division_auditoria);
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult EditDivisionAuditoria(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            c_division_auditoria c_division_auditoria = db.c_division_auditoria.Find(id);
            if (c_division_auditoria == null)
            {
                return HttpNotFound();
            }

            return View(c_division_auditoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessAudit(Funcion = "adminAudit")]
        [NotOnlyRead]
        public ActionResult EditDivisionAuditoria([Bind(Include = "id_division_auditoria,nb_division_auditoria,ds_division_auditoria")] c_division_auditoria c_division_auditoria)
        {
            if (ModelState.IsValid)
            {
                c_division_auditoria.esta_activo = true;
                db.Entry(c_division_auditoria).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexDivisionAuditoria");
            }

            return View(c_division_auditoria);
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult DeleteDivisionAuditoria(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            c_division_auditoria c_division_auditoria = db.c_division_auditoria.Find(id);
            if (c_division_auditoria == null)
            {
                return HttpNotFound();
            }

            return View(c_division_auditoria);
        }

        [AccessAudit(Funcion = "adminAudit")]
        [HttpPost, ActionName("DeleteDivisionAuditoria")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmedDivisionAuditoria(int id)
        {
            c_division_auditoria c_division_auditoria = db.c_division_auditoria.Find(id);

            //DeleteActions.DeleteEstatusProgramaObjects(c_rating_auditoria, db);

            c_division_auditoria.esta_activo = false;
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("CantErase", "Error", null);
            }

            return RedirectToAction("IndexDivisionAuditoria");
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult AsignaUsuariosDivisionAuditoria(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_division_auditoria c_division_auditoria = db.c_division_auditoria.Find(id);
            if (c_division_auditoria == null)
            {
                return HttpNotFound();
            }
            AsignaUsuarioRolViewModel model = new AsignaUsuarioRolViewModel();
            model.id_rol = c_division_auditoria.id_division_auditoria;
            ViewBag.nb_division_auditoria = c_division_auditoria.ds_division_auditoria;


            var usuarios = c_division_auditoria.c_usuario.ToList();
            ViewBag.usuarios = DropDown.UsuariosAuditoriaMS(usuarios.Select(u => u.id_usuario).ToArray());
            return View(model);
        }

        [AccessAudit(Funcion = "adminAudit")]
        [HttpPost, ActionName("AsignaUsuariosDivisionAuditoria")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult AsignaUsuariosDivisionAuditoria([Bind(Include = "id_rol,id_usuario")] AsignaUsuarioRolViewModel model)
        {
            c_division_auditoria c_division_auditoria = db.c_division_auditoria.Find(model.id_rol);
            if (c_division_auditoria == null)
            {
                return HttpNotFound();
            }

            try
            {
                c_division_auditoria.c_usuario.Clear();
                if (model.id_usuario == null)
                {
                    db.SaveChanges();
                    return RedirectToAction("IndexDivisionAuditoria");
                }
                foreach (int id_usr in model.id_usuario)
                {
                    c_usuario u = db.c_usuario.Find(id_usr);
                    c_division_auditoria.c_usuario.Add(u);
                }
                db.SaveChanges();
                return RedirectToAction("IndexDivisionAuditoria");
            }
            catch
            {
                ViewBag.nb_division_auditoria = c_division_auditoria.ds_division_auditoria;
                var usuarios = c_division_auditoria.c_usuario.ToList();
                ViewBag.usuarios = DropDown.UsuariosAuditoriaMS(usuarios.Select(u => u.id_usuario).ToArray());
                return View(model);
            }
        }


        #endregion

        #region ABC Criticidad programa de trabajo (c_criticidad_programa_trabajo)
        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult IndexCriticidad()
        {
            return View(db.c_criticidad_programa_trabajo.ToList());
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult CreateCriticidad()
        {
            return View();
        }

        [AccessAudit(Funcion = "adminAudit")]
        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        public ActionResult CreateCriticidad([Bind(Include = "id_criticidad_programa_trabajo,nb_criticidad_programa_trabajo,cl_criticidad_programa_trabajo,valor")] c_criticidad_programa_trabajo c_criticidad_programa_trabajo)
        {
            if (ModelState.IsValid)
            {
                db.c_criticidad_programa_trabajo.Add(c_criticidad_programa_trabajo);
                db.SaveChanges();

                return RedirectToAction("IndexCriticidad");
            }

            return View(c_criticidad_programa_trabajo);
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult EditCriticidad(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            c_criticidad_programa_trabajo c_criticidad_programa_trabajo = db.c_criticidad_programa_trabajo.Find(id);
            if (c_criticidad_programa_trabajo == null)
            {
                return HttpNotFound();
            }

            return View(c_criticidad_programa_trabajo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessAudit(Funcion = "adminAudit")]
        [NotOnlyRead]
        public ActionResult EditCriticidad([Bind(Include = "id_criticidad_programa_trabajo,nb_criticidad_programa_trabajo,cl_criticidad_programa_trabajo,valor")] c_criticidad_programa_trabajo c_criticidad_programa_trabajo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_criticidad_programa_trabajo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexCriticidad");
            }

            return View(c_criticidad_programa_trabajo);
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult DeleteCriticidad(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            c_criticidad_programa_trabajo c_criticidad_programa_trabajo = db.c_criticidad_programa_trabajo.Find(id);
            if (c_criticidad_programa_trabajo == null)
            {
                return HttpNotFound();
            }

            return View(c_criticidad_programa_trabajo);
        }

        [AccessAudit(Funcion = "adminAudit")]
        [HttpPost, ActionName("DeleteCriticidad")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmedCriticidad(int id)
        {
            c_criticidad_programa_trabajo c_criticidad_programa_trabajo = db.c_criticidad_programa_trabajo.Find(id);

            var kProgramas = c_criticidad_programa_trabajo.k_programa_trabajo.ToList();
            foreach (var programa in kProgramas)
            {
                programa.id_criticidad_programa_trabajo = null;
            }


            db.c_criticidad_programa_trabajo.Remove(c_criticidad_programa_trabajo);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("CantErase", "Error", null);
            }

            return RedirectToAction("IndexCriticidad");
        }
        #endregion

        #region ABC Campo Auditoria (c_campo_auditoria)
        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult IndexCampoAuditoria()
        {
            return View(db.c_campo_auditoria.ToList());
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult CreateCampoAuditoria()
        {
            IList<SelectListItem> colores = Utilidades.Utilidades.ColoresMetaCampos();
            IList<SelectListItem> cl_tipo_campo = Utilidades.Utilidades.TiposCampo();

            ViewBag.cl_tipo_campo = cl_tipo_campo;
            ViewBag.cl_color_borde = colores;
            ViewBag.cl_color_fondo = colores;

            return View();
        }

        [HttpPost] //, ValidateAntiForgeryToken, NotOnlyRead, OnlySuperUser]
        [AccessAudit(Funcion = "adminAudit")]
        [NotOnlyRead]
        public ActionResult CreateCampoAuditoria([Bind(Include = "id_campo_auditoria,aparece_en_planeacion,aparece_en_informe,cl_campo,nb_campo,cl_tipo_campo,longitud,es_editable,es_requerido,es_auditoria_regulada,cl_color_borde,cl_color_fondo,msg_ayuda")] c_campo_auditoria c_campo_auditoria)
        {
            if (ModelState.IsValid)
            {
                db.c_campo_auditoria.Add(c_campo_auditoria);
                db.SaveChanges();

                return RedirectToAction("IndexCampoAuditoria");
            }

            IList<SelectListItem> colores = Utilidades.Utilidades.ColoresMetaCampos();
            IList<SelectListItem> cl_tipo_campo = Utilidades.Utilidades.TiposCampo();

            ViewBag.cl_tipo_campo = cl_tipo_campo;
            ViewBag.cl_color_borde = colores;
            ViewBag.cl_color_fondo = colores;

            return View(c_campo_auditoria);
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult EditCampoAuditoria(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            c_campo_auditoria c_campo_auditoria = db.c_campo_auditoria.Find(id);
            if (c_campo_auditoria == null)
            {
                return HttpNotFound();
            }

            IList<SelectListItem> colores = Utilidades.Utilidades.ColoresMetaCampos();
            IList<SelectListItem> cl_tipo_campo = Utilidades.Utilidades.TiposCampo();

            ViewBag.cl_tipo_campo = cl_tipo_campo;
            ViewBag.cl_color_borde = colores;
            ViewBag.cl_color_fondo = colores;

            return View(c_campo_auditoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult EditCampoAuditoria([Bind(Include = "id_campo_auditoria,aparece_en_planeacion,aparece_en_informe,cl_campo,nb_campo,cl_tipo_campo,longitud,es_editable,es_requerido,es_auditoria_regulada,cl_color_borde,cl_color_fondo,msg_ayuda")] c_campo_auditoria c_campo_auditoria)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_campo_auditoria).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexCampoAuditoria");
            }

            IList<SelectListItem> colores = Utilidades.Utilidades.ColoresMetaCampos();
            IList<SelectListItem> cl_tipo_campo = Utilidades.Utilidades.TiposCampo();

            ViewBag.cl_tipo_campo = cl_tipo_campo;
            ViewBag.cl_color_borde = colores;
            ViewBag.cl_color_fondo = colores;

            return View(c_campo_auditoria);
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult DeleteCampoAuditoria(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            c_campo_auditoria c_campo_auditoria = db.c_campo_auditoria.Find(id);
            if (c_campo_auditoria == null)
            {
                return HttpNotFound();
            }

            return View(c_campo_auditoria);
        }

        [HttpPost, ActionName("DeleteCampoAuditoria")]
        [AccessAudit(Funcion = "adminAudit")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmedCampoAuditoria(int id)
        {
            c_campo_auditoria c_campo_auditoria = db.c_campo_auditoria.Find(id);

            DeleteActions.DeleteCampoExtraAuditoriaObjects(c_campo_auditoria, db);

            db.c_campo_auditoria.Remove(c_campo_auditoria);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("CantErase", "Error", null);
            }

            return RedirectToAction("IndexCampoAuditoria");
        }
        #endregion

        #region ABC Campo Programa (c_campo_programa)
        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult IndexCampoPrograma()
        {
            return View(db.c_campo_programa.ToList());
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult CreateCampoPrograma()
        {
            IList<SelectListItem> colores = Utilidades.Utilidades.ColoresMetaCampos();
            IList<SelectListItem> cl_tipo_campo = Utilidades.Utilidades.TiposCampo();

            ViewBag.cl_tipo_campo = cl_tipo_campo;
            ViewBag.cl_color_borde = colores;
            ViewBag.cl_color_fondo = colores;

            return View();
        }

        [HttpPost] //, ValidateAntiForgeryToken, NotOnlyRead, OnlySuperUser]
        [AccessAudit(Funcion = "adminAudit")]
        [NotOnlyRead]
        public ActionResult CreateCampoPrograma([Bind(Include = "id_campo_programa,cl_campo,nb_campo,cl_tipo_campo,longitud,es_editable,es_requerido,cl_color_borde,cl_color_fondo,msg_ayuda")] c_campo_programa c_campo_programa)
        {
            if (ModelState.IsValid)
            {
                db.c_campo_programa.Add(c_campo_programa);
                db.SaveChanges();

                return RedirectToAction("IndexCampoPrograma");
            }

            IList<SelectListItem> colores = Utilidades.Utilidades.ColoresMetaCampos();
            IList<SelectListItem> cl_tipo_campo = Utilidades.Utilidades.TiposCampo();

            ViewBag.cl_tipo_campo = cl_tipo_campo;
            ViewBag.cl_color_borde = colores;
            ViewBag.cl_color_fondo = colores;

            return View(c_campo_programa);
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult EditCampoPrograma(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            c_campo_programa c_campo_programa = db.c_campo_programa.Find(id);
            if (c_campo_programa == null)
            {
                return HttpNotFound();
            }

            IList<SelectListItem> colores = Utilidades.Utilidades.ColoresMetaCampos();
            IList<SelectListItem> cl_tipo_campo = Utilidades.Utilidades.TiposCampo();

            ViewBag.cl_tipo_campo = cl_tipo_campo;
            ViewBag.cl_color_borde = colores;
            ViewBag.cl_color_fondo = colores;

            return View(c_campo_programa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult EditCampoPrograma([Bind(Include = "id_campo_programa,cl_campo,nb_campo,cl_tipo_campo,longitud,es_editable,es_requerido,cl_color_borde,cl_color_fondo,msg_ayuda")] c_campo_programa c_campo_programa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_campo_programa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexCampoPrograma");
            }

            IList<SelectListItem> colores = Utilidades.Utilidades.ColoresMetaCampos();
            IList<SelectListItem> cl_tipo_campo = Utilidades.Utilidades.TiposCampo();

            ViewBag.cl_tipo_campo = cl_tipo_campo;
            ViewBag.cl_color_borde = colores;
            ViewBag.cl_color_fondo = colores;

            return View(c_campo_programa);
        }

        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult DeleteCampoPrograma(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            c_campo_programa c_campo_programa = db.c_campo_programa.Find(id);
            if (c_campo_programa == null)
            {
                return HttpNotFound();
            }

            return View(c_campo_programa);
        }

        [HttpPost, ActionName("DeleteCampoPrograma")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        [AccessAudit(Funcion = "adminAudit")]
        public ActionResult DeleteConfirmedCampoPrograma(int id)
        {
            c_campo_programa c_campo_programa = db.c_campo_programa.Find(id);
            DeleteActions.DeleteCampoExtraProgramaObjects(c_campo_programa, db);

            db.c_campo_programa.Remove(c_campo_programa);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("CantErase", "Error", null);
            }

            return RedirectToAction("IndexCampoPrograma");
        }
        #endregion

        #region ABC Universo Auditable (c_auditoria)
        [AccessAudit(Funcion = "UniAud")]
        public ActionResult IndexUniversoAuditable()
        {
            return View(db.c_auditoria.ToList());
        }

        [AccessAudit(Funcion = "UniAud")]
        public ActionResult CreateUniversoAuditable()
        {
            ViewBag.id_entidad = DropDown.Entidades();
            ViewBag.id_area = DropDown.Areas();
            ViewBag.id_solicitante_auditoria = DropDown.SolicitantesAuditoria();
            ViewBag.id_periodo_auditoria = DropDown.PeriodosAuditoria();
            ViewBag.id_mes_entrega = DropDown.Meses();
            ViewBag.id_division_auditoriaL = DropDown.DivisionesAuditoria();

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        [AccessAudit(Funcion = "UniAud")]
        public ActionResult CreateUniversoAuditable(c_auditoria c_auditoria)
        {
            if (ModelState.IsValid)
            {

                int id = c_auditoria.id_auditoria;

                //Ligar con todas los rangos de costeo
                var areas_costeo = db.c_rango_costeo;
                foreach (var ac in areas_costeo)
                {
                    var acsp = new c_rango_costeo_auditoria { id_rango_costeo = ac.id_rango_costeo, id_auditoria = id };
                    db.c_rango_costeo_auditoria.Add(acsp);
                }

                db.c_auditoria.Add(c_auditoria);
                db.SaveChanges();

                return RedirectToAction("IndexUniversoAuditable");
            }

            ViewBag.id_entidad = DropDown.Entidades();
            ViewBag.id_area = DropDown.Areas();
            ViewBag.id_solicitante_auditoria = DropDown.SolicitantesAuditoria();
            ViewBag.id_periodo_auditoria = DropDown.PeriodosAuditoria();
            ViewBag.id_mes_entrega = DropDown.Meses();
            ViewBag.id_division_auditoriaL = DropDown.DivisionesAuditoria();

            return View(c_auditoria);
        }

        [AccessAudit(Funcion = "UniAud")]
        public ActionResult EditUniversoAuditable(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            c_auditoria c_auditoria = db.c_auditoria.Find(id);
            if (c_auditoria == null)
            {
                return HttpNotFound();
            }

            ViewBag.entidad = DropDown.Entidades(c_auditoria.id_entidad);
            ViewBag.area = DropDown.Areas(c_auditoria.id_area);
            ViewBag.solicitante_auditoria = DropDown.SolicitantesAuditoria(c_auditoria.id_solicitante_auditoria);
            ViewBag.periodo_auditoria = DropDown.PeriodosAuditoria(c_auditoria.id_periodo_auditoria);
            ViewBag.mes_entrega = DropDown.Meses(c_auditoria.id_mes_entrega);
            ViewBag.id_division_auditoriaL = DropDown.DivisionesAuditoria(c_auditoria.id_division_auditoria ?? 0);

            return View(c_auditoria);
        }

        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        [AccessAudit(Funcion = "UniAud")]
        public ActionResult EditUniversoAuditable(c_auditoria c_auditoria, int ids)
        {
            int id;

            if (ModelState.IsValid)
            {

                db.Entry(c_auditoria).State = EntityState.Modified;
                db.SaveChanges();


                // Guarda los datos de las tablas externas
                // id = c_auditoria.id_auditoria;

                // var model = db.c_rango_costeo.FirstOrDefaultAsync(ids);

                // Guarda las áreas de costeo
                //if (model.id_rango_costeo != null)
                //{
                //List<c_rango_costeo_auditoria> c_ac_sp = db.c_rango_costeo_auditoria.Where(c => c.id_auditoria == id).ToList();
                //foreach (var cacsp in c_ac_sp)
                //{
                // if (!(model.id_rango_costeo.Contains(cacsp.id_rango_costeo)))
                //{
                // db.c_rango_costeo_auditoria.Remove(cacsp);
                // }
                // }

                // foreach (int id_area_costeo in model.id_areas_costeo)
                //{
                //if (!(db.c_rango_costeo_auditoria.Any(c => c.id_rango_costeo == id_area_costeo && c.id_auditoria == id)))
                // {
                //   var acsp = new c_rango_costeo_auditoria { id_rango_costeo = id_area_costeo, id_auditoria = id };
                //   db.c_rango_costeo_auditoria.Add(acsp);
                // }
                //  }
                //   db.SaveChanges();
                // }
                //  else
                // {
                // try
                //{
                // List<c_rango_costeo_auditoria> c_ac_sp = db.c_rango_costeo_auditoria.Where(c => c.id_auditoria == id).ToList();
                // foreach (var cacsp in c_ac_sp)
                // {
                //  db.c_rango_costeo_auditoria.Remove(cacsp);
                // }
                //  db.SaveChanges();
                // }
                // catch
                // {

                // }
                // }


                return RedirectToAction("IndexUniversoAuditable");
            }



            ViewBag.entidad = Utilidades.DropDown.Entidades(c_auditoria.id_entidad);
            ViewBag.area = Utilidades.DropDown.Areas(c_auditoria.id_area);
            ViewBag.solicitante_auditoria = Utilidades.DropDown.SolicitantesAuditoria(c_auditoria.id_solicitante_auditoria);
            ViewBag.periodo_auditoria = Utilidades.DropDown.PeriodosAuditoria(c_auditoria.id_periodo_auditoria);
            ViewBag.mes_entrega = Utilidades.DropDown.Meses(c_auditoria.id_mes_entrega);
            ViewBag.id_division_auditoriaL = DropDown.DivisionesAuditoria(c_auditoria.id_division_auditoria ?? 0);

            return View(c_auditoria);
        }

        [AccessAudit(Funcion = "UniAud")]
        public ActionResult DeleteUniversoAuditable(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            c_auditoria c_auditoria = db.c_auditoria.Find(id);
            if (c_auditoria == null)
            {
                return HttpNotFound();
            }

            DeleteActions.checkRedirect(redirect);

            return View(c_auditoria);
        }

        // POST: Proceso/Delete/5
        [HttpPost, ActionName("DeleteUniversoAuditable")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        [AccessAudit(Funcion = "UniAud")]
        public ActionResult DeleteConfirmedUniversoAuditable(int id)
        {
            c_auditoria c_auditoria = db.c_auditoria.Find(id);

            DeleteActions.DeleteAuditoriaObjects(c_auditoria, db, true);

            db.c_auditoria.Remove(c_auditoria);
            try
            {
                db.SaveChanges();
            }
            catch
            {
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
                return RedirectToAction("IndexUniversoAuditable");

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
                    return RedirectToAction("IndexUniversoAuditable");
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

        public ActionResult DetailsCosteo(int? id)
        {
            var au = db.c_auditoria.Find(id);

            var c_rangos_costeo = db.c_rango_costeo.ToList();
            var c_rango = db.c_rango.FirstOrDefault();
            if (c_rango == null)
            {
                c_rango = new c_rango();
                c_rango.cl_rango = "N/A";
                c_rango.nb_rango = "N/A";
                c_rango.cl_color_campo = "Black";
                c_rango.valor = "0";
                c_rango.segmentacion_calificacion = "0";
                c_rango.segmentacion_calificacion2 = "0";

                db.c_rango.Add(c_rango);
                db.SaveChanges();
            }


            foreach (var crc in c_rangos_costeo)
            {
                if (!au.c_rango_costeo_auditoria.Any(crca => crca.id_rango_costeo == crc.id_rango_costeo))
                {
                    c_rango_costeo_auditoria newc = new c_rango_costeo_auditoria();
                    newc.id_auditoria = au.id_auditoria;
                    newc.id_rango = 0;
                    newc.pr_costeo = 0;
                    newc.id_rango_costeo = crc.id_rango_costeo;
                    db.c_rango_costeo_auditoria.Add(newc);
                    db.SaveChanges();
                }
            }

            decimal suma = au.c_rango_costeo_auditoria.Sum(item => item.pr_costeo);
            ViewBag.Suma = suma;

            //var cal = db.c_rango.Select(s => new { s.segmentacion_calificacion, s.segmentacion_calificacion2 }).First();
            //var val1 = decimal.Parse(cal.segmentacion_calificacion);
            //var val2 = decimal.Parse(cal.segmentacion_calificacion2);

            var rangos = db.c_rango.ToList();

            foreach (var item in rangos)
            {
                var val1 = decimal.Parse(item.segmentacion_calificacion);
                var val2 = decimal.Parse(item.segmentacion_calificacion2);
                if (suma >= val1 && suma <= val2)
                {
                    var nom = item;
                    ViewBag.nombre = nom.nb_rango;
                    ViewBag.color = nom.cl_color_campo;
                    break;
                }
            }
            ViewBag.valorL = Utilidades.DropDown.ValorRango();
            //c_rango c_rango = new c_rango();
            //c_rango.id_rango = rango.id_rango;
            //ViewBag.valor = new SelectList(db.c_rango, "id_rango", "nb_rango", "valor",rango.id_rango);

            return PartialView("Modal/DetailsCosteo", au);
        }

        [HttpPost]
        public double DCosteo(int valor, int pr_costeo, int id_au)
        {
            double por = .01;
            var rangos = db.c_rango.FirstOrDefault(n => n.id_rango == valor);
            var pr_cost = db.c_rango_costeo.FirstOrDefault(n => n.id_rango_costeo == pr_costeo);
            var pr_coste = pr_cost.pr_costeo;

            //  var id_audi = db.c_auditoria.FirstOrDefault(n => n.id_auditoria == id_au && n.);

            double resultado = int.Parse(rangos.valor) * (double)pr_coste * por;
            // ViewBag.Riesgo_Inherente = String.Format("${0:N2}", ((double)riesgo.monto_impacto * (double)impacto.pr_probabilidad_ocurrencia) * .01);
            // id_audi.c_rango_costeo_auditoria.Where(n => n.id_rango_costeo == pr_coste).First().pr_costeo = (decimal)resultado;

            var objetoaeditar = db.c_rango_costeo_auditoria.FirstOrDefault(c => c.id_auditoria == id_au && c.id_rango_costeo == pr_costeo);

            if (objetoaeditar != null)
            {
                objetoaeditar.pr_costeo = (decimal)resultado;
               objetoaeditar.id_rango = rangos.id_rango;
            }

            db.SaveChanges();



            decimal suma = db.c_rango_costeo_auditoria.Where(k => k.id_auditoria == id_au).Sum(item => item.pr_costeo);

            //db.c_auditoria.FirstOrDefault(h => h.id_auditoria == id_au &&  h.calificacion_audi = suma);
            //db.SaveChanges();

            var rango = db.c_rango.ToList();

            foreach (var item in rango)
            {
                var val1 = decimal.Parse(item.segmentacion_calificacion);
                var val2 = decimal.Parse(item.segmentacion_calificacion2);
                if (suma >= val1 && suma <= val2)
                {
                    var audi =  db.c_auditoria.FirstOrDefault(i => i.id_auditoria == id_au);
                    audi.cl_color_campo = item.cl_color_campo;
                    audi.riesgo = item.nb_rango;

                    //db.c_auditoria.FirstOrDefault(i => i.id_auditoria == id_au && i.cl_color_campo = item.cl_color_campo);
                    //db.c_auditoria.FirstOrDefault(t => t.id_auditoria == id_au && t.riesgo = item.nb_rango);
                    db.SaveChanges(); 

                    break;
                }
            }

            return resultado;

        }

        public ActionResult Details(int id_audi)
        {
            var id_au = db.c_auditoria.Find(id_audi);

            return PartialView("Modal/DetailsCosteo", id_au);
        }

        #endregion

        #region ABC Programa de Auditoría (Prueba Auditoria (c_prueba_auditoria))
        [AccessAudit(Funcion = "UniAud")]
        public ActionResult IndexPruebaAuditoria(int id)
        {
            c_auditoria c_auditoria = db.c_auditoria.Find(id);

            ViewBag.id_auditoria = c_auditoria.id_auditoria;
            ViewBag.cl_auditoria = c_auditoria.cl_auditoria;
            ViewBag.nb_auditoria = c_auditoria.nb_auditoria;

            return View(db.c_prueba_auditoria.Where(r => r.id_auditoria == id).ToList());
        }

        [AccessAudit(Funcion = "UniAud")]
        public ActionResult CreatePruebaAuditoria(int id)
        {
            c_auditoria c_auditoria = db.c_auditoria.Find(id);

            ViewBag.id_auditoria = c_auditoria.id_auditoria;
            ViewBag.cl_auditoria = c_auditoria.cl_auditoria;
            ViewBag.nb_auditoria = c_auditoria.nb_auditoria;

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        [AccessAudit(Funcion = "UniAud")]
        public ActionResult CreatePruebaAuditoria(c_prueba_auditoria c_prueba_auditoria)
        {
            if (ModelState.IsValid)
            {
                db.c_prueba_auditoria.Add(c_prueba_auditoria);
                db.SaveChanges();

                return RedirectToAction("IndexPruebaAuditoria", new { id = c_prueba_auditoria.id_auditoria });
            }

            return View(c_prueba_auditoria);
        }

        [AccessAudit(Funcion = "UniAud")]
        public ActionResult EditPruebaAuditoria(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            c_prueba_auditoria c_prueba_auditoria = db.c_prueba_auditoria.Find(id);
            if (c_prueba_auditoria == null)
            {
                return HttpNotFound();
            }

            return View(c_prueba_auditoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        [AccessAudit(Funcion = "UniAud")]
        public ActionResult EditPruebaAuditoria(c_prueba_auditoria c_prueba_auditoria)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_prueba_auditoria).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexPruebaAuditoria", new { id = c_prueba_auditoria.id_auditoria });
            }

            return View(c_prueba_auditoria);
        }

        [AccessAudit(Funcion = "UniAud")]
        public ActionResult DeletePruebaAuditoria(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            c_prueba_auditoria c_prueba_auditoria = db.c_prueba_auditoria.Find(id);
            if (c_prueba_auditoria == null)
            {
                return HttpNotFound();
            }

            return View(c_prueba_auditoria);
        }

        [HttpPost, ActionName("DeletePruebaAuditoria")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        [AccessAudit(Funcion = "UniAud")]
        public ActionResult DeleteConfirmedPruebaAuditoria(int id)
        {
            c_prueba_auditoria c_prueba_auditoria = db.c_prueba_auditoria.Find(id);

            var kProgramas = c_prueba_auditoria.k_programa_trabajo.ToList();

            foreach (var kPrograma in kProgramas)
            {
                DeleteActions.DeleteKProgramaObjects(kPrograma, db, true);
                db.k_programa_trabajo.Remove(kPrograma);
            }

            int idAud = c_prueba_auditoria.id_auditoria;

            db.c_prueba_auditoria.Remove(c_prueba_auditoria);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("CantErase", "Error", null);
            }

            return RedirectToAction("IndexPruebaAuditoria", new { id = idAud });
        }
        #endregion

        #region ABC Seguimiento de Acciones (k_programa_trabajo)
        [AccessAudit(Funcion = "SegAud")]
        public ActionResult IndexSeguimientoObservaciones()
        {
            var id_usuario = ((IdentityPersonalizado)User.Identity).Id_usuario;
            var us = db.c_usuario.Find(id_usuario);

            var idsDivisiones = us.c_division_auditoria.Select(d => d.id_division_auditoria).ToList();

            var audits = db.k_auditoria.Where(k => idsDivisiones.Contains(k.c_auditoria.id_division_auditoria ?? 0)).ToList();

            var model = audits.SelectMany(a => a.k_programa_trabajo).Where(a => a.tiene_incidencia).ToList();

            List<string> años = new List<string>();

            foreach (var programa in model)
            {
                var kAud = programa.k_auditoria;

                var año = kAud.fe_inicial_planeada.Value.Year.ToString();

                if (!años.Contains(año))
                    años.Add(año);

            }

            ViewBag.años = años;

            return View(model);
        }

        [AccessAudit(Funcion = "SegAud")]
        public ActionResult IndexTableSeguimientoObservaciones(int year)
        {
            var id_usuario = ((IdentityPersonalizado)User.Identity).Id_usuario;
            var us = db.c_usuario.Find(id_usuario);

            var idsDivisiones = us.c_division_auditoria.Select(d => d.id_division_auditoria).ToList();

            var audits = db.k_auditoria.Where(k => idsDivisiones.Contains(k.c_auditoria.id_division_auditoria ?? 0)).ToList();

            var model = audits.SelectMany(a => a.k_programa_trabajo).Where(a => a.tiene_incidencia).ToList();

            List<k_programa_trabajo> Model = new List<k_programa_trabajo>();

            foreach (var programa in model)
            {
                var kAud = programa.k_auditoria;

                var año = kAud.fe_inicial_planeada.Value.Year;

                if (año == year)
                    Model.Add(programa);

            }

            return PartialView(Model);
        }

        [AccessAudit(Funcion = "SegAud")]
        public ActionResult EditSeguimientoObservaciones(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            k_programa_trabajo k_programa_trabajo = db.k_programa_trabajo.Find(id);
            if (k_programa_trabajo == null)
            {
                return HttpNotFound();
            }

            ViewBag.DateFormat = Utilidades.Utilidades.GetDateFormat();

            return View(k_programa_trabajo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        [AccessAudit(Funcion = "SegAud")]
        public ActionResult EditSeguimientoObservaciones(k_programa_trabajo k_programa_trabajo, int[] files)
        {
            var register = db.k_programa_trabajo.Find(k_programa_trabajo.idd_programa_trabajo);

            if (k_programa_trabajo.esta_atendida)
            {
                if (k_programa_trabajo.fe_resolucion == null)
                {
                    ModelState.AddModelError("fe_resolucion", Strings.getMSG("AuditoriaEditSeguimientoObservaciones003"));
                }

                register.fe_resolucion = k_programa_trabajo.fe_resolucion;
            }
            else
            {
                register.fe_resolucion = null;
            }
            register.esta_atendida = k_programa_trabajo.esta_atendida;
            register.comentarios = k_programa_trabajo.comentarios;


            if (ModelState.IsValid)
            {
                db.SaveChanges();

                if (files != null)
                    foreach (var idar in files)
                    {
                        var file = db.c_archivo.Find(idar);
                        register.c_archivo1.Add(file);
                    }

                db.SaveChanges();

                return RedirectToAction("IndexSeguimientoObservaciones");
            }

            ViewBag.DateFormat = Utilidades.Utilidades.GetDateFormat();


            return View(k_programa_trabajo);
        }


        [AccessAudit(Funcion = "SegAud")]
        public ActionResult IndexSeguimientosPrograma(int id)
        {

            k_programa_trabajo k_programa_trabajo = db.k_programa_trabajo.Find(id);
            if (k_programa_trabajo == null)
            {
                return HttpNotFound();
            }

            return View(k_programa_trabajo);
        }

        [AccessAudit(Funcion = "SegAud")]
        public ActionResult CreateComentario(int id)
        {

            k_programa_trabajo k_programa_trabajo = db.k_programa_trabajo.Find(id);
            if (k_programa_trabajo == null)
            {
                return HttpNotFound();
            }

            var comentario = new k_comentario_programa_trabajo
            {
                idd_programa_trabajo = id,
                fe_comentario = DateTime.Now
            };


            return View(comentario);
        }

        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        [AccessAudit(Funcion = "SegAud")]
        public ActionResult CreateComentario(k_comentario_programa_trabajo model)
        {
            var user = (IdentityPersonalizado)User.Identity;

            if (ModelState.IsValid)
            {
                model.fe_comentario = DateTime.Now;
                model.nb_responsable = user.Nb_usuario;

                db.k_comentario_programa_trabajo.Add(model);
                db.SaveChanges();

                return RedirectToAction("IndexSeguimientosPrograma", new { id = model.idd_programa_trabajo });
            }

            return View(model);
        }



        [AccessAudit(Funcion = "SegAud")]
        public ActionResult EditComentario(int id)
        {

            k_comentario_programa_trabajo model = db.k_comentario_programa_trabajo.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }


            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        [AccessAudit(Funcion = "SegAud")]
        public ActionResult EditComentario(k_comentario_programa_trabajo model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("IndexSeguimientosPrograma", new { id = model.idd_programa_trabajo });
            }

            return View(model);
        }

        [AccessAudit(Funcion = "SegAud")]
        public ActionResult DeleteComentario(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            k_comentario_programa_trabajo k_comentario_programa_trabajo = db.k_comentario_programa_trabajo.Find(id);
            if (k_comentario_programa_trabajo == null)
            {
                return HttpNotFound();
            }

            return View(k_comentario_programa_trabajo);
        }

        [AccessAudit(Funcion = "SegAud")]
        [HttpPost, ActionName("DeleteComentario")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmedComentario(int id)
        {
            k_comentario_programa_trabajo k_comentario_programa_trabajo = db.k_comentario_programa_trabajo.Find(id);


            var idKprograma = k_comentario_programa_trabajo.idd_programa_trabajo;

            db.k_comentario_programa_trabajo.Remove(k_comentario_programa_trabajo);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("CantErase", "Error", null);
            }
            return RedirectToAction("IndexSeguimientosPrograma", new { id = idKprograma });
        }
        #endregion

    }
}