using System;
using System.Collections.Generic;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KafkaStreams
{
    class Program
    {
        static void Main(string[] args)
        {
            var topicName = "test-topic";

            var configBuilder = new ConfigurationBuilder()
                                .AddCommandLine(args);
            //.AddEnvironmentVariables()
            //.AddJsonFile()
            //.AddInMemoryCollection();

            var config = configBuilder.Build();
            var broker = config["broker"];

            var factory = new LoggerFactory()
                            .AddConsole(LogLevel.Debug, true);

            var logger = factory.CreateLogger("main");

            logger.LogInformation($"Kafka broker: {broker}");

            //MessageProducer producer = new MessageProducer("10.50.240.50:9092", topicName);
            MessageProducer producer = new MessageProducer(broker, topicName);

            Console.WriteLine($"{producer.Name} producing on topic {topicName}. q to exit.");
            var text = "";
            while ((text = Console.ReadLine()) != "q")
            {
                producer.Produce(topicName, text);
            }


        }
    }
}