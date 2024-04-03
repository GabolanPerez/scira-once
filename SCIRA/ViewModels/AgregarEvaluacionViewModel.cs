using System.Collections.Generic;
using System.Web.Mvc;

namespace SCIRA.ViewModels
{
    public class AgregarEvaluacionViewModel
    {
        public AgregarEvaluacionViewModel()
        {
            Entidades = new List<SelectListItem>();
            Indicadores = new List<SelectListItem>();
        }

        public int id_entidad { get; set; }
        public IList<SelectListItem> Entidades { get; set; }

        public int id_indicador { get; set; }
        public IList<SelectListItem> Indicadores { get; set; }
    }
}