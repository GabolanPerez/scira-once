using System;
using System.Security.Principal;

namespace SCIRA.Seguridad
{
    public class PrincipalPersonalizado : IPrincipal
    {
        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }

        public IIdentity Identity { get; private set; }

        public IdentityPersonalizado MiIdentidadPersonalizada
        {
            get { return (IdentityPersonalizado)Identity; }
            set { Identity = value; }
        }

        public PrincipalPersonalizado(IdentityPersonalizado identity)
        {
            Identity = identity;
        }
    }
}