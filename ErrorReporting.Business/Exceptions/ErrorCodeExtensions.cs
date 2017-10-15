using ErrorReporting.Dal.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorReporting.Business.Exceptions
{
    public static class ErrorCodeExtensions
    {
        public static string ToQualifiedString(this DalErrorType errorType)
        {
            return "DalErrorType." + errorType.ToString();
        }

        public static string ToQualifiedString(this BusinessErrorType errorType) 
        {
            return "BusinessErrorType." + errorType.ToString();
        }
    }
}
