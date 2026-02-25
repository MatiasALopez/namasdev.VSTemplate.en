using System.Web;
using System.Web.Optimization;

namespace MyApp.Web.Portal
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/scripts/site").Include(
                "~/Scripts/site.js"));

            EnableOptimizationsWithoutTransformationsOnDebugMode(bundles);
        }

        private static void EnableOptimizationsWithoutTransformationsOnDebugMode(BundleCollection bundles)
        {
            if (HttpContext.Current.IsDebuggingEnabled)
            {
                foreach (var b in bundles)
                {
                    b.Transforms.Clear();
                }

                BundleTable.EnableOptimizations = true;
            }
        }
    }
}
