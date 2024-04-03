using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "SECSET", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class AjustesSeguridadController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: Actividad
        public ActionResult Index()
        {
            //Restringir el acceso si se está usando ActiveDirectory
            if (((IdentityPersonalizado)User.Identity).activeD) {
                return RedirectToAction("Denied", "Error");
            }

            var model = new AjustesSeguridadViewModel();

            model.MaxLongitudPass = Utilidades.Utilidades.GetIntSecurityProp("MaxLongitudPass", "64");
            model.MinLongitudPass = Utilidades.Utilidades.GetIntSecurityProp("MinLongitudPass", "6");
            model.TiempoCaducidad = Utilidades.Utilidades.GetSecurityProp("TiempoCaducidad", "30/00/00 00:00:00");
            model.BloqueoNoIngreso = Utilidades.Utilidades.GetSecurityProp("BloqueoNoIngreso", "30/00/00 00:00:00");
            model.IntentosMaximos = Utilidades.Utilidades.GetIntSecurityProp("IntentosMaximos", "3");
            model.TiempoEntreIntentos = Utilidades.Utilidades.GetIntSecurityProp("TiempoEntreIntentos", "5");
            model.TiempoSesion = Utilidades.Utilidades.GetIntSecurityProp("TiempoSesion", "30");
            model.ReutilizarPass = Utilidades.Utilidades.GetBoolSecurityProp("ReutilizarPass", "true");
            model.Mayuscula = Utilidades.Utilidades.GetBoolSecurityProp("Mayuscula", "false");
            model.Numero = Utilidades.Utilidades.GetBoolSecurityProp("Numero", "false");
            model.RepetirUsuario = Utilidades.Utilidades.GetBoolSecurityProp("RepetirUsuario", "false");
            model.BSI = Utilidades.Utilidades.GetBoolSecurityProp("BSI", "false");
            model.timeZone = Utilidades.Utilidades.GetSecurityProp("TimeZone", TimeZoneInfo.Local.Id);

            ViewBag.TimeZones = DropDown.TimeZones(model.timeZone);


            model = GetTimeSettings(model, 0);
            model = GetTimeSettings(model, 1);

            return View(model);
        }


        // POST: AjustesSeguridad/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Index(AjustesSeguridadViewModel model)
        //public ActionResult Edit()
        {
            //AjustesSeguridadViewModel model = new AjustesSeguridadViewModel();

            if (model.MaxLongitudPass < model.MinLongitudPass)
            {
                ModelState.AddModelError("MaxLongitudPass", "El tamaño máximo de una contraseña no puede ser mas pequeño que el mínimo.");
            }


            if (ModelState.IsValid)
            {
                model = SetTimeSettings(model, 0);
                model = SetTimeSettings(model, 1);

                //Guardamos los valores de todos los atributos
                Utilidades.Utilidades.SetSecurityProp("MaxLongitudPass", model.MaxLongitudPass.ToString());
                Utilidades.Utilidades.SetSecurityProp("MinLongitudPass", model.MinLongitudPass.ToString());
                Utilidades.Utilidades.SetSecurityProp("BloqueoNoIngreso", model.BloqueoNoIngreso.ToString());
                Utilidades.Utilidades.SetSecurityProp("IntentosMaximos", model.IntentosMaximos.ToString());
                Utilidades.Utilidades.SetSecurityProp("Mayuscula", model.Mayuscula.ToString());
                Utilidades.Utilidades.SetSecurityProp("Numero", model.Numero.ToString());
                Utilidades.Utilidades.SetSecurityProp("RepetirUsuario", model.RepetirUsuario.ToString());
                Utilidades.Utilidades.SetSecurityProp("ReutilizarPass", model.ReutilizarPass.ToString());
                Utilidades.Utilidades.SetSecurityProp("TiempoCaducidad", model.TiempoCaducidad.ToString());
                Utilidades.Utilidades.SetSecurityProp("TiempoEntreIntentos", model.TiempoEntreIntentos.ToString());
                Utilidades.Utilidades.SetSecurityProp("TiempoSesion", model.TiempoSesion.ToString());
                Utilidades.Utilidades.SetSecurityProp("BSI", model.BSI.ToString());
                Utilidades.Utilidades.SetSecurityProp("TimeZone", model.timeZone);

                ViewBag.Mensaje = Strings.getMSG("CosteoIndex006");
                return RedirectToAction("Index");
            }
            return View();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private AjustesSeguridadViewModel GetTimeSettings(AjustesSeguridadViewModel model, int op)
        {
            string[] FECHAYHORA;
            string[] DDMMAA;
            string[] HORA;
            bool aplica;

            switch (op)
            {
                case 0: //obtener los tiempos para TiempoCaducidad
                    if (model.TiempoCaducidad == null || model.TiempoCaducidad == "")
                    {
                        model.DDtc = 0; model.MMMtc = 1; model.AAtc = 0;
                        model.hhtc = 0; model.mmtc = 0; model.sstc = 0;
                    }
                    else
                    {
                        FECHAYHORA = model.TiempoCaducidad.Split(new Char[] { ' ' });
                        aplica = FECHAYHORA.Length == 2;
                        if (aplica)
                        {
                            DDMMAA = FECHAYHORA[0].Split(new Char[] { '/' });
                            HORA = FECHAYHORA[1].Split(new Char[] { ':' });
                            if (DDMMAA.Length == 3 && HORA.Length == 3)
                            {
                                model.DDtc = Int32.Parse(DDMMAA[0]);
                                model.MMMtc = Int32.Parse(DDMMAA[1]);
                                model.AAtc = Int32.Parse(DDMMAA[2]);
                                model.hhtc = Int32.Parse(HORA[0]);
                                model.mmtc = Int32.Parse(HORA[1]);
                                model.sstc = Int32.Parse(HORA[2]);
                            }
                            else
                            {
                                model.DDtc = 0; model.MMMtc = 1; model.AAtc = 0;
                                model.hhtc = 0; model.mmtc = 0; model.sstc = 0;
                            }
                        }
                    }
                    break;
                case 1: //obtener los tiempos para BloqueoNoIngreso
                    if (model.BloqueoNoIngreso == null || model.BloqueoNoIngreso == "")
                    {
                        model.DDbni = 0; model.MMMbni = 1; model.AAbni = 0;
                        model.hhbni = 0; model.mmbni = 0; model.ssbni = 0;
                    }
                    else
                    {
                        FECHAYHORA = model.BloqueoNoIngreso.Split(new Char[] { ' ' });
                        aplica = FECHAYHORA.Length == 2;
                        if (aplica)
                        {
                            DDMMAA = FECHAYHORA[0].Split(new Char[] { '/' });
                            HORA = FECHAYHORA[1].Split(new Char[] { ':' });
                            if (DDMMAA.Length == 3 && HORA.Length == 3)
                            {
                                model.DDbni = Int32.Parse(DDMMAA[0]);
                                model.MMMbni = Int32.Parse(DDMMAA[1]);
                                model.AAbni = Int32.Parse(DDMMAA[2]);
                                model.hhbni = Int32.Parse(HORA[0]);
                                model.mmbni = Int32.Parse(HORA[1]);
                                model.ssbni = Int32.Parse(HORA[2]);
                            }
                            else
                            {
                                model.DDbni = 0; model.MMMbni = 1; model.AAbni = 0;
                                model.hhbni = 0; model.mmbni = 0; model.ssbni = 0;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            return model;
        }

        private AjustesSeguridadViewModel SetTimeSettings(AjustesSeguridadViewModel model, int op)
        {
            switch (op)
            {
                case 0: //obtener los tiempos para TiempoCaducidad
                    model.TiempoCaducidad = model.DDtc + "/" + model.MMMtc + "/" + model.AAtc + " " + model.hhtc + ":" + model.mmtc + ":" + model.sstc;
                    break;
                case 1: //obtener los tiempos para BloqueoNoIngreso
                    model.BloqueoNoIngreso = model.DDbni + "/" + model.MMMbni + "/" + model.AAbni + " " + model.hhbni + ":" + model.mmbni + ":" + model.ssbni;
                    break;
                default:
                    break;
            }

            return model;
        }
    }
}
