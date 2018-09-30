using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace clientconsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var receive = new Receive();
            //receive.Message();
            //receive.SendMessage();
            Console.WriteLine(" [x] Please enter any message");
            while (true)
            {
                var msgSend = "";
                if (!string.IsNullOrWhiteSpace((msgSend = Console.ReadLine())))
                {
                    var rpcClient = new RpcClient();

                    
                    var response = rpcClient.Call(msgSend);

                    //Console.WriteLine(" [.] Response from server >> '{0}'", response);
                    var receive = new Receive();
                    receive.Message();
                    rpcClient.Close();

                    

                    //Console.ReadLine();
                    Console.ReadKey();
                }
            }
                    
        }
    }
    class Receive
    {
        public void Message()
        {
            var host = GetConfigSetting();
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "queue_2",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] {0}", message);
                };
                channel.BasicConsume(queue: "queue_2",
                                     autoAck: true,
                                     consumer: consumer);




                //channel.ExchangeDeclare(exchange: "queue_2", type: "fanout");

                //var consumer = new EventingBasicConsumer(channel);
                //consumer.Received += (model, ea) =>
                //{
                //    var body = ea.Body;
                //    var message = Encoding.UTF8.GetString(body);
                //    Console.WriteLine(" [x] Received {0}", message);
                //};
                //channel.BasicConsume(queue: "queue_2",
                //                     autoAck: true,
                //                     consumer: consumer);



                //channel.QueueDeclare(queue: "queue_2",
                //                     durable: true,
                //                     exclusive: false,
                //                     autoDelete: false,
                //                     arguments: null);

                //var consumer = new EventingBasicConsumer(channel);
                //consumer.Received += (model, ea) =>
                //{
                //    var body = ea.Body;
                //    var message = Encoding.UTF8.GetString(body);
                //    Console.WriteLine(" [x] Received {0}", message);
                //};
                //channel.BasicConsume(queue: "queue_2",
                //                     autoAck: true,
                //                     consumer: consumer);

                //Console.WriteLine(" Press [enter] to exit.");
                //Console.ReadLine();
            }
            //var factory = new ConnectionFactory() { HostName = host.HostIPAddress };

            //using (var connection = factory.CreateConnection())
            //using (var channel = connection.CreateModel())
            //{
            //    //From winsock to rabbitmq
            //    //channel.QueueDeclare(queue: "queue_1",
            //    //                 durable: false,
            //    //                 exclusive: false,
            //    //                 autoDelete: false,
            //    //                 arguments: null);

            //    //var consumer = new EventingBasicConsumer(channel);
            //    //consumer.Received += (model, ea) =>
            //    //{
            //    //    var body = ea.Body;
            //    //    var message = Encoding.UTF8.GetString(body);
            //    //    Console.WriteLine(" [x] Received {0}", message);
            //    //};
            //    //channel.BasicConsume(queue: "queue_1",
            //    //                     autoAck: true,
            //    //                     consumer: consumer);

            //    //Console.WriteLine(" Press [enter] to exit.");
            //    //Console.ReadLine();
            //    //End from winsock to rabbitmq
            //    //From rabbitmq to winsock
            //    //var msgSend = string.Empty;
            //    //while (true)
            //    //{
            //    //    if (!string.IsNullOrWhiteSpace((msgSend = Console.ReadLine())))
            //    //    {
            //    //        channel.ExchangeDeclare(exchange: "queue_2", type: "fanout");

            //    //        var body = Encoding.UTF8.GetBytes(msgSend);
            //    //        channel.BasicPublish(exchange: "queue_2",
            //    //                             routingKey: "",
            //    //                             basicProperties: null,
            //    //                             body: body);
            //    //        Console.WriteLine(" [x] Sent {0}", msgSend);

            //    //        //var bodySend = Encoding.UTF8.GetBytes(msgSend);

            //    //        //channel.ExchangeDeclare(exchange: "queue_2", type: "fanout");

            //    //        //channel.BasicPublish(exchange: "queue_2",
            //    //        //                     routingKey: "",
            //    //        //                     basicProperties: null,
            //    //        //                     body: bodySend);
            //    //    }
            //    //}
            //    //End from rabbitmq to winsock
            //}
        }
        //public void SendMessage()
        //{
        //    var host = GetConfigSetting();
        //    var factory_send = new ConnectionFactory() { HostName = host.HostIPAddress };


        //    using (var connection_send = factory_send.CreateConnection())
        //    using (var channel_send = connection_send.CreateModel())
        //    {
        //        //From rabbitmq to winsock
        //        var msgSend = string.Empty;
        //        while (true)
        //        {
        //            if (!string.IsNullOrWhiteSpace((msgSend = Console.ReadLine())))
        //            {
        //                channel_send.ExchangeDeclare(exchange: "queue_2", type: "fanout");

        //                var body = Encoding.UTF8.GetBytes(msgSend);
        //                channel_send.BasicPublish(exchange: "queue_2",
        //                                     routingKey: "",
        //                                     basicProperties: null,
        //                                     body: body);
        //                Console.WriteLine(" [x] Sent {0}", msgSend);

        //                //var bodySend = Encoding.UTF8.GetBytes(msgSend);

        //                //channel.ExchangeDeclare(exchange: "queue_2", type: "fanout");

        //                //channel.BasicPublish(exchange: "queue_2",
        //                //                     routingKey: "",
        //                //                     basicProperties: null,
        //                //                     body: bodySend);
        //            }
        //        }
        //        //End from rabbitmq to winsock
        //    }
        //}
        private static ConfigSetting GetConfigSetting()
        {
            var hostIPAddress = ConfigurationManager.AppSettings["HostIPAddress"];

            return new ConfigSetting
            {
                HostIPAddress = hostIPAddress
            };
        }
        public class ConfigSetting
        {
            public string HostIPAddress { get; set; }
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

        public string Call(string msgSend)
        {
            var messageBytes = Encoding.UTF8.GetBytes(msgSend);
            channel.BasicPublish(
                exchange: "",
                routingKey: "queue_1",
                basicProperties: props,
                body: messageBytes);


            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);

            return respQueue.Take();

        }

        public void Close()
        {
            connection.Close();
        }
    }
}