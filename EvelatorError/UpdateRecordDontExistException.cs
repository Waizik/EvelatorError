﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EvelatorError
{   
    /// <summary>
    /// Throw if some ID dont exists
    /// </summary>
    class UpdateRecordDontExistException : UpdateException
    {
       public  UpdateRecordDontExistException()
        {
            base.message = "Record dont exist";
        }
    }

    
}
