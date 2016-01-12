using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
/// <summary>
/// /
/// </summary>

namespace EvelatorError
{
    class Program
    {
        static WebHost webHost;

        static void Main(string[] args)
        {
            /*
            Error error = new Error();
            error.ErrorCode = 1;
            error.EvelatorID = 2;
            error.Floor = 3;
            error.TimeStamp = DateTime.Now;
            Database.start();
            try
            {                
               Database.InsertError(error);
               //Database.UpdateEvelator(2, 73222, "Mlsna", 52, "Orlova", 3);
               //Database.UpdateEvelatorToNew(3, 4, 73222, "Mlsna", 32, "Orlova");
                
            }
            catch(Exception e) when (e is UpdateRecordDontExistException)
            {
                Console.WriteLine("Record with this ID dont exits");
            }
            catch (Exception e) when (e is UpdateDuplicateKeyException)
            {
                Console.WriteLine("Duplicate record");
            }
            catch(Exception e) when (e is UpdateRecordPoint)
            {
                Console.WriteLine(((UpdateRecordPoint)e).message);
            }
            Console.ReadKey();

           // TcpServer.AsynchronousSocketListener.StartListening();
            */
            webHost = new WebHost();
            Console.WriteLine("WebApi", "Starting Web hosting application.");
            try
            {
                Thread oThread = new Thread(new ThreadStart(webHost.Run));
                // background thread
                oThread.IsBackground = true;
                // start thread
                oThread.Start();
            }
            catch(ThreadStateException)
            {
                Console.WriteLine("WebApi", "Web hosting application could not be started.");
            }
            Console.WriteLine("Database", "Starting Database");
            try
            {
                Database.Start();
            }
            catch           
            {
                Console.WriteLine("Chyba databaze");
            }
            Console.WriteLine("TCPserver", "Starting TCPserver");
            try
            {
                TcpServer.AsynchronousSocketListener.StartListening();
            }
            catch(Exception e)
            {
                Console.WriteLine("Chyba Tcp servru" + e.ToString());
            }
        }
    }
}
