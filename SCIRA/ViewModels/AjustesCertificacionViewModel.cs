using SCIRA.Properties;
using System.ComponentModel.DataAnnotations;

namespace SCIRA.ViewModels
{
    public class AjustesCertificacionViewModel
    {
        public bool CertificacionSegura { get; set; } //Certificación Segura
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [MaxLength(512, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ActividadCosteoIndex002")]
        public string LeyendaCertificacionE { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [MaxLength(512, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ActividadCosteoIndex002")]
        public string LeyendaCertificacionM { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [MaxLength(512, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ActividadCosteoIndex002")]
        public string LeyendaCertificacionS { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [MaxLength(512, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ActividadCosteoIndex002")]
        public string LeyendaCertificacionP { get; set; }

    }
}