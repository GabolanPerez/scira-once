using SCIRA.Properties;
using System;
using System.ComponentModel.DataAnnotations;

namespace SCIRA.ViewModels
{
    public class AjustesSeguridadViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [Range(5, Int32.MaxValue, ErrorMessage = "La longitud máxima no puede ser menor a 5.")]
        public int MaxLongitudPass { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [Range(0, 10, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "AjustesSeguridadIndex027")]
        public int MinLongitudPass { get; set; }
        public string TiempoCaducidad { get; set; }
        public string BloqueoNoIngreso { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [Range(3, Int32.MaxValue, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "AjustesSeguridadIndex028")]
        public int IntentosMaximos { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [Range(5, 3600, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "AjustesSeguridadIndex029")]
        public int TiempoEntreIntentos { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [Range(5, 3600, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "AjustesSeguridadIndex029")]
        public int TiempoSesion { get; set; }
        public bool ReutilizarPass { get; set; }
        public bool Mayuscula { get; set; }
        public bool Numero { get; set; }
        public bool RepetirUsuario { get; set; }
        public bool BSI { get; set; } //bloqueo por superar el maximo de intentos

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [Range(1, 365, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "AjustesSeguridadIndex030")]
        public int DDtc { get; set; }
        public int MMMtc { get; set; }
        public int AAtc { get; set; }
        public int hhtc { get; set; }
        public int mmtc { get; set; }
        public int sstc { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [Range(1, 365, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "AjustesSeguridadIndex030")]
        public int DDbni { get; set; }
        public int MMMbni { get; set; }
        public int AAbni { get; set; }
        public int hhbni { get; set; }
        public int mmbni { get; set; }
        public int ssbni { get; set; }

        public string timeZone { get; set; }

    }
}