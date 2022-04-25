﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using producer.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace producer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private static readonly HttpClient client = new HttpClient();

        [HttpPost]
        public void Post([FromBody] Credentials tasks)
        {
            // make a post call to https://reqres.in/api/login
            // if received token, continue
            var values = new Dictionary<string, string>
              {
                  { "email", tasks.Email },
                  { "password", tasks.Password },
                  { "task", tasks.Task }
              };

            var content = new FormUrlEncodedContent(values);

            var response = client.PostAsync("https://reqres.in/api/login ", content);

            var responseString =response.Result;

            if (responseString == null)
            {
                return;
            }

            var factory = new ConnectionFactory()
            {
                //HostName = "localhost" , 
                //Port = 30724
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
                Port = Convert.ToInt32(Environment.GetEnvironmentVariable("RABBITMQ_PORT"))
            };

            Console.WriteLine(factory.HostName + ":" + factory.Port);
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "TaskQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = tasks.Task;
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "TaskQueue",
                                     basicProperties: null,
                                     body: body);
            }
        }
    }
}
