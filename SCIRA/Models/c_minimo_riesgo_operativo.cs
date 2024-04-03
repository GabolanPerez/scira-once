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

    public partial class c_minimo_riesgo_operativo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public c_minimo_riesgo_operativo()
        {
            this.k_bdei = new HashSet<k_bdei>();
        }
    
        public int id_minimo_riesgo_operativo { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud20")]
        public string cl_minimo_riesgo_operativo { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud100")]
        public string nb_minimo_riesgo_operativo { get; set; }
        public Nullable<bool> esta_activo { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<k_bdei> k_bdei { get; set; }
    }
}
