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
    
    public partial class c_criticidad_ro
    {
        public int id_frecuencia_riesgo_operacional { get; set; }
        public int id_impacto_riesgo_operacional { get; set; }
        public int id_criticidad_riesgo_ro { get; set; }
    
        public virtual c_criticidad_riesgo_ro c_criticidad_riesgo_ro { get; set; }
        public virtual c_frecuencia_riesgo_operacional c_frecuencia_riesgo_operacional { get; set; }
        public virtual c_impacto_riesgo_operacional c_impacto_riesgo_operacional { get; set; }
    }
}
