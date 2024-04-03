using Newtonsoft.Json;
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
    [Access(Funcion = "Fichas-Stats", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class GraphicsFichasController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            ViewBag.puestos = (MultiSelectList)Utilidades.DropDown.PuestosMS();
            ViewBag.usuarios = (MultiSelectList)Utilidades.DropDown.UsuariosMS();
            ViewBag.niveles = (MultiSelectList)Utilidades.DropDown.NivelesPuestosMS();


            var user = (IdentityPersonalizado)User.Identity;
            var us = db.c_usuario.Find(user.Id_usuario);

            var Fichas = Utilidades.Utilidades.RTCObject(us, db, "r_evento").Cast<r_evento>().ToList();


            var Fichas2 = new List<r_evento>();

            foreach (var ficha in Fichas)
            {
                string registro_ligado = Utilidades.Utilidades.registroLigado(ficha);
                if (registro_ligado == null)
                {
                    DeleteActions.DeleteFichaObjects(ficha, db, true);
                    db.r_evento.Remove(ficha);
                    db.SaveChanges();
                }
                else
                {
                    Fichas2.Add(ficha);
                }
            }


            ViewBag.d0 = DatosG0(Fichas);

            return View();
        }

        #region Detalle de las barras
        public ActionResult Details(string[] puestos, string[] usuarios, string[] niveles, string check1, string check2, string check3, int TPg0, int TPg1, int TPg2)
        {
            var Fichas = getFilteredElements(puestos, usuarios, niveles, check1, check2, check3);

            var Fichas0 = ListaFichas0(Fichas, TPg0);
            var Fichas1 = ListaFichas1(Fichas0, TPg1);
            var Fichas2 = ListaFichas2(Fichas1, TPg0, TPg2);

            return PartialView("DetailViews/DetailsEV", Fichas2);
        }
        #endregion

        #region Filtros1
        public ActionResult loadFilters(string[] puestos, string[] usuarios, string[] niveles, string check1, string check2, string check3, int? TPg0, int? TPg1)
        {
            var Fichas = getFilteredElements(puestos, usuarios, niveles, check1, check2, check3);

            ViewBag.d0 = DatosG0(Fichas);
            if (TPg0 != null)
            {
                ViewBag.d1 = DG1S(Fichas, (int)TPg0);
                if (TPg1 != null)
                {
                    ViewBag.d2 = DG2S(Fichas, (int)TPg0, (int)TPg1);
                }
                else
                {
                    ViewBag.d2 = null;
                }
            }
            else
            {
                ViewBag.d1 = null;
            }

            return PartialView();
        }

        #region Obtener Elementos Filtrados
        private List<r_evento> getFilteredElements(string[] puestos, string[] usuarios, string[] niveles, string check1, string check2, string check3)
        {
            List<r_evento> Fichas = new List<r_evento>();
            List<r_evento> FichasA = new List<r_evento>();


            bool ch1 = check1 == "on";
            bool ch2 = check2 == "on";
            bool ch3 = check3 == "on";

            List<c_usuario> LAU = new List<c_usuario>();
            List<c_puesto> LAPU = new List<c_puesto>();
            List<c_puesto> LAPU2 = new List<c_puesto>();
            List<c_puesto> LAPU3 = new List<c_puesto>();

            var user = (IdentityPersonalizado)User.Identity;
            var Us = db.c_usuario.Find(user.Id_usuario);
            var TC = Utilidades.Utilidades.TramoControlInferior(Us.id_usuario, db);
            TC.Add(Us);


            if (usuarios != null)
            {
                foreach (var id in usuarios)
                {
                    int ID = int.Parse(id);
                    var us = db.c_usuario.Find(ID);

                    if (TC.Contains(us)) Fichas.AddRange(us.r_evento);
                }
            }

            //Obtener lista de elementos al filtrar por puestos
            //primero se debeb encontrar todos los usuarios pertenecientes a los puestos

            //incluir condicion para obtener puestos inferiores si la opcion está habilitada
            if (puestos != null)
            {
                foreach (var id in puestos)
                {
                    int ID = int.Parse(id);
                    var puesto = db.c_puesto.Find(ID);
                    LAPU.Add(puesto);
                    if (ch2) //Incluir puestos inferiores
                    {
                        LAPU2 = Utilidades.Utilidades.puestosInferiores(puesto, db);
                        LAPU = LAPU.Union(LAPU2).ToList();
                    }

                }
                LAPU = LAPU.Union(LAPU).ToList();

                foreach (var ps in LAPU)
                {
                    var users = ps.c_usuario.ToList();

                    LAU = LAU.Union(users).ToList();
                }//Aquí tenemos la lista de todos los usuarios pertenecientes a los puestos seleccionados

                foreach (var us in LAU)
                {
                    if (TC.Contains(us)) FichasA.AddRange(us.r_evento);
                }

                if (ch1) //Unimos
                {
                    Fichas = Fichas.Union(FichasA).ToList();
                }
                else if (usuarios != null)//Intersectamos
                {
                    Fichas = Fichas.Intersect(FichasA).ToList();
                }
                else
                {
                    Fichas = FichasA;
                }
            }

            FichasA = new List<r_evento>();

            //obtenemos los puestos que tengan los niveles seleccionados
            if (niveles != null)
            {
                LAPU = new List<c_puesto>();
                foreach (var lvl in niveles)
                {
                    var cl = string.Format("{0:00}", int.Parse(lvl));
                    LAPU2 = db.c_puesto.Where(p => p.cl_puesto == cl).ToList();
                    LAPU.AddRange(LAPU2);
                    if (ch3)
                    {
                        foreach (var ps in LAPU2)
                        {
                            LAPU.AddRange(Utilidades.Utilidades.puestosInferiores(ps, db));
                        }
                    }
                }
                LAPU = LAPU.Union(LAPU).ToList();

                foreach (var ps in LAPU)
                {
                    var users = ps.c_usuario.ToList();

                    LAU = LAU.Union(users).ToList();
                }//Aquí tenemos la lista de todos los usuarios pertenecientes a los puestos seleccionados

                foreach (var us in LAU)
                {
                    if (TC.Contains(us)) FichasA.AddRange(us.r_evento);
                }

                if (ch1) //Unimos
                {
                    Fichas = Fichas.Union(FichasA).ToList();
                }
                else if (usuarios != null || puestos != null)//Intersectamos
                {
                    Fichas = Fichas.Intersect(FichasA).ToList();
                }
                else
                {
                    Fichas = FichasA;
                }
            }


            if (usuarios == null && puestos == null && niveles == null)
            {
                Fichas = Utilidades.Utilidades.RTCObject(Us, db, "r_evento").Cast<r_evento>().ToList();
            }
            return Fichas;
        }
        #endregion

        #endregion

        #region Funciones Cliente
        public string DG1(string[] puestos, string[] usuarios, string[] niveles, string check1, string check2, string check3, int TPg0)
        {
            var Fichas = getFilteredElements(puestos, usuarios, niveles, check1, check2, check3);
            return DG1S(Fichas, TPg0);
        }

        public string DG2(string[] puestos, string[] usuarios, string[] niveles, string check1, string check2, string check3, int TPg0, int TPg1)
        {
            var Fichas = getFilteredElements(puestos, usuarios, niveles, check1, check2, check3);
            return DG2S(Fichas, TPg0, TPg1);
        }



        private string DG1S(List<r_evento> Fichas, int TPg0)
        {
            var data = "";

            if (TPg0 == 0)
            {
                var lFichas = Fichas.Where(r => r.tipo == "0001").ToList();
                data = DatosG1(lFichas);
            }
            if (TPg0 == 1)
            {
                var lFichas = Fichas.Where(r => r.tipo == "0002").ToList();
                data = DatosG1(lFichas);
            }
            if (TPg0 == 2)
            {
                var lFichas = Fichas.Where(r => r.tipo == "0003").ToList();
                data = DatosG1(lFichas);
            }


            var d2 = data.Replace("\n", "");
            return d2;
        }

        public string DG2S(List<r_evento> Fichas, int TPg0, int TPg1)
        {
            List<r_evento> Fichas2 = new List<r_evento>();
            List<r_evento> lFichas = new List<r_evento>();
            var data = "";


            if (TPg0 == 0)
            {
                lFichas = Fichas.Where(r => r.tipo == "0001").ToList();
            }
            if (TPg0 == 1)
            {
                lFichas = Fichas.Where(r => r.tipo == "0002").ToList();
            }
            if (TPg0 == 2)
            {
                lFichas = Fichas.Where(r => r.tipo == "0003").ToList();
            }


            var FichasSeparadas = GetG1Count(lFichas);

            if (TPg1 == 0) Fichas2 = lFichas;
            else if (TPg1 == 1) Fichas2 = FichasSeparadas.Vencidas;
            else if (TPg1 == 2) Fichas2 = FichasSeparadas.Atendidas;
            else if (TPg1 == 3) Fichas2 = FichasSeparadas.Pendientes;

            if (TPg0 == 0) data = DatosG2_1(Fichas2);
            if (TPg0 == 1) data = DatosG2_2(Fichas2);
            if (TPg0 == 2) data = DatosG2_3(Fichas2);


            var d2 = data.Replace("\n", "");
            return d2;
        }

        #endregion

        #region Datos
        private string DatosG0(List<r_evento> Fichas) //datos para la gráfica 0 "Conteo General"
        {
            //datos de entrada
            int NFichasNorm = Fichas.Where(r => r.tipo == "0001").Count();
            int NfichasOfic = Fichas.Where(r => r.tipo == "0002").Count();
            int NfichasInc = Fichas.Where(r => r.tipo == "0003").Count();


            double[] data = { (double)NFichasNorm, (double)NfichasOfic, (double)NfichasInc };
            string[] bgc = Utilidades.Utilidades.getColorArray(3);

            var dataset = new GraphicsViewModel.Dataset(Strings.getMSG("No. Fichas"), 1, data, bgc, null);

            string[] labels = { Strings.getMSG("Menu015"), Strings.getMSG("GraphicsOficioIndex015"), Strings.getMSG("Incidencias") };
            GraphicsViewModel.Dataset[] datasets = { dataset };


            var Data0 = new GraphicsViewModel.Data()
            {
                labels = labels,
                datasets = datasets
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(Data0);
        }

        private string DatosG1(List<r_evento> Fichas) //datos para la gráfica 1 "Detalle Fichas"
        {
            //datos de entrada
            int NFichas = Fichas.Count();
            var Detalle = GetG1Count(Fichas);
            int NFichasVencidas = Detalle.Vencidas.Count;
            int NFichasAtendidas = Detalle.Atendidas.Count;
            int NFichasPendientes = Detalle.Pendientes.Count;


            double[] data = {
                (double)NFichas,
                (double)NFichasVencidas,
                (double)NFichasAtendidas,
                (double)NFichasPendientes
            };


            string[] bgc = {
                "#39E30B",
                "#E3330B",
                "#1084E3",
                "#FDA90D"
            };

            var dataset = new GraphicsViewModel.Dataset(Strings.getMSG("No. Fichas"), 1, data, bgc, null);

            string[] labels = {
                Strings.getMSG("GraphicsPlanesRemIndex006"),
                Strings.getMSG("GraphicsFichasIndex003"),
                Strings.getMSG("AuditoriaIndexSeguimientoObservaciones006"),
                Strings.getMSG("AuditoriaIndexSeguimientoObservaciones005")
            };

            GraphicsViewModel.Dataset[] datasets = {
                dataset
            };


            var Data0 = new GraphicsViewModel.Data()
            {
                labels = labels,
                datasets = datasets
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(Data0);
        }

        private string DatosG2_1(List<r_evento> Fichas) //datos para la gráfica 2 "Detalle por tipo de ficha" Tipo Normatividad
        {
            //datos de entrada
            var Detalle = GetG2NormCount(Fichas);
            var NNorm = Detalle.Count();

            double[] data = new double[NNorm];
            string[] labels = new string[NNorm];
            int[] ids = new int[NNorm];

            for (int i = 0; i < NNorm; i++)
            {
                var auxElement = Detalle.ElementAt(i);

                data[i] = auxElement.fichas.Count();
                labels[i] = auxElement.cl_normatividad;
                ids[i] = auxElement.id_contenido;
            }

            string[] bgc = Utilidades.Utilidades.getColorArray(NNorm);

            var dataset = new GraphicsViewModel.Dataset(Strings.getMSG("No. Fichas"), 1, data, bgc, null);

            GraphicsViewModel.Dataset[] datasets = {
                dataset
            };


            var Data0 = new GraphicsViewModel.Data()
            {
                labels = labels,
                datasets = datasets,
                ids = ids
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(Data0);
        }

        private string DatosG2_2(List<r_evento> Fichas) //datos para la gráfica 2 "Detalle por tipo de ficha" Oficios
        {
            //datos de entrada
            var Detalle = GetG2OficCount(Fichas);
            var NObj = Detalle.Count();

            double[] data = new double[NObj];
            string[] labels = new string[NObj];
            int[] ids = new int[NObj];

            for (int i = 0; i < NObj; i++)
            {
                var auxElement = Detalle.ElementAt(i);

                data[i] = auxElement.fichas.Count();
                labels[i] = auxElement.nb_objeto;
                ids[i] = auxElement.id_objeto;
            }

            string[] bgc = Utilidades.Utilidades.getColorArray(NObj);

            var dataset = new GraphicsViewModel.Dataset(Strings.getMSG("No. Fichas"), 1, data, bgc, null);

            GraphicsViewModel.Dataset[] datasets = {
                dataset
            };


            var Data0 = new GraphicsViewModel.Data()
            {
                labels = labels,
                datasets = datasets,
                ids = ids
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(Data0);
        }

        private string DatosG2_3(List<r_evento> Fichas) //datos para la gráfica 2 "Detalle por tipo de ficha" Incidencias
        {
            //datos de entrada
            var Detalle = GetG2IncCount(Fichas);
            var NObj = Detalle.Count();

            double[] data = new double[NObj];
            string[] labels = new string[NObj];
            int[] ids = new int[NObj];

            for (int i = 0; i < NObj; i++)
            {
                var auxElement = Detalle.ElementAt(i);

                data[i] = auxElement.fichas.Count();
                labels[i] = auxElement.nb_incidencia;
                ids[i] = auxElement.id_incidencia;
            }

            string[] bgc = Utilidades.Utilidades.getColorArray(NObj);

            var dataset = new GraphicsViewModel.Dataset(Strings.getMSG("No. Fichas"), 1, data, bgc, null);

            GraphicsViewModel.Dataset[] datasets = {
                dataset
            };


            var Data0 = new GraphicsViewModel.Data()
            {
                labels = labels,
                datasets = datasets,
                ids = ids
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(Data0);
        }
        #endregion

        #region Auxiliares
        private FichasG1 GetG1Count(List<r_evento> Fichas)
        {
            FichasG1 model = new FichasG1();
            model.Todas = Fichas;


            foreach (var ficha in Fichas)
            {
                var status = Utilidades.Utilidades.GetStatus(ficha);

                if (status == Strings.getMSG("CertificacionInforme037"))
                {
                    model.Atendidas.Add(ficha);
                }
                else
                {
                    if (status == Strings.getMSG("CertificacionInforme038"))
                    {
                        model.Pendientes.Add(ficha);
                    }
                    else
                    {
                        model.Vencidas.Add(ficha);
                    }
                }

            }
            return model;
        }

        #region G2Counts
        private List<FichasG2Norm> GetG2NormCount(List<r_evento> Fichas)
        {
            List<FichasG2Norm> model = new List<FichasG2Norm>();

            foreach (var ficha in Fichas)
            {
                var config = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0001>(ficha.config);
                var cont = db.c_contenido_normatividad.Find(config.id);
                var Norm = Utilidades.Utilidades.getRoot(db, cont);

                if (model.Exists(m => m.cl_normatividad == Norm.cl_contenido_normatividad))
                {
                    model.Where(m => m.cl_normatividad == Norm.cl_contenido_normatividad).First().fichas.Add(ficha);
                }
                else
                {
                    var maux = new FichasG2Norm()
                    {
                        cl_normatividad = Norm.cl_contenido_normatividad,
                        id_contenido = Norm.id_contenido_normatividad

                    };
                    maux.fichas.Add(ficha);
                    model.Add(maux);
                }
            }
            return model;
        }

        private List<FichasG2Ofic> GetG2OficCount(List<r_evento> Fichas)
        {
            List<FichasG2Ofic> model = new List<FichasG2Ofic>();

            foreach (var ficha in Fichas)
            {
                var config = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0002>(ficha.config);
                var obj = db.k_objeto.Find(config.id);

                if (model.Exists(o => o.nb_objeto == obj.nb_objeto))
                {
                    model.Where(o => o.nb_objeto == obj.nb_objeto).First().fichas.Add(ficha);
                }
                else
                {
                    var maux = new FichasG2Ofic()
                    {
                        nb_objeto = obj.nb_objeto,
                        id_objeto = obj.id_objeto

                    };
                    maux.fichas.Add(ficha);
                    model.Add(maux);
                }
            }
            return model;
        }

        private List<FichasG2Inc> GetG2IncCount(List<r_evento> Fichas)
        {
            List<FichasG2Inc> model = new List<FichasG2Inc>();

            foreach (var ficha in Fichas)
            {
                var config = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0003>(ficha.config);
                var inc = db.k_incidencia.Find(config.id);

                int tipoFuente = 0;

                if (inc.id_objeto != null) tipoFuente = inc.k_objeto.tipo_objeto;
                if (inc.id_certificacion_control != null) tipoFuente = 4;
                if (inc.id_control != null) tipoFuente = 5;


                var name = "";

                switch (tipoFuente)
                {
                    case 1: name = "Incidencias de Oficios"; break;
                    case 2: name = "Incidencias de Informes AuExt."; break;
                    case 3: name = "Incidencias de Informes AuInt"; break;
                    case 4: name = "Incidencias de Certificaciones"; break;
                    case 5: name = "Incidencias de MRyC"; break;
                    case 6: name = "Incidencias de Otras Fuentes"; break;
                }


                if (model.Exists(m => m.nb_incidencia == name))
                {
                    model.Where(m => m.nb_incidencia == name).First().fichas.Add(ficha);
                }
                else
                {
                    var maux = new FichasG2Inc()
                    {
                        nb_incidencia = name,
                        id_incidencia = tipoFuente
                    };
                    maux.fichas.Add(ficha);
                    model.Add(maux);
                }
            }
            return model;
        }
        #endregion

        private List<r_evento> ListaFichas0(List<r_evento> Fichas, int TPg0)
        {
            var Fichas0 = new List<r_evento>();
            if (TPg0 == 0) Fichas0 = Fichas.Where(r => r.tipo == "0001").ToList();
            if (TPg0 == 1) Fichas0 = Fichas.Where(r => r.tipo == "0002").ToList();
            if (TPg0 == 2) Fichas0 = Fichas.Where(r => r.tipo == "0003").ToList();


            return Fichas0;
        }

        private List<r_evento> ListaFichas1(List<r_evento> Fichas, int TPg1)
        {
            var Fichas1 = new List<r_evento>();

            FichasG1 FichasSeparadas = GetG1Count(Fichas);

            if (TPg1 == 0) Fichas1 = FichasSeparadas.Todas;
            if (TPg1 == 1) Fichas1 = FichasSeparadas.Vencidas;
            if (TPg1 == 2) Fichas1 = FichasSeparadas.Atendidas;
            if (TPg1 == 3) Fichas1 = FichasSeparadas.Pendientes;

            return Fichas1;
        }

        private List<r_evento> ListaFichas2(List<r_evento> Fichas, int TPg0, int TPg2)
        {
            var Fichas2 = new List<r_evento>();

            // Si venimos de normatividad regresaremos las fichas dentro de la lista que pertenezcan a la normatividad 
            if (TPg0 == 0)
            {
                foreach (var ficha in Fichas)
                {
                    var conf = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0001>(ficha.config);
                    var cont = db.c_contenido_normatividad.Find(conf.id);
                    var Norm = Utilidades.Utilidades.getRoot(db, cont);

                    if (Norm.id_contenido_normatividad == TPg2) Fichas2.Add(ficha);
                }
            }

            // Si venimos de normatividad regresaremos las fichas dentro de la lista que pertenezcan al Oficio Incidencia
            if (TPg0 == 1)
            {
                foreach (var ficha in Fichas)
                {
                    var conf = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0002>(ficha.config);
                    var obj = db.k_objeto.Find(conf.id);

                    if (obj.id_objeto == TPg2) Fichas2.Add(ficha);
                }
            }

            if (TPg0 == 2)
            {
                foreach (var ficha in Fichas)
                {
                    var conf = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0003>(ficha.config);
                    var inc = db.k_incidencia.Find(conf.id);

                    var tipoFuente = 0;

                    if (inc.id_objeto != null) tipoFuente = inc.k_objeto.tipo_objeto;
                    if (inc.id_certificacion_control != null) tipoFuente = 4;
                    if (inc.id_control != null) tipoFuente = 5;

                    if (tipoFuente == TPg2) Fichas2.Add(ficha);
                }
            }

            return Fichas2;
        }

        #region Clases
        private class FichasG1
        {
            public List<r_evento> Vencidas { get; set; }
            public List<r_evento> Atendidas { get; set; }
            public List<r_evento> Pendientes { get; set; }
            public List<r_evento> Todas { get; set; }

            public FichasG1()
            {
                Vencidas = new List<r_evento>();
                Atendidas = new List<r_evento>();
                Pendientes = new List<r_evento>();
                Todas = new List<r_evento>();
            }
        }

        private class FichasG2Norm
        {
            public int id_contenido { get; set; }
            public string cl_normatividad { get; set; }
            public List<r_evento> fichas { get; set; }
            public FichasG2Norm()
            {
                fichas = new List<r_evento>();
            }
        }

        private class FichasG2Ofic
        {
            public int id_objeto { get; set; }
            public string nb_objeto { get; set; }
            public List<r_evento> fichas { get; set; }
            public FichasG2Ofic()
            {
                fichas = new List<r_evento>();
            }
        }

        private class FichasG2Inc
        {
            public int id_incidencia { get; set; }
            public string nb_incidencia { get; set; }
            public List<r_evento> fichas { get; set; }
            public FichasG2Inc()
            {
                fichas = new List<r_evento>();
            }
        }
        #endregion

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
