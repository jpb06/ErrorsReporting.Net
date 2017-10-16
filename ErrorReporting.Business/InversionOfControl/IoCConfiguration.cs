using ErrorReporting.Business.Internal;
using ErrorReporting.Business.Internal.Contracts;
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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ErrorReporting.Business.InversionOfControl
{
    internal static class IoCConfiguration
    {
        public static readonly UnityContainer container;

        static IoCConfiguration() 
        {
            container = new UnityContainer();

            container.RegisterType<IErrorsReportingContext, ErrorsReportingContext>(new HierarchicalLifetimeManager());
            container.RegisterType(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            container.RegisterType<IErrorsReportingService, ErrorsReportingService>();

            container.RegisterType<IErrorsReportingManager, ErrorsReportingManager>();
        }
    }
}
