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
    using System;
    using System.Collections.Generic;
    
    public partial class c_impacto_monetario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public c_impacto_monetario()
        {
            this.c_criticidad1 = new HashSet<c_criticidad1>();
        }
    
        public int id_impacto_monetario { get; set; }
        public string cl_impacto_monetario { get; set; }
        public string nb_impacto_monetario { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<c_criticidad1> c_criticidad1 { get; set; }
    }
}
