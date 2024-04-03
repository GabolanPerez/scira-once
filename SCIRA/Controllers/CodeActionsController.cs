using Newtonsoft.Json.Linq;
using SCIRA.Models;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "CodeActions", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class CodeActionsController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: Actividad
        public ActionResult Index()
        {
            List<ModuleVigency> MVSModel = new List<ModuleVigency>();
            ViewBag.Code = Utilidades.Utilidades.getKey();

            return View(MVSModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Index(string code)
        {
            List<ModuleVigency> MVSModel = new List<ModuleVigency>();

            if (code != null)
            {
                var key = Utilidades.Utilidades.getKey();
                try
                {
                    var clearText = Utilidades.Utilidades.SDFK(key, code);
                    JObject jsonObject = JObject.Parse(clearText);


                    //leer código JSON hacer subrutinas para leer cada uno de los elementos

                    //rutina 0: verificar la vigencia del código;
                    if (!checkExpiration(jsonObject))
                    {
                        ViewBag.Error = @Strings.getMSG("AjustesSeguridadIndex031");
                        ViewBag.Code = Utilidades.Utilidades.getKey();
                        return View(MVSModel);
                    }

                    //rutina 1: leer el elemento Vigencias y por cada elemento realizar la acción
                    MVSModel = EditVigency(jsonObject);

                }
                catch
                {
                    ViewBag.Error = @Strings.getMSG("AjustesSeguridadIndex031");
                }
            }
            else
            {
                ViewBag.Error = @Strings.getMSG("CodeActionsIndex008");
            }

            ViewBag.Code = Utilidades.Utilidades.getKey();
            return View(MVSModel);
        }

        private bool checkExpiration(JObject jsonObject)
        {
            try     //Intentamos obtener la fecha de expiración del código, en caso de ser menor a la fecha actual o de no encontrarse, retornaremos un error
            {
                JProperty expiracion = jsonObject.Property("Expiracion");
                DateTime expFe = DateTime.ParseExact(expiracion.Values().First().ToString(), "dd/MM/yyyy",
                                            System.Globalization.CultureInfo.InvariantCulture);
                if (expFe > DateTime.Now)
                {
                    return true;
                }
            }
            catch
            {
                ViewBag.Code = Utilidades.Utilidades.getKey();
                return false;
            }
            return false;
        }

        private List<ModuleVigency> EditVigency(JObject jsonObject)
        {
            List<ModuleVigency> MVS = new List<ModuleVigency>();
            List<ModuleVigency> MVSModel = new List<ModuleVigency>();
            //JsonTextReader reader = new JsonTextReader(new StringReader(Json));
            try
            {
                var vigencias = jsonObject["Vigencias"];
                var key = Utilidades.Utilidades.getKey();

                foreach (JProperty vig in vigencias)
                {
                    ModuleVigency mv = new ModuleVigency() { ModuleCode = vig.Name, Vigency = vig.Value.ToString() };
                    ModuleVigency mvModel = new ModuleVigency() { ModuleCode = ModuleName(vig.Name), Vigency = vig.Value.ToString() };
                    MVS.Add(mv);
                    MVSModel.Add(mvModel);
                }

                foreach (var mv in MVS)
                {
                    //obtenemos el valor del parametro o lo creamos en caso de no existir

                    var cipherVigency = Utilidades.Utilidades.SCFK(key, "01/01/1000 " + mv.ModuleCode);


                    Utilidades.Utilidades.GetSecurityProp(mv.ModuleCode, cipherVigency);
                    //Utilidades.Utilidades.GetSecurityProp(mv.ModuleCode, "01/01/3000");
                    //obtenemos el parametro
                    db = new SICIEntities();
                    c_parametro ModVig = db.c_parametro.Where(p => p.nb_parametro == mv.ModuleCode).First();


                    cipherVigency = Utilidades.Utilidades.SCFK(key, mv.Vigency + " " + mv.ModuleCode);


                    ModVig.valor_parametro = cipherVigency;
                    //ModVig.valor_parametro = mv.Vigency;


                    db.SaveChanges();
                }
            }
            catch
            {
                return MVSModel;
            }

            return MVSModel;
        }

        private string ModuleName(string moduleCode)
        {
            string moduleName = "";

            switch (moduleCode)
            {
                case "MSICI000":
                    moduleName = "Módulo Base";
                    break;
                case "MSICI001":
                    moduleName = "Módulo de Indicadores";
                    break;
                case "MSICI002":
                    moduleName = "Módulo de Normatividad";
                    break;
                case "MSICI003":
                    moduleName = "Módulo MRyC";
                    break;
                case "MSICI004":
                    moduleName = "Módulo de Certificación";
                    break;
                case "MSICI005":
                    moduleName = "Módulo BDEI";
                    break;
                case "MSICI006":
                    moduleName = "Módulo de Incidencias y Planes de Remediación";
                    break;
                case "MSICI007":
                    moduleName = "Módulo de Benchmarck";
                    break;
                case "MSICI008":
                    moduleName = "Módulo de Indicadores Diarios";
                    break;
                case "MSICI009":
                    moduleName = "Módulo de Auditoría";
                    break;
                case "MSICI010":
                    moduleName = "Módulo de Manuales";
                    break;
                case "MSICI011":
                    moduleName = "Módulo de Validación de Controles";
                    break;
                case "MSICI012":
                    moduleName = "Módulo de Diagramas de Flujo";
                    break;
                case "MSICI013":
                    moduleName = "Módulo de Flujos y Narrativas";
                    break;
            }

            return moduleName;
        }

        public class ModuleVigency
        {
            public string ModuleCode { get; set; }
            public string Vigency { get; set; }
        }

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
