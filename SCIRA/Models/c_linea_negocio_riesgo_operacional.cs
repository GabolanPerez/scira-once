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
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class c_linea_negocio_riesgo_operacional
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public c_linea_negocio_riesgo_operacional()
        {
            this.k_bdei = new HashSet<k_bdei>();
            this.k_riesgo = new HashSet<k_riesgo>();
        }
    
        public int id_linea_negocio_riesgo_operacional { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "LineaNegocioRiesgoOperacionalCreate002")]
        [Range(1, int.MaxValue, ErrorMessage = "LineaNegocioRiesgoOperacionalCreate002")]
        public int id_categoria_linea_negocio_riesgo_operacional { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud20")]
        public string cl_linea_negocio_riesgo_operacional { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(256, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud256")]
        public string nb_linea_negocio_riesgo_operacional { get; set; }
        public Nullable<bool> esta_activo { get; set; }
    
        public virtual c_categoria_linea_negocio_riesgo_operacional c_categoria_linea_negocio_riesgo_operacional { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<k_bdei> k_bdei { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<k_riesgo> k_riesgo { get; set; }
    }
}
