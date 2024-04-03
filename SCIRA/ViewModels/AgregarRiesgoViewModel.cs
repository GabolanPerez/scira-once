using SCIRA.Models;
using SCIRA.Properties;
using SCIRA.Validaciones;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SCIRA.ViewModels
{
    public class AgregarRiesgoViewModel
    {
        public AgregarRiesgoViewModel()
        {
            CategoriasRiesgo = new List<SelectListItem>();
            TiposRiesgo = new List<SelectListItem>();

            ClasesTipologiaRiesgo = new List<SelectListItem>();
            SubClasesTipologiaRiesgo = new List<SelectListItem>();
            TipologiasRiesgo = new List<SelectListItem>();

            this.c_archivo = new HashSet<c_archivo>();
        }





        public int monto_impacto { get; set; }



        public int id_riesgo { get; set; }
        public c_sub_proceso c_sub_proceso { get; set; }

        // Riesgo
        public int id_sub_proceso { get; set; }
        public string cl_riesgo { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud20")]
        public string nb_riesgo { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(4000, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud4000")]
        public string evento { get; set; }

        //
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public int id_categoria_riesgo { get; set; }
        public IList<SelectListItem> CategoriasRiesgo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public int id_tipo_riesgo { get; set; }
        public IList<SelectListItem> TiposRiesgo { get; set; }

        //
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public int id_clase_tipologia_riesgo { get; set; }
        public IList<SelectListItem> ClasesTipologiaRiesgo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public int id_sub_clase_tipologia_riesgo { get; set; }
        public IList<SelectListItem> SubClasesTipologiaRiesgo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public int id_tipologia_riesgo { get; set; }
        public IList<SelectListItem> TipologiasRiesgo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public Nullable<int> id_probabilidad_ocurrencia { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public Nullable<int> id_tipo_impacto { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public Nullable<int> id_magnitud_impacto { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public string criticidad { get; set; }
        public bool tiene_afectacion_contable { get; set; }
        public string supuesto_normativo { get; set; }
        public string euc { get; set; }


        //riesgo operativo
        public bool es_riesgo_operativo { get; set; }
        public int? id_proceso_riesgo_operacional { get; set; }
        public int? id_sub_tipo_producto_riesgo_operacional { get; set; }
        public int? id_sub_tipo_riesgo_operacional { get; set; }
        public int? id_linea_negocio_riesgo_operacional { get; set; }
        public int? id_frecuencia_riesgo_operacional { get; set; }
        public int? id_impacto_riesgo_operacional { get; set; }




        // Control
        public int id_control { get; set; }
        [AccionCorrectora(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(4000, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud4000")]
        public string actividad_control { get; set; }
        [AccionCorrectora(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(1024, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud1024")]
        public string relacion_control { get; set; }
        [AccionCorrectora(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(4000, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud4000")]
        public string evidencia_control { get; set; }
        [AccionCorrectora(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public Nullable<int> id_ejecutor { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public int id_responsable { get; set; }
        public bool es_control_clave { get; set; }

        [AccionCorrectora(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public Nullable<int> id_grado_cobertura { get; set; }
        [AccionCorrectora(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public Nullable<int> id_frecuencia_control { get; set; }
        [AccionCorrectora(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public Nullable<int> id_naturaleza_control { get; set; }

        [AccionCorrectora(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public string nb_aplicacion { get; set; }

        [AccionCorrectora(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public Nullable<int> id_tipologia_control { get; set; }
        [AccionCorrectora(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public Nullable<int> id_categoria_control { get; set; }
        [AccionCorrectora(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public Nullable<int> id_tipo_evidencia { get; set; }
        public bool tiene_accion_correctora { get; set; }
        public string accion_correctora { get; set; }


        public string campo01 { get; set; }
        public string campo02 { get; set; }
        public string campo03 { get; set; }
        public string campo04 { get; set; }
        public string campo05 { get; set; }
        public string campo06 { get; set; }
        public string campo07 { get; set; }
        public string campo08 { get; set; }
        public string campo09 { get; set; }
        public string campo10 { get; set; }
        public string campo11 { get; set; }
        public string campo12 { get; set; }
        public string campo13 { get; set; }
        public string campo14 { get; set; }
        public string campo15 { get; set; }
        public string campo16 { get; set; }
        public string campo17 { get; set; }
        public string campo18 { get; set; }
        public string campo19 { get; set; }
        public string campo20 { get; set; }


        public string campor01 { get; set; }
        public string campor02 { get; set; }
        public string campor03 { get; set; }
        public string campor04 { get; set; }
        public string campor05 { get; set; }
        public string campor06 { get; set; }
        public string campor07 { get; set; }
        public string campor08 { get; set; }
        public string campor09 { get; set; }
        public string campor10 { get; set; }
        public string campor11 { get; set; }
        public string campor12 { get; set; }
        public string campor13 { get; set; }
        public string campor14 { get; set; }
        public string campor15 { get; set; }
        public string campor16 { get; set; }
        public string campor17 { get; set; }
        public string campor18 { get; set; }
        public string campor19 { get; set; }
        public string campor20 { get; set; }

        //Datos para la Incidencia
        public int? id_responsable_i { get; set; }
        public int? id_clasificacion_incidencia { get; set; }
        public string ds_incidencia { get; set; }
        public string js_incidencia { get; set; }
        public bool requiere_plan { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<c_archivo> c_archivo { get; set; }
    }
}