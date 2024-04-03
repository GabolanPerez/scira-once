using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "BDEI", ModuleCode = "MSICI005")]
    [CustomErrorHandler]
    public class BDEIController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            return View(db.k_bdei.ToList());
        }

        // GET: BDEI/Create
        // Agrega un riesgo asociado a un subproceso que se envia como parámetro
        public ActionResult Create()
        {


            //Entrega de listas a la vista
            ViewBag.id_entidadL = Utilidades.DropDown.Entidades();
            ViewBag.id_estatus_bdeiL = Utilidades.DropDown.EstatusBDEI();
            ViewBag.id_sub_procesoL = Utilidades.DropDown.SubProcesos();
            ViewBag.id_tipo_solucionL = Utilidades.DropDown.TipoSolucion();
            ViewBag.id_monedaL = Utilidades.DropDown.Moneda();
            ViewBag.id_cuenta_contable_perdidaL = new List<SelectListItem>();
            ViewBag.id_cuenta_contable_costoL = new List<SelectListItem>();
            ViewBag.id_cuenta_contable_recuperacionL = new List<SelectListItem>();
            ViewBag.id_centro_costoL = Utilidades.DropDown.CentroCosto();
            ViewBag.id_responsable_recuperacionL = Utilidades.DropDown.Usuario();
            ViewBag.id_tipo_riesgo_operacionalL = Utilidades.DropDown.TipoRiesgoOperacional();
            ViewBag.id_ambito_riesgo_operacionalL = Utilidades.DropDown.AmbitoRiesgoOperacional();
            ViewBag.id_producto_riesgo_operacionalL = Utilidades.DropDown.ProductoRiesgoOperacional();
            ViewBag.id_canal_riesgo_operacionalL = Utilidades.DropDown.CanalRiesgoOperacional();
            ViewBag.id_categoria_linea_negocio_riesgo_operacionalL = Utilidades.DropDown.CategoriaLineaNegocioRiesgoOperacional();
            ViewBag.id_frecuencia_riesgo_operacionalL = Utilidades.DropDown.FrecuenciaRiesgoOperacional();
            ViewBag.id_impacto_riesgo_operacionalL = Utilidades.DropDown.ImpactoRiesgoOperacional();
            ViewBag.id_riesgo_asociado_bdeiL = Utilidades.DropDown.RiesgoAsociadoBDEI();
            //ViewBag.id_minimo_riesgo_operativoL = Utilidades.DropDown.MinimoRiesgoOperativo();
            ViewBag.id_causa_bdeiL = Utilidades.DropDown.CausaBDEI();
            ViewBag.id_catalogo_conceptoL = Utilidades.DropDown.CatalogoConceptosBDEI();


            Utilidades.Utilidades.GetDateFormat();

            //Modelo vacio para evitar errores de nullidad en las operaciones con el modelo
            ViewBag.Model = "null";
            k_bdei model = new k_bdei();

            var maxEvS = db.k_bdei.Max(r => r.no_evento_sencillo);
            ViewBag.UltimoNumeroEventoSencillo = maxEvS;

            return View(model);
        }

        // POST: BDEI/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create(k_bdei k_bdei)
        {
            IdentityPersonalizado ident = (IdentityPersonalizado)ControllerContext.HttpContext.User.Identity;

            k_bdei.id_responsable_captura = ident.Id_usuario;
            k_bdei.fe_alta = System.DateTime.Now;

            if (k_bdei.id_sub_proceso == 0) k_bdei.id_sub_proceso = null;
            if (k_bdei.id_responsable_recuperacion == 0) k_bdei.id_responsable_recuperacion = null;
            if (k_bdei.id_cuenta_contable_recuperacion == 0) k_bdei.id_cuenta_contable_recuperacion = null;
            if (k_bdei.id_cuenta_contable_costo == 0) k_bdei.id_cuenta_contable_costo = null;


            if (ModelState.IsValid)
            {
                db.k_bdei.Add(k_bdei);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            int TRO = 0, STRO = 0, CERO = k_bdei.id_clase_evento ?? 0;
            int ARO = 0, PRO = k_bdei.id_proceso_riesgo_operacional ?? 0;
            int PRRO = 0, STPRRO = k_bdei.id_sub_tipo_producto_riesgo_operacional ?? 0;
            int CLNRO = 0, LNRO = k_bdei.id_linea_negocio_riesgo_operacional ?? 0;

            if (CERO != 0)
            {
                var cero = db.c_clase_evento.Find(CERO);
                var stro = cero.c_sub_tipo_riesgo_operacional;
                STRO = stro.id_sub_tipo_riesgo_operacional;
                TRO = stro.id_tipo_riesgo_operacional;

                ViewBag.id_clase_eventoL = Utilidades.DropDown.ClaseEventoRiesgoOperacional(STRO, CERO);
                ViewBag.id_sub_tipo_riesgo_operacionalL = Utilidades.DropDown.SubTipoRiesgoOperacional(TRO, STRO);
            }

            if (PRO != 0)
            {
                var pro = db.c_proceso_riesgo_operacional.Find(PRO);
                ARO = pro.id_ambito_riesgo_operacional;

                ViewBag.id_proceso_riesgo_operacionalL = Utilidades.DropDown.ProcesoRiesgoOperacional(ARO, PRO);
            }

            if (STPRRO != 0)
            {
                var stprro = db.c_sub_tipo_producto_riesgo_operacional.Find(STPRRO);
                PRRO = stprro.id_producto_riesgo_operacional;

                ViewBag.id_sub_tipo_producto_riesgo_operacionalL = Utilidades.DropDown.SubTipoProductoRiesgoOperacional(PRRO, STPRRO);
            }

            if (LNRO != 0)
            {
                var lnro = db.c_linea_negocio_riesgo_operacional.Find(LNRO);
                CLNRO = lnro.id_categoria_linea_negocio_riesgo_operacional;

                ViewBag.id_linea_negocio_riesgo_operacionalL = Utilidades.DropDown.LineaNegocioRiesgoOperacional(CLNRO, LNRO);
            }


            //declaracion de todas las listas seleccionables
            ViewBag.id_entidadL = Utilidades.DropDown.Entidades(k_bdei.id_entidad);
            ViewBag.id_estatus_bdeiL = Utilidades.DropDown.EstatusBDEI(k_bdei.id_estatus_bdei);
            ViewBag.id_sub_procesoL = Utilidades.DropDown.SubProcesos(k_bdei.id_sub_proceso ?? 0);
            ViewBag.id_tipo_solucionL = Utilidades.DropDown.TipoSolucion(k_bdei.id_tipo_solucion ?? 0);
            ViewBag.id_monedaL = Utilidades.DropDown.Moneda(k_bdei.id_moneda ?? 0);


            if (k_bdei.id_entidad != 0)
            {
                ViewBag.id_cuenta_contable_perdidaL = Utilidades.DropDown.CuentaContable(k_bdei.id_entidad, k_bdei.id_cuenta_contable_perdida ?? 0);
                ViewBag.id_cuenta_contable_costoL = Utilidades.DropDown.CuentaContable(k_bdei.id_entidad, k_bdei.id_cuenta_contable_costo ?? 0);
                ViewBag.id_cuenta_contable_recuperacionL = Utilidades.DropDown.CuentaContable(k_bdei.id_entidad, k_bdei.id_cuenta_contable_recuperacion ?? 0);
            }
            else
            {
                ViewBag.id_cuenta_contable_perdidaL = new List<SelectListItem>();
                ViewBag.id_cuenta_contable_costoL = new List<SelectListItem>();
                ViewBag.id_cuenta_contable_recuperacionL = new List<SelectListItem>();
            }



            ViewBag.id_centro_costoL = Utilidades.DropDown.CentroCosto(k_bdei.id_centro_costo ?? 0);
            ViewBag.id_responsable_recuperacionL = Utilidades.DropDown.Usuario(k_bdei.id_responsable_recuperacion ?? 0);
            ViewBag.id_canal_riesgo_operacionalL = Utilidades.DropDown.CanalRiesgoOperacional(k_bdei.id_canal_riesgo_operacional ?? 0);
            ViewBag.id_frecuencia_riesgo_operacionalL = Utilidades.DropDown.FrecuenciaRiesgoOperacional(k_bdei.id_frecuencia_riesgo_operacional ?? 0);
            ViewBag.id_impacto_riesgo_operacionalL = Utilidades.DropDown.ImpactoRiesgoOperacional(k_bdei.id_impacto_riesgo_operacional ?? 0);

            ViewBag.id_tipo_riesgo_operacionalL = Utilidades.DropDown.TipoRiesgoOperacional(TRO);
            ViewBag.id_ambito_riesgo_operacionalL = Utilidades.DropDown.AmbitoRiesgoOperacional(ARO);
            ViewBag.id_producto_riesgo_operacionalL = Utilidades.DropDown.ProductoRiesgoOperacional(PRRO);
            ViewBag.id_categoria_linea_negocio_riesgo_operacionalL = Utilidades.DropDown.CategoriaLineaNegocioRiesgoOperacional(CLNRO);


            ViewBag.id_riesgo_asociado_bdeiL = Utilidades.DropDown.RiesgoAsociadoBDEI(k_bdei.id_riesgo_asociado_bdei ?? 0);
            //ViewBag.id_minimo_riesgo_operativoL = Utilidades.DropDown.MinimoRiesgoOperativo(k_bdei.id_minimo_riesgo_operativo ?? 0);
            ViewBag.id_causa_bdeiL = Utilidades.DropDown.CausaBDEI(k_bdei.id_causa_bdei ?? 0);
            ViewBag.id_catalogo_conceptoL = Utilidades.DropDown.CatalogoConceptosBDEI(k_bdei.id_catalogo_concepto);

            Utilidades.Utilidades.GetDateFormat();

            var maxEvS = db.k_bdei.Max(r => r.no_evento_sencillo);
            ViewBag.UltimoNumeroEventoSencillo = maxEvS;

            return View(k_bdei);
        }


        // Edit/5  GET
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_bdei k_bdei = db.k_bdei.Find(id);
            if (k_bdei == null)
            {
                return HttpNotFound();
            }

            int TRO = 0, STRO = 0, CERO = k_bdei.id_clase_evento ?? 0;
            int ARO = 0, PRO = k_bdei.id_proceso_riesgo_operacional ?? 0;
            int PRRO = 0, STPRRO = k_bdei.id_sub_tipo_producto_riesgo_operacional ?? 0;
            int CLNRO = 0, LNRO = k_bdei.id_linea_negocio_riesgo_operacional ?? 0;

            if (CERO != 0)
            {
                var cero = db.c_clase_evento.Find(CERO);
                var stro = cero.c_sub_tipo_riesgo_operacional;
                STRO = stro.id_sub_tipo_riesgo_operacional;
                TRO = stro.id_tipo_riesgo_operacional;

                ViewBag.id_clase_eventoL = Utilidades.DropDown.ClaseEventoRiesgoOperacional(STRO, CERO);
                ViewBag.id_sub_tipo_riesgo_operacionalL = Utilidades.DropDown.SubTipoRiesgoOperacional(TRO, STRO);
            }

            if (PRO != 0)
            {
                var pro = db.c_proceso_riesgo_operacional.Find(PRO);
                ARO = pro.id_ambito_riesgo_operacional;

                ViewBag.id_proceso_riesgo_operacionalL = Utilidades.DropDown.ProcesoRiesgoOperacional(ARO, PRO);
            }

            if (STPRRO != 0)
            {
                var stprro = db.c_sub_tipo_producto_riesgo_operacional.Find(STPRRO);
                PRRO = stprro.id_producto_riesgo_operacional;

                ViewBag.id_sub_tipo_producto_riesgo_operacionalL = Utilidades.DropDown.SubTipoProductoRiesgoOperacional(PRRO, STPRRO);
            }

            if (LNRO != 0)
            {
                var lnro = db.c_linea_negocio_riesgo_operacional.Find(LNRO);
                CLNRO = lnro.id_categoria_linea_negocio_riesgo_operacional;

                ViewBag.id_linea_negocio_riesgo_operacionalL = Utilidades.DropDown.LineaNegocioRiesgoOperacional(CLNRO, LNRO);
            }


            //declaracion de todas las listas seleccionables
            ViewBag.id_entidadL = Utilidades.DropDown.Entidades(k_bdei.id_entidad);
            ViewBag.id_estatus_bdeiL = Utilidades.DropDown.EstatusBDEI(k_bdei.id_estatus_bdei);
            ViewBag.id_sub_procesoL = Utilidades.DropDown.SubProcesos(k_bdei.id_sub_proceso ?? 0);
            ViewBag.id_tipo_solucionL = Utilidades.DropDown.TipoSolucion(k_bdei.id_tipo_solucion ?? 0);
            ViewBag.id_monedaL = Utilidades.DropDown.Moneda(k_bdei.id_moneda ?? 0);
            ViewBag.id_cuenta_contable_perdidaL = Utilidades.DropDown.CuentaContable(k_bdei.id_entidad, k_bdei.id_cuenta_contable_perdida ?? 0);
            ViewBag.id_cuenta_contable_costoL = Utilidades.DropDown.CuentaContable(k_bdei.id_entidad, k_bdei.id_cuenta_contable_costo ?? 0);
            ViewBag.id_cuenta_contable_recuperacionL = Utilidades.DropDown.CuentaContable(k_bdei.id_entidad, k_bdei.id_cuenta_contable_recuperacion ?? 0);
            ViewBag.id_centro_costoL = Utilidades.DropDown.CentroCosto(k_bdei.id_centro_costo ?? 0);
            ViewBag.id_responsable_recuperacionL = Utilidades.DropDown.Usuario(k_bdei.id_responsable_recuperacion ?? 0);
            ViewBag.id_canal_riesgo_operacionalL = Utilidades.DropDown.CanalRiesgoOperacional(k_bdei.id_canal_riesgo_operacional ?? 0);
            ViewBag.id_frecuencia_riesgo_operacionalL = Utilidades.DropDown.FrecuenciaRiesgoOperacional(k_bdei.id_frecuencia_riesgo_operacional ?? 0);
            ViewBag.id_impacto_riesgo_operacionalL = Utilidades.DropDown.ImpactoRiesgoOperacional(k_bdei.id_impacto_riesgo_operacional ?? 0);

            ViewBag.id_tipo_riesgo_operacionalL = Utilidades.DropDown.TipoRiesgoOperacional(TRO);
            ViewBag.id_ambito_riesgo_operacionalL = Utilidades.DropDown.AmbitoRiesgoOperacional(ARO);
            ViewBag.id_producto_riesgo_operacionalL = Utilidades.DropDown.ProductoRiesgoOperacional(PRRO);
            ViewBag.id_categoria_linea_negocio_riesgo_operacionalL = Utilidades.DropDown.CategoriaLineaNegocioRiesgoOperacional(CLNRO);

            ViewBag.id_riesgo_asociado_bdeiL = Utilidades.DropDown.RiesgoAsociadoBDEI(k_bdei.id_riesgo_asociado_bdei ?? 0);
            ViewBag.id_minimo_riesgo_operativoL = Utilidades.DropDown.MinimoRiesgoOperativo(k_bdei.id_minimo_riesgo_operativo ?? 0);
            ViewBag.id_causa_bdeiL = Utilidades.DropDown.CausaBDEI(k_bdei.id_causa_bdei ?? 0);
            ViewBag.id_catalogo_conceptoL = Utilidades.DropDown.CatalogoConceptosBDEI(k_bdei.id_catalogo_concepto);


            Utilidades.Utilidades.GetDateFormat();


            return View(k_bdei);
        }

        // POST: BDEI/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(k_bdei k_bdei)
        {

            if (k_bdei.id_sub_proceso == 0) k_bdei.id_sub_proceso = null;
            //if (k_bdei.id_linea_negocio == 0) k_bdei.id_linea_negocio = null;
            if (k_bdei.id_responsable_recuperacion == 0) k_bdei.id_responsable_recuperacion = null;
            if (k_bdei.id_cuenta_contable_recuperacion == 0) k_bdei.id_cuenta_contable_recuperacion = null;
            if (k_bdei.id_cuenta_contable_costo == 0) k_bdei.id_cuenta_contable_costo = null;


            if (ModelState.IsValid)
            {
                db.Entry(k_bdei).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            int TRO = 0, STRO = 0, CERO = k_bdei.id_clase_evento ?? 0;
            int ARO = 0, PRO = k_bdei.id_proceso_riesgo_operacional ?? 0;
            int PRRO = 0, STPRRO = k_bdei.id_sub_tipo_producto_riesgo_operacional ?? 0;
            int CLNRO = 0, LNRO = k_bdei.id_linea_negocio_riesgo_operacional ?? 0;

            if (CERO != 0)
            {
                var cero = db.c_clase_evento.Find(CERO);
                var stro = cero.c_sub_tipo_riesgo_operacional;
                STRO = stro.id_sub_tipo_riesgo_operacional;
                TRO = stro.id_tipo_riesgo_operacional;

                ViewBag.id_clase_eventoL = Utilidades.DropDown.ClaseEventoRiesgoOperacional(STRO, CERO);
                ViewBag.id_sub_tipo_riesgo_operacionalL = Utilidades.DropDown.SubTipoRiesgoOperacional(TRO, STRO);
            }

            if (PRO != 0)
            {
                var pro = db.c_proceso_riesgo_operacional.Find(PRO);
                ARO = pro.id_ambito_riesgo_operacional;

                ViewBag.id_proceso_riesgo_operacionalL = Utilidades.DropDown.ProcesoRiesgoOperacional(ARO, PRO);
            }

            if (STPRRO != 0)
            {
                var stprro = db.c_sub_tipo_producto_riesgo_operacional.Find(STPRRO);
                PRRO = stprro.id_producto_riesgo_operacional;

                ViewBag.id_sub_tipo_producto_riesgo_operacionalL = Utilidades.DropDown.SubTipoProductoRiesgoOperacional(PRRO, STPRRO);
            }

            if (LNRO != 0)
            {
                var lnro = db.c_linea_negocio_riesgo_operacional.Find(LNRO);
                CLNRO = lnro.id_categoria_linea_negocio_riesgo_operacional;

                ViewBag.id_linea_negocio_riesgo_operacionalL = Utilidades.DropDown.LineaNegocioRiesgoOperacional(CLNRO, LNRO);
            }


            //declaracion de todas las listas seleccionables
            ViewBag.id_entidadL = Utilidades.DropDown.Entidades(k_bdei.id_entidad);
            ViewBag.id_estatus_bdeiL = Utilidades.DropDown.EstatusBDEI(k_bdei.id_estatus_bdei);
            ViewBag.id_sub_procesoL = Utilidades.DropDown.SubProcesos(k_bdei.id_sub_proceso ?? 0);
            ViewBag.id_tipo_solucionL = Utilidades.DropDown.TipoSolucion(k_bdei.id_tipo_solucion ?? 0);
            ViewBag.id_monedaL = Utilidades.DropDown.Moneda(k_bdei.id_moneda ?? 0);
            if (k_bdei.id_entidad != 0)
            {
                ViewBag.id_cuenta_contable_perdidaL = Utilidades.DropDown.CuentaContable(k_bdei.id_entidad, k_bdei.id_cuenta_contable_perdida ?? 0);
                ViewBag.id_cuenta_contable_costoL = Utilidades.DropDown.CuentaContable(k_bdei.id_entidad, k_bdei.id_cuenta_contable_costo ?? 0);
                ViewBag.id_cuenta_contable_recuperacionL = Utilidades.DropDown.CuentaContable(k_bdei.id_entidad, k_bdei.id_cuenta_contable_recuperacion ?? 0);
            }
            else
            {
                ViewBag.id_cuenta_contable_perdidaL = new List<SelectListItem>();
                ViewBag.id_cuenta_contable_costoL = new List<SelectListItem>();
                ViewBag.id_cuenta_contable_recuperacionL = new List<SelectListItem>();
            }
            ViewBag.id_centro_costoL = Utilidades.DropDown.CentroCosto(k_bdei.id_centro_costo ?? 0);
            ViewBag.id_responsable_recuperacionL = Utilidades.DropDown.Usuario(k_bdei.id_responsable_recuperacion ?? 0);
            ViewBag.id_canal_riesgo_operacionalL = Utilidades.DropDown.CanalRiesgoOperacional(k_bdei.id_canal_riesgo_operacional ?? 0);
            ViewBag.id_frecuencia_riesgo_operacionalL = Utilidades.DropDown.FrecuenciaRiesgoOperacional(k_bdei.id_frecuencia_riesgo_operacional ?? 0);
            ViewBag.id_impacto_riesgo_operacionalL = Utilidades.DropDown.ImpactoRiesgoOperacional(k_bdei.id_impacto_riesgo_operacional ?? 0);

            ViewBag.id_tipo_riesgo_operacionalL = Utilidades.DropDown.TipoRiesgoOperacional(TRO);
            ViewBag.id_ambito_riesgo_operacionalL = Utilidades.DropDown.AmbitoRiesgoOperacional(ARO);
            ViewBag.id_producto_riesgo_operacionalL = Utilidades.DropDown.ProductoRiesgoOperacional(PRRO);
            ViewBag.id_categoria_linea_negocio_riesgo_operacionalL = Utilidades.DropDown.CategoriaLineaNegocioRiesgoOperacional(CLNRO);

            ViewBag.id_riesgo_asociado_bdeiL = Utilidades.DropDown.RiesgoAsociadoBDEI(k_bdei.id_riesgo_asociado_bdei ?? 0);
            ViewBag.id_minimo_riesgo_operativoL = Utilidades.DropDown.MinimoRiesgoOperativo(k_bdei.id_minimo_riesgo_operativo ?? 0);
            ViewBag.id_causa_bdeiL = Utilidades.DropDown.CausaBDEI(k_bdei.id_causa_bdei ?? 0);
            ViewBag.id_catalogo_conceptoL = Utilidades.DropDown.CatalogoConceptosBDEI(k_bdei.id_catalogo_concepto);


            ViewBag.DateFormat = Utilidades.Utilidades.GetDateFormat();

            return View(k_bdei);
        }
        // GET: BDEI/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_bdei k_bdei = db.k_bdei.Where(b => b.id_bdei == id).First();
            if (k_bdei == null)
            {
                return HttpNotFound();
            }

            Utilidades.DeleteActions.checkRedirect(redirect);


            return View(k_bdei);
        }

        // POST: BDEI/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            k_bdei k_bdei = db.k_bdei.Find(id);
            db.k_bdei.Remove(k_bdei);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("CantErase", "Error", null);
            }
            //En caso de que el registro se haya eliminado correctamente, redireccionar dependiendo desde donde se haya accesado al menú de eliminar
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

        public string GetCriticidad(int idfr, int idi)
        {
            return Utilidades.Utilidades.Criticidad(idfr, idi);
        }

        public ActionResult infoCanal()
        {
            //Comprobar si existe el archivo
            string pathPDF = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/BDEIAuxInfo/canal.pdf");
            string pathPNG = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/BDEIAuxInfo/canal.png");

            ViewBag.path = "canal";

            if (System.IO.File.Exists(pathPDF))
            {
                ViewBag.type = "pdf";
            }
            else if (System.IO.File.Exists(pathPNG))
            {
                ViewBag.type = "png";
            }
            else
            {
                ViewBag.type = "none";
            }

            return PartialView("DetailViews/info");
        }

        public ActionResult infoCategoriaLN()
        {
            //Comprobar si existe el archivo
            string pathPDF = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/BDEIAuxInfo/categoriaLN.pdf");
            string pathPNG = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/BDEIAuxInfo/categoriaLN.png");

            ViewBag.path = "categoriaLN";

            if (System.IO.File.Exists(pathPDF))
            {
                ViewBag.type = "pdf";
            }
            else if (System.IO.File.Exists(pathPNG))
            {
                ViewBag.type = "png";
            }
            else
            {
                ViewBag.type = "none";
            }

            return PartialView("DetailViews/info");
        }

        public ActionResult infoSubTipoProducto()
        {
            //Comprobar si existe el archivo
            string pathPDF = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/BDEIAuxInfo/subTipoProducto.pdf");
            string pathPNG = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/BDEIAuxInfo/subTipoProducto.png");

            ViewBag.path = "subTipoProducto";

            if (System.IO.File.Exists(pathPDF))
            {
                ViewBag.type = "pdf";
            }
            else if (System.IO.File.Exists(pathPNG))
            {
                ViewBag.type = "png";
            }
            else
            {
                ViewBag.type = "none";
            }

            return PartialView("DetailViews/info");
        }

        public ActionResult infoProceso()
        {
            //Comprobar si existe el archivo
            string pathPDF = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/BDEIAuxInfo/proceso.pdf");
            string pathPNG = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/BDEIAuxInfo/proceso.png");

            ViewBag.path = "proceso";

            if (System.IO.File.Exists(pathPDF))
            {
                ViewBag.type = "pdf";
            }
            else if (System.IO.File.Exists(pathPNG))
            {
                ViewBag.type = "png";
            }
            else
            {
                ViewBag.type = "none";
            }

            return PartialView("DetailViews/info");
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
