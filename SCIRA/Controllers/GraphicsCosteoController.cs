using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "Costeo-Stats", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class GraphicsCosteoController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            ViewBag.entidades = (MultiSelectList)Utilidades.DropDown.EntidadesMS();
            ViewBag.puestos = (MultiSelectList)Utilidades.DropDown.PuestosMS();
            ViewBag.usuarios = (MultiSelectList)Utilidades.DropDown.UsuariosMS();
            ViewBag.niveles = (MultiSelectList)Utilidades.DropDown.NivelesPuestosMS();

            var user = (IdentityPersonalizado)User.Identity;
            var us = db.c_usuario.Find(user.Id_usuario);

            var sps = Utilidades.Utilidades.RTCObject(us, db, "c_sub_proceso").Cast<c_sub_proceso>().ToList();
            var spct = SPconTiempos(sps);
            var spst = sps.Except(spct).ToList();

            ViewBag.d1 = spct.Count;
            ViewBag.d2 = spst.Count;

            ViewBag.data1 = CritcData(sps);

            return View();
        }

        #region otros
        private List<c_sub_proceso> SPconTiempos(List<c_sub_proceso> sps)
        {
            List<c_sub_proceso> spct = new List<c_sub_proceso>();

            foreach (var sp in sps)
            {
                var usps = sp.c_usuario_sub_proceso.ToList();
                foreach (var usp in usps)
                {
                    if (usp.tiempo_sub_proceso > 0)
                    {
                        spct.Add(sp);
                        break;
                    }
                }
            }

            return spct;
        }

        #endregion

        #region Detalle de las barras
        public ActionResult DetailsSPCT(string[] entidades, string[] puestos, string[] usuarios, string[] niveles, string check1, string check2, string check3)
        {
            ModelList model = getFilteredElements(entidades, puestos, usuarios, niveles, check1, check2, check3);

            var sps = model.LSP;
            var spct = SPconTiempos(sps);

            return PartialView("DetailsSP", spct);
        }

        public ActionResult DetailsSPST(string[] entidades, string[] puestos, string[] usuarios, string[] niveles, string check1, string check2, string check3)
        {
            ModelList model = getFilteredElements(entidades, puestos, usuarios, niveles, check1, check2, check3);

            var sps = model.LSP;
            var spct = SPconTiempos(sps);
            var spst = sps.Except(spct).ToList();

            return PartialView("DetailsSP", spst);
        }
        #endregion

        #region Filtros1
        public ActionResult loadFilters(string[] entidades, string[] puestos, string[] usuarios, string[] niveles, string check1, string check2, string check3)
        {
            ModelList model = getFilteredElements(entidades, puestos, usuarios, niveles, check1, check2, check3);

            var spct = SPconTiempos(model.LSP);
            var spst = model.LSP.Except(spct).ToList();

            ViewBag.d1 = spct.Count;
            ViewBag.d2 = spst.Count;

            ViewBag.data1 = CritcData(model.LSP);

            return PartialView();
        }


        private ModelList getFilteredElements(string[] entidades, string[] puestos, string[] usuarios, string[] niveles, string check1, string check2, string check3)
        {
            ModelList model = new ModelList();

            bool ch1 = check1 == "on";
            bool ch2 = check2 == "on";
            bool ch3 = check3 == "on";

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

            var user = (IdentityPersonalizado)User.Identity;
            var us = db.c_usuario.Find(user.Id_usuario);

            var mps = Utilidades.Utilidades.RTCObject(us, db, "c_macro_proceso").Cast<c_macro_proceso>().ToList();
            var prs = Utilidades.Utilidades.RTCObject(us, db, "c_proceso").Cast<c_proceso>().ToList();
            var sps = Utilidades.Utilidades.RTCObject(us, db, "c_sub_proceso").Cast<c_sub_proceso>().ToList();
            var riesgos = Utilidades.Utilidades.RTCRiesgo(us, db);
            var controles = Utilidades.Utilidades.RTCObject(us, db, "k_control", "1").Cast<k_control>().ToList();

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
                    LAR = LAR.Union(riesgos.Where(p => p.c_sub_proceso.id_responsable == ID)).ToList();
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
                    LAR = LAR.Union(riesgos.Where(p => p.c_sub_proceso.id_responsable == ID)).ToList();
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
                    LAR = LAR.Union(riesgos.Where(p => p.c_sub_proceso.id_responsable == ID)).ToList();
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
                model.LMP = mps.ToList();
                model.LP = prs.ToList();
                model.LSP = sps.ToList();
                model.LR = riesgos.ToList();
                model.LC = controles.ToList();
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

        #region Areas de Costeo
        public string CritcData(List<c_sub_proceso> sub_procesos)
        {
            string data1 = "["; //datos del conteo
            string data2 = "["; //colores de las barras
            string data3 = "["; //etiquetas de las barras
            string data4 = "["; //ids de las barras

            var AreasCosteo = db.c_area_costeo.ToList();
            foreach (var ac in AreasCosteo)
            {
                int auxC = 0;

                var acspLA = ac.c_area_costeo_sub_proceso.ToList();
                List<c_area_costeo_sub_proceso> acspL = new List<c_area_costeo_sub_proceso>();


                foreach (var acsp in acspLA)//Eliminar registros que no pertenezcan a esta lista de sub procesos
                {
                    var sp = acsp.c_sub_proceso;
                    if (sub_procesos.Contains(sp)) acspL.Add(acsp);
                }


                var spTotales = sub_procesos.Count;

                foreach (var acsp in acspL)
                {
                    auxC += (int)(acsp.pr_costeo);
                }

                var val = (double)((double)auxC / (double)spTotales);
                var val2 = string.Format("{0:0.00}", val);

                data1 += val2.Replace(',', '.') + ",";
                //data1 += ((double)((double)auxC / (double)spTotales)).ToString().Replace(',','.') + ",";
                data2 += "'#7C8195',";
                data3 += "'" + ac.nb_area_costeo + "',";
                data4 += ac.id_area_costeo + ",";
            }
            data1 += "]";
            data2 += "]";
            data3 += "]";
            data4 += "]};";

            string d1 = "{datasets: [{label: 'Porcentaje Total',data:";
            string d2 = ",backgroundColor: ";
            string d3 = "}],labels: ";
            string d4 = ",ids: ";

            string res = d1 + data1 + d2 + data2 + d3 + data3 + d4 + data4;
            return res;
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
