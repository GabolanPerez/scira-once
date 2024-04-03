using SCIRA.Models;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "UnidadIndicador", ModuleCode = "MSICI001")]
    [CustomErrorHandler]
    public class UnidadIndicadorController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: UnidadIndicador
        public ActionResult Index()
        {
            return View(db.c_unidad_indicador.ToList());
        }

        // GET: UnidadIndicador/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_unidad_indicador c_unidad_indicador = db.c_unidad_indicador.Find(id);
            if (c_unidad_indicador == null)
            {
                return HttpNotFound();
            }
            return View(c_unidad_indicador);
        }

        // GET: UnidadIndicador/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UnidadIndicador/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create([Bind(Include = "id_unidad_indicador,cl_unidad_indicador,nb_unidad_indicador")] c_unidad_indicador c_unidad_indicador)
        {
            if (ModelState.IsValid)
            {
                db.c_unidad_indicador.Add(c_unidad_indicador);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_unidad_indicador);
        }

        // GET: UnidadIndicador/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_unidad_indicador c_unidad_indicador = db.c_unidad_indicador.Find(id);
            if (c_unidad_indicador == null)
            {
                return HttpNotFound();
            }
            return View(c_unidad_indicador);
        }

        // POST: UnidadIndicador/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit([Bind(Include = "id_unidad_indicador,cl_unidad_indicador,nb_unidad_indicador")] c_unidad_indicador c_unidad_indicador)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_unidad_indicador).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_unidad_indicador);
        }

        // GET: UnidadIndicador/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_unidad_indicador c_unidad_indicador = db.c_unidad_indicador.Find(id);
            if (c_unidad_indicador == null)
            {
                return HttpNotFound();
            }

            if (redirect != null)
            {
                if (redirect != "bfo")
                {
                    //obtenemos el valor del numero de salto
                    int ns;
                    try
                    {
                        ns = (int)HttpContext.Session["JumpCounter"];
                    }
                    catch
                    {
                        ns = 0;
                    }
                    //Si ns es 0, creamos un nuevo array, agregamos la direccion actual y lo asignamos a la variable "Directions" y establecemos "JumpCounter" = 1
                    if (ns == 0)
                    {
                        List<string> directions = new List<string>();
                        directions.Add(redirect);
                        HttpContext.Session["JumpCounter"] = 1;
                        HttpContext.Session["Directions"] = directions;

                    }//En caso de que ns sea distinto a 0, obtenemos el Array "Directions", agregamos la direccion actual, aumentamos el contador y salvamos ambas variables globales
                    else
                    {
                        ns++;
                        List<string> directions = (List<string>)HttpContext.Session["Directions"];
                        directions.Add(redirect);
                        HttpContext.Session["JumpCounter"] = ns;
                        HttpContext.Session["Directions"] = directions;
                    }
                }
            }
            else
            {
                HttpContext.Session["JumpCounter"] = null;
                HttpContext.Session["Directions"] = null;
            }

            //Obtener todos los elementos a los que puede estar ligado este elemento.
            //creamos la lista que contendra a todos los registros relacionados
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();

            //solo incluiremos riesgos
            var r_indicador = db.c_indicador.Where(b => b.id_unidad_indicador == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_indicador.Count > 0)
            {
                foreach (var indicador in r_indicador)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Indicadores";
                    rr.cl_registro = indicador.cl_indicador;
                    rr.nb_registro = indicador.nb_indicador;
                    rr.accion = "Delete";
                    rr.controlador = "Indicador";
                    rr.id_registro = indicador.id_indicador.ToString();

                    RR.Add(rr);
                }
            }

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;

            return View(c_unidad_indicador);
        }

        // POST: UnidadIndicador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_unidad_indicador c_unidad_indicador = db.c_unidad_indicador.Find(id);
            db.c_unidad_indicador.Remove(c_unidad_indicador);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("CantErase", "Error", null);
            }
            return RedirectToAction("Index");
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
