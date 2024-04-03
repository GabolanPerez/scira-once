using SCIRA.Models;
using System.Collections.Generic;

namespace SCIRA.ViewModels
{
    public class SPCUBViewModel
    {
        public SPCUBViewModel()
        {
            Contenido_Normatividad = new List<c_contenido_normatividad>();
            SubProcesos = new List<ListaSubProcesosFNViewModel>();
        }
        //id del subproceso y la normatividad a ligar
        /*[Required(ErrorMessage = "Seleccione un Sub Proceso")]
        [Range(1,int.MaxValue,ErrorMessage = "Seleccione un Sub Proceso")]
        public int id_sub_proceso { get; set; }
        [Required(ErrorMessage = "Seleccione una Normatividad")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una Normatividad")]
        public int id_normatividad { get; set; }*/

        public int[] spids { get; set; }

        public int[] nmids { get; set; }

        public List<c_contenido_normatividad> Contenido_Normatividad { get; set; }
        public List<ListaSubProcesosFNViewModel> SubProcesos { get; set; }
    }
}