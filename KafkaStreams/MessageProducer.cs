using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace KafkaStreams
{
    internal class MessageProducer
    {
        private Producer<Null, String> producer;

        public string Name
        {
            get
            {
                return this.producer.Name;
            }
        }



        public MessageProducer(string brokers, string topicName)
        {
            var config = new Dictionary<string, object> { { "bootstrap.servers", brokers } };
            producer = new Producer<Null, String>(config, null, new StringSerializer(Encoding.UTF8));
        }

        public void Produce(string topicName, string value)
        {
            var deliveryReport = producer.ProduceAsync(topicName, null, value);
            deliveryReport.ContinueWith(task =>
            {
                Console.WriteLine($"Topic: {task.Result.Topic }, Partition: {task.Result.Partition}, Offset: {task.Result.Offset}");
            });

        }
    }
}
