using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using ServerConsole.Net;
using System.Configuration;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;

namespace rabbitmq
{
    class Program
    {
        private static TcpListener _tcpListener;
        public static Dictionary<Guid, TcpClient> _clients = new Dictionary<Guid, TcpClient>();
        private static object _lock = new object();
        public static List<Guid> lsClientId = new List<Guid>();
        public static void Main()
        {
            //Initialize();
            //StartListening();
            Connect();
            //GetMessageFromRabbitMQClient();
        }

        public static void Connect()
        {
            TcpClient _clientSocket = new TcpClient();
            var hostIPAddress = ConfigurationManager.AppSettings["HostIPAddress"];
            var hostPort = ConfigurationManager.AppSettings["Port"];
            try
            {
                _clientSocket.Connect(hostIPAddress, Convert.ToInt32(hostPort));
                Console.WriteLine("client connected!!");

                //Console.WriteLine("Enter any message:");

                var networkStream = _clientSocket.GetStream();

                var t = new Thread(o => ReceiveData((TcpClient)o));
                t.Start(_clientSocket);

                //var buf = Encoding.ASCII.GetBytes("Winsock connected!");
                //networkStream.Write(buf, 0, buf.Length);

                //var s = string.Empty;
                //while (!string.IsNullOrWhiteSpace((s = Console.ReadLine())))
                //{
                //    var buffer = Encoding.ASCII.GetBytes(s);
                //    networkStream.Write(buffer, 0, buffer.Length);
                //}

                var message = GetMessageFromRabbitMQClient(networkStream,_clientSocket);

                Console.WriteLine("disconnect from server!!");
                Console.ReadKey();
                //while (true)
                //{
                    
                //}

                _clientSocket.Client.Shutdown(SocketShutdown.Send);
                t.Join();

                networkStream.Close();
                _clientSocket.Close();

            }
            catch (SocketException socketEx)
            {
                //throw new POCSocketException(socketEx.Message, socketEx);
            }
        }
        public static string GetMessageFromRabbitMQClient(NetworkStream networkStream,TcpClient tcpClient)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var message = "";
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "queue_1", durable: false, exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: "queue_1", autoAck: false, consumer: consumer);
                Console.WriteLine(" [x] Awaiting RPC requests");

                consumer.Received += (model, ea) =>
                {
                    string response = null;

                    var body = ea.Body;
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;

                    try
                    {
                        message = Encoding.UTF8.GetString(body);
                        //int n = int.Parse(message);
                        Console.WriteLine("Received: " + message);
                        response = "Send back message: " + message + " at " + DateTime.Now.ToString("HH:mm:ss");//fib(n).ToString();
                        var buffer = Encoding.ASCII.GetBytes(message);
                        networkStream.Write(buffer, 0, buffer.Length);
                        
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" [.] " + e.Message);
                        response = "";
                    }
                    finally
                    {
                        var responseBytes = Encoding.UTF8.GetBytes(response);
                        channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                };
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
                return message;
                
            }
        }
        public static void ReceiveResponseFromWinSock(TcpClient client)
        {
            var networkStream = client.GetStream();
            var receivedBytes = new byte[1024];
            int byteCount = 0;
          
            while ((byteCount = networkStream.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
            {
                Console.WriteLine("From winsock: " + Encoding.ASCII.GetString(receivedBytes, 0, byteCount));
            }
        }
        public static void Message(string data)
        {
            //Console.Write("Enter Host Name: ");
            ////var hostName = Console.ReadLine();
            var host = GetConfigSetting();
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "queue_2",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                    //string message = "Hello World!";
                    var body = Encoding.UTF8.GetBytes(data);

                    channel.BasicPublish(exchange: "",
                                         routingKey: "queue_2",
                                         basicProperties: null,
                                         body: body);
                    Console.WriteLine(" [x] Sent {0}", data);


                    ////From winsock to rabbitmq
                    ////Console.WriteLine(" Press [x] to exit.");
                    ////Console.Write("Enter The Message: ");

                    ////From winsock to rabbitmq client
                    //channel.ExchangeDeclare(exchange: "queue_2", type: "fanout");

                    //var message = data;
                    ////Console.ReadLine();
                    //var body = Encoding.UTF8.GetBytes(message);

                    //channel.BasicPublish(exchange: "queue_2",
                    //                     routingKey: "",
                    //                     basicProperties: null,
                    //                     body: body);
                    //Console.WriteLine(" [>>] Sent {0}", message);

                    ////Console.ReadLine();
                    ////end winsock to rabbitmq client
                }
            }


            //var factory = new ConnectionFactory() { HostName = "localhost" };
            //using (var connection = factory.CreateConnection())
            //using (var channel = connection.CreateModel())
            //{
            //    channel.QueueDeclare(queue: "hello",
            //                         durable: false,
            //                         exclusive: false,
            //                         autoDelete: false,
            //                         arguments: null);

            //    //string message = "Hello World!";
            //    var body = Encoding.UTF8.GetBytes(data);

            //    channel.BasicPublish(exchange: "",
            //                         routingKey: "hello",
            //                         basicProperties: null,
            //                         body: body);
            //    Console.WriteLine(" [x] Sent {0}", data);
            //}

            //Console.WriteLine(" Press [enter] to exit.");
            //Console.ReadLine();

        }

        public static void StartListening()
        {
            var hostIPAddress = ConfigurationManager.AppSettings["HostIPAddress"];
            var hostPort = ConfigurationManager.AppSettings["Port"];
            _tcpListener = new TcpListener(IPAddress.Parse(hostIPAddress), Convert.ToInt32(hostPort));
            if (_tcpListener == null)
            {
                Console.WriteLine("Something went wrong. Consider calling Initialize() method first");
            }

            _tcpListener.Start();
            Console.WriteLine(" >> " + "Server Started");

            while (true)
            {
                var tcpClient = _tcpListener.AcceptTcpClient();
                var clientId = Guid.NewGuid();

                lock (_lock)
                {
                    _clients[clientId] = tcpClient;
                }
                lsClientId.Add(clientId);
                Console.WriteLine($"{clientId} Someone connected!!");
                //GetMessageFromRabbitMQClient();
                var t = new Thread(HandleClients);
                t.Start(clientId);
            }
        }

        public static void SendClient(string data)
        {
            var hostIPAddress = ConfigurationManager.AppSettings["HostIPAddress"];
            var hostPort = ConfigurationManager.AppSettings["Port"];
            try
            {
                foreach (var item in lsClientId)
                {
                    var tcpClient = _clients[item];

                    if (tcpClient.Connected)
                    {
                        var networkStream = tcpClient.GetStream();

                        var t = new Thread(o => ReceiveData((TcpClient)o));
                        t.Start(tcpClient);

                        var buffer = Encoding.ASCII.GetBytes(data);
                        networkStream.Write(buffer, 0, buffer.Length);

                        tcpClient.Client.Shutdown(SocketShutdown.Send);
                        t.Join();
                        networkStream.Close();
                        tcpClient.Close();
                    }
                }

            }
            catch (SocketException socketEx)
            {
                //throw new POCSocketException(socketEx.Message, socketEx);
                Console.WriteLine(socketEx.Message, socketEx);
            }
        }
        private static void ReceiveData(TcpClient client)
        {
            if (client.Connected)
            {
                var networkStream = client.GetStream();
                var receivedBytes = new byte[1024];
                int byteCount = 0;

                while ((byteCount = networkStream.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
                {
                    var msg = Encoding.ASCII.GetString(receivedBytes, 0, byteCount);
                    Console.Write("From winsock: " + msg);
                    Message(msg);
                }
            }
        }

        public static void SendMessageWinSock()
        {


        }
        public static void Initialize()
        {
            var hostIPAddress = ConfigurationManager.AppSettings["HostIPAddress"];
            var hostPort = ConfigurationManager.AppSettings["Port"];

            try
            {
                _tcpListener = new TcpListener(IPAddress.Parse(hostIPAddress), Convert.ToInt32(hostPort));
            }
            catch (ArgumentNullException argNullEx)
            {
                //throw new POCSocketException(argNullEx.Message, argNullEx);
                Console.WriteLine(argNullEx.Message, argNullEx);
            }
            catch (ArgumentOutOfRangeException argOutOfRangeEx)
            {
                Console.WriteLine(argOutOfRangeEx.Message, argOutOfRangeEx);
                //throw new POCSocketException(argOutOfRangeEx.Message, argOutOfRangeEx);
            }
            catch (Exception generalEx)
            {
                Console.WriteLine(generalEx.Message, generalEx);
                //throw new POCSocketException(generalEx.Message, generalEx);
            }
        }

        private static void HandleClients(object id)
        {
            var guid = (Guid)id;
            TcpClient client = null;

            lock (_lock)
            {
                client = _clients[guid];
            }

            while (client.Connected)
            {
                var networkStream = client.GetStream();
                var buffer = new byte[1024];

                var byteCount = networkStream.Read(buffer, 0, buffer.Length);
                if (byteCount == 0)
                {
                    break;
                }

                var data = Encoding.ASCII.GetString(buffer, 0, byteCount);
                Console.WriteLine(data);
                Message(data);
                //SendMessageWinSock();
            }

            lock (_lock)
            {
                _clients.Remove(guid);
                client.Client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
        }

        public static ConfigSetting GetConfigSetting()
        {
            var hostIPAddress = ConfigurationManager.AppSettings["HostIPAddress"];
            var hostPort = ConfigurationManager.AppSettings["Port"];

            return new ConfigSetting
            {
                HostIPAddress = hostIPAddress,
                Port = Convert.ToInt32(hostPort)
            };
        }
        public class ConfigSetting
        {
            public string HostIPAddress { get; set; }
            public int Port { get; set; }
        }
    }

}
public class RpcClient
{
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string replyQueueName;
    private readonly EventingBasicConsumer consumer;
    private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
    private readonly IBasicProperties props;

    public RpcClient()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };

        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        replyQueueName = channel.QueueDeclare().QueueName;
        consumer = new EventingBasicConsumer(channel);

        props = channel.CreateBasicProperties();
        var correlationId = Guid.NewGuid().ToString();
        props.CorrelationId = correlationId;
        props.ReplyTo = replyQueueName;

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body;
            var response = Encoding.UTF8.GetString(body);
            if (ea.BasicProperties.CorrelationId == correlationId)
            {
                respQueue.Add(response);
            }
        };
    }

    public string Call(string message)
    {

        var messageBytes = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(
            exchange: "",
            routingKey: "rpc_queue",
            basicProperties: props,
            body: messageBytes);


        channel.BasicConsume(
            consumer: consumer,
            queue: replyQueueName,
            autoAck: true);

        return respQueue.Take(); ;
    }

    public void Close()
    {
        connection.Close();
    }
}