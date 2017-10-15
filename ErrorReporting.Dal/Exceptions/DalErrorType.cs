using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorReporting.Dal.Exceptions
{
    public class DalErrorType
    {
        // --------------------------------------------------------------------------
        //                                                                    Generic 

        // --------------------------------------------------------------------------
        //                                                                 Exceptions 
        public static readonly string SqlError = "SqlError";
        public static readonly string SqlUniqueConstraintViolation = "SqlUniqueConstraintViolation";
        public static readonly string SqlConstraintCheckViolation = "SqlConstraintCheckViolation";
        // --------------------------------------------------------------------------
        //                                                         Repositories stack 

        // --------------------------------------------------------------------------
        //                                                             Services stack

        // --------------------------------------------------------------------------
    }
}
