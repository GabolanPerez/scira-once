using SCIRA.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    public class SelectListController : Controller
    {
        private SICIEntities db = new SICIEntities();

        private ISelectListRepository _repository;

        public SelectListController() : this(new SelectListRepository())
        {
        }

        public SelectListController(ISelectListRepository repository)
        {
            _repository = repository;
        }

        #region Estructura
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ObtieneMacroProcesos(string IdEntidad)
        {
            if (String.IsNullOrEmpty(IdEntidad))
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
            int id = 0;
            bool isValid = Int32.TryParse(IdEntidad, out id);
            var macroProcesos = _repository.ObtieneMacroProcesos(id);
            var result = (from s in macroProcesos.OrderBy(x => x.cl_macro_proceso)
                          select new
                          {
                              id = s.id_macro_proceso,
                              name = s.cl_macro_proceso + " - " + s.nb_macro_proceso
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ObtieneProcesos(string IdMacroProceso)
        {
            if (String.IsNullOrEmpty(IdMacroProceso))
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
            int id = 0;
            bool isValid = Int32.TryParse(IdMacroProceso, out id);
            var procesos = _repository.ObtieneProcesos(id);
            var result = (from s in procesos.OrderBy(x => x.cl_proceso)
                          select new
                          {
                              id = s.id_proceso,
                              name = s.cl_proceso + " - " + s.nb_proceso
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ObtieneSubProcesos(string IdProceso)
        {
            if (String.IsNullOrEmpty(IdProceso))
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
            int id = 0;
            bool isValid = Int32.TryParse(IdProceso, out id);
            var subProcesos = _repository.ObtieneSubProcesos(id);
            var result = (from s in subProcesos.OrderBy(x => x.cl_sub_proceso)
                          select new
                          {
                              id = s.id_sub_proceso,
                              name = s.cl_sub_proceso + " - " + s.nb_sub_proceso
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion


        // Tipo de Riesgo

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ObtieneTiposRiesgo(string IdCategoriaRiesgo)
        {
            if (String.IsNullOrEmpty(IdCategoriaRiesgo))
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
            int id = 0;
            bool isValid = Int32.TryParse(IdCategoriaRiesgo, out id);
            var tiposRiesgo = _repository.ObtieneTiposRiesgo(id);
            var result = (from s in tiposRiesgo.OrderBy(x => x.cl_tipo_riesgo)
                          select new
                          {
                              id = s.id_tipo_riesgo,
                              name = s.cl_tipo_riesgo + " - " + s.nb_tipo_riesgo
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // Tipología Riesgo

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ObtieneSubClasesTipologiaRiesgo(string idClaseTipologiaRiesgo)
        {
            if (String.IsNullOrEmpty(idClaseTipologiaRiesgo))
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
            int id = 0;
            bool isValid = Int32.TryParse(idClaseTipologiaRiesgo, out id);
            var subClasesTipologiaRiesgo = _repository.ObtieneSubClasesTipologiaRiesgo(id);
            var result = (from s in subClasesTipologiaRiesgo.OrderBy(x => x.cl_sub_clase_tipologia_riesgo)
                          select new
                          {
                              id = s.id_sub_clase_tipologia_riesgo,
                              name = s.cl_sub_clase_tipologia_riesgo + " - " + s.nb_sub_clase_tipologia_riesgo
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ObtieneTipologiasRiesgo(string idSubClaseTipologiaRiesgo)
        {
            if (String.IsNullOrEmpty(idSubClaseTipologiaRiesgo))
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
            int id = 0;
            bool isValid = Int32.TryParse(idSubClaseTipologiaRiesgo, out id);
            var tipologiasRiesgo = _repository.ObtieneTipologiasRiesgo(id);
            var result = (from s in tipologiasRiesgo.OrderBy(x => x.cl_tipologia_riesgo)
                          select new
                          {
                              id = s.id_tipologia_riesgo,
                              name = s.cl_tipologia_riesgo + " - " + s.nb_tipologia_riesgo
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ObtieneMacroProcesosBenchmark(string idEntidad)
        {
            if (String.IsNullOrEmpty(idEntidad))
            {
                var objectsAX = db.c_actividad.Where(obj => obj.id_entidad == null).ToList();
                var resultAx = (from o in objectsAX
                                select new
                                {
                                    id = o.id_actividad,
                                    name = o.cl_actividad + " - " + o.nb_actividad
                                }).ToList();
                return Json(resultAx, JsonRequestBehavior.AllowGet);
            }
            int id = 0;
            bool isValid = Int32.TryParse(idEntidad, out id);
            var objects = db.c_actividad.Where(obj => obj.id_entidad.ToString() == idEntidad || obj.id_entidad == null).ToList();
            var result = (from o in objects
                          select new
                          {
                              id = o.id_actividad,
                              name = o.cl_actividad + " - " + o.nb_actividad
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ObtieneProcesosBenchmark(string idActividad)
        {
            if (String.IsNullOrEmpty(idActividad))
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
            int id = 0;
            bool isValid = Int32.TryParse(idActividad, out id);
            var Procesos = db.c_proceso_benchmark.Where(pr => pr.id_actividad.ToString() == idActividad).ToList();
            var result = (from p in Procesos
                          select new
                          {
                              id = p.id_proceso_benchmark,
                              name = p.cl_proceso_benchmark + " - " + p.nb_proceso_benchmark
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ObtieneSubProcesosBenchmark(string idProcesoBenchmark)
        {
            if (String.IsNullOrEmpty(idProcesoBenchmark))
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
            int id = 0;
            bool isValid = Int32.TryParse(idProcesoBenchmark, out id);
            var subProcesos = db.c_sub_proceso_benchmark.Where(spb => spb.id_proceso_benchmark.ToString() == idProcesoBenchmark).ToList();
            var result = (from s in subProcesos
                          select new
                          {
                              id = s.id_sub_proceso_benchmark,
                              name = s.cl_sub_proceso_benchmark + " - " + s.nb_sub_proceso_benchmark
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ObtieneEventosBenchmark(string idSubProcesoBenchmark)
        {
            if (String.IsNullOrEmpty(idSubProcesoBenchmark))
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
            int id = 0;
            bool isValid = Int32.TryParse(idSubProcesoBenchmark, out id);
            var EventosRiesgo = db.c_evento_riesgo.Where(er => er.id_sub_proceso_benchmark.ToString() == idSubProcesoBenchmark).ToList();
            var result = (from er in EventosRiesgo
                          select new
                          {
                              id = er.id_evento_riesgo,
                              name = er.cl_evento_riesgo + " - " + er.nb_evento_riesgo
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        #region BDEI


        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ObtieneSubTiposRiesgoOperacional(string idTRO)
        {
            if (String.IsNullOrEmpty(idTRO))
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
            int id = 0;
            bool isValid = Int32.TryParse(idTRO, out id);
            var subTiposRiesgo = _repository.ObtieneSubTiposRiesgoOperacional(id);
            var result = (from s in subTiposRiesgo.OrderBy(x => x.cl_sub_tipo_riesgo_operacional)
                          select new
                          {
                              id = s.id_sub_tipo_riesgo_operacional,
                              name = s.cl_sub_tipo_riesgo_operacional + " - " + s.nb_sub_tipo_riesgo_operacional
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ObtieneClasesEventoRiesgoOperacional(string idSTRO)
        {
            if (String.IsNullOrEmpty(idSTRO))
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
            int id = 0;
            bool isValid = Int32.TryParse(idSTRO, out id);
            var clasesEvento = _repository.ObtieneClasesEventoRiesgoOperacional(id);
            var result = (from s in clasesEvento.OrderBy(x => x.cl_clase_evento)
                          select new
                          {
                              id = s.id_clase_evento,
                              name = s.cl_clase_evento + " - " + s.nb_clase_evento
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ObtieneProcesosRiesgoOperacional(string idARO)
        {
            if (String.IsNullOrEmpty(idARO))
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
            int id = 0;
            bool isValid = Int32.TryParse(idARO, out id);
            var Procesos = _repository.ObtieneProcesosRiesgoOperacional(id);
            var result = (from s in Procesos.OrderBy(x => x.cl_proceso_riesgo_operacional)
                          select new
                          {
                              id = s.id_proceso_riesgo_operacional,
                              name = s.cl_proceso_riesgo_operacional + " - " + s.nb_proceso_riesgo_operacional
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ObtieneSubTiposProductoRiesgoOperacional(string idPRRO)
        {
            if (String.IsNullOrEmpty(idPRRO))
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
            int id = 0;
            bool isValid = Int32.TryParse(idPRRO, out id);
            var STP = _repository.ObtieneSubTiposProductoRiesgoOperacional(id);
            var result = (from s in STP.OrderBy(x => x.cl_sub_tipo_producto_riesgo_operacional)
                          select new
                          {
                              id = s.id_sub_tipo_producto_riesgo_operacional,
                              name = s.cl_sub_tipo_producto_riesgo_operacional + " - " + s.nb_sub_tipo_producto_riesgo_operacional
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ObtieneLineasNegocioRiesgoOperacional(string idCLNRO)
        {
            if (String.IsNullOrEmpty(idCLNRO))
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
            int id = 0;
            bool isValid = Int32.TryParse(idCLNRO, out id);
            var LN = _repository.ObtieneLineasNegocioRiesgoOperacional(id);
            var result = (from s in LN.OrderBy(x => x.cl_linea_negocio_riesgo_operacional)
                          select new
                          {
                              id = s.id_linea_negocio_riesgo_operacional,
                              name = s.cl_linea_negocio_riesgo_operacional + " - " + s.nb_linea_negocio_riesgo_operacional
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ObtieneCuentasContables(string idEN)
        {
            if (String.IsNullOrEmpty(idEN))
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
            int id = 0;
            bool isValid = Int32.TryParse(idEN, out id);
            var CC = _repository.ObtieneCuentasContables(id);

            //encontrar cada grupo y crear una lista de ccItem temporal
            List<ccItem> aux = new List<ccItem>();

            foreach (var cc in CC)
            {
                var group = cc.c_grupo_cuenta_contable.nb_grupo_cuenta_contable;
                var groupID = cc.c_grupo_cuenta_contable.id_grupo_cuenta_contable;
                if (!aux.Any(i => i.type == "group" && i.name == group))
                {
                    //añadir un nuevo grupo a la lista
                    aux.Add(new ccItem { id = groupID, group = "", name = group, type = "group" });
                }

                aux.Add(new ccItem { id = cc.id_cuenta_contable, group = group + groupID, name = cc.cl_cuenta_contable + " - " + cc.nb_cuenta_contable, type = "item" });

            }


            var result = (from s in aux
                          select new
                          {
                              Id = s.id,
                              Group = s.@group,
                              Name = s.name,
                              Type = s.type
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public class ccItem
        {
            public int id { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public string group { get; set; }
        }

        #endregion

        // Dispose
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