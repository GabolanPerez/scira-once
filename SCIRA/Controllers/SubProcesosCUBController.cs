using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "SPCUB", ModuleCode = "MSICI002")]
    [CustomErrorHandler]
    public class SubProcesosCUBController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            var user = (IdentityPersonalizado)User.Identity;


            List<ListaSubProcesosCUBViewModel> model = new List<ListaSubProcesosCUBViewModel>();

            List<c_sub_proceso_normatividad> Contenidos = new List<c_sub_proceso_normatividad>();


            //Obtener lista de todos los nodos raiz de relaciones sub_proceso-normatividad
            if (user.Es_super_usuario)
                Contenidos = db.c_sub_proceso_normatividad.Where(spn => spn.es_raiz).ToList();
            else
                Contenidos = db.c_sub_proceso_normatividad.Where(spn => spn.es_raiz && spn.c_sub_proceso.id_responsable == user.Id_usuario).ToList();

            //Inicializar datos para la lista y agregarlos al modelo
            foreach (var spn in Contenidos)
            {
                ListaSubProcesosCUBViewModel aux = new ListaSubProcesosCUBViewModel();
                aux.ruta_sub_proceso = "Entidad " + spn.c_sub_proceso.c_proceso.c_macro_proceso.c_entidad.cl_entidad + ">>" + spn.c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso + ">>" + spn.c_sub_proceso.c_proceso.cl_proceso;
                aux.cn_sub_proceso = spn.c_sub_proceso.cl_sub_proceso + " - " + spn.c_sub_proceso.nb_sub_proceso;
                aux.id_sub_proceso = spn.c_sub_proceso.id_sub_proceso;
                aux.id_normatividad = spn.c_contenido_normatividad.id_contenido_normatividad;
                aux.nb_normatividad = spn.c_contenido_normatividad.cl_contenido_normatividad + " - " + spn.c_contenido_normatividad.ds_contenido_normatividad;
                aux.nivel_normatividad = spn.c_contenido_normatividad.c_nivel_normatividad.nb_nivel_normatividad;
                aux.ruta_normatividad = "";

                //Obtener la ruta de la normatividad
                int? id_padre = spn.c_contenido_normatividad.id_contenido_normatividad_padre;
                while (id_padre != null)
                {
                    //encontramos al padre del contenido actual
                    c_contenido_normatividad aux3 = db.c_contenido_normatividad.Find(id_padre);
                    //aniadimos el nombre al inicio de la ruta
                    aux.ruta_normatividad = aux3.cl_contenido_normatividad + ">>" + aux.ruta_normatividad;
                    id_padre = aux3.id_contenido_normatividad_padre;
                }
                model.Add(aux);
            }

            return View(model);
        }

        public ActionResult SPCUB()
        {
            //Declaracion del modelo SPCUB
            SPCUBViewModel model = new SPCUBViewModel();

            //Enviar Lista de SubProcesos

            try
            {
                IdentityPersonalizado identity = (IdentityPersonalizado)ControllerContext.HttpContext.User.Identity;
                int id_responsable = identity.Id_usuario;
                bool super_usuario = identity.Es_super_usuario;

                string sql = "exec obtiene_sub_procesos " + (super_usuario ? "0" : id_responsable.ToString());
                model.SubProcesos = db.Database.SqlQuery<ListaSubProcesosFNViewModel>(sql).ToList();
            }
            catch
            {
                return View("Error");
            }

            //Enviar lista de Contenido Normatividad
            model.Contenido_Normatividad = db.c_contenido_normatividad
                .Include(c => c.c_nivel_normatividad)
                .Where(c => c.id_contenido_normatividad_padre == null)
                .OrderBy(c => c.id_contenido_normatividad)
                .ToList();

            ViewBag.nb_sub_proceso = "null";
            ViewBag.nb_normatividad = "null";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult SPCUB(SPCUBViewModel model)
        {
            int cspids = 0;
            int cnmids = 0;

            try
            {
                cspids = model.spids.Count();
            }
            catch
            {
                cspids = 0;
            }

            try
            {
                cnmids = model.nmids.Count();
            }
            catch
            {
                cnmids = 0;
            }


            if (cspids <= 0)
            {
                ModelState.AddModelError("spids", Strings.getMSG("SubProcesosCUBSPCUBCreate001"));
            }
            if (cnmids <= 0)
            {
                ModelState.AddModelError("nmids", Strings.getMSG("SubProcesosCUBSPCUBCreate002"));
            }


            if (ModelState.IsValid)
            {


                foreach (var id_sub_proceso in model.spids)
                {
                    foreach (var id_normatividad in model.nmids)
                    {
                        //Encontrar el sub proceso y la normatividad
                        c_sub_proceso sub_proceso = db.c_sub_proceso.Find(id_sub_proceso);
                        c_contenido_normatividad contenido = db.c_contenido_normatividad.Find(id_normatividad);

                        //Encontrar todos los hijos de la normatividad y todos sus descendientes y crear tablas para cada uno, incluyendo el actual

                        ligarSPCUB(contenido, sub_proceso, true);
                    }
                }

                Debug.WriteLine("Operaciones terminadas, guardando cambios");

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            try
            {
                IdentityPersonalizado identity = (IdentityPersonalizado)ControllerContext.HttpContext.User.Identity;
                int id_responsable = identity.Id_usuario;
                bool super_usuario = identity.Es_super_usuario;

                string sql = "exec obtiene_sub_procesos " + (super_usuario ? "0" : id_responsable.ToString());
                model.SubProcesos = db.Database.SqlQuery<ListaSubProcesosFNViewModel>(sql).ToList();
            }
            catch
            {
                return View("Error");
            }


            //Enviar lista de Contenido Normatividad
            model.Contenido_Normatividad = db.c_contenido_normatividad
                .Include(c => c.c_nivel_normatividad)
                .Where(c => c.id_contenido_normatividad_padre == null)
                .OrderBy(c => c.id_contenido_normatividad)
                .ToList();

            ViewBag.nb_normatividad = "null";
            ViewBag.nb_sub_proceso = "null";
            return View(model);
        }

        private void ligarSPCUB(c_contenido_normatividad contenido, c_sub_proceso sub_proceso, bool raiz = false, bool descendant = false)
        {
            //si tiene descendientes
            if (db.c_contenido_normatividad.Any(c => c.id_contenido_normatividad_padre == contenido.id_contenido_normatividad))
            {
                //Debug.WriteLine("Leyendo hijos de "+contenido.ds_contenido_normatividad);
                //obtener lista de todos los descendientes


                //Repetir la operacion para cada descendiente
                if (descendant)
                {
                    List<c_contenido_normatividad> listaContenido = db.c_contenido_normatividad.Where(c => c.id_contenido_normatividad_padre == contenido.id_contenido_normatividad).ToList();
                    foreach (c_contenido_normatividad cont in listaContenido)
                    {
                        ligarSPCUB(cont, sub_proceso);
                    }
                }
            }
            //cuando ya no se encuentren descendientes
            //Crear la tabla de esta normatividad (solo en caso de que no exista)

            //if (raiz == true) Debug.WriteLine("Se leyeron todos los hijos del nodo original");
            if (!(db.c_sub_proceso_normatividad.Any(spn => spn.id_sub_proceso == sub_proceso.id_sub_proceso && spn.id_contenido_normatividad == contenido.id_contenido_normatividad)))
            {
                //Debug.WriteLine("No se encontraron mas hijos para " + contenido.ds_contenido_normatividad);
                var aux = new c_sub_proceso_normatividad();
                aux.id_contenido_normatividad = contenido.id_contenido_normatividad;
                aux.id_sub_proceso = sub_proceso.id_sub_proceso;
                aux.es_raiz = raiz;
                db.c_sub_proceso_normatividad.Add(aux);
            }

            return;
        }

        public ActionResult Contenido(int? id)
        {
            c_contenido_normatividad contenido;
            if (id == null)
            {
                return PartialView
                (
                    "Normatividades",
                    db.c_contenido_normatividad
                    .Include(c => c.c_nivel_normatividad)
                    .Where(c => c.id_contenido_normatividad_padre == null)
                    .OrderBy(c => c.id_contenido_normatividad)
                    .ToList()
                );
            }

            //necesitamos id y el nombre del padre del padre del contenido actual, hasta llegar a un contenido sin padre
            int? id_padre = id;
            List<int?> aux = new List<int?>();
            List<string> aux2 = new List<string>();
            int auxCont = 0;
            while (id_padre != null)
            {
                c_contenido_normatividad aux3 = db.c_contenido_normatividad.Find(id_padre);
                aux.Add(aux3.id_contenido_normatividad_padre);
                aux2.Add(aux3.cl_contenido_normatividad);
                id_padre = aux3.id_contenido_normatividad_padre;
                auxCont++;
            }


            ViewBag.IDs = aux;
            ViewBag.Claves = aux2;
            ViewBag.Contador = auxCont;

            contenido = db.c_contenido_normatividad.Find(id);
            ViewBag.idAnterior = contenido.id_contenido_normatividad_padre;

            return PartialView("Normatividades", db.c_contenido_normatividad.Where(c => c.id_contenido_normatividad_padre == id).OrderBy(c => c.id_contenido_normatividad).ToList());
        }

        // GET: SubProcesosCUB/Delete/5
        public ActionResult Delete(int? idSP, int? idNM)
        {
            if (idSP == null || idNM == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_sub_proceso sp = db.c_sub_proceso.Find(idSP);
            c_contenido_normatividad c_contenido_normatividad = db.c_contenido_normatividad.Find(idNM);
            if (sp == null || c_contenido_normatividad == null)
            {
                return HttpNotFound();
            }

            ListaSubProcesosCUBViewModel model = new ListaSubProcesosCUBViewModel();

            model.ruta_sub_proceso = "Entidad " + sp.c_proceso.c_macro_proceso.c_entidad.cl_entidad + ">>" + sp.c_proceso.c_macro_proceso.cl_macro_proceso + ">>" + sp.c_proceso.cl_proceso;
            model.ruta_normatividad = "";

            //Obtener la ruta de la normatividad
            int? id_padre = c_contenido_normatividad.id_contenido_normatividad_padre;
            while (id_padre != null)
            {
                //encontramos al padre del contenido actual
                c_contenido_normatividad aux3 = db.c_contenido_normatividad.Find(id_padre);
                //aniadimos el nombre al inicio de la ruta
                model.ruta_normatividad = aux3.cl_contenido_normatividad + ">>" + model.ruta_normatividad;
                id_padre = aux3.id_contenido_normatividad_padre;
            }
            model.id_sub_proceso = sp.id_sub_proceso;
            model.id_normatividad = c_contenido_normatividad.id_contenido_normatividad;
            model.nivel_normatividad = c_contenido_normatividad.c_nivel_normatividad.nb_nivel_normatividad;
            model.cn_sub_proceso = sp.cl_sub_proceso + " - " + sp.nb_sub_proceso;
            model.nb_normatividad = c_contenido_normatividad.ds_contenido_normatividad;

            return View(model);
        }

        // POST: SubProcesosCub/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int idSP, int idNM)
        {
            c_sub_proceso sp = db.c_sub_proceso.Find(idSP);
            c_contenido_normatividad c_contenido_normatividad = db.c_contenido_normatividad.Find(idNM);
            if (sp == null || c_contenido_normatividad == null)
            {
                return HttpNotFound();
            }

            Desligar(c_contenido_normatividad, sp);

            try
            {
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("CantErase", "Error", null);
            }

            return RedirectToAction("Index", "SubProcesosCUB");
        }







        private void Desligar(c_contenido_normatividad normatividad, c_sub_proceso sp, bool descendant = false)
        {
            //obtener todos los hijos de esta normatividad (todos deberian tener la misma tabla que hay que borrar)

            if (descendant)
            {
                List<c_contenido_normatividad> hijos = db.c_contenido_normatividad.Where(c => c.id_contenido_normatividad_padre == normatividad.id_contenido_normatividad).ToList();
                foreach (var hijo in hijos)
                {
                    Desligar(hijo, sp);
                }
            }

            var tablas = db.c_sub_proceso_normatividad.Where(spn => spn.id_contenido_normatividad == normatividad.id_contenido_normatividad && spn.id_sub_proceso == sp.id_sub_proceso);

            foreach (var tabla in tablas)
            {
                db.c_sub_proceso_normatividad.Remove(tabla);
            }
        }
    }
}
