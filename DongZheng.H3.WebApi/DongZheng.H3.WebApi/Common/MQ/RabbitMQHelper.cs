using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace DongZheng.H3.WebApi.Common
{
    public class RabbitMQHelper
    {
        ConnectionFactory factory;
        public RabbitMQHelper(string HostName, string UserName, string Password)
        {
            factory = new ConnectionFactory();
            factory.HostName = HostName;
            factory.UserName = UserName;
            factory.Password = Password;
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.AutomaticRecoveryEnabled = true;
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);
        }

        public RabbitMQHelper(string HostName, string UserName, string Password,string VirtualHost,int Port)
        {
            factory = new ConnectionFactory();
            factory.HostName = HostName;
            factory.UserName = UserName;
            factory.Password = Password;
            factory.VirtualHost = VirtualHost;
            factory.Port = Port;
            factory.AutomaticRecoveryEnabled = true;
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);
        }


        public void SendMsg(string message, string task_queue_name)
        {
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    bool durable = true;//队列是否持久化；
                    channel.QueueDeclare(task_queue_name, durable, false, false, null);

                    var properties = channel.CreateBasicProperties();

                    properties.Persistent = true;//消息是否持久化  等于properties.DeliveryMode = 2;

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish("", task_queue_name, properties, body);

                    Console.WriteLine(string.Format("MQ Send Message Queue-->[{0}],Message-->[{1}]", task_queue_name, message));
                }
            }
        }

        public delegate bool MessageProcess(string Message);

        public void ReceiveMessage(string task_queue_name,MessageProcess messageProcess)
        {
            var connection = factory.CreateConnection();

            var channel = connection.CreateModel();
            var durable = true;
            channel.QueueDeclare(task_queue_name, durable, false, false, null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("Message Received {0}", message);
                if (messageProcess(message))
                {
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                else
                {
                    channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };
            channel.BasicConsume(queue: task_queue_name, autoAck: false, consumer: consumer);
        }

        public void ReceiveMsg(string task_queue_name)
        {
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    bool durable = true;
                    channel.QueueDeclare(task_queue_name, durable, false, false, null);
                    //channel.BasicQos(0, 1, false);//公平分发；

                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(task_queue_name, false, consumer);

                    while (true)
                    {
                        var ea = consumer.Queue.Dequeue();

                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);

                        Console.WriteLine("Received {0}", message);

                        channel.BasicAck(ea.DeliveryTag, false);
                        Thread.Sleep(100);
                    }

                }
            }
        }
    }
}