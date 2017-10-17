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
        public static readonly string SqlError = "Dal.SqlError";
        public static readonly string SqlUniqueConstraintViolation = "Dal.SqlUniqueConstraintViolation";
        public static readonly string SqlConstraintCheckViolation = "Dal.SqlConstraintCheckViolation";
        // --------------------------------------------------------------------------
        //                                                         Repositories stack 

        // --------------------------------------------------------------------------
        //                                                             Services stack

        // --------------------------------------------------------------------------
    }
}
