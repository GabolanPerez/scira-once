using System.Collections.Generic;
using System.Linq;

namespace SCIRA.Models
{
    public class SelectListRepository : ISelectListRepository
    {
        private SICIEntities db = new SICIEntities();

        // Estructura

        public IList<c_entidad> ObtieneEntidades()
        {
            var c_entidad = db.c_entidad; //.OrderBy(x => x.cl_entidad);
            return c_entidad.ToList();
        }

        public IList<c_macro_proceso> ObtieneMacroProcesos(int idEntidad)
        {
            var c_macro_proceso = db.c_macro_proceso.Where(id => id.id_entidad == idEntidad); //.OrderBy(x => x.cl_macro_proceso);
            return c_macro_proceso.ToList();
        }

        public IList<c_proceso> ObtieneProcesos(int idMacroProceso)
        {
            var c_proceso = db.c_proceso.Where(id => id.id_macro_proceso == idMacroProceso); //.OrderBy(x => x.cl_proceso);
            return c_proceso.ToList();
        }

        public IList<c_sub_proceso> ObtieneSubProcesos(int idProceso)
        {
            var c_sub_proceso = db.c_sub_proceso.Where(id => id.id_proceso == idProceso); //.OrderBy(x => x.cl_sub_proceso);
            return c_sub_proceso.ToList();
        }

        // Tipo de Riesgo

        public IList<c_categoria_riesgo> ObtieneCategoriasRiesgo()
        {
            var c_categoria_riesgo = db.c_categoria_riesgo; //.OrderBy(x => x.nb_categoria_riesgo);
            return c_categoria_riesgo.ToList();
        }

        public IList<c_tipo_riesgo> ObtieneTiposRiesgo(int idCategoriaRiesgo)
        {
            var c_tipo_riesgo = db.c_tipo_riesgo.Where(id => id.id_categoria_riesgo == idCategoriaRiesgo); //.OrderBy(x => x.nb_tipo_riesgo);
            return c_tipo_riesgo.ToList();
        }

        // Tipología de Riesgo

        public IList<c_clase_tipologia_riesgo> ObtieneClasesTipologiaRiesgo()
        {
            var c_clase_tipologia_riesgo = db.c_clase_tipologia_riesgo; //.OrderBy(x => x.nb_clase_tipologia_riesgo);
            return c_clase_tipologia_riesgo.ToList();
        }

        public IList<c_sub_clase_tipologia_riesgo> ObtieneSubClasesTipologiaRiesgo(int idClaseTipologiaRiesgo)
        {
            var c_sub_clase_tipologia_riesgo = db.c_sub_clase_tipologia_riesgo.Where(id => id.id_clase_tipologia_riesgo == idClaseTipologiaRiesgo); //.OrderBy(x => x.nb_sub_clase_tipologia_riesgo);
            return c_sub_clase_tipologia_riesgo.ToList();
        }

        public IList<c_tipologia_riesgo> ObtieneTipologiasRiesgo(int idSubClaseTipologiaRiesgo)
        {
            var c_tipologia_riesgo = db.c_tipologia_riesgo.Where(id => id.id_sub_clase_tipologia_riesgo == idSubClaseTipologiaRiesgo); //.OrderBy(x => x.nb_tipologia_riesgo);
            return c_tipologia_riesgo.ToList();
        }

        //Benchmark
        public IList<c_actividad> ObtieneMacroProcesosBenchmark(int? idEntidad)
        {
            var c_actividad = db.c_actividad.Where(a => a.id_entidad == idEntidad);
            return c_actividad.ToList();
        }

        public IList<c_proceso_benchmark> ObtieneProcesosBenchmark(int idActividad)
        {
            var c_proceso = db.c_proceso_benchmark.Where(p => p.id_actividad == idActividad);
            return c_proceso.ToList();
        }

        public IList<c_sub_proceso_benchmark> ObtieneSubProcesosBenchmark(int idProcesoBenchmark)
        {
            var c_spb = db.c_sub_proceso_benchmark.Where(spb => spb.id_proceso_benchmark == idProcesoBenchmark);
            return c_spb.ToList();
        }

        public IList<c_evento_riesgo> ObtieneEventosBenchmark(int idSubProcesoBenchmark)
        {
            var c_er = db.c_evento_riesgo.Where(er => er.id_sub_proceso_benchmark == idSubProcesoBenchmark);
            return c_er.ToList();
        }

        //BDEI
        public IList<c_sub_tipo_riesgo_operacional> ObtieneSubTiposRiesgoOperacional(int idTRO)
        {
            var c_er = db.c_sub_tipo_riesgo_operacional.Where(er => er.id_tipo_riesgo_operacional == idTRO && (er.esta_activo ?? false));
            return c_er.ToList();
        }

        public IList<c_clase_evento> ObtieneClasesEventoRiesgoOperacional(int idSTRO)
        {
            var c_er = db.c_clase_evento.Where(er => er.id_sub_tipo_riesgo_operacional == idSTRO && (er.esta_activo ?? false));
            return c_er.ToList();
        }

        public IList<c_proceso_riesgo_operacional> ObtieneProcesosRiesgoOperacional(int idARO)
        {
            var c_er = db.c_proceso_riesgo_operacional.Where(er => er.id_ambito_riesgo_operacional == idARO && (er.esta_activo ?? false));
            return c_er.ToList();
        }

        public IList<c_sub_tipo_producto_riesgo_operacional> ObtieneSubTiposProductoRiesgoOperacional(int idPRRO)
        {
            var c_er = db.c_sub_tipo_producto_riesgo_operacional.Where(er => er.id_producto_riesgo_operacional == idPRRO && (er.esta_activo ?? false));
            return c_er.ToList();
        }

        public IList<c_linea_negocio_riesgo_operacional> ObtieneLineasNegocioRiesgoOperacional(int idCLNRO)
        {
            var c_er = db.c_linea_negocio_riesgo_operacional.Where(er => er.id_categoria_linea_negocio_riesgo_operacional == idCLNRO && (er.esta_activo ?? false));
            return c_er.ToList();
        }


        public IList<c_cuenta_contable> ObtieneCuentasContables(int idEN)
        {
            var grupos = db.c_grupo_cuenta_contable.Where(g => g.id_entidad == idEN && (g.esta_activo ?? false));

            List<c_cuenta_contable> cuentas = new List<c_cuenta_contable>();

            foreach (var g in grupos)
            {
                cuentas.AddRange(g.c_cuenta_contable.Where(cc => cc.esta_activo ?? false));
            }

            return cuentas.OrderBy(c => c.cl_cuenta_contable).ToList();
        }
    }
}