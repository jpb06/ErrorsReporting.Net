using ErrorReporting.Business.Internal.Contracts;
using ErrorReporting.Dal.Manipulation.Services.Contracts;
using ErrorReporting.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ErrorReporting.Business.Internal
{
    internal class ErrorsReportingManager : IErrorsReportingManager
    {
        private IErrorsReportingService reportingService;

        public ErrorsReportingManager(IErrorsReportingService reportingService) 
        {
            this.reportingService = reportingService;
        }

        public void LogError(Exception exception, AssemblyName assemblyName, string errorCode)
        {
            string applicationName = assemblyName.Name;
            string applicationVersion = assemblyName.Version.ToString();

            ErrorReportApplication application = this.reportingService.GetApplication(applicationName, applicationVersion);
            if (application == null)
                application = this.reportingService.CreateApplication(applicationName, applicationVersion);

            this.reportingService.LogException(application.Id, exception, errorCode);
        }
    }
}
