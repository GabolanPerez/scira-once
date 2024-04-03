﻿using SCIRA.Models;
using SCIRA.Validaciones;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "RepSPNORM", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ReporteSPNMController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            var Normatividades = db.c_normatividad.ToList();

            return View(Normatividades);
        }

        public ActionResult Report(int id)
        {
            var norm = db.c_normatividad.Find(id);
            var raiz = db.c_contenido_normatividad.Find(norm.id_root_contenido);
            var niveles = db.c_nivel_normatividad.Where(n => n.id_normatividad == id).OrderBy(n => n.no_orden).ToList();
            ViewBag.niveles = niveles;

            List<c_contenido_normatividad> contenidos = new List<c_contenido_normatividad>();

            contenidos = getContents(raiz);



            return PartialView(contenidos);
        }

        private List<c_contenido_normatividad> getContents(c_contenido_normatividad cont)
        {
            List<c_contenido_normatividad> contenidos = new List<c_contenido_normatividad>();
            List<c_contenido_normatividad> aux = new List<c_contenido_normatividad>();



            //var sql = "select * from c_contenido_normatividad where id_contenido_normatividad_padre = " + cont.id_contenido_normatividad;
            //var hijos = db.c_contenido_normatividad.SqlQuery(sql).ToList();

            var hijos = db.c_contenido_normatividad.Where(c => c.id_contenido_normatividad_padre == cont.id_contenido_normatividad).ToList();

            hijos = hijos.OrderBy(m => m.cl_contenido_normatividad).ToList();

            if (hijos.Count == 0)
            {
                contenidos.Add(cont);
            }
            else
            {
                foreach (var hijo in hijos)
                {
                    aux = getContents(hijo);
                    contenidos.AddRange(aux);
                }
            }

            return contenidos;
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