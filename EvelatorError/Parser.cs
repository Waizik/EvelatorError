using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvelatorError
{
    /// <summary>
    /// Class with error information
    /// </summary>
    class Error
    {
        /// <summary>
        /// EvelatorID property
        /// </summary>
        /// <value> ID of evelator </value>
        public int? EvelatorID { get; set;}
        /// <summary>
        /// ErrorCode property
        /// </summary>
        /// <value>Specific error</value>
        public int? ErrorCode { get; set;}
        /// <summary>
        /// Floor property
        /// </summary>
        /// <value> Floor where Evelator is</value>
        public int? Floor { get; set;}
        /// <summary>
        /// Timestamp property
        /// </summary>
        /// <value> When the error comes to server</value>
        public DateTime TimeStamp { get; set; }
    }

    /// <summary>
    /// Class for parse incomming error
    /// </summary>
    /// <remarks> Error from evelator comes with format example:SerialID:5|Timestamp:15.4.2015..|... in string and parser
    /// parse this to Error class (see <see cref="Error"/>) </remarks>
    static class Parser
    {
        /// <summary>
        /// Static method to error parse
        /// </summary>
        /// <param name="recieveError"> Recieved error in string</param>
        /// <returns>Return Error class with all information (see <see cref="Error"/>)</returns>
        public static Error ErrorParse(string recieveError)
        {
            /// variable to parse error string
            int evelatorID;
            int errorCode;
            int floor;
        //    DateTime timeStamp;

            ///Split error to recieve array with | delimiter
            string[] recieveArray;
            recieveArray = recieveError.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            ///Delete everithink before : in each position of array
            for(int i=0; i < recieveArray.Length; i++)
            {
                int index = recieveArray[i].IndexOf(":");
                if (index > 0)
                    recieveArray[i] = recieveArray[i].Substring(index+1);
            }
            //Console.WriteLine("SerialID:{0}, timeStamp:{1}, errorCode:{2}, floor:{3}", recieveArray[0], recieveArray[1], recieveArray[2], recieveArray[3]);
            ///Parse each position of array
            if (!Int32.TryParse(recieveArray[0], out evelatorID))
                Console.WriteLine("SerialID\nSerialID\nSerialID\nSerialID\nSerialID\nSerialID\nSerialID\nSerialID\nSerialID\nSerialID\nSerialID\nSerialID\nSerialID\nSerialID\nSerialID\nSerialID\nSerialID\nSerialID\n");
           /* if(!DateTime.TryParse(recieveArray[1], out timeStamp))
                Console.WriteLine("timeStamp\ntimeStamp\ntimeStamp\ntimeStamp\ntimeStamp\ntimeStamp\ntimeStamp\ntimeStamp\ntimeStamp\ntimeStamp\ntimeStamp\ntimeStamp\ntimeStamp\ntimeStamp\ntimeStamp\ntimeStamp\n");
            */if(!Int32.TryParse(recieveArray[2], out errorCode))
                Console.WriteLine("errorCode\nerrorCode\nerrorCode\nerrorCode\nerrorCode\nerrorCode\nerrorCode\nerrorCode\nerrorCode\nerrorCode\nerrorCode\nerrorCode\nerrorCode\nerrorCode\nerrorCode\nerrorCode\n");
            if(!Int32.TryParse(recieveArray[3], out floor))
                Console.WriteLine("floor\nfloor\nfloor\nfloor\nfloor\nfloor\nfloor\nfloor\nfloor\nfloor\nfloor\nfloor\nfloor\nfloor\nfloor\nfloor\nfloor\nfloor\nfloor\nfloor\nfloor\nfloor\nfloor\nfloor\n");
            ///Save to Error and return
            Error error = new Error();
            error.EvelatorID = evelatorID;
            error.ErrorCode = errorCode;
            error.Floor = floor;
           // error.TimeStamp = timeStamp;
            return error;
        }
    }
}
