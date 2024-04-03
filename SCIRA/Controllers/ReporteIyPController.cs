using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "RepIyP", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ReporteIyPController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: ReporteIyP
        public ActionResult Index()
        {
            var user = ((IdentityPersonalizado)User.Identity);

            //var objetos = Utilidades.Utilidades.RTCObjeto(db.c_usuario.Find(user.Id_usuario), db);
            var objetos = Utilidades.Utilidades.RTCObject(db.c_usuario.Find(user.Id_usuario), db, "k_objeto").Cast<k_objeto>().ToList();

            //Primero obtendremos todos los seguimientos de los planes de remediacion
            ViewBag.tipo1 = FillIyP1(objetos);
            ViewBag.tipo2 = FillIyP2(objetos);
            ViewBag.tipo3 = FillIyP3(objetos);
            ViewBag.tipo4 = FillIyP4();
            ViewBag.tipo5 = FillIyP5();
            ViewBag.tipo6 = FillIyP6(objetos);



            return View();
        }

        #region Info Oficios
        public List<IyP1RepModel> FillIyP1(List<k_objeto> OBJS)
        {
            var res = new List<IyP1RepModel>();

            //obtener oficios

            var objetos = OBJS.Where(o => o.tipo_objeto == 1).ToList();

            foreach (var obj in objetos)
            {
                try
                {
                    //Llenar la primera parte del modelo
                    var model = new IyP1RepModel();

                    model.ID = obj.id_objeto;
                    model.nb_objeto = obj.nb_objeto;
                    model.no_incidencias = obj.no_incidencias;
                    model.fe_objeto = obj.fe_alta;
                    model.fe_vencimiento = obj.fe_vencimiento;
                    model.fe_contestacion = obj.fe_contestacion;
                    model.cnb_entidad = obj.c_entidad.cl_entidad + " - " + obj.c_entidad.nb_entidad;
                    model.tipo_objeto = obj.tipo_objeto;
                    model.cnb_autoridad = obj.c_origen_autoridad.cl_origen_autoridad + " - " + obj.c_origen_autoridad.nb_origen_autoridad;
                    model.nb_resp_oficio = obj.c_usuario.nb_usuario;

                    var incidencias = obj.k_incidencia.ToList();

                    res.AddRange(fillModel(incidencias, model));
                }
                catch
                {
                    Utilidades.Utilidades.CreateErrorReg("Error al insertar " + obj.nb_objeto + "(" + obj.id_objeto + ")", "ReoirteIyPController/FillIyP1");
                }
                
            }
            return res;
        }
        #endregion

        #region Info Informes AuExt
        public List<IyP1RepModel> FillIyP2(List<k_objeto> OBJS)
        {
            var res = new List<IyP1RepModel>();

            //obtener oficios

            //esta funcion se cambiará por obtener los oficios dependiendo de si se es super usuario o no
            var objetos = OBJS.Where(o => o.tipo_objeto == 2).ToList();

            foreach (var obj in objetos)
            {
                //Llenar la primera parte del modelo
                var model = new IyP1RepModel();

                model.ID = obj.id_objeto;
                model.nb_objeto = obj.nb_objeto;
                model.no_incidencias = obj.no_incidencias;
                model.fe_objeto = obj.fe_alta;
                model.fe_contestacion = obj.fe_contestacion;
                model.cnb_entidad = obj.c_entidad.cl_entidad + " - " + obj.c_entidad.nb_entidad;
                model.tipo_objeto = obj.tipo_objeto;
                model.nb_resp_oficio = obj.c_usuario.nb_usuario;

                var incidencias = obj.k_incidencia.ToList();

                res.AddRange(fillModel(incidencias, model));
            }
            return res;
        }
        #endregion

        #region Info Informes AuExt
        public List<IyP1RepModel> FillIyP3(List<k_objeto> OBJS)
        {
            var res = new List<IyP1RepModel>();

            //obtener oficios

            //esta funcion se cambiará por obtener los oficios dependiendo de si se es super usuario o no
            var objetos = OBJS.Where(o => o.tipo_objeto == 3).ToList();

            foreach (var obj in objetos)
            {
                //Llenar la primera parte del modelo
                var model = new IyP1RepModel();

                model.ID = obj.id_objeto;
                model.nb_objeto = obj.nb_objeto;
                model.no_incidencias = obj.no_incidencias;
                model.fe_objeto = obj.fe_alta;
                model.fe_contestacion = obj.fe_contestacion;
                model.cnb_entidad = obj.c_entidad.cl_entidad + " - " + obj.c_entidad.nb_entidad;
                model.tipo_objeto = obj.tipo_objeto;
                model.nb_resp_oficio = obj.c_usuario.nb_usuario;

                var incidencias = obj.k_incidencia.ToList();

                res.AddRange(fillModel(incidencias, model));
            }
            return res;
        }
        #endregion

        #region Info Cert
        public List<IyP1RepModel> FillIyP4()
        {
            var res = new List<IyP1RepModel>();

            //obtener Certificaciones

            var certificaciones = Utilidades.Utilidades.RTCCertificacion(db.c_usuario.Find(((IdentityPersonalizado)User.Identity).Id_usuario), db);
            certificaciones = certificaciones.Where(c => !c.tiene_disenio_efectivo || !c.tiene_funcionamiento_efectivo).ToList();

            foreach (var obj in certificaciones)
            {
                //Llenar la primera parte del modelo
                var model = new IyP1RepModel();
                var control = obj.k_control;
                var riesgo = control.k_riesgo.First();
                var sp = control.c_sub_proceso;
                var pr = sp.c_proceso;
                var mp = pr.c_macro_proceso;
                var en = mp.c_entidad;


                model.ruta_control = en.cl_entidad + "/" + mp.cl_macro_proceso + "/" + pr.cl_proceso + "/" + sp.cl_sub_proceso + "/" + riesgo.nb_riesgo;
                model.codigo_control = control.relacion_control;
                model.cl_certificacion_control = obj.cl_certificacion_control;
                model.periodo_certificacion = obj.c_periodo_certificacion.nb_periodo_certificacion;
                model.ds_procedimiento_certificacion = obj.ds_procedimiento_certificacion;

                var incidencias = obj.k_incidencia.ToList();

                res.AddRange(fillModel(incidencias, model));
            }
            return res;
        }
        #endregion

        #region Info MRyC
        public List<IyP1RepModel> FillIyP5()
        {
            var res = new List<IyP1RepModel>();

            //obtener Certificaciones

            var controles = Utilidades.Utilidades.RTCObject(db.c_usuario.Find(((IdentityPersonalizado)User.Identity).Id_usuario), db, "k_control", "1").Cast<k_control>().ToList();
            controles = controles.Where(c => c.tiene_accion_correctora).ToList();

            foreach (var obj in controles)
            {
                //Llenar la primera parte del modelo
                var model = new IyP1RepModel();
                var riesgo = obj.k_riesgo.First();
                var sp = obj.c_sub_proceso;
                var pr = sp.c_proceso;
                var mp = pr.c_macro_proceso;
                var en = mp.c_entidad;


                model.ruta_control = en.cl_entidad + "/" + mp.cl_macro_proceso + "/" + pr.cl_proceso + "/" + sp.cl_sub_proceso + "/" + riesgo.nb_riesgo;
                model.codigo_riesgo = riesgo.nb_riesgo;
                model.evento_riesgo = riesgo.evento;
                model.accion_correctora = obj.accion_correctora;

                var incidencias = obj.k_incidencia.ToList();

                res.AddRange(fillModel(incidencias, model));
            }
            return res;
        }
        #endregion

        #region Info Otros
        public List<IyP1RepModel> FillIyP6(List<k_objeto> OBJS)
        {
            var res = new List<IyP1RepModel>();

            //obtener oficios

            //esta funcion se cambiará por obtener los oficios dependiendo de si se es super usuario o no
            var objetos = OBJS.Where(o => o.tipo_objeto == 6).ToList();

            foreach (var obj in objetos)
            {
                //Llenar la primera parte del modelo
                var model = new IyP1RepModel();

                model.ID = obj.id_objeto;
                model.nb_objeto = obj.nb_objeto;
                model.fe_objeto = obj.fe_alta;
                model.cnb_entidad = obj.c_entidad.cl_entidad + " - " + obj.c_entidad.nb_entidad;
                model.tipo_objeto = obj.tipo_objeto;
                model.ds_objeto = obj.ds_objeto;

                var incidencias = obj.k_incidencia.ToList();

                res.AddRange(fillModel(incidencias, model));
            }
            return res;
        }
        #endregion


        #region Auxiliares
        private List<IyP1RepModel> fillModel(List<k_incidencia> incidencias, IyP1RepModel model)
        {
            var res = new List<IyP1RepModel>();

            if (incidencias.Count > 0)
            {
                foreach (var inc in incidencias)
                {
                    var modeL2 = (IyP1RepModel)Utilidades.Utilidades.CopyObject(model, new IyP1RepModel());

                    modeL2.lvl_1 = inc.lvl_1;
                    modeL2.lvl_2 = inc.lvl_2;
                    modeL2.lvl_3 = inc.lvl_3;
                    modeL2.lvl_4 = inc.lvl_4;
                    modeL2.lvl_5 = inc.lvl_5;
                    modeL2.id_incidencia = inc.id_incidencia;
                    modeL2.ds_incidencia = inc.ds_incidencia;
                    modeL2.nb_resp_inc = inc.c_usuario.nb_usuario;
                    modeL2.cnb_clasificacion_incidencia = inc.c_clasificacion_incidencia.cl_clasificacion_incidencia + " - " + inc.c_clasificacion_incidencia.nb_clasificacion_incidencia;
                    modeL2.requiere_plan = inc.requiere_plan;

                    var planes = inc.k_plan.ToList();

                    if (planes.Count > 0)
                    {
                        foreach (var pl in planes)
                        {
                            var modeL3 = (IyP1RepModel)Utilidades.Utilidades.CopyObject(modeL2, new IyP1RepModel());
                            modeL3.id_plan = pl.id_plan;
                            modeL3.nb_plan = pl.nb_plan;
                            modeL3.ds_plan = pl.ds_plan;
                            modeL3.cnb_area = pl.c_area.cl_area + " - " + pl.c_area.nb_area;
                            modeL3.nb_resp_plan = pl.c_usuario.nb_usuario;
                            modeL3.nb_resp_seguimiento = pl.c_usuario1.nb_usuario;
                            modeL3.fe_alta_p = pl.fe_alta;
                            modeL3.fe_estimada_implantacion = pl.fe_estimada_implantacion;
                            modeL3.fe_real_solucion = pl.fe_real_solucion;

                            var seguimientos = pl.r_seguimiento;

                            if (seguimientos.Count > 0)
                            {
                                foreach (var seg in seguimientos)
                                {
                                    var modeL4 = (IyP1RepModel)Utilidades.Utilidades.CopyObject(modeL3, new IyP1RepModel());

                                    modeL4.fe_seguimiento = seg.fe_seguimiento;
                                    modeL4.obs_seguimiento = seg.observaciones;

                                    res.Add(modeL4);
                                }
                            }
                            else
                            {
                                res.Add(modeL3);
                            }
                        }
                    }
                    else
                    {
                        res.Add(modeL2);
                    }
                }
            }
            else
            {
                res.Add(model);
            }

            return res;
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