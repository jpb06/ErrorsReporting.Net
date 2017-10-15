using ErrorReporting.Business.IoC;
using ErrorReporting.Business.Managers.Contracts;
using ErrorReporting.Front.AssemblyInformation;
using ErrorReporting.Front.Exceptions;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorReporting.Front
{
    class Program
    {
        static void Main(string[] args)
        {
            int a = 2, b = 0;

            try 
            {
                int c = a / b;
            }
            catch (Exception exception) 
            {
                IErrorsReportingManager m = BusinessLocator.GetManager<IErrorsReportingManager>();
                m.LogError(exception, AssemblyHelper.AssemblyName, FrontErrorType.DivideByZero);
            }
        }
    }
}
