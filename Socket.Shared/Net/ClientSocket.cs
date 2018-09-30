using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using log4net;

namespace Socket.Shared.Net
{

    public class ClientSocket : IDisposable
    {
        public static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public byte[] STX => new byte[0x02];
        public byte[] ETX => new byte[0x03];

        private readonly TcpClient clientSocket;
		private readonly string hostName;
		private readonly int port;
        private NetworkStream networkStream;
        private Thread t;

        public ClientSocket(string HostName, int Port)
		{
			this.hostName = HostName;
			this.port = Port;
			this.clientSocket = new TcpClient();
            clientSocket.Connect(hostName, port);
            networkStream = clientSocket.GetStream();
        }

        public void OpenSocket()
        {
            try
            {
                clientSocket.Connect(hostName, port);
                log.Debug("client connected!!");
                networkStream = clientSocket.GetStream();
            }
            catch (SocketException socketEx)
            {
                throw new POCSocketException(socketEx.Message, socketEx);
            }
        }        

        /// <summary>
        /// Make the Command line for the Pump, will add STX, ETX and Calc CD byte and Add to the data stream
        /// </summary>
        /// <param name="data"></param>
        public byte[] MakeCMDLine(string data)
        {
            ControlBase CB = new ControlBase();
            var buffer = Encoding.ASCII.GetBytes(data);
            byte[] CD = new byte[] { CB.CalcCRC(buffer) };

            List<byte> Send = new List<byte>();
            Send.AddRange(STX);
            Send.AddRange(buffer);
            Send.AddRange(ETX);
            Send.AddRange(CD);
            return Send.ToArray();
        }        
        /// <summary>
        /// Send Command to Pump, will add STX, ETX and Calc CD byte and Add to the data stream
        /// </summary>
        /// <param name="Message"></param>
        public void SendCMD(string Message)
        {
            byte[] Send = MakeCMDLine(Message);
            networkStream.Write(Send, 0, Send.Length);
        }

        /// <summary>
        /// Write Pure data to networkStream, no test, no calc CRC.
        /// </summary>
        /// <param name="data"></param>
        public void WriteSocket(String data)
        {
            var buffer = Encoding.ASCII.GetBytes(data);
            networkStream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Read data from the NetworkStream
        /// </summary>
        /// <returns></returns>
        public string ReadSocket()
        {
            var receivedBytes = new byte[1024];
            int byteCount = 0;
            String answer = "";

            while ((byteCount = networkStream.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
            {
                answer += Encoding.ASCII.GetString(receivedBytes, 0, byteCount);
            }

            return answer;
        }

        /// <summary>
        /// Connects to the host
        /// </summary>
        /// <exception cref="POCSocketException"></exception>
        public void ConnectSocket()
        {
            try
            {
                clientSocket.Connect(hostName, port);
                log.Debug("client connected!!");

                networkStream = clientSocket.GetStream();

                t = new Thread(o => ReceiveData((TcpClient)o));
                t.Start(clientSocket);

                var s = string.Empty;
                while (!string.IsNullOrWhiteSpace((s = Console.ReadLine())))
                {
                    var buffer = Encoding.ASCII.GetBytes(s);
                    networkStream.Write(buffer, 0, buffer.Length);
                }
            }
            catch (SocketException socketEx)
            {
                throw new POCSocketException(socketEx.Message, socketEx);
            }
        }

        private void ReceiveData(TcpClient client)
        {
            var networkStream = client.GetStream();
            var receivedBytes = new byte[1024];
            int byteCount = 0;

            while ((byteCount = networkStream.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
            {
                log.Debug(Encoding.ASCII.GetString(receivedBytes, 0, byteCount));
            }
        }
        public void CloseSocket()
        {
            clientSocket.Client.Shutdown(SocketShutdown.Send);
            t.Join();

            networkStream.Close();
            clientSocket.Close();
        }

        //Function to convert byte into string:
        private string GetString(byte[] data)
        {
            return Encoding.UTF8.GetString(data, 0, data.Length);
        }

        //Function to convert string into byte:
        private byte[] GetBytes(string message)
        {
            return Encoding.UTF8.GetBytes(message);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ServerSocket() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
