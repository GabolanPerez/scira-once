using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "BDEI-Stats", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class GraphicsBDEIController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            //Filtros
            ViewBag.entidades = (MultiSelectList)Utilidades.DropDown.EntidadesMS();
            ViewBag.moneda = Utilidades.DropDown.Moneda();

            var moneda = db.c_moneda.First();
            var bdeiList = getFilteredElements(db.c_entidad.Select(e => e.id_entidad.ToString()).ToArray(), moneda.id_moneda);

            ViewBag.d0 = BDEIData(bdeiList, moneda);

            return View();
        }

        #region Detalle de las barras
        public ActionResult Details(string[] entidades, int moneda)
        {
            var bdeiList = getFilteredElements(entidades, moneda);

            return PartialView(bdeiList);
        }
        #endregion

        #region Filtros1
        public ActionResult loadFilters(string[] entidades, int moneda)
        {
            var bdeiList = getFilteredElements(entidades, moneda);

            ViewBag.d0 = BDEIData(bdeiList, db.c_moneda.Find(moneda));


            return PartialView();
        }


        private List<k_bdei> getFilteredElements(string[] entidades, int moneda)
        {
            var user = (IdentityPersonalizado)User.Identity;
            var us = db.c_usuario.Find(user.Id_usuario);

            var BDEI = Utilidades.Utilidades.RTCObject(us, db, "k_bdei").Cast<k_bdei>().ToList();

            var model = new List<k_bdei>();

            if (entidades != null)
            {
                foreach (var id in entidades)
                {
                    int ID = int.Parse(id);
                    model = model.Union(BDEI.Where(b => b.id_moneda == moneda && b.id_entidad == ID)).ToList();
                }
            }
            else
            {
                model = BDEI.Where(b => b.id_moneda == moneda).ToList();
            }

            return model;
        }
        #endregion


        #region datosBDEI()
        public string BDEIData(List<k_bdei> bdeiList, c_moneda moneda) //genera la cadena con los datos para las criticidades
        {
            double[] res = new double[3];

            double PR = 0, R = 0, PP = 0; //Perdidas Realizadas, Recuperaciones, PerdidasProbables

            foreach (var bdei in bdeiList)
            {
                try
                {
                    PR += (double)bdei.mn_quebranto;
                }
                catch
                {
                }
                try
                {
                    PP += (double)bdei.mn_exposicion;
                }
                catch
                {
                }
                try
                {
                    R += (double)bdei.mn_recuperacion;
                }
                catch
                {
                }
            }

            PR /= 1000;
            R /= 1000;
            PP /= 1000;

            string d1 = PR.ToString().Replace(",", ".");
            string d2 = PP.ToString().Replace(",", ".");
            string d3 = R.ToString().Replace(",", ".");


            string data = @"
            {
                labels:['Pérdidas Realizadas','Recuperaciones','Pérdidas Probables'],
                datasets:[
                    {label:'Miles de " + moneda.nb_moneda + @"',
                    backgroundColor:['red','green','orange'],
                    data:['" + d1 + "','" + d3 + "','" + d2 + "']}" +
                    "]" +
            "};";


            res[0] = PR;
            res[1] = R;
            res[2] = PP;
            //return res;

            List<string> bgc = new List<string>();
            bgc.Add("red");
            bgc.Add("green");
            bgc.Add("orange");

            var dataset = new GraphicsViewModel.Dataset(Strings.getMSG("GraphicsBDEIIndex007"), 1, res, bgc.ToArray(), null);

            string[] labels = { Strings.getMSG("Pérdidas Realizadas"), Strings.getMSG("GraphicsBDEIIndex002"), Strings.getMSG("GraphicsBDEIIndex003") };
            GraphicsViewModel.Dataset[] datasets = { dataset };


            var Data0 = new GraphicsViewModel.Data()
            {
                labels = labels,
                datasets = datasets,
                ids = new int[] { 1, 2, 3 }
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(Data0);


        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
