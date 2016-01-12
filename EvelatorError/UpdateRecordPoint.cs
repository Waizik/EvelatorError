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
    class UpdateRecordPoint : Exception
    {
        /// <summary>
        /// Private attribute point
        /// </summary>
        /// <value> What number is in NewEvelatorID</value>
        private int point;
        /// <summary>
        /// message property
        /// </summary>
        /// <value>Message, thats is should be print, if ex. is throw</value>
        public string message { get; private set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="point">Param to save in attribute point</param>
        public UpdateRecordPoint(int point)
        {
            this.point = point;
            this.message = String.Format("This record points to {0}", point);
        }
        

    }
}
