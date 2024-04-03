using LinqKit;
using Microsoft.Ajax.Utilities;
using SCIRA.Models;
using SCIRA.Validaciones;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "RepAcceso", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ReporteAccesoController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: ReporteGeneralBDEI
        public ActionResult Index()
        {
            //var today = DateTime.Now;

            //List<h_acceso> vista = db.h_acceso.Where(a => a.fe_acceso.Year == today.Year
            //&& a.fe_acceso.Month == today.Month
            //&& a.fe_acceso.Day == today.Day).ToList();


            //ViewBag.DateFormat = Utilidades.Utilidades.GetDateFormat();

            //return View(vista);
            
            return View();
        }

        public string nextRows(int id)
        {
            List<h_acceso> Regs = new List<h_acceso>();

            var nRegs = db.h_acceso.Count();
            if ((id + 1) * 1000 > nRegs)
            {
                if (id * 1000 < nRegs)
                {
                    var range = nRegs - (id * 1000);
                    Regs = db.h_acceso.OrderByDescending(a => a.fe_acceso).ToList().GetRange(1000 * id, range);
                }
                else
                {
                    return "error";
                }
            }
            else
            {
                Regs = db.h_acceso.OrderByDescending(a => a.fe_acceso).ToList().GetRange(1000 * id, 1000);
            }


            var res = "[";

            var nRegsR = Regs.Count();
            var counter = 0;

            foreach (var r in Regs)
            {
                counter++;
                res += "[\"" + r.fe_acceso + "\",\"" + r.c_usuario.nb_usuario + "\",\"" + r.nb_funcion + "\"]";
                if (counter != nRegsR)
                {
                    res += ",";
                }
            }

            res += "]";

            return res;
        }


        public string Rows(string date)
        {
            var splitedString = string.Format("{0:dd/MM/yyyy}", date).Split(new char[] { '/' });

            var day = int.Parse(splitedString[0]);
            var month = int.Parse(splitedString[1]);
            var year = int.Parse(splitedString[2]);

            List<h_acceso> Regs = db.h_acceso.Where(a => a.fe_acceso.Year == year
            && a.fe_acceso.Month == month
            && a.fe_acceso.Day == day).ToList();

            //List<h_acceso> Regs = new List<h_acceso>();

            //var nRegs = db.h_acceso.Count();
            //if ((id + 1) * 1000 > nRegs)
            //{
            //    if (id * 1000 < nRegs)
            //    {
            //        var range = nRegs - (id * 1000);
            //        Regs = db.h_acceso.OrderByDescending(a => a.fe_acceso).ToList().GetRange(1000 * id, range);
            //    }
            //    else
            //    {
            //        return "error";
            //    }
            //}
            //else
            //{
            //    Regs = db.h_acceso.OrderByDescending(a => a.fe_acceso).ToList().GetRange(1000 * id, 1000);
            //}


            var res = "[";

            var nRegsR = Regs.Count();
            var counter = 0;

            foreach (var r in Regs)
            {
                counter++;
                res += "[\"" + r.fe_acceso + "\",\"" + r.c_usuario.nb_usuario + "\",\"" + r.nb_funcion + "\"]";
                if (counter != nRegsR)
                {
                    res += ",";
                }
            }

            res += "]";

            return res;
        }


        List<string> columns = new List<string>{
            "fe_acceso",
            "c_usuario.nb_usuario",
            "nb_funcion"
        };

        [HttpPost]
        public JsonResult ajaxData(DataTableAjaxPostModel model)
        {
            // action inside a standard controller
            int filteredResultsCount;
            int totalResultsCount;
            var res = filteredData(model, out filteredResultsCount, out totalResultsCount);

            var data = ReportTemplateModel.generateList(res);

            return Json(new
            {
                // this is what datatables wants sending back
                draw = model.draw,
                recordsTotal = totalResultsCount,
                recordsFiltered = filteredResultsCount,
                data = data
            });
        }

        private List<h_acceso> filteredData(DataTableAjaxPostModel model, out int filteredResultsCount, out int totalResultsCount)
        {
            var searchBy = (model.search != null) ? model.search.value : null;
            var take = model.length;
            var skip = model.start;

            var whereClause = BuildDynamicWhereClause(searchBy);


            if (string.IsNullOrEmpty(searchBy))
            {
                // si tenemos una busqueda en blanco, solamente ordenamos los resultados por id
                
            }

            string sortBy = columns.ElementAt(model.order[0].column);
            bool sortDir = model.order[0].dir == "asc";

            List<h_acceso> result = db.h_acceso
                .AsExpandable()
                .Where(whereClause)
                .OrderBy(sortBy + (sortDir ? "":" descending"))
                .Skip(skip)
                .Take(take)
                .ToList();


            //var result = db.h_acceso
            //               .AsExpandable()
            //               .Where(whereClause)
            //               .Select(m => new YourCustomSearchClass
            //               {
            //                   Id = m.Id,
            //                   Firstname = m.Firstname,
            //                   Lastname = m.Lastname,
            //                   Address1 = m.Address1,
            //                   Address2 = m.Address2,
            //                   Address3 = m.Address3,
            //                   Address4 = m.Address4,
            //                   Phone = m.Phone,
            //                   Postcode = m.Postcode,
            //               })
            //               .OrderBy(sortBy, sortDir) // have to give a default order when skipping .. so use the PK
            //               .Skip(skip)
            //               .Take(take)
            //               .ToList();

            filteredResultsCount = db.h_acceso
                .AsExpandable()
                .Where(whereClause).Count();
            totalResultsCount = db.h_acceso
                .AsExpandable().Count();

            return result;
        }


        private Expression<Func<h_acceso, bool>> BuildDynamicWhereClause(string searchValue)
        {
            // simple method to dynamically plugin a where clause
            var predicate = PredicateBuilder.New<h_acceso>(true); // true -where(true) return all
            if (String.IsNullOrWhiteSpace(searchValue) == false)
            {
                // as we only have 2 cols allow the user type in name 'firstname lastname' then use the list to search the first and last name of dbase
                var searchTerms = searchValue.Split(' ').ToList().ConvertAll(x => x.ToLower());


                

                predicate = predicate.Or(s => searchTerms.Any(srch => (DbFunctions.Right("00" + s.fe_acceso.Day, 2) + "/" + DbFunctions.Right("00" + s.fe_acceso.Month, 2) + "/"+ SqlFunctions.DatePart("yyyy", s.fe_acceso)).Contains(srch)));
                predicate = predicate.Or(s => searchTerms.Any(srch => s.c_usuario.nb_usuario.ToLower().Contains(srch)));
                predicate = predicate.Or(s => searchTerms.Any(srch => s.nb_funcion.ToLower().Contains(srch)));
            }
            return predicate;
        }


        private string feFormat(DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
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