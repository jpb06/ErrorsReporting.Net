using ErrorReporting.Business.Exceptions;
using ErrorReporting.Dal.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ErrorReporting.Business.Managers.Contracts
{
    public interface IErrorsReportingManager
    {
        void LogError(Exception exception, string errorCode);
    }
}
