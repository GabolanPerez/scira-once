//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SCIRA.Models
{
    using SCIRA.Properties;
    using SCIRA.Validaciones;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class c_sub_proceso
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public c_sub_proceso()
        {
            this.c_area_costeo_n3_sub_proceso = new HashSet<c_area_costeo_n3_sub_proceso>();
            this.c_area_costeo_sub_proceso = new HashSet<c_area_costeo_sub_proceso>();
            this.c_sub_proceso_normatividad = new HashSet<c_sub_proceso_normatividad>();
            this.c_usuario_sub_proceso = new HashSet<c_usuario_sub_proceso>();
            this.k_benchmarck = new HashSet<k_benchmarck>();
            this.k_control = new HashSet<k_control>();
            this.k_riesgo_derogado = new HashSet<k_riesgo_derogado>();
            this.k_riesgo = new HashSet<k_riesgo>();
            this.r_sub_proceso = new HashSet<r_sub_proceso>();
            this.k_bdei = new HashSet<k_bdei>();
            this.k_certificacion_estructura = new HashSet<k_certificacion_estructura>();
            this.c_linea_negocio = new HashSet<c_linea_negocio>();
            this.c_contenido_manual = new HashSet<c_contenido_manual>();
        }
    
        public int id_sub_proceso { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "SubProcesoCreate008")]
        public int id_proceso { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "EntidadCreate003")]
        [StringLength(50, ErrorMessage = "La clave puede contener hasta 50 caracteres")]
        //[RegularExpression("^(SP)[0-9]{4}$", ErrorMessage = "La clave debe estar conformada por SP y 4 digitos.")]
        [spCode]
        public string cl_sub_proceso { get; set; }
        [Required(ErrorMessage = "El nombre es un campo requerido.")]
        [StringLength(256, ErrorMessage = "El nombre puede tener hasta 256 caracteres.")]
        public string nb_sub_proceso { get; set; }
        [Required(ErrorMessage = "La descripción es un campo requerido.")]
        [StringLength(4000, ErrorMessage = "La descripción puede tener hasta 4000 caracteres.")]
        public string ds_sub_proceso { get; set; }
        [Required(ErrorMessage = "Seleccione un Responsable.")]
        public int id_responsable { get; set; }
        public int no_meses_antiguedad { get; set; }
        [Required(ErrorMessage = "El código de manual es un campo requerido.")]
        [StringLength(20, ErrorMessage = "El código del manual puede tener hasta 20 caracteres.")]
        public string cl_manual { get; set; }
        [StringLength(256, ErrorMessage = "Este campo puede tener hasta 256 caracteres.")]
        public string cl_sp_anterior { get; set; }
        [StringLength(256, ErrorMessage = "Este campo puede tener hasta 256 caracteres.")]
        public string cl_sp_siguiente { get; set; }
        [Required(ErrorMessage = "La tipología es un campo requerido.")]
        public Nullable<int> id_tipologia_sub_proceso { get; set; }
        public Nullable<int> id_etapa { get; set; }
        public Nullable<int> id_sub_etapa { get; set; }
        [Required(ErrorMessage = "Las áreas involucradas son un campo requerido.")]
        [StringLength(512, ErrorMessage = "Las áreas involucradas pueden tener hasta 512 caracteres.")]
        public string ds_areas_involucradas { get; set; }
        [Required(ErrorMessage = "Las aplicaciones involucradas son un campo requerido.")]
        [StringLength(512, ErrorMessage = "Las aplicaciones involucradas pueden tener hasta 512 caracteres.")]
        public string ds_aplicaciones_relacionadas { get; set; }
        public string nb_archivo_manual { get; set; }
        public string nb_archivo_flujo { get; set; }
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
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<c_area_costeo_n3_sub_proceso> c_area_costeo_n3_sub_proceso { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<c_area_costeo_sub_proceso> c_area_costeo_sub_proceso { get; set; }
        public virtual c_etapa c_etapa { get; set; }
        public virtual c_proceso c_proceso { get; set; }
        public virtual c_sub_etapa c_sub_etapa { get; set; }
        public virtual c_usuario c_usuario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<c_sub_proceso_normatividad> c_sub_proceso_normatividad { get; set; }
        public virtual c_tipologia_sub_proceso c_tipologia_sub_proceso { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<c_usuario_sub_proceso> c_usuario_sub_proceso { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<k_benchmarck> k_benchmarck { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<k_control> k_control { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<k_riesgo_derogado> k_riesgo_derogado { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<k_riesgo> k_riesgo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<r_sub_proceso> r_sub_proceso { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<k_bdei> k_bdei { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<k_certificacion_estructura> k_certificacion_estructura { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<c_linea_negocio> c_linea_negocio { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<c_contenido_manual> c_contenido_manual { get; set; }
    }
}
