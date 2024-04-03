using System.Collections.Generic;

namespace SCIRA.Models
{
    public interface ISelectListRepository
    {
        // Estructura
        IList<c_entidad> ObtieneEntidades();
        IList<c_macro_proceso> ObtieneMacroProcesos(int idEntidad);
        IList<c_proceso> ObtieneProcesos(int idMacroProceso);
        IList<c_sub_proceso> ObtieneSubProcesos(int idProceso);

        // Tipo de Riesgo
        IList<c_categoria_riesgo> ObtieneCategoriasRiesgo();
        IList<c_tipo_riesgo> ObtieneTiposRiesgo(int idCategoriaRiesgo);

        // Tipología de Riesgo
        IList<c_clase_tipologia_riesgo> ObtieneClasesTipologiaRiesgo();
        IList<c_sub_clase_tipologia_riesgo> ObtieneSubClasesTipologiaRiesgo(int idClaseTipologiaRiesgo);
        IList<c_tipologia_riesgo> ObtieneTipologiasRiesgo(int idSubClaseTipologiaRiesgo);

        // Benchmark
        IList<c_actividad> ObtieneMacroProcesosBenchmark(int? idEntidad);
        IList<c_proceso_benchmark> ObtieneProcesosBenchmark(int idActividad);
        IList<c_sub_proceso_benchmark> ObtieneSubProcesosBenchmark(int idProcesoBenchmark);
        IList<c_evento_riesgo> ObtieneEventosBenchmark(int idSubProcesoBenchmark);

        //BDEI
        IList<c_sub_tipo_riesgo_operacional> ObtieneSubTiposRiesgoOperacional(int idTRO);
        IList<c_clase_evento> ObtieneClasesEventoRiesgoOperacional(int idSTRO);
        IList<c_proceso_riesgo_operacional> ObtieneProcesosRiesgoOperacional(int idSTRO);
        IList<c_sub_tipo_producto_riesgo_operacional> ObtieneSubTiposProductoRiesgoOperacional(int idSTRO);
        IList<c_linea_negocio_riesgo_operacional> ObtieneLineasNegocioRiesgoOperacional(int idSTRO);
        IList<c_cuenta_contable> ObtieneCuentasContables(int idEN);
    }
}