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
    public partial class c_periodo_certificacion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public c_periodo_certificacion()
        {
            this.k_certificacion_control = new HashSet<k_certificacion_control>();
            this.k_certificacion_estructura = new HashSet<k_certificacion_estructura>();
        }
    
        public int id_periodo_certificacion { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud20")]
        public string cl_periodo_certificacion { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(256, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud256")]
        public string nb_periodo_certificacion { get; set; }
        public bool esta_activo { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "IndicadorDiarioCreate006")]
        [Range(1950, 2200, ErrorMessage = "Elija un año válido.")]
        public Nullable<int> anio { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<k_certificacion_control> k_certificacion_control { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<k_certificacion_estructura> k_certificacion_estructura { get; set; }
    }
}
