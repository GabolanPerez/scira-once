using SCIRA.Models;
using SCIRA.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SCIRA.ViewModels
{
    public class AgregarControlViewModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AgregarControlViewModel()
        {

            this.c_archivo = new HashSet<c_archivo>();
        }

        public k_riesgo k_riesgo { get; set; }
        public k_control k_control { get; set; }
        public c_sub_proceso c_sub_proceso { get; set; }

        // Control
        public int id_sub_proceso { get; set; }
        public int id_control { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(4000, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud4000")]
        public string actividad_control { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(1024, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud1024")]
        public string relacion_control { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(4000, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud4000")]
        public string evidencia_control { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public Nullable<int> id_ejecutor { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public Nullable<int> id_responsable { get; set; }
        public bool es_control_clave { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public Nullable<int> id_grado_cobertura { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public Nullable<int> id_frecuencia_control { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public Nullable<int> id_naturaleza_control { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public string nb_aplicacion { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public Nullable<int> id_tipologia_control { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public Nullable<int> id_categoria_control { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
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