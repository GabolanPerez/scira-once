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
    [Access(Funcion = "MRyC-Stats", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class GraphicsMRyCController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            var user = (IdentityPersonalizado)User.Identity;
            var us = db.c_usuario.Find(user.Id_usuario);

            var mps = Utilidades.Utilidades.RTCObject(us, db, "c_macro_proceso").Cast<c_macro_proceso>().ToList();
            var prs = Utilidades.Utilidades.RTCObject(us, db, "c_proceso").Cast<c_proceso>().ToList();
            var sps = Utilidades.Utilidades.RTCObject(us, db, "c_sub_proceso").Cast<c_sub_proceso>().ToList();
            var riesgos = Utilidades.Utilidades.RTCRiesgo(us, db);
            var controles = Utilidades.Utilidades.RTCObject(us, db, "k_control", "1").Cast<k_control>().ToList();

            ViewBag.d0 = DatosG0(mps, prs, sps, riesgos, controles);
            ViewBag.d1 = CritcData(riesgos);
            ViewBag.d2 = CertifData(controles, getPeriodos());
            ViewBag.d3 = NCertifData(controles, getPeriodos());   //Regresar datos para las criticidades de las certificaciones negativas

            ViewBag.entidades = (MultiSelectList)Utilidades.DropDown.EntidadesMS();
            ViewBag.puestos = (MultiSelectList)Utilidades.DropDown.PuestosMS();
            ViewBag.usuarios = (MultiSelectList)Utilidades.DropDown.UsuariosMS();
            ViewBag.niveles = (MultiSelectList)Utilidades.DropDown.NivelesPuestosMS();
            ViewBag.periodos = (MultiSelectList)Utilidades.DropDown.PeriodosCertificacionMS();

            return View();
        }

        #region Detalle de las barras
        public ActionResult DetailsMP(string[] entidades, string[] puestos, string[] usuarios, string[] niveles, string check1, string check2, string check3)
        {
            ModelList model = getFilteredElements(entidades, puestos, usuarios, niveles, check1, check2, check3);

            return PartialView(model.LMP);
        }

        public ActionResult DetailsP(string[] entidades, string[] puestos, string[] usuarios, string[] niveles, string check1, string check2, string check3)
        {
            ModelList model = getFilteredElements(entidades, puestos, usuarios, niveles, check1, check2, check3);

            return PartialView(model.LP);
        }

        public ActionResult DetailsSP(string[] entidades, string[] puestos, string[] usuarios, string[] niveles, string check1, string check2, string check3)
        {
            ModelList model = getFilteredElements(entidades, puestos, usuarios, niveles, check1, check2, check3);

            return PartialView(model.LSP);
        }

        public ActionResult DetailsR(string[] entidades, string[] puestos, string[] usuarios, string[] niveles, string check1, string check2, string check3)
        {
            ModelList model = getFilteredElements(entidades, puestos, usuarios, niveles, check1, check2, check3);

            return PartialView(model.LR);
        }

        public ActionResult DetailsC(string[] entidades, string[] puestos, string[] usuarios, string[] niveles, string check1, string check2, string check3)
        {
            ModelList model = getFilteredElements(entidades, puestos, usuarios, niveles, check1, check2, check3);

            return PartialView(model.LC);
        }
        #endregion

        #region Filtros1
        public ActionResult loadFilters(string[] entidades, string[] puestos, string[] usuarios, string[] niveles, int[] periodos, string check1, string check2, string check3)
        {
            ModelList model = getFilteredElements(entidades, puestos, usuarios, niveles, check1, check2, check3);

            ViewBag.d0 = DatosG0(model.LMP, model.LP, model.LSP, model.LR, model.LC);
            ViewBag.d1 = CritcData(model.LR);
            ViewBag.d2 = CertifData(model.LC, getPeriodos(periodos));
            ViewBag.d3 = NCertifData(model.LC, getPeriodos(periodos));


            return PartialView();
        }


        private ModelList getFilteredElements(string[] entidades, string[] puestos, string[] usuarios, string[] niveles, string check1, string check2, string check3)
        {
            ModelList model = new ModelList();

            bool ch1 = check1 == "on";
            bool ch2 = check2 == "on";
            bool ch3 = check3 == "on";

            var user = (IdentityPersonalizado)User.Identity;
            var us = db.c_usuario.Find(user.Id_usuario);

            var mps = Utilidades.Utilidades.RTCObject(us, db, "c_macro_proceso").Cast<c_macro_proceso>().ToList();
            var prs = Utilidades.Utilidades.RTCObject(us, db, "c_proceso").Cast<c_proceso>().ToList();
            var sps = Utilidades.Utilidades.RTCObject(us, db, "c_sub_proceso").Cast<c_sub_proceso>().ToList();
            var riesgos = Utilidades.Utilidades.RTCRiesgo(us, db);
            var controles = Utilidades.Utilidades.RTCObject(us, db, "k_control", "1").Cast<k_control>().ToList();

            List<c_macro_proceso> LMP = new List<c_macro_proceso>();
            List<c_proceso> LP = new List<c_proceso>();
            List<c_sub_proceso> LSP = new List<c_sub_proceso>();
            List<k_riesgo> LR = new List<k_riesgo>();
            List<k_control> LC = new List<k_control>();

            List<c_macro_proceso> LAMP = new List<c_macro_proceso>();
            List<c_proceso> LAP = new List<c_proceso>();
            List<c_sub_proceso> LASP = new List<c_sub_proceso>();
            List<k_riesgo> LAR = new List<k_riesgo>();
            List<k_control> LAC = new List<k_control>();

            List<c_usuario> LAU = new List<c_usuario>();
            List<c_puesto> LAPU = new List<c_puesto>();
            List<c_puesto> LAPU2 = new List<c_puesto>();
            List<c_puesto> LAPU3 = new List<c_puesto>();


            //Obtener lista de elementos al filtrar por entidades
            if (entidades != null)
            {
                foreach (var id in entidades)
                {
                    int ID = int.Parse(id);
                    LMP = LMP.Union(mps.Where(mp => mp.id_entidad == ID).ToList()).ToList();
                    LP = LP.Union(prs.Where(p => p.c_macro_proceso.id_entidad == ID)).ToList();
                    LSP = LSP.Union(sps.Where(p => p.c_proceso.c_macro_proceso.id_entidad == ID)).ToList();
                    LR = LR.Union(riesgos.Where(c => c.c_sub_proceso.c_proceso.c_macro_proceso.id_entidad == ID)).ToList();
                    LC = LC.Union(controles.Where(c => c.c_sub_proceso.c_proceso.c_macro_proceso.id_entidad == ID)).ToList();
                }
            }

            if (usuarios != null)
            {
                foreach (var id in usuarios)
                {
                    int ID = int.Parse(id);
                    LAMP = LAMP.Union(mps.Where(mp => mp.id_responsable == ID).ToList()).ToList();
                    LAP = LAP.Union(prs.Where(p => p.id_responsable == ID)).ToList();
                    LASP = LASP.Union(sps.Where(p => p.id_responsable == ID)).ToList();
                    LAR = LAR.Union(riesgos.Where(r => r.c_sub_proceso.id_responsable == ID)).ToList();
                    LAC = LAC.Union(controles.Where(c => c.id_responsable == ID)).ToList();
                }

                if (ch1) //Unimos
                {
                    LMP = LMP.Union(LAMP).ToList();
                    LP = LP.Union(LAP).ToList();
                    LSP = LSP.Union(LASP).ToList();
                    LR = LR.Union(LAR).ToList();
                    LC = LC.Union(LAC).ToList();
                }
                else if (entidades != null)//Intersectamos
                {
                    LMP = LMP.Intersect(LAMP).ToList();
                    LP = LP.Intersect(LAP).ToList();
                    LSP = LSP.Intersect(LASP).ToList();
                    LR = LR.Intersect(LAR).ToList();
                    LC = LC.Intersect(LAC).ToList();
                }
                else
                {
                    LMP = LAMP;
                    LP = LAP;
                    LSP = LASP;
                    LR = LAR;
                    LC = LAC;
                }
            }




            LAMP = new List<c_macro_proceso>();
            LAP = new List<c_proceso>();
            LASP = new List<c_sub_proceso>();
            LAR = new List<k_riesgo>();
            LAC = new List<k_control>();

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

                foreach (var uss in LAU)
                {
                    int ID = uss.id_usuario;
                    LAMP = LAMP.Union(mps.Where(mp => mp.id_responsable == ID).ToList()).ToList();
                    LAP = LAP.Union(prs.Where(p => p.id_responsable == ID)).ToList();
                    LASP = LASP.Union(sps.Where(p => p.id_responsable == ID)).ToList();
                    LAR = LAR.Union(riesgos.Where(r => r.c_sub_proceso.id_responsable == ID)).ToList();
                    LAC = LAC.Union(controles.Where(c => c.id_responsable == ID)).ToList();
                }

                if (ch1) //Unimos
                {
                    LMP = LMP.Union(LAMP).ToList();
                    LP = LP.Union(LAP).ToList();
                    LSP = LSP.Union(LASP).ToList();
                    LR = LR.Union(LAR).ToList();
                    LC = LC.Union(LAC).ToList();
                }
                else if (entidades != null || usuarios != null)//Intersectamos
                {
                    LMP = LMP.Intersect(LAMP).ToList();
                    LP = LP.Intersect(LAP).ToList();
                    LSP = LSP.Intersect(LASP).ToList();
                    LR = LR.Intersect(LAR).ToList();
                    LC = LC.Intersect(LAC).ToList();
                }
                else
                {
                    LMP = LAMP;
                    LP = LAP;
                    LSP = LASP;
                    LR = LAR;
                    LC = LAC;
                }
            }


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

                foreach (var uss in LAU)
                {
                    int ID = uss.id_usuario;
                    LAMP = LAMP.Union(mps.Where(mp => mp.id_responsable == ID).ToList()).ToList();
                    LAP = LAP.Union(prs.Where(p => p.id_responsable == ID)).ToList();
                    LASP = LASP.Union(sps.Where(p => p.id_responsable == ID)).ToList();
                    LAR = LAR.Union(riesgos.Where(r => r.c_sub_proceso.id_responsable == ID)).ToList();
                    LAC = LAC.Union(controles.Where(c => c.id_responsable == ID)).ToList();
                }

                if (ch1) //Unimos
                {
                    LMP = LMP.Union(LAMP).ToList();
                    LP = LP.Union(LAP).ToList();
                    LSP = LSP.Union(LASP).ToList();
                    LR = LR.Union(LAR).ToList();
                    LC = LC.Union(LAC).ToList();
                }
                else if (entidades != null || usuarios != null || puestos != null)//Intersectamos
                {
                    LMP = LMP.Intersect(LAMP).ToList();
                    LP = LP.Intersect(LAP).ToList();
                    LSP = LSP.Intersect(LASP).ToList();
                    LR = LR.Intersect(LAR).ToList();
                    LC = LC.Intersect(LAC).ToList();
                }
                else
                {
                    LMP = LAMP;
                    LP = LAP;
                    LSP = LASP;
                    LR = LAR;
                    LC = LAC;
                }
            }


            if (entidades == null && usuarios == null && puestos == null && niveles == null)
            {
                model.LMP = mps;
                model.LP = prs;
                model.LSP = sps;
                model.LR = riesgos;
                model.LC = controles;
            }
            else
            {
                model.LMP = LMP;
                model.LP = LP;
                model.LSP = LSP;
                model.LR = LR;
                model.LC = LC;
            }
            return model;
        }


        private class ModelList
        {

            public ModelList()
            {
                List<c_macro_proceso> LMP = new List<c_macro_proceso>();
                List<c_proceso> LP = new List<c_proceso>();
                List<c_sub_proceso> LSP = new List<c_sub_proceso>();
                List<k_riesgo> LR = new List<k_riesgo>();
                List<k_control> LC = new List<k_control>();
            }
            public List<c_macro_proceso> LMP;
            public List<c_proceso> LP;
            public List<c_sub_proceso> LSP;
            public List<k_riesgo> LR;
            public List<k_control> LC;
        }
        #endregion

        #region Certificación
        public string NCertifData(List<k_control> controles, List<c_periodo_certificacion> periodos)
        {
            List<k_certificacion_control> certificaciones = new List<k_certificacion_control>();
            foreach (var control in controles)
            {
                certificaciones.AddRange(db.k_certificacion_control.Where(c => c.id_control == control.id_control).ToList());
            }

            //obtener criticidades
            var criticidades_riesgo = db.c_criticidad_riesgo.ToList();

            var LAC1 = new List<k_control>(); //Lista de controles certificados
            var LAC2 = new List<k_control>(); //Lista de controles certificados negativos


            List<double> d1 = new List<double>(); //Datos de los controles certificados positivos
            List<double> d2 = new List<double>(); //Datos de los controles certificados negativos
            List<double> d3 = new List<double>(); //Datos de los controles no certificados
            List<string> Labels = new List<string>(); //Etiquetas de las barras
            List<int> Ids = new List<int>(); //Etiquetas de las barras

            List<GraphicsViewModel.Dataset> DataSets = new List<GraphicsViewModel.Dataset>();

            string data1 = "["; //nombres de los periodos
            List<string> data2 = new List<string>();
            string data02 = "";
            string data3 = "["; //ids

            foreach (var periodo in periodos)
            {
                //data1 += "'" + periodo.nb_periodo_certificacion + "',";
                //data3 += periodo.id_periodo_certificacion + ",";
                Labels.Add(periodo.nb_periodo_certificacion);
                Ids.Add(periodo.id_periodo_certificacion);
            }
            data1 += "]";
            data3 += "]};";

            foreach (var cr in criticidades_riesgo)
            {

                var data = new List<double>();

                //incluir conteos
                foreach (var periodo in periodos)
                {
                    LAC1 = controles.Where(c => c.k_certificacion_control.Any(cert => cert.id_periodo_certificacion == periodo.id_periodo_certificacion)).ToList();//obtencion de los controles certificados en este periodo
                    LAC2 = LAC1.Where(c => !c.k_certificacion_control.Where(cert => cert.id_periodo_certificacion == periodo.id_periodo_certificacion).First().tiene_disenio_efectivo || !c.k_certificacion_control.Where(cert => cert.id_periodo_certificacion == periodo.id_periodo_certificacion).First().tiene_funcionamiento_efectivo).ToList();
                    //Cuales de estos periodos pertenecen a esta criticidad?
                    int count = 0;
                    var crts = cr.c_criticidad.ToList();
                    //para cada criticidad perteneciente a esta criticidad_riesgo, checar cuales controles pertenecen a un riesgo ligado a esta criticidad
                    foreach (var crs in crts)
                    {
                        var la1 = LAC2.Where(c => c.k_riesgo.First().id_magnitud_impacto == crs.id_magnitud_impacto && c.k_riesgo.First().id_probabilidad_ocurrencia == crs.id_probabilidad_ocurrencia).ToList();
                        count += la1.Count();
                    }
                    data.Add(count);
                }

                DataSets.Add(new GraphicsViewModel.Dataset(cr.nb_criticidad_riesgo, cr.id_criticidad_riesgo, data.ToArray(), cr.cl_color_campo, null));
            }

            foreach (var data in data2)
            {
                data02 += data;
            }

            /*
            string d1 = "{labels:";
            string d2 = ", datasets:[";
            string d3 = "], ids: ";
            return d1 + data1 + d2 + data02 + d3 + data3;*/



            GraphicsViewModel.Dataset[] datasets = DataSets.ToArray();

            var Data0 = new GraphicsViewModel.Data()
            {
                labels = Labels.ToArray(),
                datasets = datasets,
                ids = Ids.ToArray()
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(Data0);
        }

        public string CertifData(List<k_control> controles, List<c_periodo_certificacion> periodos)
        {
            //obtenemos todas las certificaciones de los controles provistos
            List<k_certificacion_control> certificaciones = new List<k_certificacion_control>();
            foreach (var control in controles)
            {
                certificaciones.AddRange(db.k_certificacion_control.Where(c => c.id_control == control.id_control).ToList());
            }


            var LAC1 = new List<k_control>();
            var LAC2 = new List<k_control>();


            List<double> d1 = new List<double>(); //Datos de los controles certificados positivos
            List<double> d2 = new List<double>(); //Datos de los controles certificados negativos
            List<double> d3 = new List<double>(); //Datos de los controles no certificados
            List<string> Labels = new List<string>(); //Etiquetas de las barras
            List<int> Ids = new List<int>(); //Etiquetas de las barras


            int a1, a2, a3;

            foreach (var periodo in periodos)
            {

                LAC1 = controles.Where(c => c.k_certificacion_control.Any(cert => cert.id_periodo_certificacion == periodo.id_periodo_certificacion)).ToList();//obtencion de los controles certificados en este periodo
                LAC2 = controles.Where(c => !c.k_certificacion_control.Any(cert => cert.id_periodo_certificacion == periodo.id_periodo_certificacion)).ToList();//obtencion de los controles no certificados en este periodo

                a1 = LAC1.Where(c => c.k_certificacion_control.Where(cert => cert.id_periodo_certificacion == periodo.id_periodo_certificacion).First().tiene_disenio_efectivo && c.k_certificacion_control.Where(cert => cert.id_periodo_certificacion == periodo.id_periodo_certificacion).First().tiene_funcionamiento_efectivo).ToList().Count;
                a2 = LAC1.Count - a1;
                a3 = LAC2.Count;

                Labels.Add(periodo.nb_periodo_certificacion);
                d1.Add(a1);
                d2.Add(a2);
                d3.Add(a3);
                Ids.Add(periodo.id_periodo_certificacion);
            }

            var dts1 = new GraphicsViewModel.Dataset(Strings.getMSG("GraphicsMRyCIndex014"), 1, d1.ToArray(), "#008000", null);
            var dts2 = new GraphicsViewModel.Dataset(Strings.getMSG("GraphicsMRyCIndex015"), 2, d2.ToArray(), "#FF0000", null);
            var dts3 = new GraphicsViewModel.Dataset(Strings.getMSG("Menu131"), 3, d3.ToArray(), "#808080", null);

            GraphicsViewModel.Dataset[] datasets = { dts1, dts2, dts3 };

            var Data0 = new GraphicsViewModel.Data()
            {
                labels = Labels.ToArray(),
                datasets = datasets,
                ids = Ids.ToArray()
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(Data0);
        }

        public ActionResult DetailsNCert(string[] entidades, string[] puestos, string[] usuarios, string[] niveles, string check1, string check2, string check3, int id_pc, int id_pb)
        {
            ModelList model = getFilteredElements(entidades, puestos, usuarios, niveles, check1, check2, check3);
            //obtenemos las criticidades pertenencientes a la criticidad_riesgo
            var periodo = db.c_periodo_certificacion.Find(id_pc);
            var CR = db.c_criticidad_riesgo.Find(id_pb);
            var crts = CR.c_criticidad.ToList();

            var certificaciones = db.k_certificacion_control.Where(c => c.id_periodo_certificacion == id_pc).ToList();
            var LCC = new List<k_control>();
            var LCN = new List<k_control>();
            var Model = new List<k_control>();

            LCC = model.LC.Where(c => c.k_certificacion_control.Any(cert => cert.id_periodo_certificacion == periodo.id_periodo_certificacion)).ToList(); //Controles certificados en este periodo

            //controles certificados negativos
            LCN = LCC.Where(c => !c.k_certificacion_control.Where(cert => cert.id_periodo_certificacion == periodo.id_periodo_certificacion).First().tiene_disenio_efectivo || !c.k_certificacion_control.Where(cert => cert.id_periodo_certificacion == periodo.id_periodo_certificacion).First().tiene_funcionamiento_efectivo).ToList();

            foreach (var crs in crts)
            {
                var la1 = LCN.Where(c => c.k_riesgo.First().id_magnitud_impacto == crs.id_magnitud_impacto && c.k_riesgo.First().id_probabilidad_ocurrencia == crs.id_probabilidad_ocurrencia).ToList();
                Model = Model.Union(la1).ToList();
            }


            return PartialView("DetailsC", Model);
        }

        public ActionResult DetailsCert(string[] entidades, string[] puestos, string[] usuarios, string[] niveles, string check1, string check2, string check3, int id_pc, int id_pb)
        {
            ModelList model = getFilteredElements(entidades, puestos, usuarios, niveles, check1, check2, check3);
            //obtenemos las criticidades pertenencientes a la criticidad_riesgo
            var periodo = db.c_periodo_certificacion.Find(id_pc);
            var certificaciones = db.k_certificacion_control.Where(c => c.id_periodo_certificacion == id_pc).ToList();

            var LCC = new List<k_control>();


            var LCP = new List<k_control>();
            var LCN = new List<k_control>();
            var LCNC = new List<k_control>();

            LCC = model.LC.Where(c => c.k_certificacion_control.Any(cert => cert.id_periodo_certificacion == periodo.id_periodo_certificacion)).ToList(); //periodos certificados en este periodo
            //Controles no certificados en este periodo
            LCNC = model.LC.Where(c => !c.k_certificacion_control.Any(cert => cert.id_periodo_certificacion == periodo.id_periodo_certificacion)).ToList();

            //controles certificados positivos
            LCP = LCC.Where(c => c.k_certificacion_control.Where(cert => cert.id_periodo_certificacion == periodo.id_periodo_certificacion).First().tiene_disenio_efectivo && c.k_certificacion_control.Where(cert => cert.id_periodo_certificacion == periodo.id_periodo_certificacion).First().tiene_funcionamiento_efectivo).ToList();

            //controles certificados negativos
            LCN = LCC.Where(c => !c.k_certificacion_control.Where(cert => cert.id_periodo_certificacion == periodo.id_periodo_certificacion).First().tiene_disenio_efectivo || !c.k_certificacion_control.Where(cert => cert.id_periodo_certificacion == periodo.id_periodo_certificacion).First().tiene_funcionamiento_efectivo).ToList();

            if (id_pb == 1) return PartialView("DetailsC", LCP);
            if (id_pb == 2) return PartialView("DetailsC", LCN);
            if (id_pb == 3) return PartialView("DetailsC", LCNC);

            return null;

        }

        private List<c_periodo_certificacion> getPeriodos(int[] IDperiodos = null)
        {
            List<c_periodo_certificacion> periodos = new List<c_periodo_certificacion>();

            if (IDperiodos == null)
            {
                var periodos1 = db.c_periodo_certificacion.ToList();
                int li;
                try
                {
                    li = periodos1.LastIndexOf(periodos1.Last());//Encontramos el ultimo periodo
                }
                catch
                {
                    return new List<c_periodo_certificacion>();
                }



                if (periodos1.Count >= 3)
                {
                    periodos.Add(periodos1[li - 2]);
                    periodos.Add(periodos1[li - 1]);
                    periodos.Add(periodos1[li]);
                }
                else
                {
                    periodos = periodos1;
                }
            }
            else
            {
                foreach (var idp in IDperiodos)
                {
                    var periodo = db.c_periodo_certificacion.Find(idp);
                    periodos.Add(periodo);
                }
            }

            return periodos;
        }
        #endregion

        #region Criticidad
        public string CritcData(List<k_riesgo> riesgos) //genera la cadena con los datos para las criticidades
        {
            List<double> data = new List<double>(); //datos del conteo
            List<string> bgc = new List<string>(); //colores de las barras
            List<string> Labels = new List<string>();
            List<int> ids = new List<int>(); //ids de las barras

            //Obtenemos el conteo del numero de riesgos por criticidad asi como sus colores
            var CrRs = db.c_criticidad_riesgo.ToList();
            foreach (var cr in CrRs)
            {
                int auxC = 0;
                var criticidades = db.c_criticidad.Where(c => c.id_criticidad_riesgo == cr.id_criticidad_riesgo).ToList();
                foreach (var crit in criticidades)
                {
                    auxC += riesgos.Where(r => r.id_magnitud_impacto == crit.id_magnitud_impacto && r.id_probabilidad_ocurrencia == crit.id_probabilidad_ocurrencia).ToList().Count;
                }

                data.Add(auxC);
                bgc.Add(cr.cl_color_campo);
                Labels.Add(cr.nb_criticidad_riesgo);
                ids.Add(cr.id_criticidad_riesgo);
            }

            var dataset = new GraphicsViewModel.Dataset(Strings.getMSG("GraphicsOficioIndex006"), 1, data.ToArray(), bgc.ToArray(), null);

            string[] labels = Labels.ToArray();
            GraphicsViewModel.Dataset[] datasets = { dataset };


            var Data0 = new GraphicsViewModel.Data()
            {
                labels = labels,
                datasets = datasets,
                ids = ids.ToArray()
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(Data0);
        }


        public ActionResult DetailsCrit(string[] entidades, string[] puestos, string[] usuarios, string[] niveles, string check1, string check2, string check3, int id_crit)
        {
            ModelList model = getFilteredElements(entidades, puestos, usuarios, niveles, check1, check2, check3);
            //obtenemos las criticidades pertenencientes a la criticidad_riesgo
            var crits = db.c_criticidad.Where(c => c.id_criticidad_riesgo == id_crit);
            //reducimos la lista a todos los riesgos en el modelo que pertenezcan a la criticidad
            List<k_riesgo> lista = new List<k_riesgo>();

            foreach (var crit in crits)
            {
                lista.AddRange(model.LR.Where(r => r.id_magnitud_impacto == crit.id_magnitud_impacto && r.id_probabilidad_ocurrencia == crit.id_probabilidad_ocurrencia).ToList());
            }

            return PartialView("DetailsR", lista);
        }
        #endregion

        #region Datos
        private string DatosG0(List<c_macro_proceso> mps, List<c_proceso> prs, List<c_sub_proceso> sps, List<k_riesgo> riesgos, List<k_control> controles) //datos para la gráfica 0 "Conteo General"
        {

            double[] data = { mps.Count, prs.Count, sps.Count, riesgos.Count, controles.Count };
            string[] bgc = Utilidades.Utilidades.getColorArray(5);

            var dataset = new GraphicsViewModel.Dataset(Strings.getMSG("GraphicsOficioIndex006"), 1, data, bgc, null);

            string[] labels = { Strings.getMSG("MPS"), Strings.getMSG("PS"), Strings.getMSG("SPS"), Strings.getMSG("Riesgos"), Strings.getMSG("Controles") };
            GraphicsViewModel.Dataset[] datasets = { dataset };


            var Data0 = new GraphicsViewModel.Data()
            {
                labels = labels,
                datasets = datasets
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
