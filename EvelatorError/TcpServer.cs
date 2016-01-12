using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace EvelatorError
{
    class TcpServer
    {
        
        /// <summary>
        /// Object of each client in TCP server with connection information
        /// </summary>     
        /// <remarks>Create if server accept client connection </remarks>   
         public class ClientObject
        {
            /// <summary>
            /// clientSocket attribut
            /// </summary>
            /// <value> Used for client communication</value>
            public Socket clientSocket = null;
            /// <summary>
            /// BufferRecieveSize attribut
            /// </summary>
            /// <value> Seize of recieve buffer </value>
            public const int BufferRecieveSize = 10000;
            /// <summary>
            /// Buffer attribut
            /// </summary>
            /// <value>buffer with capacity of Buffer recieve size. See <see cref="BufferRecieveSize"/> </value>
            public byte[] buffer = new byte[BufferRecieveSize];                      
        }
        /// <summary>
        /// Server for recieve error message
        /// </summary>
        public class AsynchronousSocketListener
        {           
            //Thread signal
            public static ManualResetEvent threadSignal = new ManualResetEvent(false);

            /// <summary>
            /// Static method for starting listening of client communication
            /// </summary>
            public static void StartListening()
            {
                // Establish the local endpoint for the socket.
                // The DNS name of the computer               
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                //Represents a network endpoint as an IP address and a port number.
                //IPEndPoint(ipadress, port);
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

                //TCP/IP socket
                //InterNetwork = ipv4 adresa
                Socket tcpSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);

                // Bind the socket to the local endpoint and listen for incoming connections.
                try
                {
                    tcpSocket.Bind(localEndPoint);
                    //port
                    tcpSocket.Listen(100);

                    while(true)
                    {
                        //Set event to nonsignaled state, all threads all block
                        threadSignal.Reset();

                        //Start listen for connection
                        Console.WriteLine("Wait for connection");
                        tcpSocket.BeginAccept(new AsyncCallback(AcceptCallbackFunction), tcpSocket);

                        //block curent thread untill, handle recieve a signall
                        threadSignal.WaitOne();
                    }
                }
                catch(Exception e)
                {
                    throw e;
                }
       
            }
            /// <summary>
            /// Accept client communication
            /// </summary>
            /// <param name="ar"> Server tcp socket</param>
            public static void AcceptCallbackFunction(IAsyncResult ar)//ar = tcpSocket
            {
                //One or more waiting threads proceed
                threadSignal.Set();

                //Socket to client communication (server socket)
                Socket informationSocket = (Socket)ar.AsyncState;
                //accept incoming connection and create new socket to handle remote host communication. (dalkova manipulace)
                Socket handleSocket = informationSocket.EndAccept(ar);

                //Create a client object
                ClientObject clientObject = new ClientObject();
                clientObject.clientSocket = handleSocket;
                //begin recieve data from connected socket
                handleSocket.BeginReceive(clientObject.buffer, 0, ClientObject.BufferRecieveSize, 0, new AsyncCallback(ReadCallbackFunction), clientObject);
            }
            /// <summary>
            /// Recieve data from connecting client socket
            /// </summary>
            /// <param name="ar">Client object <see cref="ClientObject"/></param>
            public static void ReadCallbackFunction(IAsyncResult ar)//ar = clientObject
            {
                // String recievedMessage = String.Empty;
                int byteRead;
                ClientObject clientObject = (ClientObject)ar.AsyncState;
                Socket handleSocket = clientObject.clientSocket;
                try
                {
                    byteRead = handleSocket.EndReceive(ar);
                    if (byteRead > 0)
                    {
                        // Console.WriteLine("Read {0} bytes from socket. \n Data: {1}", Encoding.ASCII.GetString(clientObject.buffer, 0, 52).Length, Encoding.ASCII.GetString(clientObject.buffer, 0, 52));
                        Error error = Parser.ErrorParse(Encoding.ASCII.GetString(clientObject.buffer, 0, 54));
                        error.TimeStamp = DateTime.UtcNow;
                        // Console.WriteLine("serialID:{0}, errorCode:{1}, floor: {2}, timestamp: {3}", error.serialID, error.errorCode, error.floor, error.timeStamp);                    
                        Database.InsertError(error);
                        handleSocket.BeginReceive(clientObject.buffer, 0, ClientObject.BufferRecieveSize, 0, new AsyncCallback(ReadCallbackFunction), clientObject);
                    }
                    else
                    {
                        handleSocket.Shutdown(SocketShutdown.Both);
                        handleSocket.Close();
                        Console.WriteLine("Disconect in TCP server");
                    }
                }
                catch
                {
                    handleSocket.Shutdown(SocketShutdown.Both);
                    handleSocket.Close();
                    Console.WriteLine("Disconect in TCP server");
                }
                
                
            }
        }
    }
}

