using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    public class ErrorController : Controller
    {
        //private SICIEntities db = new SICIEntities();

        public ActionResult Denied()
        {
            return View();
        }

        public ActionResult CantErase(string errorMSG = null)
        {
            ViewBag.Info = errorMSG;

            return View();
        }

        public ActionResult OnlyRead()
        {
            return View();
        }
        public ActionResult OnlySuperUser()
        {
            return View();
        }
        public ActionResult ModuleLocked()
        {
            return View();
        }
    }
}