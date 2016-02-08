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
