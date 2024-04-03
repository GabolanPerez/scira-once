using System.Collections.Generic;

namespace SCIRA.ViewModels
{
    public class ActividadesCosteoViewModel
    {
        public ActividadesCosteoViewModel()
        {
            nb_actividades = new List<string>();
            participantes = new List<PSPCosteoViewModel>();
        }

        public List<string> nb_actividades { get; set; }
        public List<PSPCosteoViewModel> participantes { get; set; }
    }
}