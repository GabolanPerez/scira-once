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
    
    public partial class c_usuario_auditoria
    {
        public int id_usuario { get; set; }
        public string password { get; set; }
        public System.DateTime fe_ultimo_acceso { get; set; }
    
        public virtual c_usuario c_usuario { get; set; }
    }
}
