using ErrorReporting.Business.Managers;
using ErrorReporting.Business.Managers.Contracts;
using ErrorReporting.Dal.Context;
using ErrorReporting.Dal.Context.Contracts;
using ErrorReporting.Dal.Manipulation.Repositories;
using ErrorReporting.Dal.Manipulation.Repositories.Contracts;
using ErrorReporting.Dal.Manipulation.Services;
using ErrorReporting.Dal.Manipulation.Services.Contracts;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorReporting.Business.IoC
{
    public static class BusinessLocator
    {
        private static readonly UnityContainer container;

        static BusinessLocator() 
        {
            container = new UnityContainer();

            container.RegisterType<IErrorsReportingContext, ErrorsReportingContext>(new HierarchicalLifetimeManager());
            container.RegisterType(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            container.RegisterType<IErrorsReportingService, ErrorsReportingService>();

            container.RegisterType<IErrorsReportingManager, ErrorsReportingManager>();
        }

        public static T GetManager<T>() where T : IManager 
        {
            return container.Resolve<T>();
        }
    }
}
