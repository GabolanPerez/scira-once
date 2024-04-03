using System.Web.Mvc;
using System.Web.Routing;

namespace SCIRA
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            // Estructura
            routes.MapRoute("ObtieneMacroProcesos", "SelectList/ObtieneMacroProcesos/", new { controller = "SelectList", action = "ObtieneMacroProcesos" }, new[] { "SCIRA.Controllers" });
            routes.MapRoute("ObtieneProcesos", "SelectList/ObtieneProcesos/", new { controller = "SelectList", action = "ObtieneProcesos" }, new[] { "SCIRA.Controllers" });
            routes.MapRoute("ObtieneSubProcesos", "SelectList/ObtieneSubProcesos/", new { controller = "SelectList", action = "ObtieneSubProcesos" }, new[] { "SCIRA.Controllers" });

            // Tipos de Riesgo
            routes.MapRoute("ObtieneTiposRiesgo", "SelectList/ObtieneTiposRiesgo/", new { controller = "SelectList", action = "ObtieneTiposRiesgo" }, new[] { "SCIRA.Controllers" });

            // Tipologías de Riesgo
            routes.MapRoute("ObtieneSubClasesTipologiaRiesgo", "SelectList/ObtieneSubClasesTipologiaRiesgo/", new { controller = "SelectList", action = "ObtieneSubClasesTipologiaRiesgo" }, new[] { "SCIRA.Controllers" });
            routes.MapRoute("ObtieneTipologiasRiesgo", "SelectList/ObtieneTipologiasRiesgo/", new { controller = "SelectList", action = "ObtieneTipologiasRiesgo" }, new[] { "SCIRA.Controllers" });

            // Benchmark
            routes.MapRoute("ObtieneMacroProcesosBenchmark", "SelectList/ObtieneMacroProcesosBenchmark/", new { controller = "SelectList", action = "ObtieneMacroProcesosBenchmark" }, new[] { "SCIRA.Controllers" });
            routes.MapRoute("ObtieneProcesosBenchmark", "SelectList/ObtieneProcesosBenchmark/", new { controller = "SelectList", action = "ObtieneProcesosBenchmark" }, new[] { "SCIRA.Controllers" });
            routes.MapRoute("ObtieneSubProcesosBenchmark", "SelectList/ObtieneSubProcesosBenchmark/", new { controller = "SelectList", action = "ObtieneSubProcesosBenchmark" }, new[] { "SCIRA.Controllers" });
            routes.MapRoute("ObtieneEventosBenchmark", "SelectList/ObtieneEventosBenchmark/", new { controller = "SelectList", action = "ObtieneEventosBenchmark" }, new[] { "SCIRA.Controllers" });

            //BDEI
            routes.MapRoute("ObtieneSubTiposRiesgoOperacional", "SelectList/ObtieneSubTiposRiesgoOperacional/", new { controller = "SelectList", action = "ObtieneSubTiposRiesgoOperacional" }, new[] { "SCIRA.Controllers" });
            routes.MapRoute("ObtieneClasesEventoRiesgoOperacional", "SelectList/ObtieneClasesEventoRiesgoOperacional/", new { controller = "SelectList", action = "ObtieneClasesEventoRiesgoOperacional" }, new[] { "SCIRA.Controllers" });
            routes.MapRoute("ObtieneProcesosRiesgoOperacional", "SelectList/ObtieneProcesosRiesgoOperacional/", new { controller = "SelectList", action = "ObtieneProcesosRiesgoOperacional" }, new[] { "SCIRA.Controllers" });
            routes.MapRoute("ObtieneSubTiposProductoRiesgoOperacional", "SelectList/ObtieneSubTiposProductoRiesgoOperacional/", new { controller = "SelectList", action = "ObtieneSubTiposProductoRiesgoOperacional" }, new[] { "SCIRA.Controllers" });
            routes.MapRoute("ObtieneLineasNegocioRiesgoOperacional", "SelectList/ObtieneLineasNegocioRiesgoOperacional/", new { controller = "SelectList", action = "ObtieneLineasNegocioRiesgoOperacional" }, new[] { "SCIRA.Controllers" });
            routes.MapRoute("ObtieneCuentasContables", "SelectList/ObtieneCuentasContables/", new { controller = "SelectList", action = "ObtieneCuentasContables" }, new[] { "SCIRA.Controllers" });

        }
    }
}
