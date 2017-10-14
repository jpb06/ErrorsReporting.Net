using ErrorReporting.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorReporting.Dal.Manipulation.Services.Contracts
{
    public interface IErrorsReportingService : IDisposable
    {
        ErrorReportApplication GetApplication(string name, string version);
        ErrorReportApplication CreateApplication(string name, string version);
        int? LogException(int versionId, Exception exception, string errorCodeFullyQualifiedName);
    }
}
