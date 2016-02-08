using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvelatorError 
{   
    /// <summary>
    /// Throw if some ID is duplicate
    /// </summary>
    class UpdateDuplicateKeyException : UpdateException
    {
        public UpdateDuplicateKeyException()
        {
            base.message = "Tento zaznam uz existuje";
        }

    }
}
