using Remotion.Data.Linq.Clauses;
using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "SubProcesos", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class SubProcesoController : Controller
    {
        private SICIEntities db = new SICIEntities();

        private ISelectListRepository _repository;

        public SubProcesoController() : this(new SelectListRepository())
        {
        }

        public SubProcesoController(ISelectListRepository repository)
        {
            _repository = repository;
        }

        // GET: SubProceso
        public ActionResult Index()
        {
            List<c_sub_proceso> sps;

            var user = ((Seguridad.IdentityPersonalizado)User.Identity);
            var Usuario = db.c_usuario.Find(user.Id_usuario);

            var su = user.Es_super_usuario;
            ViewBag.su = user.Es_super_usuario;

            //if (su)
            //{
            //    sps = db.c_sub_proceso.
            //    OrderBy(x => x.c_proceso.c_macro_proceso.c_entidad.cl_entidad).
            //    OrderBy(x => x.c_proceso.c_macro_proceso.cl_macro_proceso).
            //    OrderBy(x => x.c_proceso.cl_proceso).
            //    OrderBy(x => x.cl_sub_proceso).ToList();
            //}
            //else
            //{
            //    sps = db.c_sub_proceso.Where(p => p.id_responsable == user.Id_usuario).
            //    OrderBy(x => x.c_proceso.c_macro_proceso.c_entidad.cl_entidad).
            //    OrderBy(x => x.c_proceso.c_macro_proceso.cl_macro_proceso).
            //    OrderBy(x => x.c_proceso.cl_proceso).
            //    OrderBy(x => x.cl_sub_proceso).ToList();
            //}

            sps = Utilidades.Utilidades.RTCObject(Usuario, db, "c_sub_proceso").Cast<c_sub_proceso>().
                OrderBy(x => x.c_proceso.c_macro_proceso.c_entidad.cl_entidad).
                OrderBy(x => x.c_proceso.c_macro_proceso.cl_macro_proceso).
                OrderBy(x => x.c_proceso.cl_proceso).
                OrderBy(x => x.cl_sub_proceso).ToList();


            return View(sps);
        }

        #region Create
        public ActionResult Agregar()
        {
            var user = (IdentityPersonalizado)User.Identity;

            if (!user.Funciones.Contains("AgregarSP"))
                return RedirectToAction("Denied", "Error");

            AgregarSubProcesoViewModel model = new AgregarSubProcesoViewModel();

            var entidades = _repository.ObtieneEntidades();
            foreach (var entidad in entidades)
            {
                model.Entidades.Add(new SelectListItem()
                {
                    Text = entidad.cl_entidad + " - " + entidad.nb_entidad,
                    Value = entidad.id_entidad.ToString()
                });
            }

            //multiselects
            ViewBag.responsables = Utilidades.DropDown.UsuariosMS();
            ViewBag.lineas_negocio = Utilidades.DropDown.LineasNegocioMS();
            //ViewBag.areas_costeo = Utilidades.DropDown.AreasCosteoMS();

            ViewBag.id_etapa = Utilidades.DropDown.Etapas();
            ViewBag.id_sub_etapa = Utilidades.DropDown.SubEtapas();
            ViewBag.id_responsableL = Utilidades.DropDown.Usuario();
            ViewBag.id_tipologia_sub_procesoL = Utilidades.DropDown.TipologiaSubProceso();

            //Campos extra Sub Proceso
            ViewBag.CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);
            ViewBag.MSError = new string[20];


            return View(model);
        }

        // POST: SubProceso/Agregar
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        public ActionResult Agregar(AgregarSubProcesoViewModel model)
        {
            bool valid = ValidarCE(model);


            if (ModelState.IsValid && valid)
            {
                c_sub_proceso c_sub_proceso = new c_sub_proceso();
                c_sub_proceso.id_proceso = model.id_proceso;
                c_sub_proceso.cl_sub_proceso = model.cl_sub_proceso;
                c_sub_proceso.nb_sub_proceso = model.nb_sub_proceso;
                c_sub_proceso.ds_sub_proceso = model.ds_sub_proceso;
                c_sub_proceso.id_responsable = model.id_responsable;
                c_sub_proceso.cl_manual = model.cl_manual;
                c_sub_proceso.cl_sp_anterior = model.cl_sp_anterior;
                c_sub_proceso.cl_sp_siguiente = model.cl_sp_siguiente;
                c_sub_proceso.id_tipologia_sub_proceso = model.id_tipologia_sub_proceso;
                c_sub_proceso.id_etapa = model.id_etapa;
                c_sub_proceso.id_sub_etapa = model.id_sub_etapa;
                c_sub_proceso.ds_areas_involucradas = model.ds_areas_involucradas;
                c_sub_proceso.ds_aplicaciones_relacionadas = model.ds_aplicaciones_relacionadas;

                c_sub_proceso = llenarCamposExtra(model, c_sub_proceso);

                db.c_sub_proceso.Add(c_sub_proceso);
                db.SaveChanges();

                Utilidades.Utilidades.ObjectAsigned(c_sub_proceso);

                // Guarda los participantes
                List<int> responsables;
                try
                {
                    responsables = model.id_responsables.ToList();
                }
                catch
                {
                    responsables = new List<int>();
                }
                int id = c_sub_proceso.id_sub_proceso;

                if (!(responsables.Any(c => c == model.id_responsable)))
                {
                    responsables.Add(model.id_responsable);
                }

                if (responsables != null)
                {
                    foreach (int id_resp in responsables)
                    {
                        var usp = new c_usuario_sub_proceso { id_usuario = id_resp, id_sub_proceso = id };
                        db.c_usuario_sub_proceso.Add(usp);
                    }
                }

                //Ligar con todas las áreas de costeo
                var areas_costeo = db.c_area_costeo;
                foreach (var ac in areas_costeo)
                {
                    var acsp = new c_area_costeo_sub_proceso { id_area_costeo = ac.id_area_costeo, id_sub_proceso = id };
                    db.c_area_costeo_sub_proceso.Add(acsp);
                }

                // Guarda las lineas de negocio
                if (model.id_lineas_negocio != null)
                {
                    c_linea_negocio c_linea_negocio;
                    foreach (int id_linea_negocio in model.id_lineas_negocio)
                    {
                        c_linea_negocio = db.c_linea_negocio.Find(id_linea_negocio);
                        c_sub_proceso.c_linea_negocio.Add(c_linea_negocio);
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //AgregarSubProcesoViewModel model2 = new AgregarSubProcesoViewModel();

            var entidades = _repository.ObtieneEntidades();
            foreach (var entidad in entidades)
            {
                model.Entidades.Add(new SelectListItem()
                {
                    Text = entidad.cl_entidad + " - " + entidad.nb_entidad,
                    Value = entidad.id_entidad.ToString()
                });
            }

            if (model.id_entidad > 0)
            {
                var MacroProcesos = db.c_macro_proceso.Where(mp => mp.id_entidad == model.id_entidad).ToList();
                foreach (var MacroProceso in MacroProcesos)
                {
                    model.MacroProcesos.Add(new SelectListItem()
                    {
                        Text = MacroProceso.cl_macro_proceso + " - " + MacroProceso.nb_macro_proceso,
                        Value = MacroProceso.id_macro_proceso.ToString()
                    });
                }
            }

            if (model.id_macro_proceso > 0)
            {
                var Procesos = db.c_proceso.Where(p => p.id_macro_proceso == model.id_macro_proceso).ToList();
                foreach (var Proceso in Procesos)
                {
                    model.Procesos.Add(new SelectListItem()
                    {
                        Text = Proceso.cl_proceso + " - " + Proceso.nb_proceso,
                        Value = Proceso.id_proceso.ToString()
                    });
                }
            }


            //multiselects
            ViewBag.responsables = Utilidades.DropDown.UsuariosMS(model.id_responsables);
            ViewBag.lineas_negocio = Utilidades.DropDown.LineasNegocioMS(model.id_lineas_negocio);
            ViewBag.areas_costeo = Utilidades.DropDown.AreasCosteoMS(model.id_areas_costeo);

            ViewBag.id_etapa = Utilidades.DropDown.Etapas(model.id_etapa ?? 0);
            ViewBag.id_sub_etapa = Utilidades.DropDown.SubEtapas(model.id_sub_etapa ?? 0);
            ViewBag.id_responsableL = Utilidades.DropDown.Usuario((int)model.id_responsable);
            ViewBag.id_tipologia_sub_procesoL = Utilidades.DropDown.TipologiaSubProceso((int)model.id_tipologia_sub_proceso);

            //Campos extra Riesgos y Controles
            ViewBag.CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);
            ViewBag.MSError = new string[20];

            return View(model);
        }
        #endregion}

        #region Edit
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ObtieneMacroProcesos(string IdEntidad)
        {
            if (String.IsNullOrEmpty(IdEntidad))
            {
                throw new ArgumentNullException("IdEntidad");
                //IdEntidad = "1";
            }
            int id = 0;
            bool isValid = Int32.TryParse(IdEntidad, out id);
            var macroProcesos = _repository.ObtieneMacroProcesos(id);
            var result = (from s in macroProcesos
                          select new
                          {
                              id = s.id_macro_proceso,
                              name = s.nb_macro_proceso
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ObtieneProcesos(string IdMacroProceso)
        {
            if (String.IsNullOrEmpty(IdMacroProceso))
            {
                throw new ArgumentNullException("IdMacroProceso");
                //IdMacroProceso = "1";
            }
            int id = 0;
            bool isValid = Int32.TryParse(IdMacroProceso, out id);
            var procesos = _repository.ObtieneProcesos(id);
            var result = (from s in procesos
                          select new
                          {
                              id = s.id_proceso,
                              name = s.cl_proceso + " - " + s.nb_proceso
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: SubProceso/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            c_sub_proceso c_sub_proceso = db.c_sub_proceso.Find(id);
            if (c_sub_proceso == null) return HttpNotFound();

            AgregarSubProcesoViewModel model = new AgregarSubProcesoViewModel();
            model.id_proceso = c_sub_proceso.id_proceso;
            model.id_sub_proceso = c_sub_proceso.id_sub_proceso;
            model.cl_sub_proceso = c_sub_proceso.cl_sub_proceso;
            model.nb_sub_proceso = c_sub_proceso.nb_sub_proceso;
            model.ds_sub_proceso = c_sub_proceso.ds_sub_proceso;
            model.id_responsable = c_sub_proceso.id_responsable;
            model.cl_manual = c_sub_proceso.cl_manual;
            model.cl_sp_anterior = c_sub_proceso.cl_sp_anterior;
            model.cl_sp_siguiente = c_sub_proceso.cl_sp_siguiente;
            model.id_tipologia_sub_proceso = c_sub_proceso.id_tipologia_sub_proceso;
            model.id_etapa = c_sub_proceso.id_etapa;
            model.id_sub_etapa = c_sub_proceso.id_sub_etapa;
            model.ds_areas_involucradas = c_sub_proceso.ds_areas_involucradas;
            model.ds_aplicaciones_relacionadas = c_sub_proceso.ds_aplicaciones_relacionadas;
            model.nb_archivo_manual = c_sub_proceso.nb_archivo_manual;
            model.nb_archivo_flujo = c_sub_proceso.nb_archivo_flujo;

            model.c_etapa = c_sub_proceso.c_etapa;
            model.c_proceso = c_sub_proceso.c_proceso;
            model.c_sub_etapa = c_sub_proceso.c_sub_etapa;
            model.c_usuario = c_sub_proceso.c_usuario;
            model.c_tipologia_sub_proceso = c_sub_proceso.c_tipologia_sub_proceso;

            //llenar multiselects con valores seleccionados
            string sql = "select id_usuario from c_usuario_sub_proceso where id_sub_proceso = " + id;
            var marcados = db.Database.SqlQuery<int>(sql).ToArray();
            ViewBag.responsables = Utilidades.DropDown.UsuariosMS(marcados);

            sql = "select id_linea_negocio from c_linea_negocio_sub_proceso where id_sub_proceso = " + id;
            marcados = db.Database.SqlQuery<int>(sql).ToArray();

            ViewBag.lineas_negocio = Utilidades.DropDown.LineasNegocioMS(marcados);

            sql = "select id_area_costeo from c_area_costeo_sub_proceso where id_sub_proceso = " + id;
            marcados = db.Database.SqlQuery<int>(sql).ToArray();
            ViewBag.areas_costeo = Utilidades.DropDown.AreasCosteoMS(marcados);


            //Llenado de selects simples
            ViewBag.id_etapa = Utilidades.DropDown.Etapas(c_sub_proceso.id_etapa ?? 0);
            ViewBag.id_sub_etapa = Utilidades.DropDown.SubEtapas(c_sub_proceso.id_sub_etapa ?? 0);
            ViewBag.id_responsableL = Utilidades.DropDown.Usuario((int)c_sub_proceso.id_responsable);
            ViewBag.id_tipologia_sub_procesoL = Utilidades.DropDown.TipologiaSubProceso((int)c_sub_proceso.id_tipologia_sub_proceso);

            //Campos Extra
            ViewBag.CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);
            ViewBag.MSError = new string[20];

            //obtenemos los campos extra del sub proceso y los pasamos al modelo
            model = obtenerCamposExtra(model, c_sub_proceso);

            return View(model);
        }

        // POST: SubProceso/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(AgregarSubProcesoViewModel model, int lu)
        {
            int id;
            c_sub_proceso c_sub_proceso;

            bool valid = ValidarCE(model);

            if (ModelState.IsValid && valid)
            {
                c_sub_proceso = db.c_sub_proceso.Find(model.id_sub_proceso);

                recordChange(c_sub_proceso);

                c_sub_proceso.cl_sub_proceso = model.cl_sub_proceso;
                c_sub_proceso.nb_sub_proceso = model.nb_sub_proceso;
                c_sub_proceso.ds_sub_proceso = model.ds_sub_proceso;
                c_sub_proceso.id_responsable = model.id_responsable;
                c_sub_proceso.cl_manual = model.cl_manual;
                c_sub_proceso.cl_sp_anterior = model.cl_sp_anterior;
                c_sub_proceso.cl_sp_siguiente = model.cl_sp_siguiente;
                c_sub_proceso.id_tipologia_sub_proceso = model.id_tipologia_sub_proceso;
                c_sub_proceso.id_etapa = model.id_etapa;
                c_sub_proceso.id_sub_etapa = model.id_sub_etapa;
                c_sub_proceso.ds_areas_involucradas = model.ds_areas_involucradas;
                c_sub_proceso.ds_aplicaciones_relacionadas = model.ds_aplicaciones_relacionadas;
                c_sub_proceso.nb_archivo_flujo = model.nb_archivo_flujo;
                c_sub_proceso.nb_archivo_manual = model.nb_archivo_manual;

                c_sub_proceso = llenarCamposExtra(model, c_sub_proceso);

                

                db.Entry(c_sub_proceso).State = EntityState.Modified;
                db.SaveChanges();

                //para cad tabla existente se borran las que ya no aparezcan en la lista

                //para caa elemento de la lista, comprobar si existe tabla, en caso contrario, crearla


                // Guarda los datos de las tablas externas
                id = c_sub_proceso.id_sub_proceso;
                c_sub_proceso.c_linea_negocio.Clear();

                //Guarda los participantes

                List<int> responsables;
                try
                {
                    responsables = model.id_responsables.ToList();
                }
                catch
                {
                    responsables = new List<int>();
                }

                if (!(responsables.Any(c => c == model.id_responsable)))
                {
                    responsables.Add(model.id_responsable);
                }

                if (responsables != null)
                {
                    List<c_usuario_sub_proceso> c_us_sp = db.c_usuario_sub_proceso.Where(c => c.id_sub_proceso == id).ToList();
                    foreach (var cussp in c_us_sp)
                    {
                        if (!(responsables.Contains(cussp.id_usuario)))
                        {
                            db.c_usuario_sub_proceso.Remove(cussp);
                        }
                    }

                    foreach (int id_resp in responsables)
                    {
                        if (!(db.c_usuario_sub_proceso.Any(c => c.id_usuario == id_resp && c.id_sub_proceso == id)))
                        {
                            var usp = new c_usuario_sub_proceso { id_usuario = id_resp, id_sub_proceso = id };
                            db.c_usuario_sub_proceso.Add(usp);
                        }
                    }
                    db.SaveChanges();
                }
                else
                {
                    List<c_usuario_sub_proceso> c_us_sp = db.c_usuario_sub_proceso.Where(c => c.id_sub_proceso == id).ToList();
                    foreach (var cussp in c_us_sp)
                    {
                        db.c_usuario_sub_proceso.Remove(cussp);
                    }
                    db.SaveChanges();
                }

                // Guarda las áreas de costeo
                if (model.id_areas_costeo != null)
                {
                    List<c_area_costeo_sub_proceso> c_ac_sp = db.c_area_costeo_sub_proceso.Where(c => c.id_sub_proceso == id).ToList();
                    foreach (var cacsp in c_ac_sp)
                    {
                        if (!(model.id_areas_costeo.Contains(cacsp.id_area_costeo)))
                        {
                            db.c_area_costeo_sub_proceso.Remove(cacsp);
                        }
                    }

                    foreach (int id_area_costeo in model.id_areas_costeo)
                    {
                        if (!(db.c_area_costeo_sub_proceso.Any(c => c.id_area_costeo == id_area_costeo && c.id_sub_proceso == id)))
                        {
                            var acsp = new c_area_costeo_sub_proceso { id_area_costeo = id_area_costeo, id_sub_proceso = id };
                            db.c_area_costeo_sub_proceso.Add(acsp);
                        }
                    }
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        List<c_area_costeo_sub_proceso> c_ac_sp = db.c_area_costeo_sub_proceso.Where(c => c.id_sub_proceso == id).ToList();
                        foreach (var cacsp in c_ac_sp)
                        {
                            db.c_area_costeo_sub_proceso.Remove(cacsp);
                        }
                        db.SaveChanges();
                    }
                    catch
                    {

                    }
                }

              

                // Guarda las lineas de negocio
                if (model.id_lineas_negocio != null)
                {
                    c_linea_negocio c_linea_negocio;
                    foreach (int id_linea_negocio in model.id_lineas_negocio)
                    {
                        c_linea_negocio = db.c_linea_negocio.Find(id_linea_negocio);
                        c_sub_proceso.c_linea_negocio.Add(c_linea_negocio);
                    }
                    db.SaveChanges();
                }

                if (c_sub_proceso.id_responsable != lu) Utilidades.Utilidades.ObjectAsigned(c_sub_proceso, lu);

                return RedirectToAction("Index");
            }

            AgregarSubProcesoViewModel model2 = new AgregarSubProcesoViewModel();

            id = model.id_sub_proceso;

            c_sub_proceso = db.c_sub_proceso.Find(id);

            model2.id_proceso = c_sub_proceso.id_proceso;
            model2.id_sub_proceso = c_sub_proceso.id_sub_proceso;
            model2.cl_sub_proceso = model.cl_sub_proceso;
            model2.nb_sub_proceso = model.nb_sub_proceso;
            model2.ds_sub_proceso = model.ds_sub_proceso;
            model2.id_responsable = model.id_responsable;
            model2.cl_manual = model.cl_manual;
            model2.cl_sp_anterior = model.cl_sp_anterior;
            model2.cl_sp_siguiente = model.cl_sp_siguiente;
            model2.id_tipologia_sub_proceso = model.id_tipologia_sub_proceso;
            model2.id_etapa = model.id_etapa;
            model2.id_sub_etapa = model.id_sub_etapa;
            model2.ds_areas_involucradas = model.ds_areas_involucradas;
            model2.ds_aplicaciones_relacionadas = model.ds_aplicaciones_relacionadas;
            model2.nb_archivo_manual = c_sub_proceso.nb_archivo_manual;
            model2.nb_archivo_flujo = c_sub_proceso.nb_archivo_flujo;

            model2.c_etapa = c_sub_proceso.c_etapa;
            model2.c_proceso = c_sub_proceso.c_proceso;
            model2.c_sub_etapa = c_sub_proceso.c_sub_etapa;
            model2.c_usuario = c_sub_proceso.c_usuario;
            model2.c_tipologia_sub_proceso = c_sub_proceso.c_tipologia_sub_proceso;

            //llenar multiselects con valores seleccionados
            string sql = "select id_usuario from c_usuario_sub_proceso where id_sub_proceso = " + id;
            var marcados = db.Database.SqlQuery<int>(sql).ToArray();
            ViewBag.responsables = Utilidades.DropDown.UsuariosMS(marcados);

            sql = "select id_linea_negocio from c_linea_negocio_sub_proceso where id_sub_proceso = " + id;
            marcados = db.Database.SqlQuery<int>(sql).ToArray();
            ViewBag.lineas_negocio = Utilidades.DropDown.LineasNegocioMS(marcados);

            sql = "select id_area_costeo from c_area_costeo_sub_proceso where id_sub_proceso = " + id;
            marcados = db.Database.SqlQuery<int>(sql).ToArray();
            ViewBag.areas_costeo = Utilidades.DropDown.AreasCosteoMS(marcados);

            ViewBag.id_etapa = Utilidades.DropDown.Etapas(model.id_etapa ?? 0);
            ViewBag.id_sub_etapa = Utilidades.DropDown.SubEtapas(model.id_sub_etapa ?? 0);
            ViewBag.id_responsableL = Utilidades.DropDown.Usuario(model.id_responsable);
            ViewBag.id_tipologia_sub_procesoL = Utilidades.DropDown.TipologiaSubProceso((int)model.id_tipologia_sub_proceso);

            //Campos Extra
            ViewBag.CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);

            model = obtenerCamposExtra(model2, model);

            return View(model2);
        }


        bool recordChange(c_sub_proceso sp)
        {
            var user = (IdentityPersonalizado)User.Identity;
            var registro = new r_sub_proceso();
            //esta parte ya copia los campos extra
            registro = (r_sub_proceso)Utilidades.Utilidades.CopyObject(sp, registro);
            registro.fe_modificacion = DateTime.Now;
            registro.id_usuario = user.Id_usuario;

            registro.nb_entidad = $"{sp.c_proceso.c_macro_proceso.c_entidad.cl_entidad} - {sp.c_proceso.c_macro_proceso.c_entidad.nb_entidad}";
            registro.nb_macro_proceso = $"{sp.c_proceso.c_macro_proceso.cl_macro_proceso} - {sp.c_proceso.c_macro_proceso.nb_macro_proceso}";
            registro.nb_proceso = $"{sp.c_proceso.cl_proceso} - {sp.c_proceso.nb_proceso}";
            registro.nb_responsable = sp.c_usuario.nb_usuario;
            string[] participantesArray = sp.c_usuario_sub_proceso.Select(p => p.c_usuario.nb_usuario).ToArray();
            registro.participantes = string.Join(", ", participantesArray);
            registro.tipologia_sub_proceso = sp.c_tipologia_sub_proceso.nb_tipologia_sub_proceso;
            string[] lineasnegocioArray = sp.c_linea_negocio.Select(l => l.nb_linea_negocio).ToArray();
            registro.lineas_negocio = string.Join(", ", lineasnegocioArray);
            string[] areascosteoArray = sp.c_area_costeo_sub_proceso.Select(a => a.c_area_costeo.nb_area_costeo).ToArray();
            registro.areas_costeo = string.Join(", ", areascosteoArray);
            if (sp.c_etapa != null)
                registro.nb_etapa = sp.c_etapa.nb_etapa;
            else
                registro.nb_etapa = "";
            if (sp.c_sub_etapa != null)
                registro.nb_sub_etapa = sp.c_sub_etapa.nb_sub_etapa;
            else
                registro.nb_sub_etapa = "";





            db.r_sub_proceso.Add(registro);

            try
            {
                return db.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Historial
        public ActionResult Historial(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_sub_proceso c_sub_proceso = db.c_sub_proceso.Find(id);
            if (c_sub_proceso == null)
            {
                return HttpNotFound();
            }

            var historial = c_sub_proceso.r_sub_proceso.OrderByDescending(r => r.fe_modificacion).ToList();
            ViewBag.sp = c_sub_proceso;
            return View(historial);
        }
        #endregion

        #region Delete
        // GET: SubProceso/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_sub_proceso c_sub_proceso = db.c_sub_proceso.Find(id);
            if (c_sub_proceso == null)
            {
                return HttpNotFound();
            }

            if (redirect != null)
            {
                if (redirect != "bfo")
                {
                    //obtenemos el valor del numero de salto
                    int ns;
                    try
                    {
                        ns = (int)HttpContext.Session["JumpCounter"];
                    }
                    catch
                    {
                        ns = 0;
                    }
                    //Si ns es 0, creamos un nuevo array, agregamos la direccion actual y lo asignamos a la variable "Directions" y establecemos "JumpCounter" = 1
                    if (ns == 0)
                    {
                        List<string> directions = new List<string>();
                        directions.Add(redirect);
                        HttpContext.Session["JumpCounter"] = 1;
                        HttpContext.Session["Directions"] = directions;

                    }//En caso de que ns sea distinto a 0, obtenemos el Array "Directions", agregamos la direccion actual, aumentamos el contador y salvamos ambas variables globales
                    else
                    {
                        ns++;
                        List<string> directions = (List<string>)HttpContext.Session["Directions"];
                        directions.Add(redirect);
                        HttpContext.Session["JumpCounter"] = ns;
                        HttpContext.Session["Directions"] = directions;
                    }
                }
            }
            else
            {
                HttpContext.Session["JumpCounter"] = null;
                HttpContext.Session["Directions"] = null;
            }

            //Obtener todos los elementos a los que puede estar ligado este elemento.
            //creamos la lista que contendra a todos los registros relacionados
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();

            //Incluiremos benchmark, BDEI y Riesgos
            var r_benchmarck = db.k_benchmarck.Where(b => b.id_sub_proceso == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_benchmarck.Count > 0)
            {
                foreach (var benchmarck in r_benchmarck)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Benchmarck";
                    rr.cl_registro = benchmarck.id_benchmark.ToString();
                    rr.nb_registro = "Benchmarck ligado al Sub Proceso" + benchmarck.c_sub_proceso.nb_sub_proceso;
                    rr.accion = "Delete";
                    rr.controlador = "Benchmark";
                    rr.id_registro = benchmarck.id_benchmark.ToString();

                    RR.Add(rr);
                }
            }

            var r_bdei = db.k_bdei.Where(b => b.id_sub_proceso == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_bdei.Count > 0)
            {
                foreach (var bdei in r_bdei)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "BDEI";
                    rr.cl_registro = bdei.id_bdei.ToString();
                    rr.nb_registro = "BDEI ligado a la entidad: " + bdei.c_entidad.nb_entidad;
                    rr.accion = "Delete";
                    rr.controlador = "BDEI";
                    rr.id_registro = bdei.id_bdei.ToString();

                    RR.Add(rr);
                }
            }

            var r_riesgo = db.k_riesgo.Where(b => b.id_sub_proceso == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_riesgo.Count > 0)
            {
                foreach (var riesgo in r_riesgo)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Riesgos";
                    rr.cl_registro = riesgo.nb_riesgo;
                    rr.nb_registro = riesgo.evento;
                    rr.accion = "Delete";
                    rr.controlador = "Riesgo";
                    rr.id_registro = riesgo.id_riesgo.ToString();

                    RR.Add(rr);
                }
            }

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;

            ViewBag.CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);

            return View(c_sub_proceso);
        }

        // POST: SubProceso/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_sub_proceso c_sub_proceso = db.c_sub_proceso.Find(id);

            //Eliminar Objetos relacionados
            Utilidades.DeleteActions.DeleteSubProcesoObjects(c_sub_proceso, db);

            /*bool tf = c_sub_proceso.nb_archivo_flujo != null ? true : false;
            bool tn = c_sub_proceso.nb_archivo_manual != null ? true : false;*/

            db.c_sub_proceso.Remove(c_sub_proceso);
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
        private bool ValidarCE(AgregarSubProcesoViewModel model)
        {
            string[] campo = new string[20];
            string[] errores = new string[20];
            bool response = true;

            //Validacion del Sub Proceso
            //Asignacion de valores
            campo[0] = model.campo01; campo[1] = model.campo02;
            campo[2] = model.campo03; campo[3] = model.campo04;
            campo[4] = model.campo05; campo[5] = model.campo06;
            campo[6] = model.campo07; campo[7] = model.campo08;
            campo[8] = model.campo09; campo[9] = model.campo10;
            campo[10] = model.campo11; campo[11] = model.campo12;
            campo[12] = model.campo13; campo[13] = model.campo14;
            campo[14] = model.campo15; campo[15] = model.campo16;
            campo[16] = model.campo17; campo[17] = model.campo18;
            campo[18] = model.campo19; campo[19] = model.campo20;

            //Informacion de Meta Campos Riesgos
            var infor = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);

            //Validacion
            for (int i = 0; i < 20; i++)
            {
                var inf = infor[i];

                bool validate = inf.es_visible;

                //Si el campo es visible, comenzamos las validaciones, de otra forma lo ignoramos
                if (validate)
                {
                    if (inf.es_requerido && (campo[i] == null || campo[i] == ""))
                    {
                        errores[i] = "El campo \"" + inf.nb_campo + "\" es un campo requerido.";
                        response = false;
                    }
                    else
                    {
                        errores[i] = "";
                    }


                    if (campo[i] != null && campo[i] != "")
                    {
                        if (inf.longitud_campo < campo[i].Length)
                        {
                            errores[i] = "La longitud máxima de el campo " + inf.nb_campo + " es de: " + inf.longitud_campo + " caracteres.";
                            response = false;
                        }
                        else
                        {
                            errores[i] = "";
                        }
                    }
                }
            }

            ControllerContext.Controller.ViewBag.MSError = errores;
            return response;
        }

        c_sub_proceso llenarCamposExtra(AgregarSubProcesoViewModel model, c_sub_proceso sp)
        {
            sp.campo01 = model.campo01; sp.campo02 = model.campo02;
            sp.campo03 = model.campo03; sp.campo04 = model.campo04;
            sp.campo05 = model.campo05; sp.campo06 = model.campo06;
            sp.campo07 = model.campo07; sp.campo08 = model.campo08;
            sp.campo09 = model.campo09; sp.campo10 = model.campo10;
            sp.campo11 = model.campo11; sp.campo12 = model.campo12;
            sp.campo13 = model.campo13; sp.campo14 = model.campo14;
            sp.campo15 = model.campo15; sp.campo16 = model.campo16;
            sp.campo17 = model.campo17; sp.campo18 = model.campo18;
            sp.campo19 = model.campo19; sp.campo20 = model.campo20;

            return sp;
        }

        AgregarSubProcesoViewModel obtenerCamposExtra(AgregarSubProcesoViewModel model, c_sub_proceso sp)
        {
            model.campo01 = sp.campo01; model.campo02 = sp.campo02;
            model.campo03 = sp.campo03; model.campo04 = sp.campo04;
            model.campo05 = sp.campo05; model.campo06 = sp.campo06;
            model.campo07 = sp.campo07; model.campo08 = sp.campo08;
            model.campo09 = sp.campo09; model.campo10 = sp.campo10;
            model.campo11 = sp.campo11; model.campo12 = sp.campo12;
            model.campo13 = sp.campo13; model.campo14 = sp.campo14;
            model.campo15 = sp.campo15; model.campo16 = sp.campo16;
            model.campo17 = sp.campo17; model.campo18 = sp.campo18;
            model.campo19 = sp.campo19; model.campo20 = sp.campo20;

            return model;
        }

        AgregarSubProcesoViewModel obtenerCamposExtra(AgregarSubProcesoViewModel model, AgregarSubProcesoViewModel sp)
        {
            model.campo01 = sp.campo01; model.campo02 = sp.campo02;
            model.campo03 = sp.campo03; model.campo04 = sp.campo04;
            model.campo05 = sp.campo05; model.campo06 = sp.campo06;
            model.campo07 = sp.campo07; model.campo08 = sp.campo08;
            model.campo09 = sp.campo09; model.campo10 = sp.campo10;
            model.campo11 = sp.campo11; model.campo12 = sp.campo12;
            model.campo13 = sp.campo13; model.campo14 = sp.campo14;
            model.campo15 = sp.campo15; model.campo16 = sp.campo16;
            model.campo17 = sp.campo17; model.campo18 = sp.campo18;
            model.campo19 = sp.campo19; model.campo20 = sp.campo20;

            return model;
        }
        #endregion

        public ActionResult DirectDelete(int id)
        {
            var sp = db.c_sub_proceso.Find(id);

            Utilidades.DeleteActions.DeleteSubProcesoObjects(sp, db, true);

            db.c_sub_proceso.Remove(sp);

            db.SaveChanges();

            return RedirectToAction("Index");
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
