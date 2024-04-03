using iTextSharp.text;
using iTextSharp.text.pdf;
using SCIRA.Models;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using static iTextSharp.text.Font;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "IYP", ModuleCode = "MSICI006")]
    [CustomErrorHandler]
    public class IyPController : Controller
    {
        private SICIEntities db = new SICIEntities();
        private List<int> id_r = new List<int>();


        public ActionResult Index()
        {
            //Enviar datos necesarios para todas las pantallas
            ViewBag.Oficios = db.k_objeto.Where(o => o.tipo_objeto == 1).ToList();
            ViewBag.AuExt = db.k_objeto.Where(o => o.tipo_objeto == 2).ToList();
            ViewBag.AuInt = db.k_objeto.Where(o => o.tipo_objeto == 3).ToList();
            ViewBag.Certificacion = db.k_certificacion_control.Where(o => !o.tiene_disenio_efectivo || !o.tiene_funcionamiento_efectivo).ToList();
            ViewBag.MRyC = db.k_control.Where(o => o.tiene_accion_correctora).ToList();
            ViewBag.Otros = db.k_objeto.Where(o => o.tipo_objeto == 6).ToList();
            

            ViewBag.DateFormat = Utilidades.Utilidades.GetDateFormat();

            return View();
        }

        #region Oficio
        public ActionResult IndexOficio()
        {
            //Enviar datos de los oficios existentes
            var oficios = db.k_objeto.Where(o => o.tipo_objeto == 1).ToList();

            return PartialView("Oficio/IndexOficio", oficios);
        }

        public ActionResult CreateOficio()
        {
            var model = new k_objeto();
            ViewBag.id_entidad = Utilidades.DropDown.Entidades();
            ViewBag.id_autoridad = Utilidades.DropDown.OrigenAutoridad();
            ViewBag.id_responsable = Utilidades.DropDown.Usuario();
            ViewBag.DateFormat = Utilidades.Utilidades.GetDateFormat();
            ViewBag.oficios_relacionados = (MultiSelectList)Utilidades.DropDown.OficiosMSL();
            return PartialView("Oficio/CreateOficio", model);
        }

        // POST: ClasificacionIncidencia/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult CreateOficio(k_objeto oficio, HttpPostedFileBase file1, int[] oficiosRelacionados)
        {
            oficio.tipo_objeto = 1;
            oficio.ds_objeto = "";
            bool validate = validateObject(ModelState, oficio);
            if (ModelState.IsValid && validate)
            {
                db.k_objeto.Add(oficio);
                db.SaveChanges();
                SaveFiles(oficio, file1);

                if (oficiosRelacionados != null)
                {
                    foreach (var id_o in oficiosRelacionados)
                    {
                        var oficioP = db.k_objeto.Find(id_o);
                        oficio.k_objeto2.Add(oficioP);
                    }
                    db.SaveChanges();
                }

                Utilidades.Utilidades.TaskAsigned(oficio);
                return RedirectToAction("IndexOficio");
            }

            ViewBag.id_entidad = Utilidades.DropDown.Entidades();
            ViewBag.id_autoridad = Utilidades.DropDown.OrigenAutoridad();
            ViewBag.id_responsable = Utilidades.DropDown.Usuario();
            ViewBag.DateFormat = Utilidades.Utilidades.GetDateFormat();
            ViewBag.oficios_relacionados = (MultiSelectList)Utilidades.DropDown.OficiosMSL();

            return PartialView("Oficio/CreateOficio", oficio);
        }

        public ActionResult EditOficio(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_objeto oficio = db.k_objeto.Find(id);
            if (oficio == null)
            {
                return HttpNotFound();
            }


            ViewBag.nb_a1 = oficio.nb_archivo_1;
            ViewBag.nb_a2 = oficio.nb_archivo_2;


            ViewBag.id_autoridad = Utilidades.DropDown.OrigenAutoridad();
            ViewBag.id_responsable = Utilidades.DropDown.Usuario();
            ViewBag.DateFormat = Utilidades.Utilidades.GetDateFormat();
            ViewBag.oficios_relacionados = (MultiSelectList)Utilidades.DropDown.OficiosMSL(oficio);


            return PartialView("Oficio/EditOficio", oficio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult EditOficio(k_objeto oficio, HttpPostedFileBase file1, HttpPostedFileBase file2, int[] oficiosRelacionados, int lu)
        {
            oficio.ds_objeto = "";
            bool validate = validateObject(ModelState, oficio);
            if (ModelState.IsValid && validate)
            {
                db.Entry(oficio).State = EntityState.Modified;
                db.SaveChanges();
                SaveFiles(oficio, file1, file2, true);


                db = new SICIEntities();

                var objeto = db.k_objeto.Find(oficio.id_objeto);

                //var linkedItems = db.k_objeto.Where(o => o.k_objeto1.Any(ob => ob.id_objeto == objeto.id_objeto) );


                objeto.k_objeto2.Clear();
                db.SaveChanges();
                if (oficiosRelacionados != null)
                {
                    foreach (var id_o in oficiosRelacionados)
                    {
                        var oficioP = db.k_objeto.Find(id_o);
                        objeto.k_objeto2.Add(oficioP);
                    }
                }
                db.SaveChanges();

                if (oficio.id_responsable != lu) Utilidades.Utilidades.TaskAsigned(oficio, lu);
                return RedirectToAction("IndexOficio");
            }



            ViewBag.id_autoridad = Utilidades.DropDown.OrigenAutoridad();
            ViewBag.id_responsable = Utilidades.DropDown.Usuario();
            ViewBag.DateFormat = Utilidades.Utilidades.GetDateFormat();
            ViewBag.nb_a1 = oficio.nb_archivo_1;
            ViewBag.nb_a2 = oficio.nb_archivo_2;
            ViewBag.oficios_relacionados = (MultiSelectList)Utilidades.DropDown.OficiosMSL(db.k_objeto.Find(oficio.id_objeto));


            oficio.c_entidad = db.c_entidad.Find(oficio.id_entidad);

            return PartialView("Oficio/EditOficio", oficio);
        }

        public ActionResult DeleteOficio(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_objeto oficio = db.k_objeto.Find(id);
            if (oficio == null)
            {
                return HttpNotFound();
            }


            return PartialView("Oficio/DeleteOficio", oficio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteOficio(int id)
        {
            var oficio = db.k_objeto.Find(id);

            if (Utilidades.DeleteActions.DeleteObjetoObjects(oficio, db))
            {
                db.k_objeto.Remove(oficio);
                db.SaveChanges();
            }

            return RedirectToAction("IndexOficio");
        }


        #region DeleteOficioExt
        public ActionResult DeleteOficioExt(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_objeto k_objeto = db.k_objeto.Find(id);
            if (k_objeto == null)
            {
                return HttpNotFound();
            }

            Utilidades.DeleteActions.checkRedirect(redirect);

            //Obtener todos los elementos a los que puede estar ligado este elemento.
            //creamos la lista que contendra a todos los registros relacionados
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;
            return View(k_objeto);
        }


        [HttpPost, ActionName("DeleteOficioExt")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteOficioExt(int id)
        {
            k_objeto k_objeto = db.k_objeto.Find(id);
            Utilidades.DeleteActions.DeleteObjetoObjects(k_objeto, db);
            db.k_objeto.Remove(k_objeto);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("CantErase", "Error", null);
            }


            int ns;
            try
            {
                ns = (int)HttpContext.Session["JumpCounter"];
            }
            catch
            {
                ns = 0;
            }
            //Si ns es 0 redireccionamos al index de este controlador
            if (ns == 0)
            {
                return RedirectToAction("Index");

            }//En caso de que ns sea distinto a 0, obtenemos el Array "Directions", agregamos la direccion actual, aumentamos el contador y salvamos ambas variables globales
            else
            {
                List<string> directions = new List<string>();
                try
                {
                    directions = (List<string>)HttpContext.Session["Directions"];
                }
                catch
                {
                    directions = null;
                }

                if (directions == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    string direction = directions.Last();
                    DirectionViewModel dir = Utilidades.Utilidades.getDirection(direction);
                    //disminuimos ns y eliminamos el ultimo elemento de directions
                    ns--;
                    directions.RemoveAt(ns);

                    //Guardamos ambas variables de sesion para seguir trabajando
                    HttpContext.Session["JumpCounter"] = ns;
                    HttpContext.Session["Directions"] = directions;

                    return RedirectToAction(dir.Action, dir.Controller, new { id = dir.Id, redirect = "bfo" });
                }

            }
        }

        #endregion

        public ActionResult OficiosRelacionados(int? id)
        {
            var oficio = db.k_objeto.Find(id);

            id_r = new List<int>();
            id_r.Add((int)id);
            List<k_objeto> OR = getRO(oficio);

            id_r = new List<int>();
            id_r.Add((int)id);
            List<k_objeto> ORD = getROD(oficio);

            ViewBag.RBelow = ORD;
            ViewBag.RAbove = OR;

            return PartialView("Oficio/OficiosRelacionados");
        }
        #endregion

        #region AuExt
        public ActionResult IndexAuExt()
        {
            //Enviar datos de las Auditorias Externas existentes
            var Aud = db.k_objeto.Where(o => o.tipo_objeto == 2).ToList();
            return PartialView("AuExt/IndexAuExt", Aud);
        }

        public ActionResult CreateAuExt()
        {
            var model = new k_objeto();
            ViewBag.id_entidad = Utilidades.DropDown.Entidades();
            ViewBag.id_responsable = Utilidades.DropDown.Usuario();
            return PartialView("AuExt/CreateAuExt", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult CreateAuExt(k_objeto AuExt, HttpPostedFileBase file1)
        {
            AuExt.tipo_objeto = 2;
            AuExt.ds_objeto = "";
            bool validate = validateObject(ModelState, AuExt);
            if (ModelState.IsValid && validate)
            {
                db.k_objeto.Add(AuExt);
                db.SaveChanges();
                SaveFiles(AuExt, file1);


                Utilidades.Utilidades.TaskAsigned(AuExt);
                return RedirectToAction("IndexAuExt");
            }

            ViewBag.id_entidad = Utilidades.DropDown.Entidades();
            ViewBag.id_responsable = Utilidades.DropDown.Usuario();

            return PartialView("AuExt/CreateAuExt", AuExt);
        }

        public ActionResult EditAuExt(int? id)
        {
            //buscar modelo y regresarlo a la vista junto a los combos necesarios
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_objeto objeto = db.k_objeto.Find(id);
            if (objeto == null)
            {
                return HttpNotFound();
            }

            ViewBag.id_responsable = Utilidades.DropDown.Usuario();

            ViewBag.nb_a1 = objeto.nb_archivo_1;

            return PartialView("AuExt/EditAuExt", objeto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult EditAuExt(k_objeto AuExt, HttpPostedFileBase file1, int lu)
        {
            AuExt.ds_objeto = "";
            bool validate = validateObject(ModelState, AuExt);
            if (ModelState.IsValid && validate)
            {
                db.Entry(AuExt).State = EntityState.Modified;
                db.SaveChanges();
                SaveFiles(AuExt, file1, null, true);

                if (AuExt.id_responsable != lu) Utilidades.Utilidades.TaskAsigned(AuExt, lu);

                return RedirectToAction("IndexAuExt");
            }

            ViewBag.id_responsable = Utilidades.DropDown.Usuario();
            ViewBag.nb_a1 = AuExt.nb_archivo_1;

            AuExt.c_entidad = db.c_entidad.Find(AuExt.id_entidad);
            return PartialView("AuExt/EditAuExt", AuExt);
        }

        public ActionResult DeleteAuExt(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_objeto objeto = db.k_objeto.Find(id);
            if (objeto == null)
            {
                return HttpNotFound();
            }

            return PartialView("AuExt/DeleteAuExt", objeto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteAuExt(int id)
        {
            var objeto = db.k_objeto.Find(id);

            if (Utilidades.DeleteActions.DeleteObjetoObjects(objeto, db))
            {
                db.k_objeto.Remove(objeto);
                db.SaveChanges();
            }

            return RedirectToAction("IndexAuExt");
        }
        #endregion

        #region AuInt
        public ActionResult IndexAuInt()
        {
            //Enviar datos de las Auditorias Internas existentes
            var Aud = db.k_objeto.Where(o => o.tipo_objeto == 3).ToList();
            return PartialView("AuInt/IndexAuInt", Aud);
        }

        public ActionResult CreateAuInt()
        {
            var model = new k_objeto();
            ViewBag.id_entidad = Utilidades.DropDown.Entidades();
            ViewBag.id_responsable = Utilidades.DropDown.Usuario();
            ViewBag.DateFormat = Utilidades.Utilidades.GetDateFormat();
            return PartialView("AuInt/CreateAuInt", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult CreateAuInt(k_objeto AuInt, HttpPostedFileBase file1)
        {
            AuInt.tipo_objeto = 3;
            AuInt.ds_objeto = "";
            bool validate = validateObject(ModelState, AuInt);
            if (ModelState.IsValid && validate)
            {
                db.k_objeto.Add(AuInt);
                db.SaveChanges();
                SaveFiles(AuInt, file1);

                Utilidades.Utilidades.TaskAsigned(AuInt);
                return RedirectToAction("IndexAuInt");
            }

            ViewBag.id_entidad = Utilidades.DropDown.Entidades();
            ViewBag.id_responsable = Utilidades.DropDown.Usuario();
            ViewBag.DateFormat = Utilidades.Utilidades.GetDateFormat();

            return PartialView("AuInt/CreateAuInt", AuInt);
        }

        public ActionResult EditAuInt(int? id)
        {
            //buscar modelo y regresarlo a la vista junto a los combos necesarios
            //buscar modelo y regresarlo a la vista junto a los combos necesarios
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_objeto objeto = db.k_objeto.Find(id);
            if (objeto == null)
            {
                return HttpNotFound();
            }

            ViewBag.id_responsable = Utilidades.DropDown.Usuario();
            ViewBag.nb_a1 = objeto.nb_archivo_1;
            return PartialView("AuInt/EditAuInt", objeto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult EditAuInt(k_objeto AuInt, HttpPostedFileBase file1, int lu)
        {
            AuInt.ds_objeto = "";
            bool validate = validateObject(ModelState, AuInt);
            if (ModelState.IsValid && validate)
            {
                db.Entry(AuInt).State = EntityState.Modified;
                db.SaveChanges();
                SaveFiles(AuInt, file1, null, true);

                if (AuInt.id_responsable != lu) Utilidades.Utilidades.TaskAsigned(AuInt, lu);
                return RedirectToAction("IndexAuInt");
            }

            AuInt.c_entidad = db.c_entidad.Find(AuInt.id_entidad);
            ViewBag.id_responsable = Utilidades.DropDown.Usuario();
            return PartialView("AuInt/EditAuInt", AuInt);
        }

        public ActionResult DeleteAuInt(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_objeto objeto = db.k_objeto.Find(id);
            if (objeto == null)
            {
                return HttpNotFound();
            }

            return PartialView("AuInt/DeleteAuInt", objeto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteAuInt(int id)
        {
            var objeto = db.k_objeto.Find(id);

            if (Utilidades.DeleteActions.DeleteObjetoObjects(objeto, db))
            {
                db.k_objeto.Remove(objeto);
                db.SaveChanges();
            }

            return RedirectToAction("IndexAuInt");
        }
        #endregion

        #region Certificacion
        public ActionResult IndexCert()
        {
            //Enviar datos de las Certificaciones existentes
            var model = ViewBag.Certificacion = db.k_certificacion_control.Where(o => !o.tiene_disenio_efectivo || !o.tiene_funcionamiento_efectivo).ToList();

            return PartialView("Cert/IndexCert", model);
        }

        #endregion

        #region MRyC
        public ActionResult IndexMRyC()
        {
            //Enviar datos de la MRyC existentes
            var model = db.k_control.Where(c => c.tiene_accion_correctora).ToList();
            return PartialView("MRyC/IndexMRyC", model);
        }

        #endregion

        #region Otros
        public ActionResult IndexOtros()
        {
            //Enviar datos de las Auditorias Internas existentes
            var Otros = db.k_objeto.Where(o => o.tipo_objeto == 6).ToList();
            return PartialView("Otros/IndexOtros", Otros);
        }

        public ActionResult CreateOtros()
        {
            var model = new k_objeto();
            ViewBag.id_entidad = Utilidades.DropDown.Entidades();
            ViewBag.DateFormat = Utilidades.Utilidades.GetDateFormat();
            return PartialView("Otros/CreateOtros", model);
        }

        // POST: ClasificacionIncidencia/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult CreateOtros(k_objeto otro)
        {
            otro.fe_alta = DateTime.Now;
            otro.tipo_objeto = 6;
            bool validate = validateObject(ModelState, otro);
            if (ModelState.IsValid && validate)
            {
                db.k_objeto.Add(otro);
                db.SaveChanges();
                return RedirectToAction("IndexOtros");
            }

            ViewBag.id_entidad = Utilidades.DropDown.Entidades();
            ViewBag.DateFormat = Utilidades.Utilidades.GetDateFormat();

            return PartialView("Otros/CreateOtros", otro);
        }

        public ActionResult EditOtros(int? id)
        {
            //buscar modelo y regresarlo a la vista junto a los combos necesarios
            //buscar modelo y regresarlo a la vista junto a los combos necesarios
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_objeto objeto = db.k_objeto.Find(id);
            if (objeto == null)
            {
                return HttpNotFound();
            }


            return PartialView("Otros/EditOtros", objeto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult EditOtros(k_objeto Otro)
        {
            bool validate = validateObject(ModelState, Otro);
            if (ModelState.IsValid && validate)
            {
                db.Entry(Otro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexOtros");
            }

            Otro.c_entidad = db.c_entidad.Find(Otro.id_entidad);
            return PartialView("Otros/EditOtros", Otro);
        }

        public ActionResult DeleteOtros(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_objeto objeto = db.k_objeto.Find(id);
            if (objeto == null)
            {
                return HttpNotFound();
            }

            return PartialView("Otros/DeleteOtros", objeto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteOtros(int id)
        {
            var objeto = db.k_objeto.Find(id);

            if (Utilidades.DeleteActions.DeleteObjetoObjects(objeto, db))
            {
                db.k_objeto.Remove(objeto);
                db.SaveChanges();
            }

            return RedirectToAction("IndexOtros");
        }
        #endregion

        #region Incidencias

        //Se necesitara un número que indique que tipo de lista se esta solicitando
        //y el id del objeto, los origenes serán los siguientes
        //Oificios:         1
        //Aud Ext:          2
        //Aud Int:          3
        //Certificación     4
        //MRyC              5
        //Otros             6
        public ActionResult listaIncidencias(int Origen, int? id)
        {
            ViewBag.Origen = Origen;    //Variable para saber la ruta que deberá mostrarse
            ViewBag.id = id ?? 0;            //Variable para poder regresar a los detalles del objeto seleccionado
            List<k_incidencia> Lista = new List<k_incidencia>();
            if (Origen == 1) //si se solicita información desde un Oficio
            {
                //Generar lista de incidencias perteneciente a este oficio
                Lista = db.k_incidencia.Where(i => i.id_objeto == id).ToList();
                ViewBag.nb_objeto = db.k_objeto.Find(id).nb_objeto;
            }
            if (Origen == 2) //si se solicita información desde Auditoría Externa
            {
                //Generar lista de incidencias perteneciente a este informe
                Lista = db.k_incidencia.Where(i => i.id_objeto == id).ToList();
                ViewBag.nb_objeto = db.k_objeto.Find(id).nb_objeto;
            }
            if (Origen == 3) //si se solicita información desde Auditoría Interna
            {
                //Generar lista de incidencias perteneciente a este informe
                Lista = db.k_incidencia.Where(i => i.id_objeto == id).ToList();
                ViewBag.nb_objeto = db.k_objeto.Find(id).nb_objeto;
            }
            if (Origen == 4) //si se solicita información desde Certificaciones
            {
                //Generar lista de incidencias perteneciente a este informe
                Lista = db.k_incidencia.Where(i => i.id_certificacion_control == id).ToList();
                ViewBag.nb_objeto = db.k_certificacion_control.Find(id).cl_certificacion_control;
            }
            if (Origen == 5) //si se solicita información desde MRyC
            {
                //Generar lista de incidencias perteneciente a este informe
                Lista = db.k_incidencia.Where(i => i.id_control == id).ToList();
                ViewBag.nb_objeto = db.k_control.Find(id).k_riesgo.First().nb_riesgo;
            }
            if (Origen == 6) //si se solicita información desde Otros
            {
                //Generar lista de incidencias perteneciente a este informe
                Lista = db.k_incidencia.Where(i => i.id_objeto == id).ToList();
                ViewBag.nb_objeto = db.k_objeto.Find(id).nb_objeto;
            }

            return PartialView("Incidencias/Index", Lista);
        }

        public ActionResult CreateIncidencia(int Origen, int? id)
        {
            var incidencia = new k_incidencia();

            if (Origen == 1)
            {
                var oficio = db.k_objeto.Find(id);

                if (oficio.k_incidencia.Count() >= oficio.no_incidencias)
                {
                    return RedirectToAction("listaIncidencias", new { Origen, id });
                }

                ViewBag.nb_objeto = oficio.nb_objeto;
                incidencia.id_objeto = id;
            }
            if (Origen == 2)
            {
                var informe = db.k_objeto.Find(id);

                if (informe.k_incidencia.Count() >= informe.no_incidencias)
                {
                    return RedirectToAction("listaIncidencias", new { Origen, id });
                }

                ViewBag.nb_objeto = db.k_objeto.Find(id).nb_objeto;
                incidencia.id_objeto = id;
            }
            if (Origen == 3)
            {
                var informe = db.k_objeto.Find(id);

                if (informe.k_incidencia.Count() >= informe.no_incidencias)
                {
                    return RedirectToAction("listaIncidencias", new { Origen, id });
                }

                ViewBag.nb_objeto = db.k_objeto.Find(id).nb_objeto;
                incidencia.id_objeto = id;
            }
            if (Origen == 4)
            {
                ViewBag.nb_objeto = db.k_certificacion_control.Find(id).cl_certificacion_control;
                incidencia.id_certificacion_control = id;
            }
            if (Origen == 5)
            {
                ViewBag.nb_objeto = db.k_control.Find(id).k_riesgo.First().nb_riesgo;
                incidencia.id_control = id;
            }
            if (Origen == 6)
            {
                ViewBag.nb_objeto = db.k_objeto.Find(id).nb_objeto;
                incidencia.id_objeto = id;
            }

            ViewBag.id = id;
            ViewBag.Origen = Origen;

            //Objetos para rellenar Combos
            ViewBag.id_clasificacion_incidencia = Utilidades.DropDown.ClasificacionIncidencia();
            ViewBag.id_responsable = Utilidades.DropDown.Usuario();

            return PartialView("Incidencias/CreateIncidencia", incidencia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult CreateIncidencia(k_incidencia model, int origen, string nb_objeto)
        {
            int id = 0;

            if (origen == 1 || origen == 2 || origen == 3 || origen == 6)
            {
                id = (int)model.id_objeto;
            }
            if (origen == 4)
            {
                id = (int)model.id_certificacion_control;
            }
            if (origen == 5)
            {
                id = (int)model.id_control;
            }

            bool valid = validateIncidencia(ModelState, model);

            if (ModelState.IsValid && valid)
            {
                db.k_incidencia.Add(model);
                db.SaveChanges();

                Utilidades.Utilidades.TaskAsigned(model);

                return RedirectToAction("listaIncidencias", new { origen, id });
            }


            ViewBag.id = id;
            ViewBag.nb_objeto = nb_objeto;
            ViewBag.Origen = origen;
            //Objetos para rellenar Combos
            ViewBag.id_clasificacion_incidencia = Utilidades.DropDown.ClasificacionIncidencia();
            ViewBag.id_responsable = Utilidades.DropDown.Usuario();

            return PartialView("Incidencias/CreateIncidencia", model);
        }


        public ActionResult EditIncidencia(int Origen, int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_incidencia incidencia = db.k_incidencia.Find(id);
            if (incidencia == null)
            {
                return HttpNotFound();
            }


            if (Origen == 1 || Origen == 2 || Origen == 3 || Origen == 6)
            {
                ViewBag.id = incidencia.id_objeto;
                ViewBag.nb_objeto = incidencia.k_objeto.nb_objeto;
            }
            if (Origen == 4)
            {
                ViewBag.id = incidencia.id_certificacion_control;
                ViewBag.nb_objeto = incidencia.k_certificacion_control.cl_certificacion_control;
            }
            if (Origen == 5)
            {
                ViewBag.id = incidencia.id_control;
                ViewBag.nb_objeto = incidencia.k_control.k_riesgo.First().nb_riesgo;
            }

            ViewBag.Origen = Origen;

            //Objetos para rellenar Combos
            ViewBag.id_clasificacion_incidencia = Utilidades.DropDown.ClasificacionIncidencia();
            ViewBag.id_responsable = Utilidades.DropDown.Usuario();
            return PartialView("Incidencias/EditIncidencia", incidencia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult EditIncidencia(k_incidencia model, int origen, string nb_objeto, int lu)
        {
            int id = 0;

            if (origen == 1 || origen == 2 || origen == 3 || origen == 6)
            {
                id = (int)model.id_objeto;
            }
            if (origen == 4)
            {
                id = (int)model.id_certificacion_control;
            }
            if (origen == 5)
            {
                id = (int)model.id_control;
            }

            bool valid = validateIncidencia(ModelState, model);

            if (ModelState.IsValid && valid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();


                if (lu != model.id_responsable) Utilidades.Utilidades.TaskAsigned(model, lu);
                return RedirectToAction("listaIncidencias", new { origen, id });
            }

            ViewBag.id = id;
            ViewBag.nb_objeto = nb_objeto;
            ViewBag.Origen = origen;
            //Objetos para rellenar Combos
            ViewBag.id_clasificacion_incidencia = Utilidades.DropDown.ClasificacionIncidencia();
            ViewBag.id_responsable = Utilidades.DropDown.Usuario();

            return PartialView("Incidencias/EditIncidencia", model);
        }

        public ActionResult DeleteIncidencia(int Origen, int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_incidencia incidencia = db.k_incidencia.Find(id);
            if (incidencia == null)
            {
                return HttpNotFound();
            }

            if (Origen == 1 || Origen == 2 || Origen == 3 || Origen == 6)
            {
                ViewBag.id = incidencia.id_objeto;
                ViewBag.nb_objeto = incidencia.k_objeto.nb_objeto;
            }
            if (Origen == 4)
            {
                ViewBag.id = incidencia.id_certificacion_control;
                ViewBag.nb_objeto = incidencia.k_certificacion_control.cl_certificacion_control;
            }
            if (Origen == 5)
            {
                ViewBag.id = incidencia.id_control;
                ViewBag.nb_objeto = incidencia.k_control.k_riesgo.First().nb_riesgo;
            }

            ViewBag.Origen = Origen;
            return PartialView("Incidencias/DeleteIncidencia", incidencia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteIncidencia(int id, int origen)
        {

            var incidencia = db.k_incidencia.Find(id);
            int id_origen = 0;
            if (origen == 1 || origen == 2 || origen == 3 || origen == 6)
            {
                id_origen = (int)incidencia.id_objeto;
            }
            if (origen == 4)
            {
                id_origen = (int)incidencia.id_certificacion_control;
            }
            if (origen == 5)
            {
                id_origen = (int)incidencia.id_control;
            }

            Utilidades.DeleteActions.DeleteIncidenciaObjects(incidencia, db);
            db.k_incidencia.Remove(incidencia);
            db.SaveChanges();

            return RedirectToAction("listaIncidencias", new { origen, id = id_origen });
        }


        #region DeleteIncidenciaExt
        public ActionResult DeleteIncidenciaExt(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_incidencia k_incidencia = db.k_incidencia.Find(id);
            if (k_incidencia == null)
            {
                return HttpNotFound();
            }

            Utilidades.DeleteActions.checkRedirect(redirect);

            //Obtener todos los elementos a los que puede estar ligado este elemento.
            //creamos la lista que contendra a todos los registros relacionados
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;
            return View(k_incidencia);
        }


        [HttpPost, ActionName("DeleteIncidenciaExt")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteIncidenciaExt(int id)
        {
            k_incidencia k_incidencia = db.k_incidencia.Find(id);
            Utilidades.DeleteActions.DeleteIncidenciaObjects(k_incidencia, db);
            db.k_incidencia.Remove(k_incidencia);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("CantErase", "Error", null);
            }


            int ns;
            try
            {
                ns = (int)HttpContext.Session["JumpCounter"];
            }
            catch
            {
                ns = 0;
            }
            //Si ns es 0 redireccionamos al index de este controlador
            if (ns == 0)
            {
                return RedirectToAction("Index");

            }//En caso de que ns sea distinto a 0, obtenemos el Array "Directions", agregamos la direccion actual, aumentamos el contador y salvamos ambas variables globales
            else
            {
                List<string> directions = new List<string>();
                try
                {
                    directions = (List<string>)HttpContext.Session["Directions"];
                }
                catch
                {
                    directions = null;
                }

                if (directions == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    string direction = directions.Last();
                    DirectionViewModel dir = Utilidades.Utilidades.getDirection(direction);
                    //disminuimos ns y eliminamos el ultimo elemento de directions
                    ns--;
                    directions.RemoveAt(ns);

                    //Guardamos ambas variables de sesion para seguir trabajando
                    HttpContext.Session["JumpCounter"] = ns;
                    HttpContext.Session["Directions"] = directions;

                    return RedirectToAction(dir.Action, dir.Controller, new { id = dir.Id, redirect = "bfo" });
                }

            }
        }

        #endregion

        #endregion

        #region Planes

        //Se necesitara un número que indique que tipo de lista se esta solicitando
        //y el id del objeto, los origenes serán los siguientes
        //Oificios:         1
        //Aud Ext:          2
        //Aud Int:          3
        //Certificación     4
        //MRyC              5
        //Otros             6
        public ActionResult IndexPlanes(int Origen, int? id)
        {
            var incidencia = db.k_incidencia.Find(id);
            var planes = incidencia.k_plan.ToList();

            int id_objeto = 0;
            if (Origen == 1 || Origen == 2 || Origen == 3 || Origen == 6)
            {
                id_objeto = (int)incidencia.id_objeto;
                ViewBag.nb_objeto = incidencia.k_objeto.nb_objeto;
            }
            if (Origen == 4)
            {
                id_objeto = (int)incidencia.id_certificacion_control;
                ViewBag.nb_objeto = incidencia.k_certificacion_control.cl_certificacion_control;
            }
            if (Origen == 5)
            {
                id_objeto = (int)incidencia.id_control;
                ViewBag.nb_objeto = incidencia.k_control.k_riesgo.First().nb_riesgo;
            }

            ViewBag.Origen = Origen;                //Variable para saber la ruta que deberá mostrarse
            ViewBag.id_objeto = id_objeto;
            ViewBag.id_incidencia = id;

            return PartialView("Planes/Index", planes);
        }

        public ActionResult CreatePlan(int Origen, int? id)
        {
            var model = new k_plan();
            var incidencia = db.k_incidencia.Find(id);
            int id_objeto = 0;
            if (Origen == 1 || Origen == 2 || Origen == 3 || Origen == 6)
            {
                id_objeto = (int)incidencia.id_objeto;
                ViewBag.nb_objeto = incidencia.k_objeto.nb_objeto;
            }
            if (Origen == 4)
            {
                id_objeto = (int)incidencia.id_certificacion_control;
                ViewBag.nb_objeto = incidencia.k_certificacion_control.cl_certificacion_control;
            }
            if (Origen == 5)
            {
                id_objeto = (int)incidencia.id_control;
                ViewBag.nb_objeto = incidencia.k_control.k_riesgo.First().nb_riesgo;
            }

            ViewBag.id_objeto = id_objeto;
            ViewBag.id_incidencia = id;
            ViewBag.Origen = Origen;

            model.id_incidencia = (int)id;


            //Listas
            ViewBag.id_area = Utilidades.DropDown.Areas();
            ViewBag.id_responsable_seguimiento = ViewBag.id_responsable = Utilidades.DropDown.Usuario();
            return PartialView("Planes/CreatePlan", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult CreatePlan(k_plan model, int origen, string nb_objeto, int[] files)
        {
            int id = 0;
            var incidencia = db.k_incidencia.Find(model.id_incidencia);

            if (origen == 1 || origen == 2 || origen == 3 || origen == 6)
            {
                id = (int)incidencia.id_objeto;
            }
            if (origen == 4)
            {
                id = (int)incidencia.id_certificacion_control;
            }
            if (origen == 5)
            {
                id = (int)incidencia.id_control;
            }

            model.fe_alta = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.k_plan.Add(model);
                db.SaveChanges();

                Utilidades.Utilidades.TaskAsigned(model);

                model = db.k_plan.Find(model.id_plan);

                //agregar los archivos
                if (files != null)
                {
                    foreach (int file in files)
                    {
                        c_archivo archivo = db.c_archivo.Find(file);

                        model.c_archivo.Add(archivo);
                    }

                    db.SaveChanges();
                }

                return RedirectToAction("IndexPlanes", new { Origen = origen, id = model.id_incidencia });
            }

            ViewBag.id_incidencia = model.id_incidencia;
            ViewBag.nb_objeto = nb_objeto;
            ViewBag.Origen = origen;

            //Objetos para rellenar Combos
            ViewBag.id_area = Utilidades.DropDown.Areas();
            ViewBag.id_responsable_seguimiento = ViewBag.id_responsable = Utilidades.DropDown.Usuario();

            return PartialView("Planes/CreatePlan", model);
        }

        public ActionResult EditPlan(int Origen, int? id)
        {
            var model = db.k_plan.Find(id);
            var incidencia = db.k_incidencia.Find(model.id_incidencia);
            int id_objeto = 0;
            if (Origen == 1 || Origen == 2 || Origen == 3 || Origen == 6)
            {
                id_objeto = (int)incidencia.id_objeto;
                ViewBag.nb_objeto = incidencia.k_objeto.nb_objeto;
            }
            if (Origen == 4)
            {
                id_objeto = (int)incidencia.id_certificacion_control;
                ViewBag.nb_objeto = incidencia.k_certificacion_control.cl_certificacion_control;
            }
            if (Origen == 5)
            {
                id_objeto = (int)incidencia.id_control;
                ViewBag.nb_objeto = incidencia.k_control.k_riesgo.First().nb_riesgo;
            }

            ViewBag.id_objeto = id_objeto;
            ViewBag.id_incidencia = model.id_incidencia;
            ViewBag.Origen = Origen;

            //Listas
            ViewBag.id_area = Utilidades.DropDown.Areas();
            ViewBag.id_responsable_seguimiento = ViewBag.id_responsable = Utilidades.DropDown.Usuario();
            return PartialView("Planes/EditPlan", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult EditPlan(k_plan model, int origen, string nb_objeto, int lu, int[] files)
        {
            int id = 0;
            var incidencia = db.k_incidencia.Find(model.id_incidencia);

            if (origen == 1 || origen == 2 || origen == 3 || origen == 6)
            {
                id = (int)incidencia.id_objeto;
            }
            if (origen == 4)
            {
                id = (int)incidencia.id_certificacion_control;
            }
            if (origen == 5)
            {
                id = (int)incidencia.id_control;
            }

            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                if (model.id_responsable != lu) Utilidades.Utilidades.TaskAsigned(model, lu);


                model = db.k_plan.Find(model.id_plan);

                //agregar los archivos
                if (files != null)
                {
                    foreach (int file in files)
                    {
                        c_archivo archivo = db.c_archivo.Find(file);

                        model.c_archivo.Add(archivo);
                    }

                    db.SaveChanges();
                }

                return RedirectToAction("IndexPlanes", new { Origen = origen, id = model.id_incidencia });
            }

            ViewBag.id_incidencia = model.id_incidencia;
            ViewBag.nb_objeto = nb_objeto;
            ViewBag.Origen = origen;

            //Objetos para rellenar Combos
            ViewBag.id_area = Utilidades.DropDown.Areas();
            ViewBag.id_responsable_seguimiento = ViewBag.id_responsable = Utilidades.DropDown.Usuario();

            return PartialView("Planes/EditPlan", model);
        }

        public ActionResult DeletePlan(int Origen, int? id)
        {
            var model = db.k_plan.Find(id);
            var incidencia = db.k_incidencia.Find(model.id_incidencia);
            int id_objeto = 0;
            if (Origen == 1 || Origen == 2 || Origen == 3 || Origen == 6)
            {
                id_objeto = (int)incidencia.id_objeto;
                ViewBag.nb_objeto = incidencia.k_objeto.nb_objeto;
            }
            if (Origen == 4)
            {
                id_objeto = (int)incidencia.id_certificacion_control;
                ViewBag.nb_objeto = incidencia.k_certificacion_control.cl_certificacion_control;
            }
            if (Origen == 5)
            {
                id_objeto = (int)incidencia.id_control;
                ViewBag.nb_objeto = incidencia.k_control.k_riesgo.First().nb_riesgo;
            }

            ViewBag.id_objeto = id_objeto;
            ViewBag.id_incidencia = model.id_incidencia;
            ViewBag.Origen = Origen;

            return PartialView("Planes/DeletePlan", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeletePlan(int id, int origen)
        {

            var plan = db.k_plan.Find(id);
            int id_incidencia = plan.id_incidencia;

            Utilidades.DeleteActions.DeletePlanRemediacionObjects(plan, db);
            db.k_plan.Remove(plan);
            db.SaveChanges();

            return RedirectToAction("IndexPlanes", new { origen, id = id_incidencia });
        }
        #endregion

        #region Seguimiento
        public ActionResult IndexSeguimiento(int Origen, int? id)
        {
            var plan = db.k_plan.Find(id);
            var incidencia = plan.k_incidencia;

            var seguimientos = plan.r_seguimiento.ToList();

            int id_objeto = 0;
            if (Origen == 1 || Origen == 2 || Origen == 3 || Origen == 6)
            {
                id_objeto = (int)incidencia.id_objeto;
                ViewBag.nb_objeto = incidencia.k_objeto.nb_objeto;
            }
            if (Origen == 4)
            {
                id_objeto = (int)incidencia.id_certificacion_control;
                ViewBag.nb_objeto = incidencia.k_certificacion_control.cl_certificacion_control;
            }
            if (Origen == 5)
            {
                id_objeto = (int)incidencia.id_control;
                ViewBag.nb_objeto = incidencia.k_control.k_riesgo.First().nb_riesgo;
            }
            ViewBag.id_objeto = id_objeto;
            ViewBag.Origen = Origen;
            ViewBag.id_incidencia = incidencia.id_incidencia;
            ViewBag.nb_plan = plan.nb_plan;
            ViewBag.id_plan = plan.id_plan;


            return PartialView("Seguimiento/IndexSeguimiento", seguimientos);
        }

        public ActionResult CreateSeguimiento(int Origen, int? id)
        {
            var plan = db.k_plan.Find(id);
            var incidencia = plan.k_incidencia;

            int id_objeto = 0;
            if (Origen == 1 || Origen == 2 || Origen == 3 || Origen == 6)
            {
                id_objeto = (int)incidencia.id_objeto;
                ViewBag.nb_objeto = incidencia.k_objeto.nb_objeto;
            }
            if (Origen == 4)
            {
                id_objeto = (int)incidencia.id_certificacion_control;
                ViewBag.nb_objeto = incidencia.k_certificacion_control.cl_certificacion_control;
            }
            if (Origen == 5)
            {
                id_objeto = (int)incidencia.id_control;
                ViewBag.nb_objeto = incidencia.k_control.k_riesgo.First().nb_riesgo;
            }
            ViewBag.id_objeto = id_objeto;
            ViewBag.Origen = Origen;
            ViewBag.id_incidencia = incidencia.id_incidencia;
            ViewBag.nb_plan = plan.nb_plan;
            ViewBag.id_plan = plan.id_plan;

            var model = new r_seguimiento();
            model.id_plan = plan.id_plan;

            return PartialView("Seguimiento/CreateSeguimiento", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult CreateSeguimiento(r_seguimiento model, int origen, string nb_objeto)
        {
            var plan = db.k_plan.Find(model.id_plan);
            var incidencia = plan.k_incidencia;

            model.fe_seguimiento = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.r_seguimiento.Add(model);
                db.SaveChanges();
                return RedirectToAction("IndexSeguimiento", new { Origen = origen, id = model.id_plan });
            }

            int id_objeto = 0;
            if (origen == 1 || origen == 2 || origen == 3 || origen == 6)
            {
                id_objeto = (int)incidencia.id_objeto;
            }
            if (origen == 4)
            {
                id_objeto = (int)incidencia.id_certificacion_control;
            }
            if (origen == 5)
            {
                id_objeto = (int)incidencia.id_control;
            }

            ViewBag.nb_objeto = nb_objeto;
            ViewBag.id_objeto = id_objeto;
            ViewBag.Origen = origen;
            ViewBag.id_incidencia = incidencia.id_incidencia;
            ViewBag.nb_plan = plan.nb_plan;
            ViewBag.id_plan = plan.id_plan;


            return PartialView("Seguimiento/CreateSeguimiento", model);
        }

        public ActionResult EditSeguimiento(int Origen, int? id)
        {
            var model = db.r_seguimiento.Find(id);
            var plan = model.k_plan;
            var incidencia = plan.k_incidencia;

            int id_objeto = 0;
            if (Origen == 1 || Origen == 2 || Origen == 3 || Origen == 6)
            {
                id_objeto = (int)incidencia.id_objeto;
                ViewBag.nb_objeto = incidencia.k_objeto.nb_objeto;
            }
            if (Origen == 4)
            {
                id_objeto = (int)incidencia.id_certificacion_control;
                ViewBag.nb_objeto = incidencia.k_certificacion_control.cl_certificacion_control;
            }
            if (Origen == 5)
            {
                id_objeto = (int)incidencia.id_control;
                ViewBag.nb_objeto = incidencia.k_control.k_riesgo.First().nb_riesgo;
            }
            ViewBag.id_objeto = id_objeto;
            ViewBag.Origen = Origen;
            ViewBag.id_incidencia = incidencia.id_incidencia;
            ViewBag.nb_plan = plan.nb_plan;
            ViewBag.id_plan = plan.id_plan;

            return PartialView("Seguimiento/EditSeguimiento", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult EditSeguimiento(r_seguimiento model, int origen, string nb_objeto)
        {
            var plan = db.k_plan.Find(model.id_plan);
            var incidencia = plan.k_incidencia;

            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexSeguimiento", new { Origen = origen, id = model.id_plan });
            }

            int id_objeto = 0;
            if (origen == 1 || origen == 2 || origen == 3 || origen == 6)
            {
                id_objeto = (int)incidencia.id_objeto;
            }
            if (origen == 4)
            {
                id_objeto = (int)incidencia.id_certificacion_control;
            }
            if (origen == 5)
            {
                id_objeto = (int)incidencia.id_control;
            }

            ViewBag.nb_objeto = nb_objeto;
            ViewBag.id_objeto = id_objeto;
            ViewBag.Origen = origen;
            ViewBag.id_incidencia = incidencia.id_incidencia;
            ViewBag.nb_plan = plan.nb_plan;
            ViewBag.id_plan = plan.id_plan;


            return PartialView("Seguimiento/EditSeguimiento", model);
        }

        public ActionResult DeleteSeguimiento(int Origen, int? id)
        {
            var model = db.r_seguimiento.Find(id);
            var plan = model.k_plan;
            var incidencia = plan.k_incidencia;

            int id_objeto = 0;
            if (Origen == 1 || Origen == 2 || Origen == 3 || Origen == 6)
            {
                id_objeto = (int)incidencia.id_objeto;
                ViewBag.nb_objeto = incidencia.k_objeto.nb_objeto;
            }
            if (Origen == 4)
            {
                id_objeto = (int)incidencia.id_certificacion_control;
                ViewBag.nb_objeto = incidencia.k_certificacion_control.cl_certificacion_control;
            }
            if (Origen == 5)
            {
                id_objeto = (int)incidencia.id_control;
                ViewBag.nb_objeto = incidencia.k_control.k_riesgo.First().nb_riesgo;
            }
            ViewBag.id_objeto = id_objeto;
            ViewBag.Origen = Origen;
            ViewBag.id_incidencia = incidencia.id_incidencia;
            ViewBag.nb_plan = plan.nb_plan;
            ViewBag.id_plan = plan.id_plan;

            return PartialView("Seguimiento/DeleteSeguimiento", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteSeguimiento(int id, int origen)
        {
            var seguimiento = db.r_seguimiento.Find(id);
            int id_plan = seguimiento.id_plan;

            db.r_seguimiento.Remove(seguimiento);
            db.SaveChanges();

            return RedirectToAction("IndexSeguimiento", new { origen, id = id_plan });
        }
        #endregion

        #region Validaciones
        private bool validateObject(ModelStateDictionary modelState, k_objeto objeto)
        {
            bool result = true;

            if (objeto.tipo_objeto == 1 || objeto.tipo_objeto == 2 || objeto.tipo_objeto == 3)
            {
                var incidenciasExistentes = db.k_incidencia.Where(i => i.id_objeto == objeto.id_objeto).ToList().Count();
                if (objeto.no_incidencias < incidenciasExistentes)
                {
                    ModelState.AddModelError("no_incidencias", String.Format("Ya existen {0} incidencias ligadas a este objeto.", incidenciasExistentes));
                    result = false;
                }
                if (objeto.no_incidencias > incidenciasExistentes)
                {
                    var contestaciones = db.r_contestacion_oficio.Where(c => c.id_objeto == objeto.id_objeto).ToList();
                    foreach (var contestacion in contestaciones)
                    {
                        db.r_contestacion_oficio.Remove(contestacion);
                    }
                    objeto.fe_contestacion = null;
                }
            }

            switch (objeto.tipo_objeto)
            {
                case 1:
                    if (objeto.fe_alta == null)
                    {
                        ModelState.AddModelError("fe_alta", Strings.getMSG("lyPCreate020"));
                        result = false;
                    }
                    if (objeto.fe_vencimiento == null)
                    {
                        ModelState.AddModelError("fe_vencimiento", Strings.getMSG("lyPCreate021"));
                        result = false;
                    }
                    if (objeto.id_autoridad == null)
                    {
                        ModelState.AddModelError("id_autoridad", Strings.getMSG("lyPCreate018"));
                        result = false;
                    }
                    if (objeto.id_responsable == null)
                    {
                        ModelState.AddModelError("id_responsable", Strings.getMSG("lyPCreate019"));
                        result = false;
                    }
                    if (objeto.no_incidencias == null)
                    {
                        ModelState.AddModelError("no_incidencias", Strings.getMSG("lyPCreate022"));
                        result = false;
                    }

                    break;
                case 2:
                    if (objeto.fe_alta == null)
                    {
                        ModelState.AddModelError("fe_alta", Strings.getMSG("lyPInformesCreate003"));
                        result = false;
                    }
                    if (objeto.id_responsable == null)
                    {
                        ModelState.AddModelError("id_responsable", Strings.getMSG("lyPInformesCreate002"));
                        result = false;
                    }
                    if (objeto.no_incidencias == null)
                    {
                        ModelState.AddModelError("no_incidencias", Strings.getMSG("lyPCreate022"));
                        result = false;
                    }
                    break;
                case 3:
                    if (objeto.fe_alta == null)
                    {
                        ModelState.AddModelError("fe_alta", Strings.getMSG("lyPInformesCreate003"));
                        result = false;
                    }
                    if (objeto.id_responsable == null)
                    {
                        ModelState.AddModelError("id_responsable", Strings.getMSG("lyPInformesCreate002"));
                        result = false;
                    }
                    if (objeto.no_incidencias == null)
                    {
                        ModelState.AddModelError("no_incidencias", Strings.getMSG("lyPCreate022"));
                        result = false;
                    }
                    break;
                case 6:
                    if (objeto.ds_objeto == null)
                    {
                        ModelState.AddModelError("ds_objeto", Strings.getMSG("CanalRiesgoOperacionalCreate004"));
                        result = false;
                    }
                    break;
                default:
                    break;
            }


            return result;
        }

        private bool validateIncidencia(ModelStateDictionary modelState, k_incidencia incidencia)
        {
            bool result = true;
            if (!incidencia.requiere_plan)
            {
                if (incidencia.js_incidencia == null)
                {
                    modelState.AddModelError("js_incidencia", Strings.getMSG("lyPIncidenciasCreate002"));
                    result = false;
                }
            }
            else
            {
                incidencia.js_incidencia = null;
            }
            return result;
        }

        #endregion

        #region Otros
        private List<k_objeto> getRO(k_objeto oficio)
        {
            //se tienen que buscar todos los oficios relacionados hasta que no se tenga un padre
            //se tienen que excluir los que se encuentren en la lista id_r
            //se tienen que agregar a la lista id_r los que vayan entrando
            List<k_objeto> oficiosR = new List<k_objeto>();
            List<k_objeto> oficiosRD = oficio.k_objeto2.ToList();

            foreach (var or in oficiosRD)
            {
                List<k_objeto> lor2 = new List<k_objeto>();
                if (!id_r.Contains(or.id_objeto))
                {
                    oficiosR.Add(or);
                    id_r.Add(or.id_objeto);

                    lor2 = getRO(or);

                    foreach (var o in lor2)
                    {
                        if (!oficiosR.Any(ob => ob.id_objeto == o.id_objeto))
                        {
                            oficiosR.Add(o);
                        }
                    }
                }
            }
            return oficiosR;
        }

        private List<k_objeto> getROD(k_objeto oficio)
        {
            //se tienen que buscar todos los oficios relacionados hasta que no se tenga un padre
            //se tienen que excluir los que se encuentren en la lista id_r
            //se tienen que agregar a la lista id_r los que vayan entrando
            List<k_objeto> oficiosR = new List<k_objeto>();
            List<k_objeto> oficiosRD = oficio.k_objeto1.ToList();

            foreach (var or in oficiosRD)
            {
                List<k_objeto> lor2 = new List<k_objeto>();
                if (!id_r.Contains(or.id_objeto))
                {
                    oficiosR.Add(or);
                    id_r.Add(or.id_objeto);

                    lor2 = getROD(or);

                    foreach (var o in lor2)
                    {
                        if (!oficiosR.Any(ob => ob.id_objeto == o.id_objeto))
                        {
                            oficiosR.Add(o);
                        }
                    }
                }
            }
            return oficiosR;
        }

        private bool SaveFiles(k_objeto objeto, HttpPostedFileBase file = null, HttpPostedFileBase file2 = null, bool edit = false)
        {
            Type m_tipo = objeto.GetType();
            PropertyInfo[] m_propiedades = m_tipo.GetProperties();
            HttpPostedFileBase[] files = { file, file2 };

            for (int i = 1; i < 3; i++)
            {
                string nombre = "a" + i + "-" + objeto.id_objeto;
                var prop = m_propiedades.Where(p => p.Name == "nb_archivo_" + i).First();
                if (files[i - 1] != null)
                {
                    files[i - 1].SaveAs(Server.MapPath("~/App_Data/Informes-Oficios/" + nombre));
                    prop.SetValue(objeto, files[i - 1].FileName);
                }
                else
                {
                    if (edit)
                    {
                        if (prop.GetValue(objeto, null) == null)
                        {
                            string path = Server.MapPath("~/App_Data/Informes-Oficios/" + nombre);
                            System.IO.File.Delete(path);
                        }
                    }
                }
            }
            db.SaveChanges();
            return true;
        }

        [NotOnlyRead]
        public ActionResult DescargaArchivo(int id, int index)
        {
            k_objeto objeto = db.k_objeto.Find(id);
            Type m_tipo = objeto.GetType();
            PropertyInfo[] m_propiedades = m_tipo.GetProperties();

            var prop = m_propiedades.Where(p => p.Name == "nb_archivo_" + index).First();

            string contentType = System.Net.Mime.MediaTypeNames.Application.Octet;
            return new FilePathResult("~/App_Data/Informes-Oficios/a" + index + "-" + id, contentType)
            {
                FileDownloadName = prop.GetValue(objeto, null).ToString(),
            };
        }

        public FileResult DisplayInforme(int id)
        {
            //Obtenemos las incidencias ligadas al objeto así como su contestación
            var objeto = db.k_objeto.Find(id);
            var contestacion = objeto.r_contestacion_oficio.First();
            var incidencias = objeto.k_incidencia.ToList();


            //--------------------------------    Crear PDF    ------------------------------

            MemoryStream os = new MemoryStream();


            //Configuración del 
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, os);
            doc.Open();
            Rectangle rec2 = new Rectangle(PageSize.A4);
            doc.SetPageSize(rec2);


            Font fontH1 = new Font(null, 10, Font.BOLD);
            Font fontH1B = new Font(null, 10, Font.BOLD, BaseColor.WHITE);
            Font fontC1 = new Font(null, 10, Font.NORMAL);
            Font font = new Font(FontFamily.HELVETICA, 14, Font.BOLD, BaseColor.WHITE);
            Font fontC = new Font(FontFamily.HELVETICA, 14, Font.BOLD);


            Paragraph nb_objeto = new Paragraph(objeto.tipo_objeto == 1 ? "Oficio: " + objeto.nb_objeto : "Informe: " + objeto.nb_objeto, fontC);
            nb_objeto.Alignment = Element.ALIGN_CENTER;
            Paragraph intro = new Paragraph(contestacion.inicio_parrafo, fontC1);
            intro.Alignment = Element.ALIGN_JUSTIFIED;
            Paragraph fin = new Paragraph(contestacion.final_parrafo, fontC1);
            fin.Alignment = Element.ALIGN_JUSTIFIED;


            doc.Add(nb_objeto);
            doc.Add(new Paragraph(".", font));
            doc.Add(intro);
            doc.Add(new Paragraph(".", font));

            int counter = 1;
            foreach (var incidencia in incidencias)
            {
                var respuesta = incidencia.r_respuesta.First();
                var obs = "Incidencia " + (counter++) + ": " + incidencia.ds_incidencia;
                var resp = "Respuesta: " + respuesta.contestacion;

                Paragraph obsP = new Paragraph(obs, fontC1);
                Paragraph respP = new Paragraph(resp, fontC1);
                obsP.Alignment = Element.ALIGN_JUSTIFIED;
                respP.Alignment = Element.ALIGN_JUSTIFIED;
                doc.Add(obsP);
                doc.Add(respP);
                doc.Add(new Paragraph(".", font));
            }

            doc.Add(fin);
            doc.Close();
            return File(os.GetBuffer(), "application/pdf", "Informe Oficio " + objeto.nb_objeto + ".pdf");
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
