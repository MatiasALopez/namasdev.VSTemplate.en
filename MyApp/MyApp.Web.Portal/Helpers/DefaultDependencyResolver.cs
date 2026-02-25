using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace MyApp.Web.Portal.Helpers
{
    public class DefaultDependencyResolver : IDependencyResolver
    {
        protected IServiceProvider serviceProvider;

        public DefaultDependencyResolver(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public object GetService(Type serviceType)
        {
            return this.serviceProvider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return this.serviceProvider.GetServices(serviceType);
        }
    }
}