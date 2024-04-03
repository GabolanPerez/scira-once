using System;
using System.Security.Principal;
using System.Web.Security;

namespace SCIRA.Seguridad
{
    public class IdentityPersonalizado : IIdentity
    {
        public string Name
        {
            get { return Nb_usuario; }
        }

        public string AuthenticationType
        {
            get { return Identity.AuthenticationType; }
        }

        public bool IsAuthenticated
        {
            get { return Identity.IsAuthenticated; }
        }

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
        public DateTime Fe_cambio_password { get; set; }
        public DateTime Fe_ultimo_acceso { get; set; }
        public bool Es_auditor { get; set; }
        public bool Es_auditor_admin { get; set; }
        public string[] Funciones { get; set; }
        public bool activeD { get; set; }

        public IIdentity Identity { get; set; }

        public IdentityPersonalizado(IIdentity identity)
        {
            this.Identity = identity;
            var us = Membership.GetUser(identity.Name) as UsuarioMembership;

            Id_usuario = us.Id_usuario;
            Cl_usuario = us.Cl_usuario;
            Nb_usuario = us.Nb_usuario;
            Password = us.Password;
            E_mail_principal = us.E_mail_principal;
            E_mail_alterno = us.E_mail_alterno;
            No_telefono = us.No_telefono;
            Esta_activo = us.Esta_activo;
            Es_super_usuario = us.Es_super_usuario;
            Nb_puesto = us.Nb_puesto;
            Id_area = us.Id_area;
            Id_estatus_usuario = us.Id_estatus_usuario;
            Solo_lectura = us.Solo_lectura;
            Fe_cambio_password = us.Fe_cambio_password;
            Fe_ultimo_acceso = us.Fe_ultimo_acceso;
            Funciones = us.Funciones;
            Es_auditor = us.Es_auditor;
            Es_auditor_admin = us.Es_auditor_admin;
            activeD = System.Configuration.ConfigurationManager.AppSettings["ActiveDirectory"] == "true";
        }
    }
}