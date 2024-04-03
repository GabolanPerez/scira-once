using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static iTextSharp.text.pdf.AcroFields;

namespace SCIRA.Models
{
    public class ReportTemplateModel
    {
        // properties are not capital due to json mapping
        public string c1 { get; set; }
        public string c2 { get; set; }
        public string c3 { get; set; }
        public string c4 { get; set; }



        public static List<ReportTemplateModel> generateList(List<h_acceso> model){
            var res = new List<ReportTemplateModel>();

            foreach(h_acceso item in model)
            {
                res.Add(new ReportTemplateModel
                {
                    c1 = string.Format("{0:dd/MM/yyyy}", item.fe_acceso),
                    c2 = item.c_usuario.nb_usuario,
                    c3 = item.nb_funcion
                });
            }
            
            return res;
        }

    }

}