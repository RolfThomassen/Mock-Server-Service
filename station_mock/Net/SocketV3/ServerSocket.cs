using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Socket.Shared;
using Socket.Shared.Net;
using station_mock;
using station_mock.Pump;
using System.Reflection;
using log4net;

namespace station_mock.Net.SocketV3
{
    public class ServerSocket : IDisposable
    {
        private readonly string hostIPAddress;
        private readonly int port;
        public TcpListener tcpListener;
        private Dictionary<Guid, TcpClient> clients;

        private readonly object lockObj = new object();

        /// <summary>
        /// Here you initialise ThisStation, with the number of Pumps on the station. 
        /// You should only install as many pumps as you have PumpStories for, eg. 17 PumpStories, 
        /// then number should be 17 pumps
        /// Maximum pumps on a new station is 32
        /// </summary>
        private Station ThisStation = new Station(17);

        private const byte ACK = 0x06;

        /// <summary>
        /// Setup and prepare Server Socket
        /// </summary>
        /// <param name="hostIPAddress"></param>
        /// <param name="port"></param>
        public ServerSocket(string hostIPAddress, int port)
        {
            this.hostIPAddress = hostIPAddress;
            this.port = port;
            clients = new Dictionary<Guid, TcpClient>();
        }

        /// <summary>
        /// Initializes the server socket
        /// </summary>
        public void Initialize()
        {
            try
            {
                LogHelp.log.Info(LogHelp.LogText($"Starting server at {hostIPAddress}:{port}"));
                tcpListener = new TcpListener(IPAddress.Parse(hostIPAddress), port);
            }
            catch (ArgumentNullException argNullEx)
            {
                throw new POCSocketException(argNullEx.Message, argNullEx);
            }
            catch (ArgumentOutOfRangeException argOutOfRangeEx)
            {
                throw new POCSocketException(argOutOfRangeEx.Message, argOutOfRangeEx);
            }
            catch (Exception generalEx)
            {
                throw new POCSocketException(generalEx.Message, generalEx);
            }
        }

        protected Guid ClientGuid;

        /// <summary>
        /// 
        /// </summary>
        public async Task<bool> StartListening()
        {
            if (tcpListener == null)
            {
                LogHelp.log.Error(LogHelp.LogText($"Something went wrong. Consider calling Initialize() method first"));
                throw new POCSocketException("Something went wrong. Consider calling Initialize() method first");
            }

            tcpListener.Start();
            LogHelp.localEndpoint = tcpListener.LocalEndpoint;
            LogHelp.log.Info(LogHelp.LogText($"Connection started at {tcpListener.LocalEndpoint}"));

            Action task = delegate ()
            {
                while (true)
                {
                    //try()
                    //{ }
                    //catch
                    //{ }
                    var tcpClient = tcpListener.AcceptTcpClient();
                    var clientId = Guid.NewGuid();

                    lock (lockObj)
                    {
                        clients[clientId] = tcpClient;
                    }

                    LogHelp.remoteEndPoint = tcpClient.Client.RemoteEndPoint;
                    LogHelp.log.Info(LogHelp.LogText($"Client connected from {tcpClient.Client.RemoteEndPoint}"));

                    var t = new Thread(HandleClients);
                    t.Start(clientId);
                };
            };

            await Task.Run(task);
            return true;
        }

        #region CommandHandler
        private bool CommandHandler(Guid guid, int byteCount, byte[] buffer)
        {
            var data = Encoding.ASCII.GetString(buffer, 0, byteCount);

            TcpClient client = null;
            lock (lockObj) { client = clients[guid]; }

            LogHelp.localEndpoint = tcpListener.LocalEndpoint;
            LogHelp.remoteEndPoint = clients[guid].Client.RemoteEndPoint;


            // If FE only show in DEBUG mode.
            if (data.Contains("FE"))
            { LogHelp.LogRead(guid, data, LogHelp.LogLevel.debug); }
           else
            {
               LogHelp.LogRead(guid, data, LogHelp.LogLevel.info);
            }

            if (buffer[0].Equals(2)) // Byte0 = STX, start of Command
            {
                int pump = 0;
                try { pump = Convert.ToInt16(data.Substring(2, 2)); } catch { }

                switch (Convert.ToChar(buffer[1]))
                {
                    /*  STX B xx C ETX CD
                        B - Message identifier
                        xx- Pump No
                        C - Cancel identifier
                        Pump No = 1
                        Sample command = STX B 01 C ETX CD
                    *
                        STX B xx xxxxxxx ETX CD
                        Finalize Command Character=B
                        Pump #=XX (01 to 32)
                        xxxxxxxx = EHM$VPL

                        • Pump 1

                          Sample Command = STX B 01 EHM$VPL ETX CD 
                    */
                    case 'B':    //Pump Cancel or Finalize
                        if (data.Substring(4, 1) == "C") // Cancel Pump 
                        {
                            ThisStation.SetStatus(pump, PumpStatusEnum.Idle);
                        }
                        else if (data.Substring(4, 7) == "EHM$VPL") // Get Finalization
                        {
                            if (ThisStation.Pumps[pump].PumpStatus.Equals(PumpStatusEnum.DispenseCompleted))
                            {
                                /*              01H2M1$0000000000V0000000000P002600L010000
                                SendCMD(guid, $"07H2M0$0000002000V0000009217P002170L002000f");
                                                07 H2 M0 $0000002000 V0000009217 P002170 L002000
                                Pump=07
                                Hose=2
                                M=0 (flag?)
                                Amount:00000020.00
                                Volume:0000009.217
                                PPU:002.170
                                Limit:0020.00
                                Final sales data for pump 7, Hose=2, Amt=20.00, Vol=9.217, PPU=2.170 (L=20.00)
                                */
                                Pump.Pump Fpump = ThisStation.Pumps[pump];
                                decimal vol = Fpump.Volume; vol = vol.Equals(0) ? 1 : vol;
                                SendCMD(guid, $"{pump:00}" + // Pump 
                                    $"H{Fpump.Hose:0}" + //Hose 
                                    $"M{Fpump.Flag:0}" + //Flag 
                                    $"${Fpump.Amount * 100:0000000000}" +  //amount * 100
                                    $"V{Fpump.Volume * 1000:0000000000}" + //volume liter * 1000
                                    $"P{(Fpump.Amount / (vol)) * 1000:000000}" +  // price per unit * 1000
                                    $"L{Fpump.Limit * 100:000000}" +      // original limith * 100
                                    $"");
                                ThisStation.SetStatus(pump, PumpStatusEnum.Idle);
                            }
                        }
                        break;

                    /*
                        STX A Pump# Hose# Flag $$$$.$$ vvv.vvv ETX CD
                            In the actual command, decimal points are implied only.
                            Authorize Command Character=A
                            Pump #=XX (01 to 32)
                            Hose #=X (0 to 8, with 0 authorizing any hose — also see Multi-grade Lock Authorization which follows)
                            Flag=X (Type of authorization — see Description)
                            Dollar Limit Amount=$$$$.$$ (0000.01 to 9999.99)
                            Volume Limit Amount=vvv.vvv (000.001 to 999.999)

                            • Pump 1
                            • Hose 1
                            • $25.00

                        Sample Command = STX A 0101002500000000 ETX CD 
                    */
                    case 'A': // Authorize
                        int hose = Convert.ToInt16(data.Substring(4, 1));
                        string flag = data.Substring(5, 1);
                        decimal dollarLimit = Convert.ToDecimal(data.Substring(6, 6).Insert(4, "."));
                        decimal volumeLimit = Convert.ToDecimal(data.Substring(12, 6).Insert(3, "."));
                        if (ThisStation.Pumps[pump].PumpStatus.Equals(PumpStatusEnum.Reserved))
                        {
                            Pump.Pump Npump = ThisStation.Pumps[pump];
                            Npump.Limit = dollarLimit;
                            Npump.Volume = volumeLimit;
                            Npump.Hose = hose;
                            Npump.Flag = flag;

                            if (ThisStation.Pumps.ContainsKey(pump)) { ThisStation.Pumps.Remove(pump); }
                            ThisStation.Pumps.Add(pump, Npump);
                            ThisStation.SetStatus(pump, PumpStatusEnum.Authorized);
                        }
                        break;

                    /*
                       1, Check current pump state which should be idle
                       2. If match, sending reserve message to ISIS by sending pumpId and ZapOrderId.
                       
                       STX M xx ETX CD

                       M Reserve identifier
                       xx Pump No
                       Pump No = 1

                       Sample command = STX M 01 ETX CD                             
                       Failure command = STX M 01 1 ETX CD
                    */
                    case 'M': // Reserve
                        if (ThisStation.Pumps[pump].PumpStatus.Equals(PumpStatusEnum.Idle))
                        {
                            ThisStation.SetStatus(pump, PumpStatusEnum.Reserved);
                        }
                        else
                        {
                            SendCMD(guid, $"M{pump:00}{0x01}");
                            LogHelp.LogWrite(guid, $"Pump #{pump:00}: is already reserved", LogHelp.LogLevel.debug);
                        }
                        break;

                    /*
                    Request Pump Status
                    Get Status Command Character=FE

                    Sample Command = STX FE ETX CD 
                    Response Command = STX 28000000000000000000000000000000000000000000000000000000000000000 ETX CD
                                             (64 chars)
                    */
                    case 'F': // Request Pump Status 
                        if (data.Substring(1, 1) != "E")
                        {
                            //Do Timer tick here
                            //storyman.StoryHandler.TimerTick();

                            string PumpStatus = "";
                            foreach (KeyValuePair<int, Pump.Pump> item in ThisStation.Pumps)
                            {
                                if (!item.Key.Equals(0))
                                {
                                    PumpStatus += item.Value.Status;
                                }
                            }
                            for (int i = ThisStation.Pumps.Count; i < 66; i++)
                            {
                                PumpStatus += 0x00; PumpStatus += 0x00;
                            }
                            PumpStatus = PumpStatus.Substring(0, 64);
                            SendCMD(guid, $"28{PumpStatus}");
                        }
                        break;

                    case 'G': // Get Status on pump
                        SendCMD(guid, $"G{pump:00}{ThisStation.Pumps[pump].Status}");
                        break;
                    case 'S': // Change Status on pump
                        string Status = data.Substring(4, 2);
                        ThisStation.SetStatusText(pump, Status);
                        break;
                    case 'V': // Change Volume on pump
                        decimal volume = Convert.ToDecimal(data.Substring(4, 6).Insert(3, "."));
                        ThisStation.SetVolume(pump, volume);
                        break;
                    case 'D': // Change Amount on pump
                        decimal Amount = Convert.ToDecimal(data.Substring(4, 6).Insert(3, "."));
                        ThisStation.SetAmount(pump, Amount);
                        break;

                    case 'I': // Pump Info
                        decimal vol1 = ThisStation.Pumps[pump].Volume; vol1 = vol1.Equals(0) ? 1 : vol1;
                        SendCMD(guid, $"I{pump:00}" + // Pump 
                            $"S{ThisStation.Pumps[pump].Status}" +
                            $"H{ThisStation.Pumps[pump].Hose:0}" + //Hose 
                            $"M{ThisStation.Pumps[pump].Flag:0}" + //Flag 
                            $"${ThisStation.Pumps[pump].Amount * 100:0000000000}" +  //amount * 100
                            $"V{ThisStation.Pumps[pump].Volume * 1000:0000000000}" + //volume liter * 1000
                            $"P{(ThisStation.Pumps[pump].Amount / (vol1)) * 1000:000000}" +  // price per unit * 1000
                            $"L{ThisStation.Pumps[pump].Limit * 100:000000}" +      // original limith * 100
                            $"");
                        break;


                    default:
                        SendCMD(guid, data);
                        break;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region HandleClients
        private void HandleClients(object id)
        {
            var guid = (Guid)id;
            TcpClient client = null;
            lock (lockObj) { client = clients[guid]; }

            bool quit = false;
            while (!quit)
            {
                var networkStream = client.GetStream();
                var buffer = new byte[1024];
                int byteCount = 0;

                try
                {
                    byteCount = networkStream.Read(buffer, 0, buffer.Length);
                }
                catch { }

                if (byteCount == 0)
                {
                    break;
                }
                else
                {
                    var data = Encoding.ASCII.GetString(buffer, 0, byteCount);
                    if (!CommandHandler(guid, byteCount, buffer))
                    {
                        switch (data)
                        {
                            case "hi":
                                Broadcast(guid, $"Hi Guest, it is now {DateTime.Now:T}");
                                break;
                            case "hello":
                                Broadcast(guid, $"hi Guest, it is the {DateTime.Now:D}");
                                break;
                            case "hej":
                                Broadcast(guid, $"Hi Guest, it is the {DateTime.Now:D} and the time is {DateTime.Now:T}");
                                break;
                            case "time":
                                SendCMD(guid, $"T{DateTime.Now:T}");
                                break;
                            case "date":
                                SendCMD(guid, $"D{DateTime.Now:D}");
                                //Broadcast(guid, DateTime.Now.ToShortDateString());
                                break;
                            case "now":
                                SendCMD(guid, $"DT{DateTime.Now:Z}");
                                break;
                            case "datetime":
                                SendCMD(guid, $"DT{DateTime.Now:D} {DateTime.Now:T}");
                                break;
                            case "quit":
                                quit = true;
                                break;
                            default:
                                SendCMD(guid, data);
                                break;
                        }
                    }
                }
            }

            LogHelp.LogWrite(guid, " Stream Closed");
            lock (lockObj)
            {
                clients.Remove(guid);
                client.Client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
        }
        #endregion


        /// <summary>
        /// Make the Command line for the Pump, will add STX, ETX and Calc CD byte and Add to the data stream
        /// </summary>
        /// <param name="data"></param>
        private byte[] MakeCMDLine(string data)
        {
            ControlBase CB = new ControlBase();
            byte[] STX = new byte[] { 0x02 };
            byte[] ETX = new byte[] { 0x03 };
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
        protected void SendCMD(Guid guid, string Message)
        {
            byte[] Send = MakeCMDLine(Message);
            lock (lockObj)
            {
                clients[guid].GetStream().Write(Send, 0, Send.Length);
            }
            string Data = System.Text.Encoding.UTF8.GetString(Send);
            if (Message.Substring(0,4).Contains("28"))
            {
                LogHelp.LogWrite(guid, Data, LogHelp.LogLevel.debug);
            }
            else
            {
                LogHelp.LogWrite(guid, Data, LogHelp.LogLevel.info);
            }
        }
    
        /// <summary>
        /// This will broadcast data to the Socket
        /// </summary>
        /// <param name="guid">Client guid</param>
        /// <param name="data">data to write</param>
        private void Broadcast(Guid guid, string data)
        {
            var buffer = Encoding.ASCII.GetBytes(data + Environment.NewLine);
            lock (lockObj)
            {
                clients[guid].GetStream().Write(buffer, 0, buffer.Length);
            }
            LogHelp.LogWrite(guid, data, LogHelp.LogLevel.debug);
        }

        public void TimerTickTask()
        {
            lock (lockObj)
            {
                LogHelp.LogLine($"TimerTickTask: Pump stories", LogHelp.LogLevel.debug);
                foreach (KeyValuePair<int, Pump.Pump> item in ThisStation.Pumps)
                {
                    ThisStation.NextStepHistory(item.Key);
                }
            }

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