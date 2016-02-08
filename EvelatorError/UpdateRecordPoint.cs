using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvelatorError
{
    /// <summary>
    /// Throw is NewEvelatorID in database is not null
    /// </summary>
    class UpdateRecordPoint : UpdateException
    {
     /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="point">Param to save in attribute point</param>
        public UpdateRecordPoint()
        {
            base.message = "Zde uz zaznam existuje";
        }
        

    }
}
