using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ErrorReporting.Dal.Exceptions
{
    public enum DalErrorType
    {
        // --------------------------------------------------------------------------
        //                                                                    Generic 

        // --------------------------------------------------------------------------
        //                                                                 Exceptions 
        SqlError,
        SqlUniqueConstraintViolation,
        SqlConstraintCheckViolation,
        // --------------------------------------------------------------------------
        //                                                         Repositories stack 

        // --------------------------------------------------------------------------
        //                                                             Services stack

        // --------------------------------------------------------------------------
    }
}
