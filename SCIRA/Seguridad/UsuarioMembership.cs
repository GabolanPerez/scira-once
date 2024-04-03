using SCIRA.Models;
using SCIRA.Utilidades;
using System;
using System.Web;
using System.Web.Security;

namespace SCIRA.Seguridad
{
    public class UsuarioMembership : MembershipUser
    {
        private SeguridadUtilidades utilidades = new SeguridadUtilidades();

        public int Id_usuario { get; set; }
        public string Cl_usuario { get; set; }
        public string Nb_usuario { get; set; }
        public string Password { get; set; }
        public string E_mail_principal { get; set; }
        public string E_mail_alterno { get; set; }
        public string No_telefono { get; set; }
        public bool Esta_activo { get; set; }
        public bool Es_super_usuario { get; set; }
        public string Nb_puesto { get; set; }
        public int Id_area { get; set; }
        public int Id_estatus_usuario { get; set; }
        public bool Solo_lectura { get; set; }
        public bool Es_auditor { get; set; }
        public bool Es_auditor_admin { get; set; }
        public DateTime Fe_cambio_password { get; set; }
        public DateTime Fe_ultimo_acceso { get; set; }
        public string[] Funciones { get; set; }

        public UsuarioMembership(c_usuario us)
        {
            Id_usuario = us.id_usuario;
            Cl_usuario = us.cl_usuario;
            Nb_usuario = us.nb_usuario;
            Password = us.password;
            E_mail_principal = us.e_mail_principal;
            E_mail_alterno = us.e_mail_alterno;
            No_telefono = us.no_telefono;
            Esta_activo = us.esta_activo;
            Es_super_usuario = us.es_super_usuario;
            Nb_puesto = us.nb_puesto;
            Id_area = us.id_area;
            Id_estatus_usuario = us.id_estatus_usuario ?? 2;
            Solo_lectura = us.solo_lectura;
            Fe_cambio_password = us.fe_cambio_password ?? DateTime.Now;
            Es_auditor = us.es_auditor;
            Es_auditor_admin = us.es_auditor_admin;

            //try
            //{
            //    Fe_ultimo_acceso = (DateTime)HttpContext.Current.Session["UltimoAcceso"];
            //}
            //catch
            //{
            //    Fe_ultimo_acceso = DateTime.Now;
            //}

            Fe_ultimo_acceso = (HttpContext.Current.Session != null) ? (DateTime)HttpContext.Current.Session["UltimoAcceso"] : DateTime.Now;
            Funciones = utilidades.ObtenerFunciones(us.id_usuario);
        }
    }
}