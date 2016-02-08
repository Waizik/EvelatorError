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
   

    /// <summary>
    /// Class for parse incomming error
    /// </summary>
    /// <remarks> Error from evelator comes with format example:SerialID:5|Timestamp:15.4.2015..|... in string and parser
    /// parse this to Error class (see <see cref="Error"/>) </remarks>
    static class Parser//todo parser |
        //bud zjistit jak nacitat  bufferu pouze jednu zpravu, nebo proc to tcp server dela, nebo mit pred serial ID | a (kvuli splitu)
        //a vracet list erroru a to pak ukladat v databazi
    {
        /// <summary>
        /// Static method to error parse
        /// </summary>
        /// <param name="recieveError"> Recieved error in string</param>
        /// <returns>Return Error class with all information (see <see cref="Error"/>)</returns>
        public static ErrorMessage ErrorParse(string recieveError)
        {
            /// variable to parse error string
            int evelatorID;
            int floor;
          
            ///Split error to recieve array with | delimiter
            string[] recieveArray;
            recieveArray = recieveError.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

            ///Delete everithink before : in each position of array + control
            Dictionary<string, int> numberOfRecorDictionary = new Dictionary<string, int>()
            {
                {"SerialID", 0},
                {"Err", 0 },
                {"State", 0 },
                {"Floor", 0}
              

            };
            
            Dictionary<string, string> recieveDictionary = new Dictionary<string, string>();
            for (int i=0; i < recieveArray.Length; i++)
            {
                int index = recieveArray[i].IndexOf(":");
                if (index > 0)
                {
                    //Console.WriteLine("RecieveError:" + recieveArray[i].Substring(0, index) + " numberOf:" + numberOfRecorDictionary["SerialID"]);
                    if("SerialID".Equals(recieveArray[i].Substring(0, index)) && numberOfRecorDictionary["SerialID"] == 0)
                    {
                        recieveDictionary.Add("SerialID", recieveArray[i].Substring(index + 1));
                        numberOfRecorDictionary["SerialID"]++;
                    }
                    else if ("Err".Equals(recieveArray[i].Substring(0, index)) && numberOfRecorDictionary["Err"] == 0)
                    {
                        recieveDictionary.Add("Err", recieveArray[i].Substring(index + 1));
                        numberOfRecorDictionary["Err"]++;
                    }
                    else if ("State".Equals(recieveArray[i].Substring(0, index)) && numberOfRecorDictionary["State"] == 0)
                    {
                        recieveDictionary.Add("State", recieveArray[i].Substring(index + 1));
                        numberOfRecorDictionary["State"]++;
                    }
                    else if ("Floor".Equals(recieveArray[i].Substring(0, index)) && numberOfRecorDictionary["Floor"] == 0)
                    {
                        recieveDictionary.Add("Floor", recieveArray[i].Substring(index + 1));
                        numberOfRecorDictionary["Floor"]++;
                    }
                    else
                    {
                        //chyba potom throw exception treba
                        Console.WriteLine("Chyba pri parsovani {0}", recieveArray[i]);
                    }
                }
            }
            string[] stringErrors;
            List<int> intErrors = new List<int>();
            string[] stringStates;
            List<int> intStates = new List<int>();

            stringErrors = recieveDictionary["Err"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            stringStates = recieveDictionary["State"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            //parse all record to int

            for (int i = 0; i < stringErrors.Length; i++)
            {
                int temp = 0;
                if (!Int32.TryParse(stringErrors[i], out temp))
                {
                    Console.WriteLine("Chyba pri prevadeni pole erroru s indexem: {0} a textem: {1} na int",i, stringErrors[i]);
                }
                else
                {
                    intErrors.Add(temp);
                }
            }

            for (int i = 0; i < stringStates.Length; i++)
            {
                int temp = 0;
                if (!Int32.TryParse(stringStates[i], out temp))
                {
                    Console.WriteLine("Chyba pri prevadeni pole stavu s indexem: {0} a textem: {1} na int", i, stringStates[i]);
                }
                else
                {
                    intStates.Add(temp);
                }
            }

            if (!Int32.TryParse(recieveDictionary["SerialID"], out evelatorID))
                Console.WriteLine("Chyba pri prevadeni SerialID:{0} na int", recieveDictionary["SerialID"]); //kdyztak vsude throw exception
            if (!Int32.TryParse(recieveDictionary["Floor"], out floor))
                Console.WriteLine("Chyba pri prevadeni Floor:{0} na int", recieveDictionary["Floor"]);

            ///Save to Error and return
            ErrorMessage errorMessage = new ErrorMessage();
            errorMessage.EvelatorID = evelatorID;
            errorMessage.Floor = floor;
            errorMessage.Errors = intErrors;
            errorMessage.States = intStates;
            
        
            return errorMessage;
        }
    }
}
