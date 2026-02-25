using System;

using namasdev.Web.ModelBinders;

namespace MyApp.Web.Portal
{
    public class ModelBinderConfig
    {
        public static void Config()
        {
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(DateTime), new DateTimeModelBinder());
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(DateTime?), new DateTimeModelBinder());
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(int), new IntegerModelBinder());
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(int?), new IntegerModelBinder());
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(short), new ShortModelBinder());
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(short?), new ShortModelBinder());
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(long), new LongModelBinder());
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(long?), new LongModelBinder());
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(decimal?), new DecimalModelBinder());
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(double), new DoubleModelBinder());
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(double?), new DoubleModelBinder());
        }
    }
}