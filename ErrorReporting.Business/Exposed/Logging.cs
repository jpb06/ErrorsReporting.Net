using ErrorReporting.Business.Internal.Contracts;
using ErrorReporting.Business.InversionOfControl;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ErrorReporting.Business.Exposed
{
    public static class Logging
    {
        public static void Save(Exception exception, AssemblyName assemblyName, string errorCode)
        {
            using (IUnityContainer unit = IoCConfiguration.container.CreateChildContainer())
            {
                IErrorsReportingManager manager = unit.Resolve<IErrorsReportingManager>();
                manager.LogError(exception, assemblyName, errorCode);
            }
        }
    }
}
